using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Marine.Database
{
    public interface IDatabase
    {
        Dictionary<string, string> GetCurConnectionInfo();
        string GetCurUserName();
        /// <summary>
        /// 获取该数据库所有的用户名称
        /// </summary>
        /// <returns></returns>
        List<string> GetDBUsers();
        List<string> GetDBTables();
        List<string> GetDBTables(string userName);
        List<string> GetDBSpaces();
        List<string> GetDBSpaces(string userName);
        List<ColumnInfo> GetTableColumns(string tableName);
    }

    public struct ColumnInfo
    {
        //列名
        string ColumnName;
        //列的描述信息
        string ColumnDescription;
        //是否允许为空值
        bool AllowNull;
        //列的数据类型
        DbType ColumnType;
    }
}
