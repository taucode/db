namespace TauCode.Db.Utils.Parsing.Core
{
    public class AcceptResult
    {
        public AcceptResult(ParsingNode nextNode, bool tokenIsConsumed)
        {
            this.NextNode = nextNode;
            this.TokenIsConsumed = tokenIsConsumed;
        }

        public ParsingNode NextNode { get; }
        public bool TokenIsConsumed { get; }
    }
}
