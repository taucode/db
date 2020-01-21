using System;
using System.Collections.Generic;
using System.Linq;
using TauCode.Db.Model;
using TauCode.Extensions;
using TauCode.Parsing;
using TauCode.Parsing.Building;
using TauCode.Parsing.Lexing;
using TauCode.Parsing.Nodes;
using TauCode.Parsing.TextClasses;
using TauCode.Parsing.TinyLisp;

namespace TauCode.Db.SQLite.Parsing.TextClasses
{
    [TextClass("identifier")]
    public class SqlIdentifierTextClass : TextClassBase
    {
        public static SqlIdentifierTextClass Instance { get; } = new SqlIdentifierTextClass();

        private SqlIdentifierTextClass()
        {
        }

        protected override string TryConvertFromImpl(string text, ITextClass anotherClass)
        {
            if (
                anotherClass is WordTextClass &&
                !SqlTestsHelper.IsReservedWord(text))
            {
                return text;
            }

            return null;
        }
    }

    // todo: separate file; rename.
    public static class SqlTestsHelper
    {
        private static readonly ILexer TheLexer = new TinyLispLexer();
        private static readonly HashSet<string> ReservedWordsHashSet;

        public static HashSet<string> ReservedWords = ReservedWordsHashSet ?? (ReservedWordsHashSet = CreateReservedWords());

        private static HashSet<string> CreateReservedWords()
        {
            var grammar = typeof(SqlTestsHelper).Assembly.GetResourceText("sql-sqlite-grammar.lisp", true);
            var tokens = TheLexer.Lexize(grammar);

            var reader = new TinyLispPseudoReader();
            var form = reader.Read(tokens);

            var nodeFactory = new SQLiteNodeFactory();
            var builder = new TreeBuilder();
            var root = builder.Build(nodeFactory, form);
            var nodes = root.FetchTree();

            var words = new List<string>();

            words.AddRange(nodes
                .Where(x => x is ExactTextNode)
                .Cast<ExactTextNode>()
                .Select(x => x.ExactText.ToLowerInvariant()));

            words.AddRange(nodes
                .Where(x => x is MultiTextNode)
                .Cast<MultiTextNode>()
                .SelectMany(x => x.Texts.Select(y => y.ToLowerInvariant())));

            return new HashSet<string>(words);
        }

        public static bool IsReservedWord(string text) => ReservedWords.Contains(text.ToLowerInvariant());

        public static SortDirection SqlToSortDirection(string sql)
        {
            // todo  checks
            switch (sql.ToLowerInvariant())
            {
                case "asc":
                    return SortDirection.Ascending;

                case "desc":
                    return SortDirection.Descending;

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
