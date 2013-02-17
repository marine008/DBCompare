using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Marine.Sample
{
    public partial class Form1 : Form
    {
        Marine.Database.DatabaseObj dbObj1 = null;
        Marine.Database.DatabaseObj dbObj2 = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            dbObj1 = GetDatabaseObj(this.txtUsername1.Text, this.txtpwd1.Text, this.txtInstance1.Text);
            dbObj2 = GetDatabaseObj(this.txtUsername2.Text, this.txtpwd2.Text, this.txtInstance2.Text);

            if (dbObj1.ValidateConnection() && dbObj2.ValidateConnection())
                MessageBox.Show("OK");
            else
                MessageBox.Show("ERROR");
        }

        private Database.DatabaseObj GetDatabaseObj(string username, string pwd, string instance)
        {
            string connectString = string.Format("Data Source={0};User Id={1};Password={2};Integrated Security=no;", instance, username, pwd);

            Database.DatabaseObj dbObj = new Database.DatabaseObj("System.Data.OracleClient", connectString);
            return dbObj;
        }

        private void btnGetInfo_Click(object sender, EventArgs e)
        {
            Database.Entity.OracleEntity oracleEntity1 = new Database.Entity.OracleEntity(dbObj1);
            Database.Entity.OracleEntity oracleEntity2 = new Database.Entity.OracleEntity(dbObj2);

            tabControl1.TabPages.Clear();
            List<string> userPrivsList1 = oracleEntity1.GetCurUserPrivs();
            FilingTabPage(userPrivsList1, "Privs", tabControl1);

            Dictionary<string, List<Marine.Database.Entity.UserObject>> userObjects = oracleEntity1.GetCurUserObject();
            foreach (string typeString in userObjects.Keys)
            {
                FilingTabPage(userObjects[typeString], typeString, tabControl1);
            }

            tabControl2.TabPages.Clear();
            List<string> userPrivsList2 = oracleEntity2.GetCurUserPrivs();
            FilingTabPage(userPrivsList2, "Privs", tabControl2);
        }

        private void FilingTabPage(List<string> values, string tabName, TabControl targetTabControl)
        {
            ListBox listControl = new ListBox();
            listControl.Dock = DockStyle.Fill;
            foreach (string value in values)
            {
                listControl.Items.Add(value);
            }

            TabPage tabPage = new TabPage(tabName);
            tabPage.Controls.Add(listControl);

            targetTabControl.TabPages.Add(tabPage);
        }

        private void FilingTabPage(List<Marine.Database.Entity.UserObject> values, string tabName, TabControl targetTabControl)
        {
            ListView listViewControl = new ListView();
            listViewControl.View = View.Details;
            listViewControl.Columns.Add("ObjectName");
            listViewControl.Columns.Add("ObjectType");
            listViewControl.Dock = DockStyle.Fill;

            foreach (Marine.Database.Entity.UserObject value in values)
            {
                ListViewItem listviewItem = new ListViewItem(new string[] { value.ObjName, value.ObjType });
                listViewControl.Items.Add(listviewItem);
            }

            TabPage tabPage = new TabPage(tabName);
            tabPage.Controls.Add(listViewControl);

            targetTabControl.TabPages.Add(tabPage);
        }

        private void btnCompare_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < this.listPrivs1.Items.Count; i++)
            {
                for (int j = 0; j < this.listPrivs2.Items.Count; j++)
                {
                    if (this.listPrivs1.Items[i].ToString() == this.listPrivs2.Items[j].ToString())
                    {
                        this.listPrivs1.Items[i] = this.listPrivs1.Items[i].ToString() + "(OK)";
                        this.listPrivs2.Items[j] = this.listPrivs2.Items[j].ToString() + "(OK)";
                    }
                }
            }
        }
    }
}