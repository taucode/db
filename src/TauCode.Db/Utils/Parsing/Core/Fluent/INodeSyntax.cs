using System;

namespace TauCode.Db.Utils.Parsing.Core.Fluent
{
    public interface INodeSyntax
    {
        ParsingNode Root { get; }

        ParsingNode Node { get; }

        INodeSyntax Attach(ParsingNode node, string nodeName = null);

        INodeSyntax Use(ParsingNode node, string nodeName = null);

        INodeSyntax Separate(string nodeName = null);

        INodeSyntax LinkTo(string linkToNodeName);

        INodeSyntax UseWord(string word, Action<Token, ParsingContext> action, bool caseSensitive = false, string nodeName = null);

        INodeSyntax UseSymbol(string symbol, Action<Token, ParsingContext> action, string nodeName = null);

        INodeSyntax UseIdentifier(Action<Token, ParsingContext> action, string nodeName = null);

        INodeSyntax UseNumber(Action<Token, ParsingContext> action, string nodeName = null);

        INodeSyntax UseString(Action<Token, ParsingContext> action, string nodeName = null);

        INodeSyntax SkipWord(string word, bool caseSensitive = false, string nodeName = null);

        INodeSyntax SkipSymbol(string symbol, string nodeName = null);

        INodeSyntax Milestone(Action<Token, ParsingContext> action, string nodeName = null);

        INodeSyntax Idle(string nodeName = null);

        INodeSyntax Custom(Action<Token, ParsingContext> action, Func<Token, bool> predicate, string nodeName = null);

        INodeSyntax End(Action<Token, ParsingContext> action, string nodeName = null);

        INodeSyntax Split(string splitNodeName);

        INodeSyntax GetNode(string nodeName);

        INodeSyntax GetSplitter(string splitNodeName);
    }
}
