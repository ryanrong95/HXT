namespace Yahv.PsWms.DappForm.Controls
{
    partial class UcStatusBar
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

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.txtPrintStatus = new System.Windows.Forms.TextBox();
            this.txtTransferStatus = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 300F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 300F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.tableLayoutPanel1.Controls.Add(this.txtPrintStatus, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.txtTransferStatus, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(800, 243);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // txtPrintStatus
            // 
            this.txtPrintStatus.BackColor = System.Drawing.SystemColors.Control;
            this.txtPrintStatus.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtPrintStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPrintStatus.Location = new System.Drawing.Point(3, 3);
            this.txtPrintStatus.Multiline = true;
            this.txtPrintStatus.Name = "txtPrintStatus";
            this.txtPrintStatus.ReadOnly = true;
            this.txtPrintStatus.Size = new System.Drawing.Size(294, 237);
            this.txtPrintStatus.TabIndex = 0;
            this.txtPrintStatus.Text = "打印状态";
            // 
            // txtTransferStatus
            // 
            this.txtTransferStatus.BackColor = System.Drawing.SystemColors.Control;
            this.txtTransferStatus.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtTransferStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtTransferStatus.Location = new System.Drawing.Point(303, 3);
            this.txtTransferStatus.Multiline = true;
            this.txtTransferStatus.Name = "txtTransferStatus";
            this.txtTransferStatus.ReadOnly = true;
            this.txtTransferStatus.Size = new System.Drawing.Size(294, 237);
            this.txtTransferStatus.TabIndex = 1;
            this.txtTransferStatus.Text = "上载与下载状态";
            // 
            // UcStatusBar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "UcStatusBar";
            this.Size = new System.Drawing.Size(800, 243);
            this.Load += new System.EventHandler(this.UcStatusBar_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox txtPrintStatus;
        private System.Windows.Forms.TextBox txtTransferStatus;
    }
}
