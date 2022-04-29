
namespace Server
{
    partial class frmMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            cDrawField = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(cDrawField)).BeginInit();
            this.SuspendLayout();
            // 
            // cDrawField
            // 
            cDrawField.Dock = System.Windows.Forms.DockStyle.Fill;
            cDrawField.Location = new System.Drawing.Point(0, 0);
            cDrawField.Name = "cDrawField";
            cDrawField.Size = new System.Drawing.Size(782, 553);
            cDrawField.TabIndex = 0;
            cDrawField.TabStop = false;
            cDrawField.MouseDown += new System.Windows.Forms.MouseEventHandler(this.cDrawField_MouseDown);
            cDrawField.MouseMove += new System.Windows.Forms.MouseEventHandler(this.cDrawField_MouseMove);
            cDrawField.MouseUp += new System.Windows.Forms.MouseEventHandler(this.cDrawField_MouseUp);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(782, 553);
            this.Controls.Add(cDrawField);
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Server";
            this.Shown += new System.EventHandler(this.frmMain_Shown);
            ((System.ComponentModel.ISupportInitialize)(cDrawField)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private static System.Windows.Forms.PictureBox cDrawField;
    }
}

