using System.Collections.Generic;
using System.Linq;
using TauCode.Db.Model;
using TauCode.Db.Utils.Dialects;
using TauCode.Db.Utils.Parsing.Core.Fluent;
using TauCode.Db.Utils.Parsing.Core.Fluent.Impl;
using TauCode.Db.Utils.Parsing.Core.Tokens;

namespace TauCode.Db.Utils.Parsing.Core.Gallery
{
    public class ColumnDefinitionBuilder : ParsingBlockBuilder
    {
        public ColumnDefinitionBuilder(IDialect dialect, ParsingBlock identityBlock)
        {
            this.Dialect = dialect;
            this.IdentityBlock = identityBlock;
        }

        protected IDialect Dialect { get; private set; }

        protected ParsingBlock IdentityBlock { get; private set; }

        protected override INodeSyntax BuildSyntaxImpl()
        {
            var syntax = new NodeSyntax();

            var singleWordTypeBlock = this.CreateSingleWordTypeBlock();
            var sizedTypeBlock = this.CreateSizedTypeBlock();
            var preciseNumberTypeBlock = this.CreatePreciseNumberTypeBlock();
            var primaryKeyBlock = this.CreatePrimaryKeyBlock();
            //var identityBlock = this.CreateIdentityBlock();
            var nullabilityBlock = this.CreateNullabilityBlock();

            syntax
                .UseIdentifier(AddColumn, "column_name")
                .Split("type")
                    .Attach(singleWordTypeBlock.InputNode)
                    .Use(singleWordTypeBlock.OutputNodes.Single())
                    .LinkTo("before_nullability")
                .GetSplitter("type")
                    .Attach(sizedTypeBlock.InputNode)
                    .Use(sizedTypeBlock.OutputNodes.Single())
                    .LinkTo("before_nullability")
                .GetSplitter("type")
                    .Attach(preciseNumberTypeBlock.InputNode)
                    .Use(preciseNumberTypeBlock.OutputNodes.Single())
                    .LinkTo("before_nullability")
                .Separate("before_nullability")
                    .Attach(nullabilityBlock.InputNode)
                    .Use(nullabilityBlock.OutputNodes.Single())
                    .Split("after_nullability")
                        .Attach(primaryKeyBlock.InputNode)
                        .Use(primaryKeyBlock.OutputNodes.Single())
                        .Split("after_primary_key")
                            .Attach(this.IdentityBlock.InputNode)
                            .Use(this.IdentityBlock.OutputNodes.Single())
                            .LinkTo("before_finish_column")
                        .GetSplitter("after_primary_key")
                            .LinkTo("before_finish_column")
                    .GetSplitter("after_nullability")
                        .LinkTo("before_finish_column")
                .Separate("before_finish_column")
                .Milestone(FinishColumn, "end");

            syntax.ResolveReferences();
            return syntax;
        }

        protected virtual void FinishColumn(Token token, ParsingContext context)
        {
            context.RemoveProperty("column");
        }

        protected virtual ParsingBlock CreateNullabilityBlock()
        {
            var syntax = new NodeSyntax();

            syntax
                .Split("nullability")
                    .UseWord(@"NOT", (token, context) => context.GetColumn().IsNullable = false)
                    .SkipWord(@"NULL", nodeName: "null")
                    .Milestone(ParsingNode.IdleAction, nodeName: "end_nullability")
                .GetSplitter("nullability")
                    .LinkTo("null")
                .GetSplitter("nullability")
                    .LinkTo("end_nullability");

            return new ParsingBlock(
                syntax.Root,
                new[]
                {
                    syntax.GetNode("end_nullability").Node
                });
        }

        protected virtual void SetInlinePrimaryKeyColumn(Token token, ParsingContext context)
        {
            var table = context.GetTable();
            var column = context.GetColumn();

            var primaryKey = new PrimaryKeyMold
            {
                Name = $"PK_{table.Name}",
                ColumnNames = new List<string>
                {
                    column.Name
                },
            };

            context.AddProperty("primary-key", primaryKey);
        }

        protected virtual ParsingBlock CreatePrimaryKeyBlock()
        {
            var syntax = new NodeSyntax();

            syntax
                .UseWord(@"PRIMARY", this.SetInlinePrimaryKeyColumn)
                .SkipWord(@"KEY", nodeName: "end");

            return new ParsingBlock(syntax.Root, syntax.GetNode("end").Node);
        }

        protected virtual ParsingBlock CreateSizedTypeBlock()
        {
            var syntax = new NodeSyntax();

            syntax
                .Custom(
                    this.SetColumnTypeName,
                    token =>
                        (
                            token is WordToken wordToken &&
                            this.Dialect.IsDialectSizedTypeName(wordToken.Word)
                        ) ||
                        (
                            token is IdentifierToken identifierToken &&
                            this.Dialect.IsDialectSizedTypeName(identifierToken.Identifier)
                        ),
                    "sized_type_name")
                .Split("after_name")
                    .Milestone(ParsingNode.IdleAction, "finish_type")
                .GetSplitter("after_name")
                    .SkipSymbol("(")
                    .UseNumber((token, context) => context.GetColumn().Type.Size = token.GetTokenInt32())
                    .SkipSymbol(")")
                    .LinkTo("finish_type");

            return new ParsingBlock(
                syntax.Root,
                new[]
                {
                    syntax.GetNode("finish_type").Node
                });
        }

        protected virtual void SetColumnTypeName(Token token, ParsingContext context)
        {
            var column = context.GetProperty<ColumnMold>("column");

            string typeName;

            if (token is WordToken wordToken)
            {
                typeName = wordToken.Word;
            }
            else if (token is IdentifierToken identifierToken)
            {
                typeName = identifierToken.Identifier;
            }
            else
            {
                throw UtilsHelper.CreateInternalSyntaxAnalyzerErrorException();
            }

            column.Type.Name = typeName.ToLower();
        }

        protected virtual ParsingBlock CreateSingleWordTypeBlock()
        {
            var syntax = new NodeSyntax();

            syntax
                .Custom(
                    this.SetColumnTypeName,
                    token =>
                        (
                            token is WordToken wordToken &&
                            this.Dialect.IsDialectSingleWordTypeName(wordToken.Word)
                        ) ||
                        (
                            token is IdentifierToken identifierToken &&
                            this.Dialect.IsDialectSingleWordTypeName(identifierToken.Identifier)
                        ),
                    "singleword_type_name")
                .Milestone(ParsingNode.IdleAction, "finish_type");

            return new ParsingBlock(
                syntax.Root,
                new[]
                {
                    syntax.GetNode("finish_type").Node
                });
        }

        protected virtual ParsingBlock CreatePreciseNumberTypeBlock()
        {
            return new PreciseNumberTypeBuilder(this.Dialect).Build();
        }

        protected virtual void AddColumn(Token token, ParsingContext context)
        {
            var table = context.GetTable();
            var columnName = token.GetTokenIdentifier();


            var column = new ColumnMold
            {
                Name = columnName.ToLower(),
                Type = new DbTypeMold(),
            };

            table.Columns.Add(column);
            context.AddProperty("column", column);
        }
    }
}
