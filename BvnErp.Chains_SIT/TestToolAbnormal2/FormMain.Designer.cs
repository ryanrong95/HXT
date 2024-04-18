namespace TestToolAbnormal2
{
    partial class FormMain
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnEnterQueueXml = new System.Windows.Forms.Button();
            this.btnRunStep = new System.Windows.Forms.Button();
            this.btnStartHandlerTimer = new System.Windows.Forms.Button();
            this.btnStopHandlerTimer = new System.Windows.Forms.Button();
            this.btnEnterQueueFailBox = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "label1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "label2";
            // 
            // btnEnterQueueXml
            // 
            this.btnEnterQueueXml.Location = new System.Drawing.Point(14, 83);
            this.btnEnterQueueXml.Name = "btnEnterQueueXml";
            this.btnEnterQueueXml.Size = new System.Drawing.Size(118, 23);
            this.btnEnterQueueXml.TabIndex = 2;
            this.btnEnterQueueXml.Text = "EnterQueueXml";
            this.btnEnterQueueXml.UseVisualStyleBackColor = true;
            this.btnEnterQueueXml.Click += new System.EventHandler(this.btnEnterQueueXml_Click);
            // 
            // btnRunStep
            // 
            this.btnRunStep.Location = new System.Drawing.Point(14, 132);
            this.btnRunStep.Name = "btnRunStep";
            this.btnRunStep.Size = new System.Drawing.Size(118, 23);
            this.btnRunStep.TabIndex = 3;
            this.btnRunStep.Text = "RunStep";
            this.btnRunStep.UseVisualStyleBackColor = true;
            this.btnRunStep.Click += new System.EventHandler(this.btnRunStep_Click);
            // 
            // btnStartHandlerTimer
            // 
            this.btnStartHandlerTimer.Location = new System.Drawing.Point(14, 186);
            this.btnStartHandlerTimer.Name = "btnStartHandlerTimer";
            this.btnStartHandlerTimer.Size = new System.Drawing.Size(118, 23);
            this.btnStartHandlerTimer.TabIndex = 4;
            this.btnStartHandlerTimer.Text = "StartHandlerTimer";
            this.btnStartHandlerTimer.UseVisualStyleBackColor = true;
            this.btnStartHandlerTimer.Click += new System.EventHandler(this.btnStartHandlerTimer_Click);
            // 
            // btnStopHandlerTimer
            // 
            this.btnStopHandlerTimer.Location = new System.Drawing.Point(158, 186);
            this.btnStopHandlerTimer.Name = "btnStopHandlerTimer";
            this.btnStopHandlerTimer.Size = new System.Drawing.Size(118, 23);
            this.btnStopHandlerTimer.TabIndex = 5;
            this.btnStopHandlerTimer.Text = "StopHandlerTimer";
            this.btnStopHandlerTimer.UseVisualStyleBackColor = true;
            this.btnStopHandlerTimer.Click += new System.EventHandler(this.btnStopHandlerTimer_Click);
            // 
            // btnEnterQueueFailBox
            // 
            this.btnEnterQueueFailBox.Location = new System.Drawing.Point(158, 83);
            this.btnEnterQueueFailBox.Name = "btnEnterQueueFailBox";
            this.btnEnterQueueFailBox.Size = new System.Drawing.Size(118, 23);
            this.btnEnterQueueFailBox.TabIndex = 6;
            this.btnEnterQueueFailBox.Text = "EnterQueueFailBox";
            this.btnEnterQueueFailBox.UseVisualStyleBackColor = true;
            this.btnEnterQueueFailBox.Click += new System.EventHandler(this.btnEnterQueueFailBox_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnEnterQueueFailBox);
            this.Controls.Add(this.btnStopHandlerTimer);
            this.Controls.Add(this.btnStartHandlerTimer);
            this.Controls.Add(this.btnRunStep);
            this.Controls.Add(this.btnEnterQueueXml);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormMain";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnEnterQueueXml;
        private System.Windows.Forms.Button btnRunStep;
        private System.Windows.Forms.Button btnStartHandlerTimer;
        private System.Windows.Forms.Button btnStopHandlerTimer;
        private System.Windows.Forms.Button btnEnterQueueFailBox;
    }
}