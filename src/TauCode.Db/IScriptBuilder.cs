﻿//namespace TauCode.Db
//{
//    public interface IScriptBuilder : IUtility
//    {
//        //IDialect Dialect { get; }

//        //char? CurrentOpeningIdentifierDelimiter { get; set; }

//        //bool AddClauseTerminator { get; set; }

//        //string BuildCreateColumnSql(ColumnMold column);

//        //string BuildPrimaryKeySql(string tableName, PrimaryKeyMold primaryKey);

//        //string BuildForeignKeySql(string tableName, ForeignKeyMold foreignKey);

//        //string BuildIndexSql(string tableName, IndexMold index);

//        //string BuildCreateTableSql(TableMold table, bool inline);

//        ////string BuildCreateDbSql(IDbConnection connection, bool addComments);

//        //string BuildSelectSql(string tableName, string[] columnNames);

//        //string BuildSelectSql(TableMold table);

//        //string BuildSelectRowByIdSql(TableMold table, out string idParamName);

//        //string BuildInsertSql(TableMold table, IDictionary<string, object> columnValues, int? indent = null);

//        //string BuildParameterizedInsertSql(
//        //    TableMold table,
//        //    out IDictionary<string, string> parameterMapping,
//        //    Func<string, bool> columnSelector = null,
//        //    //string[] columnsToInclude = null,
//        //    //string[] columnsToExclude = null,
//        //    int? indent = null);

//        //string BuildDeleteSql(string tableName);

//        //string BuildDeleteRowByIdSql(string tableName, string primaryKeyColumnName, out string paramName);

//        //string BuildUpdateRowByIdSql(
//        //    string tableName,
//        //    string primaryKeyColumnName,
//        //    string[] columnNames,
//        //    out Dictionary<string, string> parameterMapping);

//        //string BuildFillTableSql(IDbConnection connection, TableMold table);

//        ////string BuildFillDbSql(
//        ////    IDbConnection connection,
//        ////    Func<string, bool> tableSelector = null,
//        ////    //string[] tableNamesToInclude = null,
//        ////    //string[] tableNamesToExclude = null,
//        ////    bool deleteExistingData = false);

//        ////string BuildClearDbSql(
//        ////    IDbConnection connection,
//        ////    Func<string, bool> tableSelector = null,
//        ////    string[] tableNamesToInclude = null,
//        ////    string[] tableNamesToExclude = null);

//        //string BuildDropTableSql(string tableName);

//        ////string BuildDropAllTablesSql(IDbConnection connection, bool addComments);
//    }
//}
// todo clean up