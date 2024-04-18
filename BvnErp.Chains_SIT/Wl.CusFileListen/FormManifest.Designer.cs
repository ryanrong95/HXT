namespace Wl.CusFileListen
{
    partial class FormManifest
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
            this.components = new System.ComponentModel.Container();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.label1 = new System.Windows.Forms.Label();
            this.tbContractNo = new System.Windows.Forms.TextBox();
            this.BtnSearch = new System.Windows.Forms.Button();
            this.BtnDeclare = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.tbBillNo = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbVoyageNo = new System.Windows.Forms.TextBox();
            this.timerManifest = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1});
            this.dataGridView1.Location = new System.Drawing.Point(0, 51);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(801, 393);
            this.dataGridView1.TabIndex = 8;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);
            // 
            // Column1
            // 
            this.Column1.HeaderText = "选择";
            this.Column1.Name = "Column1";
            this.Column1.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Column1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Column1.Width = 40;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "合同号";
            // 
            // tbContractNo
            // 
            this.tbContractNo.Location = new System.Drawing.Point(61, 10);
            this.tbContractNo.Name = "tbContractNo";
            this.tbContractNo.Size = new System.Drawing.Size(100, 20);
            this.tbContractNo.TabIndex = 6;
            // 
            // BtnSearch
            // 
            this.BtnSearch.Location = new System.Drawing.Point(602, 10);
            this.BtnSearch.Name = "BtnSearch";
            this.BtnSearch.Size = new System.Drawing.Size(75, 23);
            this.BtnSearch.TabIndex = 5;
            this.BtnSearch.Text = "查询";
            this.BtnSearch.UseVisualStyleBackColor = true;
            this.BtnSearch.Click += new System.EventHandler(this.BtnSearch_Click);
            // 
            // BtnDeclare
            // 
            this.BtnDeclare.Location = new System.Drawing.Point(703, 11);
            this.BtnDeclare.Name = "BtnDeclare";
            this.BtnDeclare.Size = new System.Drawing.Size(75, 23);
            this.BtnDeclare.TabIndex = 9;
            this.BtnDeclare.Text = "申报";
            this.BtnDeclare.UseVisualStyleBackColor = true;
            this.BtnDeclare.Click += new System.EventHandler(this.BtnDeclare_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(188, 14);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "运单号";
            // 
            // tbBillNo
            // 
            this.tbBillNo.Location = new System.Drawing.Point(237, 11);
            this.tbBillNo.Name = "tbBillNo";
            this.tbBillNo.Size = new System.Drawing.Size(100, 20);
            this.tbBillNo.TabIndex = 10;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(365, 14);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "批次号";
            // 
            // tbVoyageNo
            // 
            this.tbVoyageNo.Location = new System.Drawing.Point(414, 11);
            this.tbVoyageNo.Name = "tbVoyageNo";
            this.tbVoyageNo.Size = new System.Drawing.Size(100, 20);
            this.tbVoyageNo.TabIndex = 12;
            // 
            // timerManifest
            // 
            this.timerManifest.Enabled = true;
            this.timerManifest.Interval = 120000;
            this.timerManifest.Tick += new System.EventHandler(this.timerManifest_Tick);
            // 
            // FormManifest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbVoyageNo);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbBillNo);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbContractNo);
            this.Controls.Add(this.BtnSearch);
            this.Controls.Add(this.BtnDeclare);
            this.Name = "FormManifest";
            this.Text = "舱单";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Column1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbContractNo;
        private System.Windows.Forms.Button BtnSearch;
        private System.Windows.Forms.Button BtnDeclare;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbBillNo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbVoyageNo;
        private System.Windows.Forms.Timer timerManifest;
    }
}