namespace TauCode.Db.Utils.Parsing.Core.Nodes
{
    public class IdleNode : SingleSuccessorNode
    {
        public IdleNode()
            : base(ParsingNode.IdleAction)
        {
        }

        public override AcceptResult Accepts(ParsingFlow flow, int forwardShift)
        {
            return new AcceptResult(this.NextNode, false);
        }
    }
}
