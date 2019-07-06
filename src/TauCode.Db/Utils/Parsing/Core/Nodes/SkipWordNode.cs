using System;
using TauCode.Db.Utils.Parsing.Core.Tokens;

namespace TauCode.Db.Utils.Parsing.Core.Nodes
{
    public sealed class SkipWordNode : SingleSuccessorNode
    {
        public SkipWordNode(string word, bool caseSensitive)
            : base(ParsingNode.IdleAction)
        {
            this.Word = word ?? throw new ArgumentNullException(nameof(word));
            this.CaseSensitive = caseSensitive;
        }

        public string Word { get; }
        public bool CaseSensitive { get; }

        public override AcceptResult Accepts(ParsingFlow flow, int forwardShift)
        {
            var token = flow.GetToken(forwardShift);
            if (token == null)
            {
                return null; // no token for me
            }

            if (token is WordToken wordToken)
            {
                var accepts = wordToken.Word.Equals(this.Word, StringComparison.InvariantCultureIgnoreCase);

                if (accepts)
                {
                    return new AcceptResult(this.NextNode, true);
                }
            }

            return null;
        }

        public override string DebugName
        {
            get
            {
                if (this.Name == null)
                {
                    return $"skip '{this.Word}'";
                }

                return base.DebugName;
            }
        }
    }
}
