using TauCode.Db.Utils.Dialects;
using TauCode.Db.Utils.Parsing.Core.Fluent;
using TauCode.Db.Utils.Parsing.Core.Fluent.Impl;

namespace TauCode.Db.Utils.Parsing.Core.Gallery
{
    public class PreciseNumberTypeBuilder : ParsingBlockBuilder
    {
        public PreciseNumberTypeBuilder(IDialect dialect)
        {
            this.Dialect = dialect;
        }

        protected IDialect Dialect { get; }

        protected virtual bool IsPreciseNumberTypeNameToken(Token token)
        {
            var identifier = token.GetTokenIdentifier();
            return this.Dialect.IsDialectPreciseNumberTypeName(identifier);
        }

        protected override INodeSyntax BuildSyntaxImpl()
        {
            var syntax = new NodeSyntax();

            syntax
                .Custom(
                    (token, context) => context.GetColumn().Name = token.GetTokenIdentifier(),
                    this.IsPreciseNumberTypeNameToken,
                    "precisenumber_type_name")
                .Split("after_name")
                    .Milestone(ParsingNode.IdleAction, "end")
                .GetSplitter("after_name")
                    .SkipSymbol("(")
                    .UseNumber((token, context) =>
                    {
                        var column = context.GetColumn();
                        column.Type.Size = null;
                        column.Type.Precision = token.GetTokenInt32();
                    })
                    .Split("after_precision")
                        .SkipSymbol(")", nodeName: "closing_bracket")
                        .LinkTo("end")
                    .GetSplitter("after_precision")
                        .SkipSymbol(",")
                        .UseNumber((token, context) =>
                        {
                            var column = context.GetColumn();
                            column.Type.Scale = token.GetTokenInt32();
                        })
                        .LinkTo("closing_bracket");

            return syntax;
        }
    }
}
