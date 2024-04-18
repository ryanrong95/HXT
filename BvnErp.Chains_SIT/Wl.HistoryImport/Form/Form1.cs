using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Wl.HistoryImport
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var containerView = new Needs.Ccs.Services.Views.BaseContainersView();

            var test = containerView.Where(t => t.Code == "21").FirstOrDefault();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (YD_LogisticsDBEntities db = new YD_LogisticsDBEntities())
            {
                var query = from c in db.T_Icgoo_Declare
                            where c.Brand == "TE"
                            select c;

                var test = query.ToList();
            }
        }
    }
}
