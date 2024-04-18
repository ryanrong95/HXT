using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Yahv.PsWms.DappForm.Services.Controls.UControls
{
    public partial class MaxPhotos : Form
    {
       internal  Image image;

        public MaxPhotos()
        {
            InitializeComponent();
        }

        private void MaxPhotos_Load(object sender, EventArgs e)
        {
            this.pictureBox1.Image = image;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.FindForm().Close();
        }

        private void MaxPhotos_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F12)
            {
                this.FindForm().Close();
            }
        }
    }
}
