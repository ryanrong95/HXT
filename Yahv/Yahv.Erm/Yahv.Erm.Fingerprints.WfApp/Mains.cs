using Gecko;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Yahv.Erm.Fingerprints.Services;

namespace Yahv.Erm.Fingerprints.WfApp
{
    public partial class Mains : Form
    {
        public Mains()
        {
            InitializeComponent();
        }

        GeckoWebBrowser gecko;

        private void Mains_Load(object sender, EventArgs e)
        {


            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.White;

            Xpcom.Initialize("Firefox");
            var gecko = this.gecko = new GeckoWebBrowser { Dock = DockStyle.Fill };
            GeckoJsToCsHelper.Initialize(gecko, typeof(GeckoHelper));
            panel2.Controls.Add(gecko);
            foreach (var control in new[] { this.pbMove })
            {
                control.MouseDown += this.control_MouseDown;
                control.MouseUp += this.control_MouseUp;
                control.MouseMove += this.control_MouseMove;
                control.Cursor = Cursors.NoMove2D;
            }

            GeckoJsToCsHelper.Initialize(gecko, typeof(GeckoHelper));
            //   gecko.Navigate(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Contents", "Html", "考勤.html"));
            gecko.Navigate(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Contents", "Html", "指纹输入.html"));
            this.Width = 1024;
            this.Height = (int)(768d / 1024d * 1000d);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(Pens.DarkOliveGreen, 0, 0, this.Width - 1, this.Height - 1);
        }

        #region 移动窗体

        bool isMouseDown = false;
        Point currentFormLocation = new Point(); //当前窗体位置
        Point currentMouseOffset = new Point(); //当前鼠标的按下位置
        private void control_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isMouseDown = true;
                currentFormLocation = this.Location;
                currentMouseOffset = Control.MousePosition;
            }
        }

        private void control_MouseUp(object sender, MouseEventArgs e)
        {
            isMouseDown = false;
        }

        private void control_MouseMove(object sender, MouseEventArgs e)
        {
            int rangeX = 0, rangeY = 0; //计算当前鼠标光标的位移，让窗体进行相同大小的位移
            if (isMouseDown)
            {
                Point pt = MousePosition;
                rangeX = currentMouseOffset.X - pt.X;
                rangeY = currentMouseOffset.Y - pt.Y;
                this.Location = new Point(currentFormLocation.X - rangeX, currentFormLocation.Y - rangeY);
            }
        }

        #endregion

        #region 窗体按钮

        private void pbMin_Click(object sender, EventArgs e)
        {
            var control = sender as PictureBox;

            if (sender == null)
            {
                return;
            }

            if (this.WindowState == FormWindowState.Maximized)
            {
                control.Image = Properties.Resources.b_max;
                this.pbMove.Show();
                this.WindowState = FormWindowState.Normal;
            }
            else
            {
                control.Image = Properties.Resources.b_min;
                this.pbMove.Hide();
                this.WindowState = FormWindowState.Maximized;
            }
        }

        private void pbClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pbMove_Click(object sender, EventArgs e)
        {

        }

        #endregion
    }
}
