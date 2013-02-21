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

        private List<string> _userPrivs;
        private List<string> _userRoles;
        private List<string> _dbSpaces;
        private Dictionary<string, List<Database.Entity.UserObject>> _userObjects;

        public List<string> UserPrivs
        {
            get { return _userPrivs; }
        }
        public List<string> UserRoles
        {
            get { return _userRoles; }
        }
        public List<string> DBSpaces
        {
            get { return _dbSpaces; }
        }
        public Dictionary<string, List<Database.Entity.UserObject>> UserObjects
        {
            get { return _userObjects; }
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
            this._userPrivs = _dbEntity.GetCurUserPrivs();
            this._userRoles = _dbEntity.GetCurUserRole();
            this._dbSpaces = _dbEntity.GetDBSpaces();
            this._userObjects = _dbEntity.GetCurUserObject();

            FilingTabPage(_userPrivs, "PRIVS", this.tabDBInfo);
            FilingTabPage(_userRoles, "ROLES", this.tabDBInfo);
            FilingTabPage(_dbSpaces, "SPACES", this.tabDBInfo);
            FilingTabPage(_userObjects, "USEROBJECTS", this.tabDBInfo);
        }

        private void FilingTabPage(List<string> values, string tabName, TabControl targetTabControl)
        {
            ListView listViewControl = new ListView();
            listViewControl.View = View.Details;
            listViewControl.Dock = DockStyle.Fill;
            listViewControl.Columns.Add("ObjectName");
            foreach (string value in values)
            {
                listViewControl.Items.Add(value);
            }

            TabPage tabPage = new TabPage(tabName);
            tabPage.Controls.Add(listViewControl);

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

        public void SetItemColor(string tabPageTitle, string itemString)
        {
            foreach (TabPage tbPage in this.tabDBInfo.TabPages)
            {
                if (tbPage.Text == tabPageTitle)
                {
                    Control pageControl = tbPage.Controls[0];
                    if (pageControl is ListView)
                    {
                        foreach (ListViewItem item in (pageControl as ListView).Items)
                        {
                            if (item.Text == itemString)
                            {
                                item.BackColor = Color.Maroon;
                                break;
                            }
                        }
                    }
                    break;
                }
            }
        }

        public Dictionary<string, Dictionary<string, List<string>>> Compare(CompareForm compareForm)
        {
            Dictionary<string, Dictionary<string, List<string>>> compareResult = new Dictionary<string, Dictionary<string, List<string>>>();

            compareResult.Add("PRIVS", CompareList(this._userPrivs, compareForm._userPrivs, compareForm.Text));
            compareResult.Add("ROLES", CompareList(this._userRoles, compareForm._userRoles, compareForm.Text));
            compareResult.Add("SPACES", CompareList(this._dbSpaces, compareForm._dbSpaces, compareForm.Text));
            //compareResult.Add("USEROBJECTS", CompareList(this._userPrivs, compareForm._userPrivs,compareForm.Text);

            return compareResult;
        }

        private Dictionary<string, List<string>> CompareList(List<string> baseList, List<string> targetList, string compareFormText)
        {
            Dictionary<string, List<string>> compareList = new Dictionary<string, List<string>>();

            List<string> baseOver = baseList.Except(targetList).ToList();
            List<string> targetOver = targetList.Except(baseList).ToList();

            compareList.Add(this.Text, baseOver);
            compareList.Add(compareFormText, targetOver);

            return compareList;
        }
    }
}