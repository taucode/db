using System;
using TauCode.Db.Utils.Parsing.Core.Tokens;

namespace TauCode.Db.Utils.Parsing.Core.Nodes
{
    public sealed class UseSymbolNode : SingleSuccessorNode
    {
        public UseSymbolNode(SymbolName symbolName, Action<Token, ParsingContext> action)
            : base(action)
        {
            this.SymbolName = symbolName;
        }

        public SymbolName SymbolName { get; }

        public override AcceptResult Accepts(ParsingFlow flow, int forwardShift)
        {
            var token = flow.GetToken(forwardShift);

            if (token == null)
            {
                return null; // no token for me
            }

            if (token is SymbolToken symbolToken)
            {
                var accepts = symbolToken.Name == this.SymbolName;

                if (accepts)
                {
                    return new AcceptResult(this.NextNode, true);
                }
            }

            return null;

        }
    }
}
