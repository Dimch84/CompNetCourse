namespace NetworkRouting
{
    partial class Form1
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
            this.randomSeedBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.generateButton = new System.Windows.Forms.Button();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.seedUsedLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.sizeBox = new System.Windows.Forms.TextBox();
            this.solveButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.sourceNodeBox = new System.Windows.Forms.TextBox();
            this.targetNodeBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.pathCostBox = new System.Windows.Forms.TextBox();
            this.timeLabel = new System.Windows.Forms.Label();
            this.allTimeBox = new System.Windows.Forms.TextBox();
            this.oneTimeBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.differenceBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // randomSeedBox
            // 
            this.randomSeedBox.Location = new System.Drawing.Point(94, 318);
            this.randomSeedBox.Name = "randomSeedBox";
            this.randomSeedBox.Size = new System.Drawing.Size(100, 20);
            this.randomSeedBox.TabIndex = 1;
            this.randomSeedBox.Text = "1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 321);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Random Seed";
            // 
            // generateButton
            // 
            this.generateButton.Location = new System.Drawing.Point(338, 315);
            this.generateButton.Name = "generateButton";
            this.generateButton.Size = new System.Drawing.Size(75, 23);
            this.generateButton.TabIndex = 3;
            this.generateButton.Text = "Generate";
            this.generateButton.UseVisualStyleBackColor = true;
            this.generateButton.Click += new System.EventHandler(this.generateButton_Click);
            // 
            // pictureBox
            // 
            this.pictureBox.BackColor = System.Drawing.Color.White;
            this.pictureBox.Location = new System.Drawing.Point(16, 12);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(600, 300);
            this.pictureBox.TabIndex = 4;
            this.pictureBox.TabStop = false;
            this.pictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseDown);
            // 
            // seedUsedLabel
            // 
            this.seedUsedLabel.AutoSize = true;
            this.seedUsedLabel.Location = new System.Drawing.Point(483, 352);
            this.seedUsedLabel.Name = "seedUsedLabel";
            this.seedUsedLabel.Size = new System.Drawing.Size(133, 13);
            this.seedUsedLabel.TabIndex = 5;
            this.seedUsedLabel.Text = "Random Seed Used: none";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(199, 321);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(27, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Size";
            // 
            // sizeBox
            // 
            this.sizeBox.Location = new System.Drawing.Point(232, 318);
            this.sizeBox.Name = "sizeBox";
            this.sizeBox.Size = new System.Drawing.Size(100, 20);
            this.sizeBox.TabIndex = 6;
            this.sizeBox.Text = "20";
            // 
            // solveButton
            // 
            this.solveButton.Location = new System.Drawing.Point(419, 315);
            this.solveButton.Name = "solveButton";
            this.solveButton.Size = new System.Drawing.Size(75, 23);
            this.solveButton.TabIndex = 8;
            this.solveButton.Text = "Solve";
            this.solveButton.UseVisualStyleBackColor = true;
            this.solveButton.Click += new System.EventHandler(this.solveButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 350);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Source Node";
            // 
            // sourceNodeBox
            // 
            this.sourceNodeBox.Enabled = false;
            this.sourceNodeBox.Location = new System.Drawing.Point(89, 344);
            this.sourceNodeBox.Name = "sourceNodeBox";
            this.sourceNodeBox.Size = new System.Drawing.Size(25, 20);
            this.sourceNodeBox.TabIndex = 11;
            // 
            // targetNodeBox
            // 
            this.targetNodeBox.Enabled = false;
            this.targetNodeBox.Location = new System.Drawing.Point(193, 343);
            this.targetNodeBox.Name = "targetNodeBox";
            this.targetNodeBox.Size = new System.Drawing.Size(25, 20);
            this.targetNodeBox.TabIndex = 13;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(120, 350);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(67, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Target Node";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(324, 350);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(80, 13);
            this.label5.TabIndex = 15;
            this.label5.Text = "Total Path Cost";
            // 
            // pathCostBox
            // 
            this.pathCostBox.Location = new System.Drawing.Point(410, 344);
            this.pathCostBox.Name = "pathCostBox";
            this.pathCostBox.Size = new System.Drawing.Size(67, 20);
            this.pathCostBox.TabIndex = 16;
            // 
            // timeLabel
            // 
            this.timeLabel.AutoSize = true;
            this.timeLabel.Location = new System.Drawing.Point(13, 373);
            this.timeLabel.Name = "timeLabel";
            this.timeLabel.Size = new System.Drawing.Size(74, 13);
            this.timeLabel.TabIndex = 17;
            this.timeLabel.Text = "All Paths Time";
            // 
            // allTimeBox
            // 
            this.allTimeBox.Location = new System.Drawing.Point(89, 370);
            this.allTimeBox.Name = "allTimeBox";
            this.allTimeBox.Size = new System.Drawing.Size(58, 20);
            this.allTimeBox.TabIndex = 18;
            // 
            // oneTimeBox
            // 
            this.oneTimeBox.Location = new System.Drawing.Point(237, 370);
            this.oneTimeBox.Name = "oneTimeBox";
            this.oneTimeBox.Size = new System.Drawing.Size(58, 20);
            this.oneTimeBox.TabIndex = 20;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(153, 373);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(78, 13);
            this.label6.TabIndex = 19;
            this.label6.Text = "One Path Time";
            // 
            // differenceBox
            // 
            this.differenceBox.Location = new System.Drawing.Point(385, 370);
            this.differenceBox.Name = "differenceBox";
            this.differenceBox.Size = new System.Drawing.Size(58, 20);
            this.differenceBox.TabIndex = 22;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(312, 373);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(67, 13);
            this.label7.TabIndex = 21;
            this.label7.Text = "% Difference";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(636, 392);
            this.Controls.Add(this.differenceBox);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.oneTimeBox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.allTimeBox);
            this.Controls.Add(this.timeLabel);
            this.Controls.Add(this.pathCostBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.targetNodeBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.sourceNodeBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.solveButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.sizeBox);
            this.Controls.Add(this.seedUsedLabel);
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.generateButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.randomSeedBox);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox randomSeedBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button generateButton;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Label seedUsedLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox sizeBox;
        private System.Windows.Forms.Button solveButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox sourceNodeBox;
        private System.Windows.Forms.TextBox targetNodeBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox pathCostBox;
        private System.Windows.Forms.Label timeLabel;
        private System.Windows.Forms.TextBox allTimeBox;
        private System.Windows.Forms.TextBox oneTimeBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox differenceBox;
        private System.Windows.Forms.Label label7;
    }
}

