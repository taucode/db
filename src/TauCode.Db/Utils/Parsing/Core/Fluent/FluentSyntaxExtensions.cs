namespace TauCode.Db.Utils.Parsing.Core.Fluent
{
    public static class FluentSyntaxExtensions
    {
        public static INodeSyntax StoreInVar(this INodeSyntax nodeSyntax, out INodeSyntax thisNodeSyntax)
        {
            return thisNodeSyntax = nodeSyntax;
        }

        public static INodeSyntax StoreNodeInVar(this INodeSyntax nodeSyntax, out ParsingNode node)
        {
            node = nodeSyntax.Node;
            return nodeSyntax;
        }
    }
}
