using System.Linq;
using TauCode.Db.Utils.Parsing.Core.Fluent;

namespace TauCode.Db.Utils.Parsing.Core
{
    public abstract class ParsingBlockBuilder
    {
        protected abstract INodeSyntax BuildSyntaxImpl();

        protected virtual string[] GetOutputNodeNames() => new[] { "end" };

        public ParsingBlock Build()
        {
            var syntax = this.BuildSyntaxImpl();
            return new ParsingBlock(
                syntax.Root,
                this.GetOutputNodeNames()
                    .Select(x => syntax.GetNode(x).Node)
                    .ToArray());
        }
    }
}
