using System;
using TauCode.Db.Utils.Parsing.Core.Tokens;

namespace TauCode.Db.Utils.Parsing.Core.Nodes
{
    public sealed class UseNumberNode : SingleSuccessorNode
    {
        public UseNumberNode(Action<Token, ParsingContext> action)
            : base(action)
        {
        }

        public override AcceptResult Accepts(ParsingFlow flow, int forwardShift)
        {
            var token = flow.GetToken(forwardShift);

            if (token == null)
            {
                return null; // no token for me
            }

            if (token is NumberToken numberToken)
            {
                return new AcceptResult(this.NextNode, true);
            }

            return null;
        }
    }
}
