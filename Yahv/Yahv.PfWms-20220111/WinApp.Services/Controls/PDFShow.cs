using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinApp.Services.Controls
{
    public partial class PDFShow : Form
    {
        public PDFShow()
        {
            InitializeComponent();
        }

        public static string pdfFile;

        private void PDFShow_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            MyOpenFileDialog();
        }

        public void MyOpenFileDialog()
        {
            //创建Adobe PDF Reader控件
            //AxAcroPDFLib.AxAcroPDF axAcroPDF1 = new AxAcroPDFLib.AxAcroPDF();
            //((System.ComponentModel.ISupportInitialize)(axAcroPDF1)).BeginInit();
            //axAcroPDF1.Location = new System.Drawing.Point(0, 24);
            //axAcroPDF1.Size = new System.Drawing.Size(292, 242);
            //axAcroPDF1.Dock = DockStyle.Fill;
            //this.Controls.Add(axAcroPDF1);

            //((System.ComponentModel.ISupportInitialize)(axAcroPDF1)).EndInit();
            //axAcroPDF1.LoadFile(pdfFile);

        }

        static PDFShow current;

        static public PDFShow Current
        {
            get
            {
                if (current == null)
                {
                    current = new PDFShow();
                }

                return current;
            }
        }
        private void PDFShow_FormClosed(object sender, FormClosedEventArgs e)
        {
            current = null;
        }
    }
}
