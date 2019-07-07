using System;

namespace TauCode.Db.Utils.Parsing.Core.Nodes
{
    public class UnresolvedLinkNode : ParsingNode
    {
        public UnresolvedLinkNode(string linkToName)
            : base(InvalidAction)
        {
            this.LinkToName = linkToName ?? throw new ArgumentNullException(nameof(linkToName));
        }

        public string LinkToName { get; }

        public override AcceptResult Accepts(ParsingFlow flow, int forwardShift)
        {
            throw new InvalidOperationException("Internal error. Should not ever get here.");
        }

        private static void InvalidAction(Token token, ParsingContext context)
        {
            throw new InvalidOperationException("Internal error. Should not ever get here.");
        }
    }
}
