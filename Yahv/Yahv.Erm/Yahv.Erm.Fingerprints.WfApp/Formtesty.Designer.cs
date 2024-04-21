namespace Yahv.Erm.Fingerprints.WfApp
{
    partial class Formtesty
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pbMin = new System.Windows.Forms.PictureBox();
            this.pbClose = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbClose)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pbMin
            // 
            this.pbMin.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.pbMin.Image = global::Yahv.Erm.Fingerprints.WfApp.Properties.Resources.b_min;
            this.pbMin.Location = new System.Drawing.Point(262, 126);
            this.pbMin.Name = "pbMin";
            this.pbMin.Size = new System.Drawing.Size(26, 26);
            this.pbMin.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbMin.TabIndex = 2;
            this.pbMin.TabStop = false;
            // 
            // pbClose
            // 
            this.pbClose.Image = global::Yahv.Erm.Fingerprints.WfApp.Properties.Resources.b_close;
            this.pbClose.Location = new System.Drawing.Point(356, 126);
            this.pbClose.Name = "pbClose";
            this.pbClose.Size = new System.Drawing.Size(26, 26);
            this.pbClose.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbClose.TabIndex = 0;
            this.pbClose.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::Yahv.Erm.Fingerprints.WfApp.Properties.Resources.b_move;
            this.pictureBox1.Location = new System.Drawing.Point(370, 211);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(22, 21);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // Formtesty
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(550, 406);
            this.Controls.Add(this.pbMin);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.pbClose);
            this.Name = "Formtesty";
            this.Text = "Formtesty";
            ((System.ComponentModel.ISupportInitialize)(this.pbMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbClose)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pbClose;
        private System.Windows.Forms.PictureBox pbMin;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}