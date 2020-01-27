namespace GUI
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
            this.DataServerBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.DBBox = new System.Windows.Forms.TextBox();
            this.SubmitButton = new System.Windows.Forms.Button();
            this.TreeViewBox = new System.Windows.Forms.TreeView();
            this.SelectLabel = new System.Windows.Forms.Label();
            this.SubmitSuppliers = new System.Windows.Forms.Button();
            this.SubmitLocationsButton = new System.Windows.Forms.Button();
            this.SubmitMaterialsButton = new System.Windows.Forms.Button();
            this.SubmitFormsButton = new System.Windows.Forms.Button();
            this.SubmitResults = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // DataServerBox
            // 
            this.DataServerBox.Location = new System.Drawing.Point(83, 12);
            this.DataServerBox.Name = "DataServerBox";
            this.DataServerBox.Size = new System.Drawing.Size(100, 20);
            this.DataServerBox.TabIndex = 1;
            this.DataServerBox.Text = "stagdata1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Data Server:";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(189, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Database:";
            // 
            // DBBox
            // 
            this.DBBox.Location = new System.Drawing.Point(251, 12);
            this.DBBox.Name = "DBBox";
            this.DBBox.Size = new System.Drawing.Size(100, 20);
            this.DBBox.TabIndex = 4;
            this.DBBox.Text = "csrhstest";
            // 
            // SubmitButton
            // 
            this.SubmitButton.Location = new System.Drawing.Point(357, 9);
            this.SubmitButton.Name = "SubmitButton";
            this.SubmitButton.Size = new System.Drawing.Size(75, 23);
            this.SubmitButton.TabIndex = 5;
            this.SubmitButton.Text = "Submit";
            this.SubmitButton.UseVisualStyleBackColor = true;
            this.SubmitButton.Click += new System.EventHandler(this.SubmitButton_Click);
            // 
            // TreeViewBox
            // 
            this.TreeViewBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TreeViewBox.CheckBoxes = true;
            this.TreeViewBox.Location = new System.Drawing.Point(16, 54);
            this.TreeViewBox.Name = "TreeViewBox";
            this.TreeViewBox.Size = new System.Drawing.Size(772, 384);
            this.TreeViewBox.TabIndex = 6;
            // 
            // SelectLabel
            // 
            this.SelectLabel.AutoSize = true;
            this.SelectLabel.Location = new System.Drawing.Point(13, 38);
            this.SelectLabel.Name = "SelectLabel";
            this.SelectLabel.Size = new System.Drawing.Size(136, 13);
            this.SelectLabel.TabIndex = 7;
            this.SelectLabel.Text = "Please Select the Suppliers";
            // 
            // SubmitSuppliers
            // 
            this.SubmitSuppliers.Location = new System.Drawing.Point(438, 9);
            this.SubmitSuppliers.Name = "SubmitSuppliers";
            this.SubmitSuppliers.Size = new System.Drawing.Size(75, 23);
            this.SubmitSuppliers.TabIndex = 8;
            this.SubmitSuppliers.Text = "Submit";
            this.SubmitSuppliers.UseVisualStyleBackColor = true;
            this.SubmitSuppliers.Visible = false;
            this.SubmitSuppliers.Click += new System.EventHandler(this.SubmitSuppliers_Click);
            // 
            // SubmitLocationsButton
            // 
            this.SubmitLocationsButton.Location = new System.Drawing.Point(519, 9);
            this.SubmitLocationsButton.Name = "SubmitLocationsButton";
            this.SubmitLocationsButton.Size = new System.Drawing.Size(75, 23);
            this.SubmitLocationsButton.TabIndex = 9;
            this.SubmitLocationsButton.Text = "Submit";
            this.SubmitLocationsButton.UseVisualStyleBackColor = true;
            this.SubmitLocationsButton.Visible = false;
            this.SubmitLocationsButton.Click += new System.EventHandler(this.SubmitLocationsButton_Click);
            // 
            // SubmitMaterialsButton
            // 
            this.SubmitMaterialsButton.Location = new System.Drawing.Point(600, 9);
            this.SubmitMaterialsButton.Name = "SubmitMaterialsButton";
            this.SubmitMaterialsButton.Size = new System.Drawing.Size(75, 23);
            this.SubmitMaterialsButton.TabIndex = 10;
            this.SubmitMaterialsButton.Text = "Submit";
            this.SubmitMaterialsButton.UseVisualStyleBackColor = true;
            this.SubmitMaterialsButton.Visible = false;
            this.SubmitMaterialsButton.Click += new System.EventHandler(this.SubmitMaterialsButton_Click);
            // 
            // SubmitFormsButton
            // 
            this.SubmitFormsButton.Enabled = false;
            this.SubmitFormsButton.Location = new System.Drawing.Point(681, 9);
            this.SubmitFormsButton.Name = "SubmitFormsButton";
            this.SubmitFormsButton.Size = new System.Drawing.Size(75, 23);
            this.SubmitFormsButton.TabIndex = 11;
            this.SubmitFormsButton.Text = "Submit";
            this.SubmitFormsButton.UseVisualStyleBackColor = true;
            this.SubmitFormsButton.Visible = false;
            this.SubmitFormsButton.Click += new System.EventHandler(this.SubmitFormsButton_Click);
            // 
            // SubmitResults
            // 
            this.SubmitResults.Enabled = false;
            this.SubmitResults.Location = new System.Drawing.Point(357, 33);
            this.SubmitResults.Name = "SubmitResults";
            this.SubmitResults.Size = new System.Drawing.Size(75, 23);
            this.SubmitResults.TabIndex = 12;
            this.SubmitResults.Text = "Submit";
            this.SubmitResults.UseVisualStyleBackColor = true;
            this.SubmitResults.Visible = false;
            this.SubmitResults.Click += new System.EventHandler(this.SubmitResults_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.SubmitResults);
            this.Controls.Add(this.SubmitFormsButton);
            this.Controls.Add(this.SubmitMaterialsButton);
            this.Controls.Add(this.SubmitLocationsButton);
            this.Controls.Add(this.SelectLabel);
            this.Controls.Add(this.TreeViewBox);
            this.Controls.Add(this.DBBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.DataServerBox);
            this.Controls.Add(this.SubmitButton);
            this.Controls.Add(this.SubmitSuppliers);
            this.Name = "Form1";
            this.Text = "Management2SQM";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox DataServerBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox DBBox;
        private System.Windows.Forms.Button SubmitButton;
        private System.Windows.Forms.TreeView TreeViewBox;
        private System.Windows.Forms.Label SelectLabel;
        private System.Windows.Forms.Button SubmitSuppliers;
        private System.Windows.Forms.Button SubmitLocationsButton;
        private System.Windows.Forms.Button SubmitMaterialsButton;
        private System.Windows.Forms.Button SubmitFormsButton;
        private System.Windows.Forms.Button SubmitResults;
    }
}

