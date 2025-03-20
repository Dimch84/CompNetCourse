namespace NetworksLab2FTPClient
{
    partial class FtpClientUI
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
            this.usernameRTxt = new System.Windows.Forms.RichTextBox();
            this.contentLstBx = new System.Windows.Forms.ListBox();
            this.passwordRTxt = new System.Windows.Forms.RichTextBox();
            this.serverAddressRTxt = new System.Windows.Forms.RichTextBox();
            this.connectBtn = new System.Windows.Forms.Button();
            this.utilityRTxt = new System.Windows.Forms.RichTextBox();
            this.createBtn = new System.Windows.Forms.Button();
            this.deleteBtn = new System.Windows.Forms.Button();
            this.updateBtn = new System.Windows.Forms.Button();
            this.retrieveBtn = new System.Windows.Forms.Button();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // usernameRTxt
            // 
            this.usernameRTxt.Location = new System.Drawing.Point(12, 12);
            this.usernameRTxt.Multiline = false;
            this.usernameRTxt.Name = "usernameRTxt";
            this.usernameRTxt.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.usernameRTxt.Size = new System.Drawing.Size(141, 25);
            this.usernameRTxt.TabIndex = 0;
            this.usernameRTxt.Text = "TestUser";
            this.usernameRTxt.TextChanged += new System.EventHandler(this.usernameRTxtChange);
            // 
            // contentLstBx
            // 
            this.contentLstBx.FormattingEnabled = true;
            this.contentLstBx.Location = new System.Drawing.Point(12, 74);
            this.contentLstBx.Name = "contentLstBx";
            this.contentLstBx.Size = new System.Drawing.Size(298, 212);
            this.contentLstBx.TabIndex = 1;
            // 
            // passwordRTxt
            // 
            this.passwordRTxt.Location = new System.Drawing.Point(12, 43);
            this.passwordRTxt.Multiline = false;
            this.passwordRTxt.Name = "passwordRTxt";
            this.passwordRTxt.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.passwordRTxt.Size = new System.Drawing.Size(141, 25);
            this.passwordRTxt.TabIndex = 2;
            this.passwordRTxt.Text = "123456";
            this.passwordRTxt.TextChanged += new System.EventHandler(this.passwordRTxtChange);
            // 
            // serverAddressRTxt
            // 
            this.serverAddressRTxt.Location = new System.Drawing.Point(159, 12);
            this.serverAddressRTxt.Multiline = false;
            this.serverAddressRTxt.Name = "serverAddressRTxt";
            this.serverAddressRTxt.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.serverAddressRTxt.Size = new System.Drawing.Size(151, 25);
            this.serverAddressRTxt.TabIndex = 3;
            this.serverAddressRTxt.Text = "127.0.0.1:21";
            this.serverAddressRTxt.TextChanged += new System.EventHandler(this.serverAddressRTxtChange);
            // 
            // connectBtn
            // 
            this.connectBtn.Location = new System.Drawing.Point(159, 45);
            this.connectBtn.Name = "connectBtn";
            this.connectBtn.Size = new System.Drawing.Size(151, 23);
            this.connectBtn.TabIndex = 4;
            this.connectBtn.Text = "Connect";
            this.connectBtn.UseVisualStyleBackColor = true;
            this.connectBtn.Click += new System.EventHandler(this.connectBtn_Click);
            // 
            // utilityRTxt
            // 
            this.utilityRTxt.Location = new System.Drawing.Point(12, 292);
            this.utilityRTxt.Name = "utilityRTxt";
            this.utilityRTxt.Size = new System.Drawing.Size(141, 24);
            this.utilityRTxt.TabIndex = 5;
            this.utilityRTxt.Text = "";
            // 
            // createBtn
            // 
            this.createBtn.Location = new System.Drawing.Point(235, 290);
            this.createBtn.Name = "createBtn";
            this.createBtn.Size = new System.Drawing.Size(75, 23);
            this.createBtn.TabIndex = 6;
            this.createBtn.Text = "Create";
            this.createBtn.UseVisualStyleBackColor = true;
            this.createBtn.Click += new System.EventHandler(this.createBtn_Click);
            // 
            // deleteBtn
            // 
            this.deleteBtn.Location = new System.Drawing.Point(235, 377);
            this.deleteBtn.Name = "deleteBtn";
            this.deleteBtn.Size = new System.Drawing.Size(75, 23);
            this.deleteBtn.TabIndex = 7;
            this.deleteBtn.Text = "Delete";
            this.deleteBtn.UseVisualStyleBackColor = true;
            this.deleteBtn.Click += new System.EventHandler(this.deleteBtnClick);
            // 
            // updateBtn
            // 
            this.updateBtn.Location = new System.Drawing.Point(235, 348);
            this.updateBtn.Name = "updateBtn";
            this.updateBtn.Size = new System.Drawing.Size(75, 23);
            this.updateBtn.TabIndex = 8;
            this.updateBtn.Text = "Update";
            this.updateBtn.UseVisualStyleBackColor = true;
            this.updateBtn.Click += new System.EventHandler(this.updateBtnClick);
            // 
            // retrieveBtn
            // 
            this.retrieveBtn.Location = new System.Drawing.Point(235, 319);
            this.retrieveBtn.Name = "retrieveBtn";
            this.retrieveBtn.Size = new System.Drawing.Size(75, 23);
            this.retrieveBtn.TabIndex = 9;
            this.retrieveBtn.Text = "Retrieve";
            this.retrieveBtn.UseVisualStyleBackColor = true;
            this.retrieveBtn.Click += new System.EventHandler(this.retrieveBtnClick);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // FtpClientUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(324, 451);
            this.Controls.Add(this.retrieveBtn);
            this.Controls.Add(this.updateBtn);
            this.Controls.Add(this.deleteBtn);
            this.Controls.Add(this.createBtn);
            this.Controls.Add(this.utilityRTxt);
            this.Controls.Add(this.connectBtn);
            this.Controls.Add(this.serverAddressRTxt);
            this.Controls.Add(this.passwordRTxt);
            this.Controls.Add(this.contentLstBx);
            this.Controls.Add(this.usernameRTxt);
            this.Name = "FtpClientUI";
            this.Text = "Ftp Client";
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox usernameRTxt;
        private System.Windows.Forms.ListBox contentLstBx;
        private System.Windows.Forms.RichTextBox passwordRTxt;
        private System.Windows.Forms.RichTextBox serverAddressRTxt;
        private System.Windows.Forms.Button connectBtn;
        private System.Windows.Forms.RichTextBox utilityRTxt;
        private System.Windows.Forms.Button createBtn;
        private System.Windows.Forms.Button deleteBtn;
        private System.Windows.Forms.Button updateBtn;
        private System.Windows.Forms.Button retrieveBtn;
        private System.Windows.Forms.ErrorProvider errorProvider1;
    }
}

