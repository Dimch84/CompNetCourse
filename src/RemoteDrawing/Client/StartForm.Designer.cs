
namespace Client
{
    partial class frmStart
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
            this.fieldIP = new System.Windows.Forms.TextBox();
            this.fieldPort = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnEstablishConnection = new System.Windows.Forms.Button();
            this.lblConnectionInfo = new System.Windows.Forms.Label();
            lblServerMessage = new System.Windows.Forms.Label();
            lblStatus = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.Label10 = new System.Windows.Forms.Label();
            this.btnClear = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(133, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "IP адрес сервера: ";
            // 
            // fieldIP
            // 
            this.fieldIP.Location = new System.Drawing.Point(151, 22);
            this.fieldIP.Name = "fieldIP";
            this.fieldIP.Size = new System.Drawing.Size(154, 27);
            this.fieldIP.TabIndex = 1;
            // 
            // fieldPort
            // 
            this.fieldPort.Location = new System.Drawing.Point(151, 59);
            this.fieldPort.Name = "fieldPort";
            this.fieldPort.Size = new System.Drawing.Size(154, 27);
            this.fieldPort.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 62);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(109, 20);
            this.label2.TabIndex = 2;
            this.label2.Text = "Номер порта: ";
            // 
            // btnEstablishConnection
            // 
            this.btnEstablishConnection.Location = new System.Drawing.Point(12, 114);
            this.btnEstablishConnection.Name = "btnEstablishConnection";
            this.btnEstablishConnection.Size = new System.Drawing.Size(458, 34);
            this.btnEstablishConnection.TabIndex = 4;
            this.btnEstablishConnection.Text = "Установить соединение с сервером";
            this.btnEstablishConnection.UseVisualStyleBackColor = true;
            this.btnEstablishConnection.Click += new System.EventHandler(this.btnEstablishConnection_Click);
            // 
            // lblConnectionInfo
            // 
            this.lblConnectionInfo.AutoSize = true;
            this.lblConnectionInfo.Location = new System.Drawing.Point(12, 170);
            this.lblConnectionInfo.Name = "lblConnectionInfo";
            this.lblConnectionInfo.Size = new System.Drawing.Size(0, 20);
            this.lblConnectionInfo.TabIndex = 5;
            // 
            // lblServerMessage
            // 
            lblServerMessage.AutoSize = true;
            lblServerMessage.Location = new System.Drawing.Point(202, 209);
            lblServerMessage.Name = "lblServerMessage";
            lblServerMessage.Size = new System.Drawing.Size(13, 20);
            lblServerMessage.TabIndex = 17;
            lblServerMessage.Text = " ";
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new System.Drawing.Point(178, 170);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new System.Drawing.Size(108, 20);
            lblStatus.TabIndex = 16;
            lblStatus.Text = "не подключен";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(18, 209);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(178, 20);
            this.label3.TabIndex = 15;
            this.label3.Text = "Сообщения от сервера: ";
            // 
            // Label10
            // 
            this.Label10.AutoSize = true;
            this.Label10.Location = new System.Drawing.Point(18, 170);
            this.Label10.Name = "Label10";
            this.Label10.Size = new System.Drawing.Size(154, 20);
            this.Label10.TabIndex = 14;
            this.Label10.Text = "Статус подключения:";
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(452, 145);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(17, 16);
            this.btnClear.TabIndex = 18;
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // frmStart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(482, 264);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(lblServerMessage);
            this.Controls.Add(lblStatus);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.Label10);
            this.Controls.Add(this.lblConnectionInfo);
            this.Controls.Add(this.btnEstablishConnection);
            this.Controls.Add(this.fieldPort);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.fieldIP);
            this.Controls.Add(this.label1);
            this.Name = "frmStart";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = " ";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox fieldIP;
        private System.Windows.Forms.TextBox fieldPort;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnEstablishConnection;
        private System.Windows.Forms.Label lblConnectionInfo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label Label10;
        private System.Windows.Forms.Button btnClear;
        public static System.Windows.Forms.Label lblServerMessage;
        public static System.Windows.Forms.Label lblStatus;
    }
}