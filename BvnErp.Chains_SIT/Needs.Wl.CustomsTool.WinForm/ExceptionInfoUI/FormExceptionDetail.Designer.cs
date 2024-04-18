namespace Needs.Wl.CustomsTool.WinForm
{
    partial class FormExceptionDetail
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormExceptionDetail));
            this.label报关单号 = new System.Windows.Forms.Label();
            this.label生成时间 = new System.Windows.Forms.Label();
            this.txt异常内容 = new System.Windows.Forms.TextBox();
            this.label异常内容 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label报关单号
            // 
            this.label报关单号.AutoSize = true;
            this.label报关单号.Location = new System.Drawing.Point(12, 9);
            this.label报关单号.Name = "label报关单号";
            this.label报关单号.Size = new System.Drawing.Size(83, 12);
            this.label报关单号.TabIndex = 0;
            this.label报关单号.Text = "label报关单号";
            // 
            // label生成时间
            // 
            this.label生成时间.AutoSize = true;
            this.label生成时间.Location = new System.Drawing.Point(12, 30);
            this.label生成时间.Name = "label生成时间";
            this.label生成时间.Size = new System.Drawing.Size(83, 12);
            this.label生成时间.TabIndex = 1;
            this.label生成时间.Text = "label生成时间";
            // 
            // txt异常内容
            // 
            this.txt异常内容.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt异常内容.Location = new System.Drawing.Point(12, 67);
            this.txt异常内容.Multiline = true;
            this.txt异常内容.Name = "txt异常内容";
            this.txt异常内容.ReadOnly = true;
            this.txt异常内容.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txt异常内容.Size = new System.Drawing.Size(434, 207);
            this.txt异常内容.TabIndex = 2;
            // 
            // label异常内容
            // 
            this.label异常内容.AutoSize = true;
            this.label异常内容.Location = new System.Drawing.Point(12, 52);
            this.label异常内容.Name = "label异常内容";
            this.label异常内容.Size = new System.Drawing.Size(65, 12);
            this.label异常内容.TabIndex = 3;
            this.label异常内容.Text = "异常内容：";
            // 
            // FormExceptionDetail
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(458, 286);
            this.Controls.Add(this.label异常内容);
            this.Controls.Add(this.txt异常内容);
            this.Controls.Add(this.label生成时间);
            this.Controls.Add(this.label报关单号);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormExceptionDetail";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "异常信息";
            this.Load += new System.EventHandler(this.FormExceptionDetail_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label报关单号;
        private System.Windows.Forms.Label label生成时间;
        private System.Windows.Forms.TextBox txt异常内容;
        private System.Windows.Forms.Label label异常内容;
    }
}