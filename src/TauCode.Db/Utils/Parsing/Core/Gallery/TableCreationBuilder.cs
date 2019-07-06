using System.Linq;
using TauCode.Db.Model;
using TauCode.Db.Utils.Dialects;
using TauCode.Db.Utils.Parsing.Core.Fluent;
using TauCode.Db.Utils.Parsing.Core.Fluent.Impl;

namespace TauCode.Db.Utils.Parsing.Core.Gallery
{
    public class TableCreationBuilder : ParsingBlockBuilder
    {
        public TableCreationBuilder(IDialect dialect, ParsingBlock identityBlock)
        {
            this.Dialect = dialect;
            this.IdentityBlock = identityBlock;
        }

        protected IDialect Dialect { get; private set; }

        protected ParsingBlock IdentityBlock { get; private set; }

        protected virtual void DeliverTableToResult(Token token, ParsingContext context)
        {
            var table = context.GetTable();
            context.SetResult(table);
            context.RemoveProperty("table");
        }

        protected virtual void FinishTable(Token token, ParsingContext context)
        {
            if (context.ContainsProperty("primary-key"))
            {
                var primaryKey = context.GetProperty<PrimaryKeyMold>("primary-key");
                var table = context.GetProperty<TableMold>("table");
                table.PrimaryKey = primaryKey;
                context.RemoveProperty("primary-key");
            }
        }

        protected override INodeSyntax BuildSyntaxImpl()
        {
            var syntax = new NodeSyntax();

            var columnBlock = this.CreateColumnBlock();
            var constraintsBlock = this.CreateTableConstraintsBlock();

            syntax
                .UseWord(
                    @"CREATE",
                    (token, context) =>
                    {
                        context.AddProperty("table", new TableMold());
                    })
                .SkipWord(@"TABLE", nodeName: "TABLE")
                .UseIdentifier((token, context) =>
                    {
                        context.GetTable().Name = token.GetTokenIdentifier();
                    })
                .SkipSymbol(@"(", nodeName: "skip table opening '('")
                .Milestone(ParsingNode.IdleAction, "enter_into_column")
                .Attach(columnBlock.InputNode)
                .Use(columnBlock.OutputNodes.Single())
                .Split("after_column_definition")
                    .SkipSymbol(@",", "skip comma")
                        .Split("after_comma")
                            .LinkTo("enter_into_column")
                        .GetSplitter("after_comma")
                            .Attach(constraintsBlock.InputNode, "constraints_block")
                            .Use(constraintsBlock.OutputNodes.Single())
                            .LinkTo("finish_table")
                .GetSplitter("after_column_definition")
                    .UseSymbol(@")", FinishTable, nodeName: "finish_table")
                    .Split("after_table_closed")
                        .End(DeliverTableToResult, "end")
                    .GetSplitter("after_table_closed")
                        .SkipSymbol(";")
                        .LinkTo("end");

            syntax.ResolveReferences();
            return syntax;
        }
        
        protected virtual ParsingBlock CreateColumnBlock()
        {
            return new ColumnDefinitionBuilder(this.Dialect, this.IdentityBlock).Build();
        }

        protected virtual void SetPrimaryKeyName(Token token, ParsingContext context)
        {
            var pkName = token.GetTokenIdentifier();
            context.AddProperty("primary-key-name", pkName);
        }

        protected virtual void AddPrimaryKey(Token token, ParsingContext context)
        {
            var primaryKey = context.GetTable().PrimaryKey = new PrimaryKeyMold();

            if (context.ContainsProperty("primary-key-name"))
            {
                primaryKey.Name = context.GetProperty<string>("primary-key-name");
                context.RemoveProperty("primary-key-name");
            }
        }

        protected virtual ParsingBlock CreatePrimaryKeyConstraintBlock()
        {
            var syntax = new NodeSyntax();
            var namesInBrackets = new NamesInBracketsBuilder(context => context.GetTable().PrimaryKey.ColumnNames).Build();

            syntax
                .Split("root")
                    .SkipWord(@"CONSTRAINT")
                    .UseIdentifier(SetPrimaryKeyName)
                    .UseWord(@"PRIMARY", AddPrimaryKey, nodeName: "primary")
                    .SkipWord(@"KEY")
                    .Attach(namesInBrackets.InputNode)
                    .Use(namesInBrackets.OutputNodes.Single())
                    .Idle("end")
                .GetSplitter("root")
                    .LinkTo("primary");

            syntax.ResolveReferences();

            return new ParsingBlock(syntax.Root, syntax.GetNode("end").Node);
        }

        protected virtual void AddForeignKey(Token token, ParsingContext context)
        {
            var foreignKey = new ForeignKeyMold();
            if (context.ContainsProperty("foreign-key-name"))
            {
                foreignKey.Name = context.GetProperty<string>("foreign-key-name");
                context.RemoveProperty("foreign-key-name");
            }
            context.AddProperty("foreign-key", foreignKey);
        }

        protected virtual void FinishTableForeignKey(Token token, ParsingContext context)
        {
            var table = context.GetProperty<TableMold>("table");
            var foreignKey = context.GetProperty<ForeignKeyMold>("foreign-key");
            table.ForeignKeys.Add(foreignKey);
            context.RemoveProperty("foreign-key");
        }

        protected virtual ParsingBlock CreateForeignKeyConstraintBlock()
        {
            var columnNamesInBrackets = new NamesInBracketsBuilder(context => context.GetForeignKey().ColumnNames).Build();
            var referencedColumnNamesInBrackets = new NamesInBracketsBuilder(context => context.GetForeignKey().ReferencedColumnNames).Build();

            var syntax = new NodeSyntax();
            syntax
                .Split("root")
                    .SkipWord(@"CONSTRAINT")
                    .UseIdentifier((token, context) => context.AddProperty("foreign-key-name", token.GetTokenIdentifier()))
                    .UseWord(@"FOREIGN", AddForeignKey, nodeName: "foreign")
                    .SkipWord(@"KEY")
                    .Attach(columnNamesInBrackets.InputNode)
                    .Use(columnNamesInBrackets.OutputNodes.Single())
                    .SkipWord(@"REFERENCES")
                    .UseIdentifier((token, context) => context.GetForeignKey().ReferencedTableName = token.GetTokenIdentifier())
                    .Attach(referencedColumnNamesInBrackets.InputNode)
                    .Use(referencedColumnNamesInBrackets.OutputNodes.Single())
                    .Milestone(FinishTableForeignKey, "end")
                .GetSplitter("root")
                    .LinkTo("foreign");

            syntax.ResolveReferences();

            return new ParsingBlock(syntax.Root, syntax.GetNode("end").Node);
        }

        protected virtual ParsingBlock CreateTableConstraintsBlock()
        {
            var pkBlock = this.CreatePrimaryKeyConstraintBlock();
            var fkBlock = this.CreateForeignKeyConstraintBlock();

            var syntax = new NodeSyntax();
            syntax
                .Split("before_pk_or_fk")
                    .Attach(pkBlock.InputNode)
                    .Use(pkBlock.OutputNodes.Single())
                    .Split("after_pk")
                        .LinkTo("end")
                    .GetSplitter("after_pk")
                        .SkipSymbol(@",")
                        .Attach(fkBlock.InputNode, "enter_fk")
                        .Use(fkBlock.OutputNodes.Single())
                        .Split("after_fk")
                            .LinkTo("end")
                        .GetSplitter("after_fk")
                            .SkipSymbol(@",")
                            .LinkTo("enter_fk")
                .GetSplitter("before_pk_or_fk")
                    .LinkTo("enter_fk")
                .Separate("end");

            syntax.ResolveReferences();

            return new ParsingBlock(syntax.Root, syntax.GetNode("end").Node);
        }
    }
}
