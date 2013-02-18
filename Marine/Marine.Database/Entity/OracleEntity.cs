using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Marine.Database.Entity
{
    public class OracleEntity
    {
        private DatabaseObj _dbExcuter = null;

        public OracleEntity(DatabaseObj dbExcuter)
        {
            if (dbExcuter == null)
                throw new ArgumentNullException("dbExcuter");
            if (!dbExcuter.DbProviderName.ToLower().Contains("oracle"))
                throw new Exception("请确认是否是oracle数据库连接对象");
            _dbExcuter = dbExcuter;
        }

        public Dictionary<string, string> GetCurConnectionInfo()
        {
            throw new NotImplementedException();
        }

        public List<string> GetDBUsers()
        {
            string cmd = "select username from all_users";
            return FillResult(cmd, "username");
        }

        public List<string> GetDBTables()
        {
            string cmd = "select table_name from user_tables";
            return FillResult(cmd, "tablespace_name");
        }

        public List<string> GetDBTables(string userName)
        {
            string cmd = string.Format("select table_name from user_tables where owner='{0}'", userName);
            return FillResult(cmd, "tablespace_name");
        }

        public List<string> GetDBSpaces()
        {
            string cmd = "select tablespace_name from  user_tablespaces";
            return FillResult(cmd, "tablespace_name");
        }

        public List<ColumnInfo> GetTableColumns(string tableName)
        {
            List<ColumnInfo> columnList = new List<ColumnInfo>();

            string cmd = "";



            return columnList;
        }

        public Dictionary<string, List<UserObject>> GetCurUserObject()
        {
            Dictionary<string, List<UserObject>> userObjDic = new Dictionary<string, List<UserObject>>();

            DataTable userObjDT = new DataTable();
            string cmd = "select * from user_objects order by object_type";
            int excuteResult = _dbExcuter.Fill(cmd, ref userObjDT);
            if (excuteResult > 0)
            {
                foreach (DataRow dtRow in userObjDT.Rows)
                {
                    string objType = dtRow["object_type"].ToString();
                    if (!userObjDic.ContainsKey(objType))
                    {
                        userObjDic.Add(objType, new List<UserObject>());
                    }

                    UserObject userObj = new UserObject();
                    userObj.ObjName = dtRow["object_name"].ToString();
                    userObj.ObjType = objType;
                    userObj.CreateTime = Convert.ToDateTime(dtRow["created"].ToString());
                    userObj.ChangeTime = Convert.ToDateTime(dtRow["last_ddl_time"].ToString());

                    userObjDic[objType].Add(userObj);
                }
            }

            return userObjDic;
        }

        public string GetCurUserName()
        {
            string userName = "";

            string cmd = "select username from v$session where sid in (select distinct sid from v$mystat)";
            object result = _dbExcuter.ExecuteScalar(cmd);
            if (result != null)
                userName = result.ToString();

            return userName;
        }

        public List<string> GetCurUserPrivs()
        {
            string cmd = "select privilege from user_sys_privs";
            return FillResult(cmd, "privilege");
        }

        public List<string> GetCurUserRole()
        {
            string cmd = "select granted_role from user_role_privs";
            return FillResult(cmd, "granted_role");
        }

        public string GeneratePrivsSQL(string privsType, string userName)
        {
            return string.Format("grant {0} to {1};", privsType, userName);
        }

        public string GenerateRoleSQL(string roleType, string userName)
        {
            return string.Format("grant {0} to {1};", roleType, userName);
        }

        public string GenerateTableSpace(string spaceName)
        {
            return string.Format("create tablespace {0}  " +
                                            "logging  " +
                                            "datafile 'D:\\oracle\\oradata\\Oracle9i\\user_data.dbf' " +
                                            "size 50m  " +
                                            "autoextend on  " +
                                            "next 50m maxsize 20480m  " +
                                            "extent management local;  ", spaceName);
        }

        private List<string> FillResult(string cmd, string columnName)
        {
            List<string> dbUser = new List<string>();
            DataTable dataTable = new DataTable();
            _dbExcuter.Fill(cmd, ref dataTable);
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                foreach (DataRow dtRow in dataTable.Rows)
                {
                    dbUser.Add(dtRow[columnName].ToString());
                }
            }

            return dbUser;
        }
    }

    public class UserObject
    {
        private string _objName;
        private string _objType;
        private DateTime _createTime;
        private DateTime _changeTime;

        public string ObjName
        {
            get { return _objName; }
            set { _objName = value; }
        }

        public string ObjType
        {
            get { return _objType; }
            set { _objType = value; }
        }

        public DateTime CreateTime
        {
            get { return _createTime; }
            set { _createTime = value; }
        }

        public DateTime ChangeTime
        {
            get { return _changeTime; }
            set { _changeTime = value; }
        }
    }
}