using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Configuration;

namespace Marine.Database
{
    public static class DatabaseFactory
    {
        private static Dictionary<string, DatabaseObj> DatabaseCollection = new Dictionary<string, DatabaseObj>();

        public static bool Add(string databaseName, DatabaseObj dbObj)
        {
            bool isAdd = false;

            try
            {
                if (DatabaseCollection.ContainsKey(databaseName))
                    return isAdd;
                DatabaseCollection.Add(databaseName, dbObj);
                isAdd = true;
            }
            catch (Exception err)
            {
                throw err;
            }

            return isAdd;
        }

        public static DatabaseObj GetDBObj(string dbName)
        {
            if (DatabaseCollection.ContainsKey(dbName))
                return DatabaseCollection[dbName];
            else
                return null;
        }

        public static DatabaseObj GetDBObjByConnetionString(string connectionString)
        {
            DatabaseObj dbObj = null;

            foreach (string dbKey in DatabaseCollection.Keys)
            {
                if (DatabaseCollection[dbKey].DbConnectionString == connectionString)
                {
                    dbObj = DatabaseCollection[dbKey];
                    break;
                }
            }
            return dbObj;
        }

        public static string ReturnDBConnectString(string server, string dbInstance, string username, string pwd, DatabaseType dbType)
        {
            string connectString = "";

            switch (dbType)
            {
                case DatabaseType.ORACLE:
                    connectString = string.Format("Data Source={0}{1};User Id={2};Password={3};Integrated Security=no;", string.IsNullOrEmpty(server) ? "" : server + "/", dbInstance, username, pwd);
                    break;
                case DatabaseType.SQLSERVER2008:
                    connectString = string.Format("Server={0};Database={1};User Id={2};Password={3};", server, dbInstance, username, pwd);
                    break;
                case DatabaseType.MYSQL:
                    connectString = string.Format("Server={0};Database={1};Uid={2};Pwd={3};", server, dbInstance, username, pwd);
                    break;
                default:
                    throw new Exception("unknown exception");
            }

            return connectString;
        }

        public static DatabaseObj ReturnDBObj(string server, string dbInstance, string username, string pwd, DatabaseType dbType)
        {
            DatabaseObj dbObj = null;

            string connectString = ReturnDBConnectString(server, dbInstance, username, pwd, dbType);
            switch (dbType)
            {
                case DatabaseType.ORACLE:
                    dbObj = new Database.DatabaseObj("System.Data.OracleClient", connectString);
                    break;
                case DatabaseType.MYSQL:
                    //dbObj = new Database.DatabaseObj("System.Data.OracleClient", connectString);
                    break;
                case DatabaseType.SQLSERVER2008:
                    dbObj = new Database.DatabaseObj("System.Data.SqlClient", connectString);
                    break;
                default:
                    throw new Exception("尚未支持的数据库类型");
            }

            return dbObj;
        }
    }

    public enum DatabaseType
    {
        ORACLE,
        SQLSERVER2008,
        MYSQL,
        ACCESS
    }
}