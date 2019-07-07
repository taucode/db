using System;
using System.Diagnostics;

namespace TauCode.Db.Utils.Parsing.Core
{
    [DebuggerDisplay("{" + nameof(DebugName) + "}")]
    public abstract class ParsingNode
    {
        private readonly Action<Token, ParsingContext> _action;

        protected ParsingNode(Action<Token, ParsingContext> action)
        {
            _action = action ?? throw new ArgumentNullException(nameof(action));
        }

        public abstract AcceptResult Accepts(ParsingFlow flow, int forwardShift);

        public void Act(Token token, ParsingContext context)
        {
            _action(token, context);
        }

        public string Name { get; set; }

        public virtual string DebugName => this.Name ?? "<no name>";

        public static void IdleAction(Token token, ParsingContext context)
        {
            // idle
        }
    }
}
