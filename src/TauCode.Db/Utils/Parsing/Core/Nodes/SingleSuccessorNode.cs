using System;

namespace TauCode.Db.Utils.Parsing.Core.Nodes
{
    public abstract class SingleSuccessorNode : ParsingNode
    {
        private ParsingNode _nextNode;

        protected SingleSuccessorNode(Action<Token, ParsingContext> action)
            : base(action)
        {
        }

        public virtual ParsingNode NextNode
        {
            get => _nextNode;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                var validValue =
                    value is SingleSuccessorNode ||
                    value is SplitNode ||
                    value is UnresolvedLinkNode ||
                    value is EndNode;

                if (!validValue)
                {
                    throw UtilsHelper.CreateInternalSyntaxAnalyzerErrorException();
                }

                if (_nextNode != null)
                {
                    throw UtilsHelper.CreateInternalSyntaxAnalyzerErrorException();
                }

                _nextNode = value;
            }
        }

        /// <summary>
        /// You gotta exactly know what you are doing
        /// </summary>
        public void RedirectNextNodeTo(ParsingNode anotherNode)
        {
            if (_nextNode == null)
            {
                throw UtilsHelper.CreateInternalSyntaxAnalyzerErrorException();
            }

            _nextNode = anotherNode ?? throw new ArgumentNullException(nameof(anotherNode));
        }
    }
}
