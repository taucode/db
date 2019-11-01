using System.Linq;
using TauCode.Db.Model;
using TauCode.Db.Utils.Parsing.Core.Fluent;
using TauCode.Db.Utils.Parsing.Core.Fluent.Impl;

namespace TauCode.Db.Utils.Parsing.Core.Gallery
{
    public class IndexCreationBuilder : ParsingBlockBuilder
    {
        protected override INodeSyntax BuildSyntaxImpl()
        {
            var syntax = new NodeSyntax();

            var indexColumnsInBrackets =
                new IndexColumnsInBracketsBuilder(context => context.GetIndex().Columns).Build();

            syntax
                .UseWord(@"CREATE", (token, context) => context.AddProperty("index", new IndexMold()))
                .Split("after_create")
                    .UseWord(@"UNIQUE", (token, context) => context.GetIndex().IsUnique = true)
                    .SkipWord(@"INDEX", nodeName: "index")
                    .UseIdentifier((token, context) => context.GetIndex().Name = token.GetTokenIdentifier())
                    .SkipWord(@"ON")
                    .UseIdentifier((token, context) => context.GetIndex().TableName = token.GetTokenIdentifier())
                    .Attach(indexColumnsInBrackets.InputNode)
                    .Use(indexColumnsInBrackets.OutputNodes.Single())
                    .Split("after_table_closed")
                        .End(DeliverIndexToResult, "end")
                    .GetSplitter("after_table_closed")
                        .SkipSymbol(";")
                        .LinkTo("end")
                .GetSplitter("after_create")
                    .LinkTo("index");

            syntax.ResolveReferences();

            return syntax;
        }

        protected virtual void DeliverIndexToResult(Token token, ParsingContext context)
        {
            var index = context.GetIndex();
            context.SetResult(index);
            context.RemoveProperty("index");
        }
    }
}
