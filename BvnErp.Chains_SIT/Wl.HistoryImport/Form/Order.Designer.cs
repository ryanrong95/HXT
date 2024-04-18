namespace Wl.HistoryImport
{
    partial class Order
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
            this.btnOrderImport = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnOrderImport
            // 
            this.btnOrderImport.Location = new System.Drawing.Point(308, 53);
            this.btnOrderImport.Name = "btnOrderImport";
            this.btnOrderImport.Size = new System.Drawing.Size(75, 23);
            this.btnOrderImport.TabIndex = 0;
            this.btnOrderImport.Text = "订单";
            this.btnOrderImport.UseVisualStyleBackColor = true;
            this.btnOrderImport.Click += new System.EventHandler(this.btnOrderImport_Click);
            // 
            // Order
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnOrderImport);
            this.Name = "Order";
            this.Text = "Order";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOrderImport;
    }
}