namespace WinApp
{
    partial class TestForm
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
            this.btnPhoto = new System.Windows.Forms.Button();
            this.btnTestUpload = new System.Windows.Forms.Button();
            this.btnSelector = new System.Windows.Forms.Button();
            this.btnKdn = new System.Windows.Forms.Button();
            this.btnOuputNotice = new System.Windows.Forms.Button();
            this.btnKdnKysy = new System.Windows.Forms.Button();
            this.btnDeliveryList = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnPhoto
            // 
            this.btnPhoto.Location = new System.Drawing.Point(12, 12);
            this.btnPhoto.Name = "btnPhoto";
            this.btnPhoto.Size = new System.Drawing.Size(75, 23);
            this.btnPhoto.TabIndex = 0;
            this.btnPhoto.Text = "显示拍照";
            this.btnPhoto.UseVisualStyleBackColor = true;
            this.btnPhoto.Click += new System.EventHandler(this.btnPhoto_Click);
            // 
            // btnTestUpload
            // 
            this.btnTestUpload.Location = new System.Drawing.Point(12, 41);
            this.btnTestUpload.Name = "btnTestUpload";
            this.btnTestUpload.Size = new System.Drawing.Size(75, 23);
            this.btnTestUpload.TabIndex = 1;
            this.btnTestUpload.Text = "测试上传";
            this.btnTestUpload.UseVisualStyleBackColor = true;
            this.btnTestUpload.Click += new System.EventHandler(this.测试上传_Click);
            // 
            // btnSelector
            // 
            this.btnSelector.Location = new System.Drawing.Point(12, 70);
            this.btnSelector.Name = "btnSelector";
            this.btnSelector.Size = new System.Drawing.Size(75, 23);
            this.btnSelector.TabIndex = 2;
            this.btnSelector.Text = "文件选择";
            this.btnSelector.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.btnSelector.UseVisualStyleBackColor = true;
            this.btnSelector.Click += new System.EventHandler(this.btnSelector_Click);
            // 
            // btnKdn
            // 
            this.btnKdn.Location = new System.Drawing.Point(12, 100);
            this.btnKdn.Name = "btnKdn";
            this.btnKdn.Size = new System.Drawing.Size(113, 23);
            this.btnKdn.TabIndex = 3;
            this.btnKdn.Text = "测试快递鸟(顺丰)";
            this.btnKdn.UseVisualStyleBackColor = true;
            this.btnKdn.Click += new System.EventHandler(this.btnKdn_Click);
            // 
            // btnOuputNotice
            // 
            this.btnOuputNotice.Location = new System.Drawing.Point(12, 129);
            this.btnOuputNotice.Name = "btnOuputNotice";
            this.btnOuputNotice.Size = new System.Drawing.Size(113, 23);
            this.btnOuputNotice.TabIndex = 4;
            this.btnOuputNotice.Text = "测试出库通知单";
            this.btnOuputNotice.UseVisualStyleBackColor = true;
            this.btnOuputNotice.Click += new System.EventHandler(this.btnOuputNotice_Click);
            // 
            // btnKdnKysy
            // 
            this.btnKdnKysy.Location = new System.Drawing.Point(131, 100);
            this.btnKdnKysy.Name = "btnKdnKysy";
            this.btnKdnKysy.Size = new System.Drawing.Size(135, 23);
            this.btnKdnKysy.TabIndex = 5;
            this.btnKdnKysy.Text = "测试快递鸟(跨越速运)";
            this.btnKdnKysy.UseVisualStyleBackColor = true;
            this.btnKdnKysy.Click += new System.EventHandler(this.btnKdnKysy_Click);
            // 
            // btnDeliveryList
            // 
            this.btnDeliveryList.Location = new System.Drawing.Point(12, 158);
            this.btnDeliveryList.Name = "btnDeliveryList";
            this.btnDeliveryList.Size = new System.Drawing.Size(75, 23);
            this.btnDeliveryList.TabIndex = 4;
            this.btnDeliveryList.Text = "测试送货文件";
            this.btnDeliveryList.UseVisualStyleBackColor = true;
            this.btnDeliveryList.Click += new System.EventHandler(this.btnDeliveryList_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(167, 168);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 6;
            this.button1.Text = "显示图片";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(13, 202);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 7;
            this.button2.Text = "打印PDF";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(167, 202);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 8;
            this.button3.Text = "打印送货单";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(167, 13);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(156, 23);
            this.button4.TabIndex = 9;
            this.button4.Text = "顺丰/跨越第二次以上打印";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(167, 42);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(121, 23);
            this.button5.TabIndex = 10;
            this.button5.Text = "EMS测试打印";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // TestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(374, 309);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnKdnKysy);
            this.Controls.Add(this.btnDeliveryList);
            this.Controls.Add(this.btnOuputNotice);
            this.Controls.Add(this.btnKdn);
            this.Controls.Add(this.btnSelector);
            this.Controls.Add(this.btnTestUpload);
            this.Controls.Add(this.btnPhoto);
            this.Name = "TestForm";
            this.Text = "TestForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnPhoto;
        private System.Windows.Forms.Button btnTestUpload;
        private System.Windows.Forms.Button btnSelector;
        private System.Windows.Forms.Button btnKdn;
        private System.Windows.Forms.Button btnOuputNotice;
        private System.Windows.Forms.Button btnKdnKysy;
        private System.Windows.Forms.Button btnDeliveryList;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
    }
}