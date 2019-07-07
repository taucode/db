using System;

namespace TauCode.Db.Utils.Parsing.Core.Nodes
{
    public class CustomNode : SingleSuccessorNode
    {
        public CustomNode(Action<Token, ParsingContext> action, Func<Token, bool> predicate)
            : base(action)
        {
            this.Predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
        }

        public Func<Token, bool> Predicate { get; }

        public override AcceptResult Accepts(ParsingFlow flow, int forwardShift)
        {
            var token = flow.GetToken(forwardShift);

            if (token == null)
            {
                return null; // no token for me
            }

            if (this.Predicate(token))
            {
                return new AcceptResult(this.NextNode, true);
            }

            return null;
        }
    }
}
