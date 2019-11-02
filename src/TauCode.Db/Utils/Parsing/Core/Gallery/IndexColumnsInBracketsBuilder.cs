using System;
using System.Collections.Generic;
using System.Linq;
using TauCode.Db.Model;
using TauCode.Db.Utils.Parsing.Core.Fluent;
using TauCode.Db.Utils.Parsing.Core.Fluent.Impl;

namespace TauCode.Db.Utils.Parsing.Core.Gallery
{
    public class IndexColumnsInBracketsBuilder : ParsingBlockBuilder
    {
        private readonly Func<ParsingContext, List<IndexColumnMold>> _listGetter;

        public IndexColumnsInBracketsBuilder(Func<ParsingContext, List<IndexColumnMold>> listGetter)
        {
            _listGetter = listGetter;
        }

        protected override INodeSyntax BuildSyntaxImpl()
        {
            var syntax = new NodeSyntax();

            syntax
                .UseSymbol(@"(", (token, context) => _listGetter(context).Clear())
                .UseIdentifier(
                    (token, context) => _listGetter(context).Add(new IndexColumnMold
                    {
                        Name = token.GetTokenIdentifier().ToLower()
                    }),
                    nodeName: "identifier")
                .Split("after_identifier")
                    .Split("after_asc_or_desc")
                        .SkipSymbol(@")", nodeName: "end")
                    .GetSplitter("after_asc_or_desc")
                        .SkipSymbol(@",")
                        .LinkTo("identifier")
                .GetSplitter("after_identifier")
                    .UseWord("ASC", (token, context) => _listGetter(context).Last().SortDirection = SortDirection.Ascending)
                    .LinkTo("after_asc_or_desc")
                .GetSplitter("after_identifier")
                    .UseWord("DESC", (token, context) => _listGetter(context).Last().SortDirection = SortDirection.Descending)
                    .LinkTo("after_asc_or_desc");

            syntax.ResolveReferences();
            return syntax;
        }
    }
}
