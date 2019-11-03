using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TauCode.Db.Utils.Parsing.Core.Nodes
{
    public sealed class SplitNode : ParsingNode
    {
        private class ConcurrentPath
        {
            internal ConcurrentPath(ParsingNode node, int forwardShift)
            {
                this.CurrentNode = node;
                this.ForwardShift = forwardShift;
            }

            internal ParsingNode CurrentNode { get; set; }
            internal int ForwardShift { get; set; }
        }

        private readonly List<ParsingNode> _ways;

        public SplitNode()
            : base(ParsingNode.IdleAction)
        {
            _ways = new List<ParsingNode>();
        }

        public void AddWay(ParsingNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            if (_ways.Contains(node))
            {
                throw UtilsHelper.CreateInternalSyntaxAnalyzerErrorException();
            }

            _ways.Add(node);
        }

        public IReadOnlyList<ParsingNode> Ways => _ways;

        public override AcceptResult Accepts(ParsingFlow flow, int forwardShift)
        {
            if (this.Ways.Count == 0)
            {
                throw UtilsHelper.CreateInternalSyntaxAnalyzerErrorException(); // undecidable
            }

            var competitors = Enumerable
                .Range(0, this.Ways.Count)
                .ToDictionary(x => x, x => new ConcurrentPath(this.Ways[x], forwardShift));

            Trace.TraceInformation($"Competitors: {this.Ways.Count}");
            foreach (var parsingNode in this.Ways)
            {
                Trace.TraceInformation($"{parsingNode.DebugName}, {parsingNode.GetType().Name}");
            }

            var outsiderIndexes = new List<int>();

            while (true)
            {
                outsiderIndexes.Clear();

                foreach (var pair in competitors)
                {
                    var index = pair.Key;
                    var path = pair.Value;

                    while (true)
                    {
                        // cycle until got progress, or lose the competition
                        if (path.CurrentNode is EndNode)
                        {
                            // path which contains end node - wins.
                            return new AcceptResult(this.Ways[index], false);
                        }

                        var acceptResult = path.CurrentNode.Accepts(flow, path.ForwardShift);
                        if (acceptResult == null)
                        {
                            // loser
                            outsiderIndexes.Add(index);
                            break;
                        }
                        else
                        {
                            if (path.CurrentNode == acceptResult.NextNode)
                            {
                                // node has not moved, that's an error.
                                throw UtilsHelper.CreateInternalSyntaxAnalyzerErrorException();
                            }

                            path.CurrentNode = acceptResult.NextNode;
                            if (acceptResult.TokenIsConsumed)
                            {
                                path.ForwardShift++; // got progress
                                break;
                            }
                        }
                    }
                }

                foreach (var outsider in outsiderIndexes)
                {
                    competitors.Remove(outsider);
                }

                if (competitors.Count == 0)
                {
                    return null;
                }
                else if (competitors.Count == 1)
                {
                    var winnerIndex = competitors.Single().Key;
                    return new AcceptResult(this.Ways[winnerIndex], false);
                }
            }
        }

        public void RedirectWay(int index, ParsingNode newWayNode)
        {
            if (newWayNode == null)
            {
                throw new ArgumentNullException(nameof(newWayNode));
            }

            if (_ways.Contains(newWayNode))
            {
                throw UtilsHelper.CreateInternalSyntaxAnalyzerErrorException();
            }

            _ways[index] = newWayNode;
        }
    }
}
