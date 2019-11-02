using System;
using System.Collections.Generic;
using System.Linq;
using TauCode.Db.Utils.Parsing.Core.Nodes;

namespace TauCode.Db.Utils.Parsing.Core.Fluent.Impl
{
    public class NodeSyntax : INodeSyntax
    {
        #region Fields

        private ParsingNode _node;
        private readonly NodeSyntax _rootSyntax;
        private readonly Dictionary<string, ParsingNode> _namedNodes;
        private readonly HashSet<ParsingNode> _nodes;
        private readonly List<UnresolvedLinkNode> _unresolvedLinkNodes;
        private bool _isResolved;

        private int _counter;

        #endregion

        #region Constructors

        public NodeSyntax()
        {
            _rootSyntax = this;
            _namedNodes = new Dictionary<string, ParsingNode>();
            _nodes = new HashSet<ParsingNode>();
            _unresolvedLinkNodes = new List<UnresolvedLinkNode>();
        }

        private NodeSyntax(INodeSyntax rootSyntax, ParsingNode node)
        {
            _rootSyntax = (NodeSyntax)rootSyntax;
            _node = node;
            _namedNodes = _rootSyntax._namedNodes;
            _nodes = _rootSyntax._nodes;
            _unresolvedLinkNodes = _rootSyntax._unresolvedLinkNodes;
        }

        #endregion

        #region Private

        private void CheckNotResolved()
        {
            if (_isResolved)
            {
                throw UtilsHelper.CreateInternalSyntaxAnalyzerErrorException();
            }
        }

        private void RememberUnresolvedLinkNode(UnresolvedLinkNode unresolvedLinkNode)
        {
            _unresolvedLinkNodes.Add(unresolvedLinkNode);
        }

        private INodeSyntax AttachOrAddSplitWay(ParsingNode node, string name)
        {
            if (_nodes.Contains(node))
            {
                throw UtilsHelper.CreateInternalSyntaxAnalyzerErrorException();
            }

            _nodes.Add(node);

            if (name != null)
            {
                node.Name = name;
                _namedNodes.Add(name, node);
            }

            if (_node == null)
            {
                if (this.IsRootSyntax())
                {
                    // I am a root syntax, simply not inited yet
                    _node = node;
                    return this;
                }
                else
                {
                    throw UtilsHelper.CreateInternalSyntaxAnalyzerErrorException();
                }
            }
            else
            {
                switch (_node)
                {
                    case SingleSuccessorNode singleSuccessorNode:
                        singleSuccessorNode.NextNode = node;
                        var childSyntax = new NodeSyntax(_rootSyntax, node);
                        return childSyntax;

                    case SplitNode splitNode:
                        splitNode.AddWay(node);
                        var splitNodeSyntax = new NodeSyntax(_rootSyntax, node);
                        return splitNodeSyntax;

                    default:
                        throw UtilsHelper.CreateInternalSyntaxAnalyzerErrorException();
                }
            }
        }

        private bool IsRootSyntax()
        {
            return _rootSyntax == this;
        }

        #endregion

        #region Public

        public void ResolveReferences()
        {
            if (!this.IsRootSyntax())
            {
                throw new InvalidOperationException("Not root syntax; cannot resolve references.");
            }

            this.CheckNotResolved();

            foreach (var unresolvedLinkNode in _unresolvedLinkNodes)
            {
                var realLinkNode = _namedNodes[unresolvedLinkNode.LinkToName];

                var linker = _nodes
                    .Where(x => x is SingleSuccessorNode)
                    .Cast<SingleSuccessorNode>()
                    .SingleOrDefault(x => x.NextNode == unresolvedLinkNode);

                if (linker != null)
                {
                    linker.RedirectNextNodeTo(realLinkNode);
                }
                else
                {
                    var split = _nodes
                        .Where(x => x is SplitNode)
                        .Cast<SplitNode>()
                        .SingleOrDefault(x => x.Ways.Contains(unresolvedLinkNode));

                    if (split == null)
                    {
                        throw UtilsHelper.CreateInternalSyntaxAnalyzerErrorException(); // no one recognizes link-to placeholder - wtf?
                    }

                    var index = -1;
                    for (var i = 0; i < split.Ways.Count; i++)
                    {
                        if (split.Ways[i] == unresolvedLinkNode)
                        {
                            index = i;
                            break;
                        }
                    }

                    split.RedirectWay(index, realLinkNode);
                }
            }

            _isResolved = true;
        }

        #endregion

        #region INodeSyntax Members

        public ParsingNode Root => _rootSyntax._node;

        public ParsingNode Node => _node;

        public INodeSyntax Attach(ParsingNode node, string nodeName = null)
        {
            this.CheckNotResolved();

            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            if (nodeName != null)
            {
                node.Name = nodeName;
            }

            return this.AttachOrAddSplitWay(node, nodeName);
        }

        public INodeSyntax Use(ParsingNode node, string nodeName = null)
        {
            this.CheckNotResolved();

            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            if (_nodes.Contains(node))
            {
                throw UtilsHelper.CreateInternalSyntaxAnalyzerErrorException();
            }

            if (nodeName != null)
            {
                node.Name = nodeName;
                _namedNodes.Add(nodeName, node);
            }

            _nodes.Add(node); // enroll the node

            var syntax = new NodeSyntax(_rootSyntax, node);
            return syntax;
        }

        public INodeSyntax Separate(string nodeName = null)
        {
            var node = new MilestoneNode(ParsingNode.IdleAction);

            return this.Use(node, nodeName);
        }

        public INodeSyntax LinkTo(string linkToNodeName)
        {
            this.CheckNotResolved();

            if (linkToNodeName == null)
            {
                throw new ArgumentNullException(nameof(linkToNodeName));
            }

            if (_node == null)
            {
                throw UtilsHelper.CreateInternalSyntaxAnalyzerErrorException();
            }

            ParsingNode linkToNode;
            if (_namedNodes.ContainsKey(linkToNodeName))
            {
                linkToNode = _namedNodes[linkToNodeName];
            }
            else
            {
                var unresolvedLinkNode = new UnresolvedLinkNode(linkToNodeName)
                {
                    Name = _rootSyntax._counter++.ToString()
                };

                this.RememberUnresolvedLinkNode(unresolvedLinkNode);
                linkToNode = unresolvedLinkNode;
            }

            if (_node is SingleSuccessorNode singleSuccessorNode)
            {
                singleSuccessorNode.NextNode = linkToNode;
            }
            else if (_node is SplitNode splitNode)
            {
                splitNode.AddWay(linkToNode);
            }
            else
            {
                throw UtilsHelper.CreateInternalSyntaxAnalyzerErrorException();
            }

            return new NodeSyntax(_rootSyntax, linkToNode);
        }

        public INodeSyntax UseWord(string word, Action<Token, ParsingContext> action, bool caseSensitive = false, string nodeName = null)
        {
            this.CheckNotResolved();

            var node = new UseWordNode(word, action, caseSensitive) { Name = nodeName };
            return this.AttachOrAddSplitWay(node, nodeName);
        }

        public INodeSyntax UseSymbol(string symbol, Action<Token, ParsingContext> action, string nodeName = null)
        {
            this.CheckNotResolved();

            var node = new UseSymbolNode(ParsingExtensions.ParseSymbol(symbol), action) { Name = nodeName };
            return this.AttachOrAddSplitWay(node, nodeName);
        }

        public INodeSyntax UseIdentifier(Action<Token, ParsingContext> action, string nodeName = null)
        {
            this.CheckNotResolved();

            var node = new UseIdentifierNode(action) { Name = nodeName };
            return this.AttachOrAddSplitWay(node, nodeName);
        }

        public INodeSyntax UseNumber(Action<Token, ParsingContext> action, string nodeName = null)
        {
            this.CheckNotResolved();

            var node = new UseNumberNode(action) { Name = nodeName };
            return this.AttachOrAddSplitWay(node, nodeName);
        }

        public INodeSyntax UseString(Action<Token, ParsingContext> action, string nodeName = null)
        {
            this.CheckNotResolved();

            var node = new UseStringNode(action) { Name = nodeName };
            return this.AttachOrAddSplitWay(node, nodeName);
        }

        public INodeSyntax SkipWord(string word, bool caseSensitive = false, string nodeName = null)
        {
            this.CheckNotResolved();

            var node = new SkipWordNode(word, caseSensitive) { Name = nodeName };
            return this.AttachOrAddSplitWay(node, nodeName);
        }

        public INodeSyntax SkipSymbol(string symbol, string nodeName = null)
        {
            this.CheckNotResolved();

            var node = new SkipSymbolNode(ParsingExtensions.ParseSymbol(symbol)) { Name = nodeName };
            return this.AttachOrAddSplitWay(node, nodeName);
        }

        public INodeSyntax Milestone(Action<Token, ParsingContext> action, string nodeName = null)
        {
            this.CheckNotResolved();

            var node = new MilestoneNode(action) { Name = nodeName };
            return this.AttachOrAddSplitWay(node, nodeName);
        }

        public INodeSyntax Idle(string nodeName = null)
        {
            this.CheckNotResolved();

            var node = new IdleNode { Name = nodeName };
            return this.AttachOrAddSplitWay(node, nodeName);
        }

        public INodeSyntax Custom(Action<Token, ParsingContext> action, Func<Token, bool> predicate, string nodeName = null)
        {
            this.CheckNotResolved();

            var node = new CustomNode(action, predicate) { Name = nodeName };
            return this.AttachOrAddSplitWay(node, nodeName);
        }

        public INodeSyntax End(Action<Token, ParsingContext> action, string nodeName = null)
        {
            this.CheckNotResolved();

            var node = new EndNode(action) { Name = nodeName };
            return this.AttachOrAddSplitWay(node, nodeName);
        }

        public INodeSyntax Split(string splitNodeName)
        {
            this.CheckNotResolved();

            if (splitNodeName == null)
            {
                throw new ArgumentNullException(nameof(splitNodeName));
            }

            var splitter = new SplitNode() { Name = splitNodeName };
            return this.AttachOrAddSplitWay(splitter, splitNodeName);
        }

        public INodeSyntax GetNode(string nodeName)
        {
            if (nodeName == null)
            {
                throw new ArgumentNullException(nameof(nodeName));
            }

            var node = _namedNodes[nodeName];
            return new NodeSyntax(_rootSyntax, node);
        }

        public INodeSyntax GetSplitter(string splitNodeName)
        {
            this.CheckNotResolved();

            if (splitNodeName == null)
            {
                throw new ArgumentNullException(nameof(splitNodeName));
            }

            var splitter = _namedNodes[splitNodeName];
            var isSplitNode = splitter is SplitNode;
            if (!isSplitNode)
            {
                throw UtilsHelper.CreateInternalSyntaxAnalyzerErrorException();
            }

            return new NodeSyntax(_rootSyntax, splitter);
        }

        #endregion
    }
}
