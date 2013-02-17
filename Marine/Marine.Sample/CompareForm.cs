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
    public partial class CompareForm : Form
    {
        private Database.Entity.OracleEntity _dbEntity;
        public Database.Entity.OracleEntity DBEntity
        {
            get { return _dbEntity; }
        }

        public CompareForm(Database.Entity.OracleEntity oracleEntity)
        {
            InitializeComponent();

            if (oracleEntity == null)
            {
                throw new NoNullAllowedException("oracleEntity");
            }
            this._dbEntity = oracleEntity;

            this.tabDBInfo.TabPages.Clear();
            FilingTabPage(_dbEntity.GetCurUserPrivs(), "PRIVS", this.tabDBInfo);
            FilingTabPage(_dbEntity.GetCurUserRole(), "ROLES", this.tabDBInfo);
            FilingTabPage(_dbEntity.GetDBSpaces(), "SPACES", this.tabDBInfo);
            FilingTabPage(_dbEntity.GetCurUserObject(), "USEROBJECTS", this.tabDBInfo);
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

        private void FilingTabPage(Dictionary<string, List<Marine.Database.Entity.UserObject>> values, string tabName, TabControl targetTabControl)
        {
            ListView listViewControl = new ListView();
            listViewControl.View = View.Details;
            listViewControl.Columns.Add("ObjectName");
            listViewControl.Columns.Add("ObjectType");
            listViewControl.Dock = DockStyle.Fill;

            foreach (string typeString in values.Keys)
            {
                foreach (Marine.Database.Entity.UserObject value in values[typeString])
                {
                    ListViewItem listviewItem = new ListViewItem(new string[] { value.ObjName, value.ObjType });
                    listViewControl.Items.Add(listviewItem);
                }
            }
            TabPage tabPage = new TabPage(tabName);
            tabPage.Controls.Add(listViewControl);

            targetTabControl.TabPages.Add(tabPage);
        }
    }
}