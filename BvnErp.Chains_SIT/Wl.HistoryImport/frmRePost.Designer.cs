namespace Wl.HistoryImport
{
    partial class frmRePost
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
            this.txtModel = new System.Windows.Forms.TextBox();
            this.btnPost = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnAllPost = new System.Windows.Forms.Button();
            this.dtPicker = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.btnInsidePost = new System.Windows.Forms.Button();
            this.btnInsideSinglePost = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtProductID = new System.Windows.Forms.TextBox();
            this.btnIcgooIDS = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtModel
            // 
            this.txtModel.Location = new System.Drawing.Point(171, 24);
            this.txtModel.Name = "txtModel";
            this.txtModel.Size = new System.Drawing.Size(306, 21);
            this.txtModel.TabIndex = 0;
            // 
            // btnPost
            // 
            this.btnPost.Location = new System.Drawing.Point(514, 174);
            this.btnPost.Name = "btnPost";
            this.btnPost.Size = new System.Drawing.Size(75, 23);
            this.btnPost.TabIndex = 1;
            this.btnPost.Text = "推送";
            this.btnPost.UseVisualStyleBackColor = true;
            this.btnPost.Click += new System.EventHandler(this.btnPost_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(32, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "型号";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(32, 108);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 20);
            this.label2.TabIndex = 3;
            this.label2.Text = "日期";
            // 
            // btnAllPost
            // 
            this.btnAllPost.Location = new System.Drawing.Point(623, 177);
            this.btnAllPost.Name = "btnAllPost";
            this.btnAllPost.Size = new System.Drawing.Size(75, 23);
            this.btnAllPost.TabIndex = 4;
            this.btnAllPost.Text = "一键推送";
            this.btnAllPost.UseVisualStyleBackColor = true;
            this.btnAllPost.Click += new System.EventHandler(this.btnAllPost_Click);
            // 
            // dtPicker
            // 
            this.dtPicker.Location = new System.Drawing.Point(171, 107);
            this.dtPicker.Name = "dtPicker";
            this.dtPicker.Size = new System.Drawing.Size(306, 21);
            this.dtPicker.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(32, 229);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(135, 20);
            this.label3.TabIndex = 6;
            this.label3.Text = "内单一键推送";
            // 
            // btnInsidePost
            // 
            this.btnInsidePost.Location = new System.Drawing.Point(623, 231);
            this.btnInsidePost.Name = "btnInsidePost";
            this.btnInsidePost.Size = new System.Drawing.Size(75, 23);
            this.btnInsidePost.TabIndex = 7;
            this.btnInsidePost.Text = "内单一键推送";
            this.btnInsidePost.UseVisualStyleBackColor = true;
            this.btnInsidePost.Click += new System.EventHandler(this.btnInsidePost_Click);
            // 
            // btnInsideSinglePost
            // 
            this.btnInsideSinglePost.Location = new System.Drawing.Point(514, 231);
            this.btnInsideSinglePost.Name = "btnInsideSinglePost";
            this.btnInsideSinglePost.Size = new System.Drawing.Size(75, 23);
            this.btnInsideSinglePost.TabIndex = 8;
            this.btnInsideSinglePost.Text = "内单推送";
            this.btnInsideSinglePost.UseVisualStyleBackColor = true;
            this.btnInsideSinglePost.Click += new System.EventHandler(this.btnInsideSinglePost_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(32, 177);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(148, 20);
            this.label4.TabIndex = 9;
            this.label4.Text = "Icgoo一键推送";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(32, 66);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(31, 20);
            this.label5.TabIndex = 10;
            this.label5.Text = "ID";
            // 
            // txtProductID
            // 
            this.txtProductID.Location = new System.Drawing.Point(171, 66);
            this.txtProductID.Name = "txtProductID";
            this.txtProductID.Size = new System.Drawing.Size(306, 21);
            this.txtProductID.TabIndex = 11;
            // 
            // btnIcgooIDS
            // 
            this.btnIcgooIDS.Location = new System.Drawing.Point(514, 291);
            this.btnIcgooIDS.Name = "btnIcgooIDS";
            this.btnIcgooIDS.Size = new System.Drawing.Size(75, 23);
            this.btnIcgooIDS.TabIndex = 12;
            this.btnIcgooIDS.Text = "内单ID";
            this.btnIcgooIDS.UseVisualStyleBackColor = true;
            this.btnIcgooIDS.Click += new System.EventHandler(this.btnIcgooIDS_Click);
            // 
            // frmRePost
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnIcgooIDS);
            this.Controls.Add(this.txtProductID);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnInsideSinglePost);
            this.Controls.Add(this.btnInsidePost);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.dtPicker);
            this.Controls.Add(this.btnAllPost);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnPost);
            this.Controls.Add(this.txtModel);
            this.Name = "frmRePost";
            this.Text = "frmRePost";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtModel;
        private System.Windows.Forms.Button btnPost;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnAllPost;
        private System.Windows.Forms.DateTimePicker dtPicker;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnInsidePost;
        private System.Windows.Forms.Button btnInsideSinglePost;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtProductID;
        private System.Windows.Forms.Button btnIcgooIDS;
    }
}