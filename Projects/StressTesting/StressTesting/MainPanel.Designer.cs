namespace StressTesting
{
    partial class MainPanel
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
            this.textHost = new System.Windows.Forms.TextBox();
            this.btnStartText = new System.Windows.Forms.Button();
            this.textPort = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textThread = new System.Windows.Forms.TextBox();
            this.textConnectionCount = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textMsg = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // textHost
            // 
            this.textHost.Font = new System.Drawing.Font("宋体", 9F);
            this.textHost.Location = new System.Drawing.Point(72, 41);
            this.textHost.MaxLength = 15;
            this.textHost.Name = "textHost";
            this.textHost.Size = new System.Drawing.Size(100, 21);
            this.textHost.TabIndex = 0;
            this.textHost.Tag = "";
            this.textHost.Text = "127.0.0.1";
            // 
            // btnStartText
            // 
            this.btnStartText.Font = new System.Drawing.Font("宋体", 9F);
            this.btnStartText.Location = new System.Drawing.Point(21, 175);
            this.btnStartText.Name = "btnStartText";
            this.btnStartText.Size = new System.Drawing.Size(75, 31);
            this.btnStartText.TabIndex = 5;
            this.btnStartText.Text = "Start Test";
            this.btnStartText.UseVisualStyleBackColor = true;
            this.btnStartText.Click += new System.EventHandler(this.btnStartText_Click);
            // 
            // textPort
            // 
            this.textPort.Font = new System.Drawing.Font("宋体", 9F);
            this.textPort.Location = new System.Drawing.Point(231, 41);
            this.textPort.MaxLength = 5;
            this.textPort.Name = "textPort";
            this.textPort.Size = new System.Drawing.Size(62, 21);
            this.textPort.TabIndex = 1;
            this.textPort.Tag = "";
            this.textPort.Text = "1875";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "Host：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(184, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "Port:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 79);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "ThreadCount:";
            // 
            // textThread
            // 
            this.textThread.Font = new System.Drawing.Font("宋体", 9F);
            this.textThread.Location = new System.Drawing.Point(104, 75);
            this.textThread.MaxLength = 15;
            this.textThread.Name = "textThread";
            this.textThread.Size = new System.Drawing.Size(70, 21);
            this.textThread.TabIndex = 2;
            this.textThread.Tag = "";
            this.textThread.Text = "10";
            // 
            // textConnectionCount
            // 
            this.textConnectionCount.Font = new System.Drawing.Font("宋体", 9F);
            this.textConnectionCount.Location = new System.Drawing.Point(363, 75);
            this.textConnectionCount.MaxLength = 15;
            this.textConnectionCount.Name = "textConnectionCount";
            this.textConnectionCount.Size = new System.Drawing.Size(70, 21);
            this.textConnectionCount.TabIndex = 3;
            this.textConnectionCount.Tag = "";
            this.textConnectionCount.Text = "100";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(182, 79);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(173, 12);
            this.label4.TabIndex = 7;
            this.label4.Text = "Connection Count Per Thread:";
            // 
            // textMsg
            // 
            this.textMsg.Font = new System.Drawing.Font("宋体", 9F);
            this.textMsg.Location = new System.Drawing.Point(54, 118);
            this.textMsg.MaxLength = 15;
            this.textMsg.Name = "textMsg";
            this.textMsg.Size = new System.Drawing.Size(303, 21);
            this.textMsg.TabIndex = 4;
            this.textMsg.Tag = "";
            this.textMsg.Text = "hello";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(19, 122);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 12);
            this.label5.TabIndex = 9;
            this.label5.Text = "Msg:";
            // 
            // MainPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(689, 393);
            this.Controls.Add(this.textMsg);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textConnectionCount);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textThread);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textPort);
            this.Controls.Add(this.btnStartText);
            this.Controls.Add(this.textHost);
            this.Name = "MainPanel";
            this.Text = "压力测试工具";
            this.Load += new System.EventHandler(this.MainPanel_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textHost;
        private System.Windows.Forms.Button btnStartText;
        private System.Windows.Forms.TextBox textPort;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textThread;
        private System.Windows.Forms.TextBox textConnectionCount;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textMsg;
        private System.Windows.Forms.Label label5;
    }
}