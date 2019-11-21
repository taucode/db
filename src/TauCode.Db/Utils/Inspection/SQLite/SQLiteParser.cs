using System.Linq;
using TauCode.Db.Model;
using TauCode.Parsing;
using TauCode.Parsing.Aide;
using TauCode.Parsing.Aide.Building;
using TauCode.Parsing.Aide.Results;
using TauCode.Parsing.Nodes;
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
            var input = this.GetType().Assembly.GetResourceText("sql-sqlite-grammar.txt", true);
            var aideLexer = new AideLexer();
            IParser parser = new Parser();
            var tokens = aideLexer.Lexize(input);
            var aideRoot = AideHelper.BuildParserRoot();
            var results = parser.Parse(aideRoot, tokens);

            IBuilder builder = new Builder();

            var sqlRoot = builder.Build("SQLite", results.Cast<BlockDefinitionResult>());
            var allSqlNodes = sqlRoot.FetchTree();

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
                .Where(x => x is WordNode wordNode && x.Name.EndsWith("_name_word"))
                .Cast<WordNode>()
                .ToList();

            #region assign job to nodes

            // table
            var createTable = (ActionNode)allSqlNodes.Single(x => x.Name == "do_create_table");
            createTable.Action = (token, accumulator) =>
            {
                var tableMold = new TableMold();
                accumulator.AddResult(tableMold);
            };

            var tableName = (ActionNode)allSqlNodes.Single(x => x.Name == "table_name");
            tableName.Action = (token, accumulator) =>
            {
                var tableMold = accumulator.GetLastResult<TableMold>();
                tableMold.Name = ((IdentifierToken)token).Identifier;
            };

            var tableNameWord = (ActionNode)allSqlNodes.Single(x => x.Name == "table_name_word");
            tableNameWord.Action = (token, accumulator) =>
            {
                var tableMold = accumulator.GetLastResult<TableMold>();
                tableMold.Name = ((WordToken)token).Word;
            };

            var columnName = (ActionNode)allSqlNodes.Single(x => x.Name == "column_name");
            columnName.Action = (token, accumulator) =>
            {
                var tableMold = accumulator.GetLastResult<TableMold>();
                var columnMold = new ColumnMold
                {
                    Name = ((IdentifierToken)token).Identifier,
                };
                tableMold.Columns.Add(columnMold);
            };

            var columnNameWord = (ActionNode)allSqlNodes.Single(x => x.Name == "column_name_word");
            columnNameWord.Action = (token, accumulator) =>
            {
                var tableMold = accumulator.GetLastResult<TableMold>();
                var columnMold = new ColumnMold
                {
                    Name = ((WordToken)token).Word,
                };
                tableMold.Columns.Add(columnMold);
            };

            var typeName = (ActionNode)allSqlNodes.Single(x => x.Name == "type_name");
            typeName.Action = (token, accumulator) =>
            {
                var tableMold = accumulator.GetLastResult<TableMold>();
                var columnMold = tableMold.Columns.Last();
                columnMold.Type.Name = ((IdentifierToken)token).Identifier;
            };

            var columnTypeWord = (ActionNode)allSqlNodes.Single(x => x.Name == "type_name_word");
            columnTypeWord.Action = (token, accumulator) =>
            {
                var tableMold = accumulator.GetLastResult<TableMold>();
                var columnMold = tableMold.Columns.Last();
                columnMold.Type.Name = ((WordToken)token).Word;
            };

            var precision = (ActionNode)allSqlNodes.Single(x => x.Name == "precision");
            precision.Action = (token, accumulator) =>
            {
                var tableMold = accumulator.GetLastResult<TableMold>();
                var columnMold = tableMold.Columns.Last();
                columnMold.Type.Precision = ((IntegerToken)token).IntegerValue.ToInt32();
            };

            var scale = (ActionNode)allSqlNodes.Single(x => x.Name == "scale");
            scale.Action = (token, accumulator) =>
            {
                var tableMold = accumulator.GetLastResult<TableMold>();
                var columnMold = tableMold.Columns.Last();
                columnMold.Type.Scale = ((IntegerToken)token).IntegerValue.ToInt32();
            };

            var nullToken = (ActionNode)allSqlNodes.Single(x => x.Name == "null");
            nullToken.Action = (token, accumulator) =>
            {
                var tableMold = accumulator.GetLastResult<TableMold>();
                var columnMold = tableMold.Columns.Last();
                columnMold.IsNullable = true;
            };

            var notNullToken = (ActionNode)allSqlNodes.Single(x => x.Name == "not_null");
            notNullToken.Action = (token, accumulator) =>
            {
                var tableMold = accumulator.GetLastResult<TableMold>();
                var columnMold = tableMold.Columns.Last();
                columnMold.IsNullable = false;
            };

            var inlinePrimaryKey = (ActionNode)allSqlNodes.Single(x => x.Name == "inline_primary_key");
            inlinePrimaryKey.Action = (token, accumulator) =>
            {
                var tableMold = accumulator.GetLastResult<TableMold>();
                var columnMold = tableMold.Columns.Last();
                columnMold.SetIsPrimaryKey(true);
            };

            var autoincrement = (ActionNode)allSqlNodes.Single(x => x.Name == "autoincrement");
            autoincrement.Action = (token, accumulator) =>
            {
                var tableMold = accumulator.GetLastResult<TableMold>();
                var columnMold = tableMold.Columns.Last();
                columnMold.SetIsAutoIncrement(true);
            };

            var defaultNull = (ActionNode)allSqlNodes.Single(x => x.Name == "default_null");
            defaultNull.Action = (token, accumulator) =>
            {
                var tableMold = accumulator.GetLastResult<TableMold>();
                var columnMold = tableMold.Columns.Last();
                columnMold.Default = "NULL";
            };

            var defaultInteger = (ActionNode)allSqlNodes.Single(x => x.Name == "default_integer");
            defaultInteger.Action = (token, accumulator) =>
            {
                var tableMold = accumulator.GetLastResult<TableMold>();
                var columnMold = tableMold.Columns.Last();
                columnMold.Default = ((IntegerToken)token).IntegerValue;
            };

            var defaultString = (ActionNode)allSqlNodes.Single(x => x.Name == "default_string");
            defaultString.Action = (token, accumulator) =>
            {
                var tableMold = accumulator.GetLastResult<TableMold>();
                var columnMold = tableMold.Columns.Last();
                columnMold.Default = $"'{((StringToken)token).String}'";
            };

            var constraintName = (ActionNode)allSqlNodes.Single(x => x.Name == "constraint_name");
            constraintName.Action = (token, accumulator) =>
            {
                var tableMold = accumulator.GetLastResult<TableMold>();
                tableMold.SetLastConstraintName(((IdentifierToken)token).Identifier);
            };

            var constraintNameWord = (ActionNode)allSqlNodes.Single(x => x.Name == "constraint_name_word");
            constraintNameWord.Action = (token, accumulator) =>
            {
                var tableMold = accumulator.GetLastResult<TableMold>();
                tableMold.SetLastConstraintName(((WordToken)token).Word);
            };

            var pk = (ActionNode)allSqlNodes.Single(x => x.Name == "do_primary_key");
            pk.Action = (token, accumulator) =>
            {
                var tableMold = accumulator.GetLastResult<TableMold>();
                tableMold.PrimaryKey = new PrimaryKeyMold
                {
                    Name = tableMold.GetLastConstraintName(),
                };
            };

            var pkColumnName = (ActionNode)allSqlNodes.Single(x => x.Name == "pk_column_name");
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

            var pkColumnNameWord = (ActionNode)allSqlNodes.Single(x => x.Name == "pk_column_name_word");
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

            var pkColumnAsc = (ActionNode)allSqlNodes.Single(x => x.Name == "asc");
            pkColumnAsc.Action = (token, accumulator) =>
            {
                var tableMold = accumulator.GetLastResult<TableMold>();
                var primaryKey = tableMold.PrimaryKey;
                var indexColumn = primaryKey.Columns.Last();
                indexColumn.SortDirection = SortDirection.Ascending;
            };

            var pkColumnDesc = (ActionNode)allSqlNodes.Single(x => x.Name == "desc");
            pkColumnDesc.Action = (token, accumulator) =>
            {
                var tableMold = accumulator.GetLastResult<TableMold>();
                var primaryKey = tableMold.PrimaryKey;
                var indexColumn = primaryKey.Columns.Last();
                indexColumn.SortDirection = SortDirection.Descending;
            };

            var fk = (ActionNode)allSqlNodes.Single(x => x.Name == "do_foreign_key");
            fk.Action = (token, accumulator) =>
            {
                var tableMold = accumulator.GetLastResult<TableMold>();
                var foreignKey = new ForeignKeyMold
                {
                    Name = tableMold.GetLastConstraintName(),
                };
                tableMold.ForeignKeys.Add(foreignKey);
            };

            var fkTableName = (ActionNode)allSqlNodes.Single(x => x.Name == "fk_referenced_table_name");
            fkTableName.Action = (token, accumulator) =>
            {
                var tableMold = accumulator.GetLastResult<TableMold>();
                var foreignKey = tableMold.ForeignKeys.Last();
                var foreignKeyTableName = ((IdentifierToken)token).Identifier;
                foreignKey.ReferencedTableName = foreignKeyTableName;
            };

            var fkTableNameWord = (ActionNode)allSqlNodes.Single(x => x.Name == "fk_referenced_table_name_word");
            fkTableNameWord.Action = (token, accumulator) =>
            {
                var tableMold = accumulator.GetLastResult<TableMold>();
                var foreignKey = tableMold.ForeignKeys.Last();
                var foreignKeyTableName = ((WordToken)token).Word;
                foreignKey.ReferencedTableName = foreignKeyTableName;
            };

            var fkColumnName = (ActionNode)allSqlNodes.Single(x => x.Name == "fk_column_name");
            fkColumnName.Action = (token, accumulator) =>
            {
                var tableMold = accumulator.GetLastResult<TableMold>();
                var foreignKey = tableMold.ForeignKeys.Last();
                var foreignKeyColumnName = ((IdentifierToken)token).Identifier;
                foreignKey.ColumnNames.Add(foreignKeyColumnName);
            };

            var fkColumnNameWord = (ActionNode)allSqlNodes.Single(x => x.Name == "fk_column_name_word");
            fkColumnNameWord.Action = (token, accumulator) =>
            {
                var tableMold = accumulator.GetLastResult<TableMold>();
                var foreignKey = tableMold.ForeignKeys.Last();
                var foreignKeyColumnName = ((WordToken)token).Word;
                foreignKey.ColumnNames.Add(foreignKeyColumnName);
            };

            var fkReferencedColumnName = (ActionNode)allSqlNodes.Single(x => x.Name == "fk_referenced_column_name");
            fkReferencedColumnName.Action = (token, accumulator) =>
            {
                var tableMold = accumulator.GetLastResult<TableMold>();
                var foreignKey = tableMold.ForeignKeys.Last();
                var foreignKeyReferencedColumnName = ((IdentifierToken)token).Identifier;
                foreignKey.ReferencedColumnNames.Add(foreignKeyReferencedColumnName);
            };

            var fkReferencedColumnNameWord = (ActionNode)allSqlNodes.Single(x => x.Name == "fk_referenced_column_name_word");
            fkReferencedColumnNameWord.Action = (token, accumulator) =>
            {
                var tableMold = accumulator.GetLastResult<TableMold>();
                var foreignKey = tableMold.ForeignKeys.Last();
                var foreignKeyReferencedColumnName = ((WordToken)token).Word;
                foreignKey.ReferencedColumnNames.Add(foreignKeyReferencedColumnName);
            };

            // index
            var createUniqueIndex = (ActionNode)allSqlNodes.Single(x => x.Name == "do_create_unique_index");
            createUniqueIndex.Action = (token, accumulator) =>
            {
                var index = new IndexMold
                {
                    IsUnique = true,
                };
                index.SetIsCreationFinalized(false);
                accumulator.AddResult(index);
            };

            var createIndex = (ActionNode)allSqlNodes.Single(x => x.Name == "do_create_index");
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

            var indexName = (ActionNode)allSqlNodes.Single(x => x.Name == "index_name");
            indexName.Action = (token, accumulator) =>
            {
                var index = accumulator.GetLastResult<IndexMold>();
                index.Name = ((IdentifierToken)token).Identifier;
            };

            var indexNameWord = (ActionNode)allSqlNodes.Single(x => x.Name == "index_name_word");
            indexNameWord.Action = (token, accumulator) =>
            {
                var index = accumulator.GetLastResult<IndexMold>();
                index.Name = ((WordToken)token).Word;
            };

            var indexTableName = (ActionNode)allSqlNodes.Single(x => x.Name == "index_table_name");
            indexTableName.Action = (token, accumulator) =>
            {
                var index = accumulator.GetLastResult<IndexMold>();
                index.TableName = ((IdentifierToken)token).Identifier;
            };

            var indexTableNameWord = (ActionNode)allSqlNodes.Single(x => x.Name == "index_table_name_word");
            indexTableNameWord.Action = (token, accumulator) =>
            {
                var index = accumulator.GetLastResult<IndexMold>();
                index.TableName = ((WordToken)token).Word;
            };

            var indexColumnName = (ActionNode)allSqlNodes.Single(x => x.Name == "index_column_name");
            indexColumnName.Action = (token, accumulator) =>
            {
                var index = accumulator.GetLastResult<IndexMold>();
                var columnMold = new IndexColumnMold
                {
                    Name = ((IdentifierToken)token).Identifier,
                };
                index.Columns.Add(columnMold);
            };

            var indexColumnNameWord = (ActionNode)allSqlNodes.Single(x => x.Name == "index_column_name_word");
            indexColumnNameWord.Action = (token, accumulator) =>
            {
                var index = accumulator.GetLastResult<IndexMold>();
                var columnMold = new IndexColumnMold
                {
                    Name = ((WordToken)token).Word,
                };
                index.Columns.Add(columnMold);
            };

            var indexColumnAsc = (ActionNode)allSqlNodes.Single(x => x.Name == "index_column_asc");
            indexColumnAsc.Action = (token, accumulator) =>
            {
                var index = accumulator.GetLastResult<IndexMold>();
                var columnMold = index.Columns.Last();
                columnMold.SortDirection = SortDirection.Ascending;
            };

            var indexColumnDesc = (ActionNode)allSqlNodes.Single(x => x.Name == "index_column_desc");
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

            return sqlRoot;
        }

        public object[] Parse(string sql)
        {
            ILexer lexer = new SQLiteLexer();
            var tokens = lexer.Lexize(sql);

            var results = _parser.Parse(_root, tokens);

            foreach (var result in results)
            {
                if (result is TableMold tableMold)
                {
                    tableMold.InitPrimaryKey();
                }
            }

            return results;
        }
    }
}
