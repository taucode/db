using System;
using System.Collections.Generic;
using TauCode.Db.Utils.Parsing.Core.Fluent;
using TauCode.Db.Utils.Parsing.Core.Fluent.Impl;

namespace TauCode.Db.Utils.Parsing.Core.Gallery
{
    public class NamesInBracketsBuilder : ParsingBlockBuilder
    {
        private readonly Func<ParsingContext, List<string>> _listGetter;

        public NamesInBracketsBuilder(Func<ParsingContext, List<string>> listGetter)
        {
            _listGetter = listGetter;
        }

        protected override INodeSyntax BuildSyntaxImpl()
        {
            var syntax = new NodeSyntax();

            syntax
                .UseSymbol(@"(", (token, context) => _listGetter(context).Clear())
                .UseIdentifier(
                    (token, context) => _listGetter(context).Add(token.GetTokenIdentifier().ToLower()),
                    nodeName: "identifier")
                .Split("after_identifier")
                    .SkipSymbol(@")", "end")
                .GetSplitter("after_identifier")
                    .SkipSymbol(@",")
                    .LinkTo("identifier");

            syntax.ResolveReferences();
            return syntax;
        }
    }
}
