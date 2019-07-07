using System;

namespace TauCode.Db.Utils.Parsing.Core.Nodes
{
    public sealed class EndNode : ParsingNode
    {
        public EndNode(Action<Token, ParsingContext> action)
            : base(action)
        {
        }

        public override AcceptResult Accepts(ParsingFlow flow, int forwardShift)
        {
            return new AcceptResult(this, false);
        }
    }
}
