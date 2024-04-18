namespace Wl.HistoryImport
{
    partial class frmHistoryImport
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
            this.btnImport = new System.Windows.Forms.Button();
            this.btnTax = new System.Windows.Forms.Button();
            this.btnImportPayExchange = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnImport
            // 
            this.btnImport.Location = new System.Drawing.Point(432, 138);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(75, 23);
            this.btnImport.TabIndex = 0;
            this.btnImport.Text = "导入";
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // btnTax
            // 
            this.btnTax.Location = new System.Drawing.Point(432, 217);
            this.btnTax.Name = "btnTax";
            this.btnTax.Size = new System.Drawing.Size(75, 23);
            this.btnTax.TabIndex = 1;
            this.btnTax.Text = "导开票";
            this.btnTax.UseVisualStyleBackColor = true;
            this.btnTax.Click += new System.EventHandler(this.btnTax_Click);
            // 
            // btnImportPayExchange
            // 
            this.btnImportPayExchange.Location = new System.Drawing.Point(432, 288);
            this.btnImportPayExchange.Name = "btnImportPayExchange";
            this.btnImportPayExchange.Size = new System.Drawing.Size(75, 23);
            this.btnImportPayExchange.TabIndex = 2;
            this.btnImportPayExchange.Text = "导付汇";
            this.btnImportPayExchange.UseVisualStyleBackColor = true;
            this.btnImportPayExchange.Click += new System.EventHandler(this.btnImportPayExchange_Click);
            // 
            // frmHistoryImport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnImportPayExchange);
            this.Controls.Add(this.btnTax);
            this.Controls.Add(this.btnImport);
            this.Name = "frmHistoryImport";
            this.Text = "frmHistoryImport";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.Button btnTax;
        private System.Windows.Forms.Button btnImportPayExchange;
    }
}