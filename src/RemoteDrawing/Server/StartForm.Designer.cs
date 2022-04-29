
namespace Server
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
            this.btnEstablishConnection = new System.Windows.Forms.Button();
            this.fieldPort = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.fieldIP = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.Label10 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            lblStatus = new System.Windows.Forms.Label();
            lblServerMessage = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnEstablishConnection
            // 
            this.btnEstablishConnection.Location = new System.Drawing.Point(12, 108);
            this.btnEstablishConnection.Name = "btnEstablishConnection";
            this.btnEstablishConnection.Size = new System.Drawing.Size(458, 34);
            this.btnEstablishConnection.TabIndex = 9;
            this.btnEstablishConnection.Text = "Запустить сервер";
            this.btnEstablishConnection.UseVisualStyleBackColor = true;
            this.btnEstablishConnection.Click += new System.EventHandler(this.btnEstablishConnection_Click);
            // 
            // fieldPort
            // 
            this.fieldPort.Location = new System.Drawing.Point(151, 53);
            this.fieldPort.Name = "fieldPort";
            this.fieldPort.Size = new System.Drawing.Size(154, 27);
            this.fieldPort.TabIndex = 8;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(109, 20);
            this.label2.TabIndex = 7;
            this.label2.Text = "Номер порта: ";
            // 
            // fieldIP
            // 
            this.fieldIP.Location = new System.Drawing.Point(151, 16);
            this.fieldIP.Name = "fieldIP";
            this.fieldIP.Size = new System.Drawing.Size(154, 27);
            this.fieldIP.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(133, 20);
            this.label1.TabIndex = 5;
            this.label1.Text = "IP адрес сервера: ";
            // 
            // Label10
            // 
            this.Label10.AutoSize = true;
            this.Label10.Location = new System.Drawing.Point(12, 171);
            this.Label10.Name = "Label10";
            this.Label10.Size = new System.Drawing.Size(154, 20);
            this.Label10.TabIndex = 10;
            this.Label10.Text = "Статус подключения:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 210);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(178, 20);
            this.label3.TabIndex = 11;
            this.label3.Text = "Сообщения от сервера: ";
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new System.Drawing.Point(172, 171);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new System.Drawing.Size(108, 20);
            lblStatus.TabIndex = 12;
            lblStatus.Text = "не подключен";
            // 
            // lblServerMessage
            // 
            lblServerMessage.AutoSize = true;
            lblServerMessage.Location = new System.Drawing.Point(196, 210);
            lblServerMessage.Name = "lblServerMessage";
            lblServerMessage.Size = new System.Drawing.Size(13, 20);
            lblServerMessage.TabIndex = 13;
            lblServerMessage.Text = " ";
            // 
            // frmStart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(482, 256);
            this.Controls.Add(lblServerMessage);
            this.Controls.Add(lblStatus);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.Label10);
            this.Controls.Add(this.btnEstablishConnection);
            this.Controls.Add(this.fieldPort);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.fieldIP);
            this.Controls.Add(this.label1);
            this.Name = "frmStart";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Стартовое окно";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnEstablishConnection;
        private System.Windows.Forms.TextBox fieldPort;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox fieldIP;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label Label10;
        private System.Windows.Forms.Label label3;
        public static System.Windows.Forms.Label lblStatus;
        public static System.Windows.Forms.Label lblServerMessage;
    }
}