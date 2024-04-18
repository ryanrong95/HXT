using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Wl.CusFileListen
{
    public partial class FormWatcher : Form
    {
        public FormWatcher()
        {
            InitializeComponent();

            MinimizeBox = false;
            MaximizeBox = false;
            ControlBox = false;
            FormBorderStyle = FormBorderStyle.None;
        }
    }
}
