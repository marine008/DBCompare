using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;

namespace Marine.Sample
{
    public partial class Form2 : Form
    {
        private Dictionary<string, Database.Entity.OracleEntity> _dbObjs = null;

        public Form2()
        {
            InitializeComponent();

            _dbObjs = new Dictionary<string, Database.Entity.OracleEntity>();
        }

        private void tsBtnAdd_Click(object sender, EventArgs e)
        {
            if (this.MdiChildren.Length < 2)
            {
                AddForm addForm = new AddForm();
                if (addForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string formText = addForm.DBType + ":" + addForm.DBUserName + "@" + addForm.DBInstance;

                    _dbObjs.Add(formText, addForm.DBEntity);
                    CreateCompareForm(addForm.DBEntity, formText);
                }
            }
            else
                MessageBox.Show("窗体数量最多2个！");
        }

        private void CreateCompareForm(Database.Entity.OracleEntity dbEntity, string formText)
        {
            CompareForm compareForm = new CompareForm(dbEntity);
            compareForm.Text = formText;

            compareForm.MdiParent = this;
            compareForm.Show();
        }

        private void tsBtnArrangeIcons_Click(object sender, EventArgs e)
        {
            this.LayoutMdi(MdiLayout.ArrangeIcons);
        }

        private void tsBtnCascade_Click(object sender, EventArgs e)
        {
            this.LayoutMdi(MdiLayout.Cascade);
        }

        private void tsBtnTileHorizontal_Click(object sender, EventArgs e)
        {
            this.LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void tsBtnTileVertical_Click(object sender, EventArgs e)
        {
            this.LayoutMdi(MdiLayout.TileVertical);
        }

        private void tsBtnCompare_Click(object sender, EventArgs e)
        {
            //在对比窗口内部进行对比
            if (this.MdiChildren.Length > 1)
            {
                if (this.MdiChildren[0] is CompareForm && this.MdiChildren[1] is CompareForm)
                {
                    CompareForm compareForm1 = this.MdiChildren[0] as CompareForm;
                    CompareForm compareForm2 = this.MdiChildren[1] as CompareForm;

                    Dictionary<string, Dictionary<string, List<string>>> compareResutl = compareForm1.Compare(compareForm2);
                    foreach (string compareType in compareResutl.Keys)
                    {
                        Dictionary<string, List<string>> typeResult = compareResutl[compareType];
                        foreach (string resultType in typeResult.Keys)
                        {
                            for (int i = 0; i < this.MdiChildren.Length; i++)
                            {
                                if (this.MdiChildren[i].Text == resultType)
                                {
                                    foreach (string value in typeResult[resultType])
                                    {
                                        if (this.MdiChildren[i] is CompareForm)
                                            (this.MdiChildren[i] as CompareForm).SetItemColor(compareType, value);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("窗体少于2个");
            }
        }

        private void tsBtnGenerate_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild != null && this.ActiveMdiChild is CompareForm)
            {

            }
        }

        private void tsBtnExport_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "xml|*.xml";
            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                Database.XmlOperation xOperation = new Database.XmlOperation();
                xOperation.Create("DatabaseConnection");

                foreach (string key in this._dbObjs.Keys)
                {
                    Database.Entity.OracleEntity oEntity = this._dbObjs[key];

                    XmlNode dbNode = xOperation.CreateXmlNode("Database", "", null);
                    XmlNode connectionNode = xOperation.CreateXmlNode("Connection", oEntity.DBExcuter.DbConnectionString, null);
                    XmlNode providerNode = xOperation.CreateXmlNode("Provider", oEntity.DBExcuter.DbProviderName, null);
                    XmlNode dbTypeNode = xOperation.CreateXmlNode("Type", typeof(Database.Entity.OracleEntity).ToString(), null);
                    XmlNode assemblyNode = xOperation.CreateXmlNode("Assembly", typeof(Database.Entity.OracleEntity).Assembly.Location, null);
                    XmlNode dbInfo = xOperation.CreateXmlNode("Info", key, null);

                    dbNode.AppendChild(connectionNode);
                    dbNode.AppendChild(providerNode);
                    dbNode.AppendChild(dbTypeNode);
                    dbNode.AppendChild(assemblyNode);
                    dbNode.AppendChild(dbInfo);

                    xOperation.RootNode.AppendChild(dbNode);
                }

                xOperation.Save(saveFileDialog.FileName);
            }
        }

        private void tsBtnImport_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "xml文件|*.xml";
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Database.XmlOperation xmlOperation = new Database.XmlOperation();
                xmlOperation.Load(openFileDialog.FileName);
                XmlNodeList nodes = xmlOperation.GetNodes("Database");
                if (nodes != null)
                {
                    foreach (XmlNode node in nodes)
                    {
                        Dictionary<string, string> innerValues = xmlOperation.GetTagElementInnerValues(node);
                        Database.DatabaseObj dbObj = new Database.DatabaseObj(innerValues["Provider"], innerValues["Connection"]);

                        System.Reflection.Assembly assembly = System.Reflection.Assembly.LoadFile(innerValues["Assembly"]);
                        Type type = assembly.GetType(innerValues["Type"]);
                        object obj = Activator.CreateInstance(type, new object[] { dbObj });

                        Database.Entity.OracleEntity oracleEntity = null;
                        if (obj is Database.Entity.OracleEntity)
                            oracleEntity = obj as Database.Entity.OracleEntity;

                        CreateCompareForm(oracleEntity, innerValues["Info"]);
                    }
                }
            }
        }
    }
}
