namespace Yahv.Erm.Fingerprints.WfApp
{
    partial class Mains
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Mains));
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.pHeader = new System.Windows.Forms.Panel();
            this.pbMove = new System.Windows.Forms.PictureBox();
            this.pbMin = new System.Windows.Forms.PictureBox();
            this.pbClose = new System.Windows.Forms.PictureBox();
            this.panel1.SuspendLayout();
            this.pHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbMove)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbClose)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.pHeader);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(878, 585);
            this.panel1.TabIndex = 2;
            // 
            // panel2
            // 
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 30);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(878, 555);
            this.panel2.TabIndex = 4;
            // 
            // pHeader
            // 
            this.pHeader.BackColor = System.Drawing.Color.Transparent;
            this.pHeader.Controls.Add(this.pbMove);
            this.pHeader.Controls.Add(this.pbMin);
            this.pHeader.Controls.Add(this.pbClose);
            this.pHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pHeader.Location = new System.Drawing.Point(0, 0);
            this.pHeader.Name = "pHeader";
            this.pHeader.Size = new System.Drawing.Size(878, 30);
            this.pHeader.TabIndex = 3;
            // 
            // pbMove
            // 
            this.pbMove.Image = ((System.Drawing.Image)(resources.GetObject("pbMove.Image")));
            this.pbMove.Location = new System.Drawing.Point(3, 4);
            this.pbMove.Name = "pbMove";
            this.pbMove.Size = new System.Drawing.Size(26, 26);
            this.pbMove.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbMove.TabIndex = 6;
            this.pbMove.TabStop = false;
            this.pbMove.Click += new System.EventHandler(this.pbMove_Click);
            // 
            // pbMin
            // 
            this.pbMin.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbMin.Image = global::Yahv.Erm.Fingerprints.WfApp.Properties.Resources.b_max;
            this.pbMin.Location = new System.Drawing.Point(817, 3);
            this.pbMin.Name = "pbMin";
            this.pbMin.Size = new System.Drawing.Size(26, 26);
            this.pbMin.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbMin.TabIndex = 4;
            this.pbMin.TabStop = false;
            this.pbMin.Click += new System.EventHandler(this.pbMin_Click);
            // 
            // pbClose
            // 
            this.pbClose.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbClose.Image = ((System.Drawing.Image)(resources.GetObject("pbClose.Image")));
            this.pbClose.Location = new System.Drawing.Point(849, 3);
            this.pbClose.Name = "pbClose";
            this.pbClose.Size = new System.Drawing.Size(26, 26);
            this.pbClose.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbClose.TabIndex = 3;
            this.pbClose.TabStop = false;
            this.pbClose.Click += new System.EventHandler(this.pbClose_Click);
            // 
            // Mains
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 591);
            this.ControlBox = false;
            this.Controls.Add(this.panel1);
            this.Name = "Mains";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.ShowIcon = false;
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Mains_Load);
            this.panel1.ResumeLayout(false);
            this.pHeader.ResumeLayout(false);
            this.pHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbMove)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbClose)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel pHeader;
        private System.Windows.Forms.PictureBox pbClose;
        private System.Windows.Forms.PictureBox pbMin;
        private System.Windows.Forms.PictureBox pbMove;
    }
}

