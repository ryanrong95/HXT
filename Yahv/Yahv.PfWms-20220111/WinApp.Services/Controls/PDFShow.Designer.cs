namespace WinApp.Services.Controls
{
    partial class PDFShow
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
            this.SuspendLayout();
            // 
            // PDFShow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1117, 633);
            this.Name = "PDFShow";
            this.Text = "PDFShow";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.PDFShow_FormClosed);
            this.Load += new System.EventHandler(this.PDFShow_Load);
            this.ResumeLayout(false);

        }

        #endregion
    }
}