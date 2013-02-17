using System.Data;
using System.Data.Common;
using System.Configuration;

namespace Marine.Test
{
    public class DBTest
    {
        public void Test1()
        {
            Configuration cfg = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            ConnectionStringSettings cfg_cs = cfg.ConnectionStrings.ConnectionStrings["myDB"];
            DbProviderFactory factory = DbProviderFactories.GetFactory(cfg_cs.ProviderName);

            DbDataSourceEnumerator e = factory.CreateDataSourceEnumerator();
            DataTable tb1 = e.GetDataSources();
            if (tb1.Rows.Count > 0)
            {
                int userChoice = 0;
                DataRow row = tb1.Rows[userChoice];
                string datasource = row["ServerName"].ToString();
                string instanceName = row["InstanceName"].ToString();
                string isClustered = row["IsClustered"].ToString();
                string version = row["Version"].ToString();

                DbConnectionStringBuilder csBuilder = factory.CreateConnectionStringBuilder();
                csBuilder.ConnectionString = cfg_cs.ConnectionString;
                if (csBuilder.ContainsKey("Server"))
                    csBuilder.Remove("Server");
                csBuilder.Add("Server", datasource);
                cfg_cs.ConnectionString = csBuilder.ConnectionString;
                cfg.Save();
            }
        }

        public void Test2()
        {
            Configuration cfg = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            ConnectionStringSettings cfg_cs = cfg.ConnectionStrings.ConnectionStrings["myDB"];
            DbProviderFactory factory = DbProviderFactories.GetFactory(cfg_cs.ProviderName);

            DbConnection cnx = factory.CreateConnection();
            DbDataAdapter adapter = factory.CreateDataAdapter();
            DbCommand cmd = factory.CreateCommand();
            cnx.ConnectionString = cfg_cs.ConnectionString;

            string vv = "";
            cnx.Open();
            DataTable dt = cnx.GetSchema("tables");
            foreach (DataRow dtRow in dt.Rows)
            {
                foreach (DataColumn dtCol in dt.Columns)
                {
                    vv += dtCol.ToString() + " = " + dtRow[dtCol].ToString() + "\r\n";
                }
            }
            using (cnx)
            {
                string cmdText = "select * from kt_flow_define t where sn = '8cf5262f147ea658'";

                cmd.Connection = cnx;
                cmd.CommandText = cmdText;

                adapter.SelectCommand = cmd;

                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
            }
        }

        public void TestDBReader()
        {
            Configuration cfg = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            ConnectionStringSettings cfg_cs = cfg.ConnectionStrings.ConnectionStrings["myDB"];
            DbProviderFactory factory = DbProviderFactories.GetFactory(cfg_cs.ProviderName);

            DbConnection cnx = factory.CreateConnection();
            DataSet dts = new DataSet();
        }

        public void test23()
        {
            DataTable dt = new DataTable();
            Configuration cfg = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            ConnectionStringSettings cfg_cs = cfg.ConnectionStrings.ConnectionStrings["myDB"];
            Marine.Database.DatabaseObj db = new Database.DatabaseObj(cfg_cs.ProviderName, cfg_cs.ConnectionString);
            db.Fill("SELECT * FROM KT_FLOW_DEFINE T", ref dt);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dtRow in dt.Rows)
                {
                    System.Console.WriteLine(dtRow["name"].ToString());
                }
            }
        }
    }
}