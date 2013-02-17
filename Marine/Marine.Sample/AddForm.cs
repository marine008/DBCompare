using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Marine.Database;

namespace Marine.Sample
{
    public partial class AddForm : Form
    {
        private Marine.Database.Entity.OracleEntity _dbEntity;

        public Marine.Database.Entity.OracleEntity DBEntity
        {
            get { return _dbEntity; }
        }
        public DatabaseType DBType
        {
            get { return (DatabaseType)(System.Enum.Parse(typeof(DatabaseType), this.cobDBType.SelectedItem.ToString())); }
        }
        public string DBInstance
        {
            get { return this.txtInstance.Text; }
        }
        public string DBUserName
        {
            get { return this.txtUserName.Text; }
        }

        public AddForm()
        {
            InitializeComponent();
            InitilizeWinFormData();
        }

        private void InitilizeWinFormData()
        {
            this.cobDBType.DataSource = System.Enum.GetNames(typeof(Database.DatabaseType));
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Database.DatabaseObj dbObj = Marine.Database.DatabaseFactory.ReturnDBObj(this.txtServer.Text, this.txtInstance.Text, this.txtUserName.Text, this.txtPwd.Text, (DatabaseType)System.Enum.Parse(typeof(DatabaseType), this.cobDBType.SelectedItem.ToString()));
            _dbEntity = new Database.Entity.OracleEntity(dbObj);

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }
    }
}