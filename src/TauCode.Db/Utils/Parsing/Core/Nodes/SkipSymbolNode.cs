using TauCode.Db.Utils.Parsing.Core.Tokens;

namespace TauCode.Db.Utils.Parsing.Core.Nodes
{
    public sealed class SkipSymbolNode : SingleSuccessorNode
    {
        public SkipSymbolNode(SymbolName symbolName)
            : base(ParsingNode.IdleAction)
        {
            this.SymbolName = symbolName;
        }

        public SymbolName SymbolName { get; }

        public override AcceptResult Accepts(ParsingFlow flow, int forwardShift)
        {
            var token = flow.GetToken(forwardShift);

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

        public override string DebugName
        {
            get
            {
                if (this.Name == null)
                {
                    return $"skip '{SymbolName.GetSymbolNameCaption()}'";
                }

                return base.DebugName;
            }
        }
    }
}
