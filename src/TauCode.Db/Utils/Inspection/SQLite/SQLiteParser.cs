using System;
using System.Linq;
using TauCode.Db.Model;
using TauCode.Parsing;
using TauCode.Parsing.Building;
using TauCode.Parsing.Lexing;
using TauCode.Parsing.Nodes;
using TauCode.Parsing.TinyLisp;
using TauCode.Parsing.Tokens;
using TauCode.Utils.Extensions;

namespace TauCode.Db.Utils.Inspection.SQLite
{
    public class SQLiteParser
    {
        public static SQLiteParser Instance { get; } = new SQLiteParser();

        private readonly INode _root;
        private readonly IParser _parser;

        private SQLiteParser()
        {
            _root = this.BuildRoot();
            _parser = new Parser();
        }

        private INode BuildRoot()
        {
            var nodeFactory = new SQLiteNodeFactory();
            var input = this.GetType().Assembly.GetResourceText("sql-sqlite-grammar.lisp", true);
            ILexer lexer = new TinyLispLexer();
            var tokens = lexer.Lexize(input);

            var reader = new TinyLispPseudoReader();
            var list = reader.Read(tokens);

            IBuilder builder = new Builder();
            var root = builder.Build(nodeFactory, list);

            this.ChargeRoot(root);

            return root;
        }

        private void ChargeRoot(INode root)
        {
            var allSqlNodes = root.FetchTree();

            var exactWordNodes = allSqlNodes
                .Where(x => x is ExactWordNode)
                .Cast<ExactWordNode>()
                .ToList();

            foreach (var exactWordNode in exactWordNodes)
            {
                exactWordNode.IsCaseSensitive = false;
            }

            var reservedWords = exactWordNodes
                .Select(x => x.Word)
                .Distinct()
                .Select(x => x.ToUpperInvariant())
                .ToMyHashSet();

            var identifiersAsWords = allSqlNodes
                .Where(x =>
                    x is WordNode wordNode &&
                    x.Name.EndsWith("-name-word", StringComparison.InvariantCultureIgnoreCase))
                .Cast<WordNode>()
                .ToList();

            #region assign job to nodes

            // table
            var createTable = (ActionNode)allSqlNodes.Single(x =>
               string.Equals(x.Name, "do-create-table", StringComparison.InvariantCultureIgnoreCase));
            createTable.Action = (token, accumulator) =>
            {
                var tableMold = new TableMold();
                accumulator.AddResult(tableMold);
            };

            var tableName = (ActionNode)allSqlNodes.Single(x =>
               string.Equals(x.Name, "table-name-ident", StringComparison.InvariantCultureIgnoreCase));
            tableName.Action = (token, accumulator) =>
            {
                var tableMold = accumulator.GetLastResult<TableMold>();
                tableMold.Name = ((IdentifierToken)token).Identifier;
            };

            var tableNameWord = (ActionNode)allSqlNodes.Single(x =>
               string.Equals(x.Name, "table-name-word", StringComparison.InvariantCultureIgnoreCase));
            tableNameWord.Action = (token, accumulator) =>
            {
                var tableMold = accumulator.GetLastResult<TableMold>();
                tableMold.Name = ((WordToken)token).Word;
            };

            var columnName = (ActionNode)allSqlNodes.Single(x =>
               string.Equals(x.Name, "column-name-ident", StringComparison.InvariantCultureIgnoreCase));
            columnName.Action = (token, accumulator) =>
            {
                var tableMold = accumulator.GetLastResult<TableMold>();
                var columnMold = new ColumnMold
                {
                    Name = ((IdentifierToken)token).Identifier,
                };
                tableMold.Columns.Add(columnMold);
            };

            var columnNameWord = (ActionNode)allSqlNodes.Single(x =>
               string.Equals(x.Name, "column-name-word", StringComparison.InvariantCultureIgnoreCase));
            columnNameWord.Action = (token, accumulator) =>
            {
                var tableMold = accumulator.GetLastResult<TableMold>();
                var columnMold = new ColumnMold
                {
                    Name = ((WordToken)token).Word,
                };
                tableMold.Columns.Add(columnMold);
            };

            var typeName = (ActionNode)allSqlNodes.Single(x =>
               string.Equals(x.Name, "type-name-ident", StringComparison.InvariantCultureIgnoreCase));
            typeName.Action = (token, accumulator) =>
            {
                var tableMold = accumulator.GetLastResult<TableMold>();
                var columnMold = tableMold.Columns.Last();
                columnMold.Type.Name = ((IdentifierToken)token).Identifier;
            };

            var columnTypeWord = (ActionNode)allSqlNodes.Single(x =>
               string.Equals(x.Name, "type-name-word", StringComparison.InvariantCultureIgnoreCase));
            columnTypeWord.Action = (token, accumulator) =>
            {
                var tableMold = accumulator.GetLastResult<TableMold>();
                var columnMold = tableMold.Columns.Last();
                columnMold.Type.Name = ((WordToken)token).Word;
            };

            var precision = (ActionNode)allSqlNodes.Single(x =>
               string.Equals(x.Name, "precision", StringComparison.InvariantCultureIgnoreCase));
            precision.Action = (token, accumulator) =>
            {
                var tableMold = accumulator.GetLastResult<TableMold>();
                var columnMold = tableMold.Columns.Last();
                columnMold.Type.Precision = ((IntegerToken)token).Value.ToInt32();
            };

            var scale = (ActionNode)allSqlNodes.Single(x =>
               string.Equals(x.Name, "scale", StringComparison.InvariantCultureIgnoreCase));
            scale.Action = (token, accumulator) =>
            {
                var tableMold = accumulator.GetLastResult<TableMold>();
                var columnMold = tableMold.Columns.Last();
                columnMold.Type.Scale = ((IntegerToken)token).Value.ToInt32();
            };

            var nullToken = (ActionNode)allSqlNodes.Single(x =>
               string.Equals(x.Name, "null", StringComparison.InvariantCultureIgnoreCase));
            nullToken.Action = (token, accumulator) =>
            {
                var tableMold = accumulator.GetLastResult<TableMold>();
                var columnMold = tableMold.Columns.Last();
                columnMold.IsNullable = true;
            };

            var notNullToken = (ActionNode)allSqlNodes.Single(x =>
               string.Equals(x.Name, "not-null", StringComparison.InvariantCultureIgnoreCase));
            notNullToken.Action = (token, accumulator) =>
            {
                var tableMold = accumulator.GetLastResult<TableMold>();
                var columnMold = tableMold.Columns.Last();
                columnMold.IsNullable = false;
            };

            var inlinePrimaryKey = (ActionNode)allSqlNodes.Single(x =>
                string.Equals(x.Name, "inline-primary-key", StringComparison.InvariantCultureIgnoreCase));

            inlinePrimaryKey.Action = (token, accumulator) =>
            {
                var tableMold = accumulator.GetLastResult<TableMold>();
                var columnMold = tableMold.Columns.Last();
                columnMold.MarkAsExplicitPrimaryKey();
            };

            var autoincrement = (ActionNode)allSqlNodes.Single(x =>
                string.Equals(x.Name, "autoincrement", StringComparison.InvariantCultureIgnoreCase));
            autoincrement.Action = (token, accumulator) =>
            {
                var tableMold = accumulator.GetLastResult<TableMold>();
                var columnMold = tableMold.Columns.Last();
                columnMold.Identity = new ColumnIdentityMold();
            };

            var defaultNull = (ActionNode)allSqlNodes.Single(x =>
                string.Equals(x.Name, "default-null", StringComparison.InvariantCultureIgnoreCase));
            defaultNull.Action = (token, accumulator) =>
            {
                var tableMold = accumulator.GetLastResult<TableMold>();
                var columnMold = tableMold.Columns.Last();
                columnMold.Default = "NULL";
            };

            var defaultInteger = (ActionNode)allSqlNodes.Single(x =>
                string.Equals(x.Name, "default-integer", StringComparison.InvariantCultureIgnoreCase));
            defaultInteger.Action = (token, accumulator) =>
            {
                var tableMold = accumulator.GetLastResult<TableMold>();
                var columnMold = tableMold.Columns.Last();
                columnMold.Default = ((IntegerToken)token).Value;
            };

            var defaultString = (ActionNode)allSqlNodes.Single(x =>
                string.Equals(x.Name, "default-string", StringComparison.InvariantCultureIgnoreCase));
            defaultString.Action = (token, accumulator) =>
            {
                var tableMold = accumulator.GetLastResult<TableMold>();
                var columnMold = tableMold.Columns.Last();
                columnMold.Default = $"'{((StringToken)token).Value}'";
            };

            var constraintName = (ActionNode)allSqlNodes.Single(x =>
               string.Equals(x.Name, "constraint-name-ident", StringComparison.InvariantCultureIgnoreCase));
            constraintName.Action = (token, accumulator) =>
            {
                var tableMold = accumulator.GetLastResult<TableMold>();
                tableMold.SetLastConstraintName(((IdentifierToken)token).Identifier);
            };

            var constraintNameWord = (ActionNode)allSqlNodes.Single(x =>
               string.Equals(x.Name, "constraint-name-word", StringComparison.InvariantCultureIgnoreCase));
            constraintNameWord.Action = (token, accumulator) =>
            {
                var tableMold = accumulator.GetLastResult<TableMold>();
                tableMold.SetLastConstraintName(((WordToken)token).Word);
            };

            var pk = (ActionNode)allSqlNodes.Single(x =>
               string.Equals(x.Name, "do-primary-key", StringComparison.InvariantCultureIgnoreCase));
            pk.Action = (token, accumulator) =>
            {
                var tableMold = accumulator.GetLastResult<TableMold>();
                tableMold.PrimaryKey = new PrimaryKeyMold
                {
                    Name = tableMold.GetLastConstraintName(),
                };
            };

            var pkColumnName = (ActionNode)allSqlNodes.Single(x =>
               string.Equals(x.Name, "pk-column-name-ident", StringComparison.InvariantCultureIgnoreCase));
            pkColumnName.Action = (token, accumulator) =>
            {
                var tableMold = accumulator.GetLastResult<TableMold>();
                var primaryKey = tableMold.PrimaryKey;
                var indexColumn = new IndexColumnMold
                {
                    Name = ((IdentifierToken)token).Identifier,
                };
                primaryKey.Columns.Add(indexColumn);
            };

            var pkColumnNameWord = (ActionNode)allSqlNodes.Single(x =>
               string.Equals(x.Name, "pk-column-name-word", StringComparison.InvariantCultureIgnoreCase));
            pkColumnNameWord.Action = (token, accumulator) =>
            {
                var tableMold = accumulator.GetLastResult<TableMold>();
                var primaryKey = tableMold.PrimaryKey;
                var indexColumn = new IndexColumnMold
                {
                    Name = ((WordToken)token).Word,
                };
                primaryKey.Columns.Add(indexColumn);
            };

            var pkColumnAsc = (ActionNode)allSqlNodes.Single(x =>
               string.Equals(x.Name, "asc", StringComparison.InvariantCultureIgnoreCase));
            pkColumnAsc.Action = (token, accumulator) =>
            {
                var tableMold = accumulator.GetLastResult<TableMold>();
                var primaryKey = tableMold.PrimaryKey;
                var indexColumn = primaryKey.Columns.Last();
                indexColumn.SortDirection = SortDirection.Ascending;
            };

            var pkColumnDesc = (ActionNode)allSqlNodes.Single(x =>
               string.Equals(x.Name, "desc", StringComparison.InvariantCultureIgnoreCase));
            pkColumnDesc.Action = (token, accumulator) =>
            {
                var tableMold = accumulator.GetLastResult<TableMold>();
                var primaryKey = tableMold.PrimaryKey;
                var indexColumn = primaryKey.Columns.Last();
                indexColumn.SortDirection = SortDirection.Descending;
            };

            var fk = (ActionNode)allSqlNodes.Single(x =>
               string.Equals(x.Name, "do-foreign-key", StringComparison.InvariantCultureIgnoreCase));
            fk.Action = (token, accumulator) =>
            {
                var tableMold = accumulator.GetLastResult<TableMold>();
                var foreignKey = new ForeignKeyMold
                {
                    Name = tableMold.GetLastConstraintName(),
                };
                tableMold.ForeignKeys.Add(foreignKey);
            };

            var fkTableName = (ActionNode)allSqlNodes.Single(x =>
               string.Equals(x.Name, "fk-referenced-table-name-ident", StringComparison.InvariantCultureIgnoreCase));
            fkTableName.Action = (token, accumulator) =>
            {
                var tableMold = accumulator.GetLastResult<TableMold>();
                var foreignKey = tableMold.ForeignKeys.Last();
                var foreignKeyTableName = ((IdentifierToken)token).Identifier;
                foreignKey.ReferencedTableName = foreignKeyTableName;
            };

            var fkTableNameWord = (ActionNode)allSqlNodes.Single(x =>
               string.Equals(x.Name, "fk-referenced-table-name-word", StringComparison.InvariantCultureIgnoreCase));
            fkTableNameWord.Action = (token, accumulator) =>
            {
                var tableMold = accumulator.GetLastResult<TableMold>();
                var foreignKey = tableMold.ForeignKeys.Last();
                var foreignKeyTableName = ((WordToken)token).Word;
                foreignKey.ReferencedTableName = foreignKeyTableName;
            };

            var fkColumnName = (ActionNode)allSqlNodes.Single(x =>
               string.Equals(x.Name, "fk-column-name-ident", StringComparison.InvariantCultureIgnoreCase));
            fkColumnName.Action = (token, accumulator) =>
            {
                var tableMold = accumulator.GetLastResult<TableMold>();
                var foreignKey = tableMold.ForeignKeys.Last();
                var foreignKeyColumnName = ((IdentifierToken)token).Identifier;
                foreignKey.ColumnNames.Add(foreignKeyColumnName);
            };

            var fkColumnNameWord = (ActionNode)allSqlNodes.Single(x =>
               string.Equals(x.Name, "fk-column-name-word", StringComparison.InvariantCultureIgnoreCase));
            fkColumnNameWord.Action = (token, accumulator) =>
            {
                var tableMold = accumulator.GetLastResult<TableMold>();
                var foreignKey = tableMold.ForeignKeys.Last();
                var foreignKeyColumnName = ((WordToken)token).Word;
                foreignKey.ColumnNames.Add(foreignKeyColumnName);
            };

            var fkReferencedColumnName = (ActionNode)allSqlNodes.Single(x =>
               string.Equals(x.Name, "fk-referenced-column-name-ident", StringComparison.InvariantCultureIgnoreCase));
            fkReferencedColumnName.Action = (token, accumulator) =>
            {
                var tableMold = accumulator.GetLastResult<TableMold>();
                var foreignKey = tableMold.ForeignKeys.Last();
                var foreignKeyReferencedColumnName = ((IdentifierToken)token).Identifier;
                foreignKey.ReferencedColumnNames.Add(foreignKeyReferencedColumnName);
            };

            var fkReferencedColumnNameWord = (ActionNode)allSqlNodes.Single(x =>
               string.Equals(x.Name, "fk-referenced-column-name-word", StringComparison.InvariantCultureIgnoreCase));
            fkReferencedColumnNameWord.Action = (token, accumulator) =>
            {
                var tableMold = accumulator.GetLastResult<TableMold>();
                var foreignKey = tableMold.ForeignKeys.Last();
                var foreignKeyReferencedColumnName = ((WordToken)token).Word;
                foreignKey.ReferencedColumnNames.Add(foreignKeyReferencedColumnName);
            };

            // index
            var createUniqueIndex = (ActionNode)allSqlNodes.Single(x =>
               string.Equals(x.Name, "do-create-unique-index", StringComparison.InvariantCultureIgnoreCase));
            createUniqueIndex.Action = (token, accumulator) =>
            {
                var index = new IndexMold
                {
                    IsUnique = true,
                };
                index.SetIsCreationFinalized(false);
                accumulator.AddResult(index);
            };

            var createIndex = (ActionNode)allSqlNodes.Single(x =>
               string.Equals(x.Name, "do-create-index", StringComparison.InvariantCultureIgnoreCase));
            createIndex.Action = (token, accumulator) =>
            {
                bool brandNewIndex;

                if (accumulator.Count == 0)
                {
                    brandNewIndex = true;
                }
                else
                {
                    var result = accumulator.Last();
                    if (result is IndexMold indexMold)
                    {
                        brandNewIndex = indexMold.GetIsCreationFinalized();
                    }
                    else
                    {
                        brandNewIndex = true;
                    }
                }

                if (brandNewIndex)
                {
                    var newIndex = new IndexMold();
                    newIndex.SetIsCreationFinalized(true);
                    accumulator.AddResult(newIndex);
                }
                else
                {
                    var existingIndexMold = accumulator.GetLastResult<IndexMold>();
                    existingIndexMold.SetIsCreationFinalized(true);
                }
            };

            var indexName = (ActionNode)allSqlNodes.Single(x =>
               string.Equals(x.Name, "index-name-ident", StringComparison.InvariantCultureIgnoreCase));
            indexName.Action = (token, accumulator) =>
            {
                var index = accumulator.GetLastResult<IndexMold>();
                index.Name = ((IdentifierToken)token).Identifier;
            };

            var indexNameWord = (ActionNode)allSqlNodes.Single(x =>
               string.Equals(x.Name, "index-name-word", StringComparison.InvariantCultureIgnoreCase));
            indexNameWord.Action = (token, accumulator) =>
            {
                var index = accumulator.GetLastResult<IndexMold>();
                index.Name = ((WordToken)token).Word;
            };

            var indexTableName = (ActionNode)allSqlNodes.Single(x =>
               string.Equals(x.Name, "index-table-name-ident", StringComparison.InvariantCultureIgnoreCase));
            indexTableName.Action = (token, accumulator) =>
            {
                var index = accumulator.GetLastResult<IndexMold>();
                index.TableName = ((IdentifierToken)token).Identifier;
            };

            var indexTableNameWord = (ActionNode)allSqlNodes.Single(x =>
               string.Equals(x.Name, "index-table-name-word", StringComparison.InvariantCultureIgnoreCase));
            indexTableNameWord.Action = (token, accumulator) =>
            {
                var index = accumulator.GetLastResult<IndexMold>();
                index.TableName = ((WordToken)token).Word;
            };

            var indexColumnName = (ActionNode)allSqlNodes.Single(x =>
               string.Equals(x.Name, "index-column-name-ident", StringComparison.InvariantCultureIgnoreCase));
            indexColumnName.Action = (token, accumulator) =>
            {
                var index = accumulator.GetLastResult<IndexMold>();
                var columnMold = new IndexColumnMold
                {
                    Name = ((IdentifierToken)token).Identifier,
                };
                index.Columns.Add(columnMold);
            };

            var indexColumnNameWord = (ActionNode)allSqlNodes.Single(x =>
               string.Equals(x.Name, "index-column-name-word", StringComparison.InvariantCultureIgnoreCase));
            indexColumnNameWord.Action = (token, accumulator) =>
            {
                var index = accumulator.GetLastResult<IndexMold>();
                var columnMold = new IndexColumnMold
                {
                    Name = ((WordToken)token).Word,
                };
                index.Columns.Add(columnMold);
            };

            var indexColumnAsc = (ActionNode)allSqlNodes.Single(x =>
               string.Equals(x.Name, "index-column-asc", StringComparison.InvariantCultureIgnoreCase));
            indexColumnAsc.Action = (token, accumulator) =>
            {
                var index = accumulator.GetLastResult<IndexMold>();
                var columnMold = index.Columns.Last();
                columnMold.SortDirection = SortDirection.Ascending;
            };

            var indexColumnDesc = (ActionNode)allSqlNodes.Single(x =>
               string.Equals(x.Name, "index-column-desc", StringComparison.InvariantCultureIgnoreCase));
            indexColumnDesc.Action = (token, accumulator) =>
            {
                var index = accumulator.GetLastResult<IndexMold>();
                var columnMold = index.Columns.Last();
                columnMold.SortDirection = SortDirection.Descending;
            };

            #endregion

            foreach (var identifiersAsWord in identifiersAsWords)
            {
                identifiersAsWord.AdditionalChecker = (token, accumulator) =>
                {
                    var wordToken = ((WordToken)token).Word.ToUpperInvariant();
                    return !reservedWords.Contains(wordToken);
                };
            }
        }

        public object[] Parse(string sql)
        {
            ILexer lexer = new SQLiteLexer();
            var tokens = lexer.Lexize(sql);

            var results = _parser.Parse(_root, tokens);
            return results;
        }
    }
}
