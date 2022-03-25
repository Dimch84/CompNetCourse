namespace NetworksLab2FTPClient
{
    partial class FileCreation
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
            this.contentRTxt = new System.Windows.Forms.RichTextBox();
            this.sendBtn = new System.Windows.Forms.Button();
            this.titleTxt = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // contentRTxt
            // 
            this.contentRTxt.Location = new System.Drawing.Point(12, 38);
            this.contentRTxt.Name = "contentRTxt";
            this.contentRTxt.Size = new System.Drawing.Size(413, 283);
            this.contentRTxt.TabIndex = 0;
            this.contentRTxt.Text = "";
            // 
            // sendBtn
            // 
            this.sendBtn.Location = new System.Drawing.Point(341, 12);
            this.sendBtn.Name = "sendBtn";
            this.sendBtn.Size = new System.Drawing.Size(75, 23);
            this.sendBtn.TabIndex = 1;
            this.sendBtn.Text = "Send";
            this.sendBtn.UseVisualStyleBackColor = true;
            this.sendBtn.Click += new System.EventHandler(this.sendBtn_Click);
            // 
            // titleTxt
            // 
            this.titleTxt.BackColor = System.Drawing.SystemColors.Control;
            this.titleTxt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.titleTxt.Location = new System.Drawing.Point(21, 15);
            this.titleTxt.Name = "titleTxt";
            this.titleTxt.Size = new System.Drawing.Size(100, 13);
            this.titleTxt.TabIndex = 2;
            // 
            // FileCreation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(437, 333);
            this.Controls.Add(this.titleTxt);
            this.Controls.Add(this.sendBtn);
            this.Controls.Add(this.contentRTxt);
            this.Name = "FileCreation";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox contentRTxt;
        private System.Windows.Forms.Button sendBtn;
        private System.Windows.Forms.TextBox titleTxt;
    }
}