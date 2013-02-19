using System;
using System.Data;
using System.Data.Common;

namespace Marine.Database
{
    /// <summary>
    /// 数据库连接对象
    /// 不把该链接对象做成静态对象的主要原因在于：
    ///     同一个程序域里面可以配置多个不同的数据库连接。
    ///     如果需要使数据库连接持久化，可以通过新增一个数据库连接存储对象将新建的数据库连接增加到里面。
    /// </summary>
    public class DatabaseObj
    {
        private string _dbProviderName = "";
        private string _dbConnectionString = "";

        private DbConnection dbCnx = null;
        private DbCommand dbCmd = null;
        private DbDataAdapter dbAdapter = null;

        public string DbProviderName
        {
            get { return _dbProviderName; }
        }
        public string DbConnectionString
        {
            get { return _dbConnectionString; }
        }

        /// <summary>
        /// 根据连接字符串及其数据提供程序类型初始化数据库连接对象
        /// </summary>
        /// <param name="providerName">数据提供程序类库名称</param>
        /// <param name="connectionString">数据库连接字符串</param>
        public DatabaseObj(string providerName, string connectionString)
        {
            this._dbProviderName = providerName;
            this._dbConnectionString = connectionString;

            DbProviderFactory factory = DbProviderFactories.GetFactory(this._dbProviderName);
            try
            {
                //初始化数据库连接
                dbCnx = factory.CreateConnection();
                dbCnx.ConnectionString = this._dbConnectionString;
                //初始化数据库命令
                dbCmd = factory.CreateCommand();
                dbCmd.Connection = dbCnx;

                dbAdapter = factory.CreateDataAdapter();
            }
            catch (Exception err)
            {
                System.Diagnostics.Debug.WriteLine(err.Message);
                throw err;
            }
        }

        public DbTransaction GetCurrentTransaction()
        {
            DbTransaction dbTransaction = dbCnx.BeginTransaction();
            return dbTransaction;
        }

        /// <summary>
        /// 验证数据库连接
        /// </summary>
        /// <returns></returns>
        public bool ValidateConnection()
        {
            bool isConnected = false;

            try
            {
                if (dbCnx.State == ConnectionState.Open)
                    dbCnx.Close();
                dbCnx.Open();
                isConnected = true;
            }
            catch (Exception err)
            {
                System.Diagnostics.Trace.Write(err.Message);
            }
            finally
            {
                if (dbCnx != null)
                    dbCnx.Close();
            }

            return isConnected;
        }

        /// <summary>
        /// 获取数据库的架构信息
        /// </summary>
        /// <param name="schemaType">架构数据类型</param>
        /// <returns></returns>
        public DataTable GetSchema(string schemaType)
        {
            DataTable dt = null;

            if (dbCnx != null)
            {
                SetCnxOpen();

                dt = dbCnx.GetSchema(schemaType);
            }

            return dt;
        }

        /// <summary>
        /// 对连接对象执行 SQL 语句
        /// </summary>
        /// <param name="cmdText">SQL语句</param>
        /// <returns></returns>
        public int ExecuteNonQuery(string cmdText)
        {
            return ExecuteNonQuery(cmdText, null);
        }
        public int ExecuteNonQuery(string cmdText, DbTransaction transaction)
        {
            int queryNum = -1;

            try
            {
                SetCnxOpen();

                dbCmd.CommandText = cmdText;
                if (transaction != null)
                    dbCmd.Transaction = transaction;
                queryNum = dbCmd.ExecuteNonQuery();
            }
            catch (DbException err)
            {
                System.Diagnostics.Debug.WriteLine(err.Message);
                throw err;
            }

            return queryNum;
        }

        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列
        /// 所有其他的列和行将被忽略
        /// </summary>
        /// <param name="cmdText"></param>
        /// <returns></returns>
        public object ExecuteScalar(string cmdText)
        {
            return ExecuteScalar(cmdText, null);
        }
        public object ExecuteScalar(string cmdText, DbTransaction transaction)
        {
            object queryResult = null;

            try
            {
                SetCnxOpen();

                dbCmd.CommandText = cmdText;
                if (transaction != null)
                    dbCmd.Transaction = transaction;
                queryResult = dbCmd.ExecuteScalar();
            }
            catch (DbException err)
            {
                System.Diagnostics.Debug.WriteLine(err.Message);
                throw err;
            }

            return queryResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        public int Fill(string cmdText, ref DataTable dataTable)
        {
            return Fill(cmdText, null, ref dataTable);
        }
        public int Fill(string cmdText, DbTransaction transaction, ref DataTable dataTable)
        {
            int queryNum = -1;

            try
            {
                dbCmd.CommandText = cmdText;
                if (transaction != null)
                    dbCmd.Transaction = transaction;
                dbAdapter.SelectCommand = dbCmd;
                queryNum = dbAdapter.Fill(dataTable);
            }
            catch (DbException err)
            {
                System.Diagnostics.Debug.WriteLine(err.Message);
                throw err;
            }

            return queryNum;
        }

        /// <summary>
        /// 尚未完成
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="transaction"></param>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        public int Update(string cmdText, DbTransaction transaction, DataTable dataTable)
        {
            int updateNum = -1;

            try
            {
                dbCmd.CommandText = cmdText;
                if (transaction != null)
                    dbCmd.Transaction = transaction;
                dbAdapter.SelectCommand = dbCmd;
                DbCommandBuilder dbCmdBuilder = ReturnCmdBuilder(dbAdapter);

                dbAdapter.Update(dataTable);
            }
            catch (DBConcurrencyException concurrencyErr)//数据冲突异常
            {
                System.Diagnostics.Debug.WriteLine(concurrencyErr.Message);
            }
            catch (DbException err)
            {
                System.Diagnostics.Debug.WriteLine(err.Message);
                throw err;
            }
            dataTable.AcceptChanges();

            return updateNum;
        }

        private DbCommandBuilder ReturnCmdBuilder(DbDataAdapter adapter)
        {
            return null;
        }

        /// <summary>
        /// 打开数据库连接
        /// </summary>
        private void SetCnxOpen()
        {
            if (dbCnx == null)
                throw new Exception("数据库连接不可用");

            if (dbCnx.State != ConnectionState.Open)
                dbCnx.Open();
        }
    }
}