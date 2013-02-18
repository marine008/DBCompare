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
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void tsBtnAdd_Click(object sender, EventArgs e)
        {
            AddForm addForm = new AddForm();
            if (addForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                CompareForm compareForm = new CompareForm(addForm.DBEntity);
                compareForm.Text = addForm.DBType + ":" + addForm.DBUserName + "@" + addForm.DBInstance;

                compareForm.MdiParent = this;
                compareForm.Show();
            }
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
    }
}
