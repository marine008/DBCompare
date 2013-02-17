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
            for (int i = 0; i < this.MdiChildren.Length; i++)
            {

            }
        }
    }
}
