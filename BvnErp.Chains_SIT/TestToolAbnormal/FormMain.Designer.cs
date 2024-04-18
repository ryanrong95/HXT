namespace TestToolAbnormal
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
            this.btnEnterQueue = new System.Windows.Forms.Button();
            this.btnRunStep = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.btnStopHandlerTimer = new System.Windows.Forms.Button();
            this.btnStartHandlerTimer = new System.Windows.Forms.Button();
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
            // btnEnterQueue
            // 
            this.btnEnterQueue.Location = new System.Drawing.Point(272, 4);
            this.btnEnterQueue.Name = "btnEnterQueue";
            this.btnEnterQueue.Size = new System.Drawing.Size(75, 23);
            this.btnEnterQueue.TabIndex = 1;
            this.btnEnterQueue.Text = "EnterQueue";
            this.btnEnterQueue.UseVisualStyleBackColor = true;
            this.btnEnterQueue.Click += new System.EventHandler(this.btnEnterQueue_Click);
            // 
            // btnRunStep
            // 
            this.btnRunStep.Location = new System.Drawing.Point(374, 4);
            this.btnRunStep.Name = "btnRunStep";
            this.btnRunStep.Size = new System.Drawing.Size(75, 23);
            this.btnRunStep.TabIndex = 2;
            this.btnRunStep.Text = "RunStep";
            this.btnRunStep.UseVisualStyleBackColor = true;
            this.btnRunStep.Click += new System.EventHandler(this.btnRunStep_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "label2";
            // 
            // btnStopHandlerTimer
            // 
            this.btnStopHandlerTimer.Location = new System.Drawing.Point(634, 4);
            this.btnStopHandlerTimer.Name = "btnStopHandlerTimer";
            this.btnStopHandlerTimer.Size = new System.Drawing.Size(126, 23);
            this.btnStopHandlerTimer.TabIndex = 4;
            this.btnStopHandlerTimer.Text = "StopHandlerTimer";
            this.btnStopHandlerTimer.UseVisualStyleBackColor = true;
            this.btnStopHandlerTimer.Click += new System.EventHandler(this.btnStopHandlerTimer_Click);
            // 
            // btnStartHandlerTimer
            // 
            this.btnStartHandlerTimer.Location = new System.Drawing.Point(477, 4);
            this.btnStartHandlerTimer.Name = "btnStartHandlerTimer";
            this.btnStartHandlerTimer.Size = new System.Drawing.Size(129, 23);
            this.btnStartHandlerTimer.TabIndex = 5;
            this.btnStartHandlerTimer.Text = "StartHandlerTimer";
            this.btnStartHandlerTimer.UseVisualStyleBackColor = true;
            this.btnStartHandlerTimer.Click += new System.EventHandler(this.btnStartHandlerTimer_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnStartHandlerTimer);
            this.Controls.Add(this.btnStopHandlerTimer);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnRunStep);
            this.Controls.Add(this.btnEnterQueue);
            this.Controls.Add(this.label1);
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormMain";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnEnterQueue;
        private System.Windows.Forms.Button btnRunStep;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnStopHandlerTimer;
        private System.Windows.Forms.Button btnStartHandlerTimer;
    }
}