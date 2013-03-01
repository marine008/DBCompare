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
        private List<Database.Entity.UserObject> _userObjects;
        //private Dictionary<string, List<Database.Entity.UserObject>> _userObjects;

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
        public List<Database.Entity.UserObject> UserObjects
        {
            get { return _userObjects; }
        }
        //public Dictionary<string, List<Database.Entity.UserObject>> UserObjects
        //{
        //    get { return _userObjects; }
        //}

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

        private void FilingTabPage(List<Database.Entity.UserObject> values, string tabName, TabControl targetTabControl)
        {
            ListView listViewControl = new ListView();
            listViewControl.View = View.Details;
            listViewControl.Dock = DockStyle.Fill;
            listViewControl.Columns.Add("ObjectName");
            foreach (Database.Entity.UserObject value in values)
            {
                listViewControl.Items.Add(value.ObjName);
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
            compareResult.Add("USEROBJECTS", CompareUserObjs(this._userObjects, compareForm._userObjects, compareForm.Text));

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

        private Dictionary<string, List<string>> CompareUserObjs(List<Database.Entity.UserObject> baseList, List<Database.Entity.UserObject> targetList, string compareFormText)
        {
            Dictionary<string, List<string>> compareList = new Dictionary<string, List<string>>();

            //List<Database.Entity.UserObject> baseOver = baseList.Except(targetList).ToList();
            //List<Database.Entity.UserObject> targetOver = targetList.Except(baseList).ToList();

            List<string> baseOverList = new List<string>();
            List<string> targetOverList = new List<string>();
            foreach (Database.Entity.UserObject userObj in baseList)
            {
                baseOverList.Add(userObj.ObjName);
            }
            foreach (Database.Entity.UserObject userObj in targetList)
            {
                targetOverList.Add(userObj.ObjName);
            }

            List<string> baseOver = baseOverList.Except(targetOverList).ToList();
            List<string> targetOver = targetOverList.Except(baseOverList).ToList();

            compareList.Add(this.Text, baseOver);
            compareList.Add(compareFormText, targetOver);

            return compareList;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            TabPage tabPage = this.tabDBInfo.SelectedTab;
            Control pageControl = tabPage.Controls[0];
            if (pageControl is ListView)
            {
                ListView listView = pageControl as ListView;

                ListViewItem item = listView.FindItemWithText(this.txtSearch.Text.Trim(), false, 0, false);
                if (item != null)
                {
                    listView.SelectedItems.Clear();
                    listView.EnsureVisible(item.Index + 1);
                    listView.Items[item.Index].Selected = true;
                    listView.Focus();
                }
            }
        }
    }
}