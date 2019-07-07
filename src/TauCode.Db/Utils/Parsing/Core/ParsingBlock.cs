using System;
using System.Linq;

namespace TauCode.Db.Utils.Parsing.Core
{
    public class ParsingBlock
    {
        public ParsingBlock(ParsingNode inputNode, ParsingNode[] outputNodes)
        {
            this.InputNode = inputNode ?? throw new ArgumentNullException(nameof(inputNode));

            if (outputNodes == null)
            {
                throw new ArgumentNullException(nameof(outputNodes));
            }

            if (outputNodes.Any(x => x == null))
            {
                throw new ArgumentException($"'outputNodes' should not contain nulls.", nameof(outputNodes));
            }

            if (outputNodes.Length == 0)
            {
                throw new ArgumentException($"'outputNodes' should not be empty.", nameof(outputNodes));
            }

            this.OutputNodes = outputNodes;
        }

        public ParsingBlock(ParsingNode inputNode, ParsingNode singleOutputNode)
            : this(inputNode, new[] { singleOutputNode })
        {

        }

        public ParsingNode InputNode { get; protected set; }

        public ParsingNode[] OutputNodes { get; protected set; }
    }
}

