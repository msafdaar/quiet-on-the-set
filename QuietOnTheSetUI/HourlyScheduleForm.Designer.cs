namespace QuietOnTheSetUI
{
    partial class HourlyScheduleForm
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
            this.scheduleGrid = new System.Windows.Forms.DataGridView();
            this.hourColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.limitColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.noteLabel = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.scheduleGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // scheduleGrid
            // 
            this.scheduleGrid.AllowUserToAddRows = false;
            this.scheduleGrid.AllowUserToDeleteRows = false;
            this.scheduleGrid.AllowUserToResizeRows = false;
            this.scheduleGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.scheduleGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.scheduleGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.scheduleGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.hourColumn,
            this.limitColumn});
            this.scheduleGrid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.scheduleGrid.Location = new System.Drawing.Point(12, 51);
            this.scheduleGrid.MultiSelect = false;
            this.scheduleGrid.Name = "scheduleGrid";
            this.scheduleGrid.RowHeadersVisible = false;
            this.scheduleGrid.RowTemplate.Height = 20;
            this.scheduleGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.scheduleGrid.Size = new System.Drawing.Size(396, 446);
            this.scheduleGrid.TabIndex = 1;
            // 
            // hourColumn
            // 
            this.hourColumn.FillWeight = 40F;
            this.hourColumn.HeaderText = "Hour";
            this.hourColumn.Name = "hourColumn";
            this.hourColumn.ReadOnly = true;
            // 
            // limitColumn
            // 
            this.limitColumn.FillWeight = 60F;
            this.limitColumn.HeaderText = "Volume Limit";
            this.limitColumn.Name = "limitColumn";
            // 
            // noteLabel
            // 
            this.noteLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.noteLabel.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.noteLabel.Location = new System.Drawing.Point(12, 9);
            this.noteLabel.Name = "noteLabel";
            this.noteLabel.Size = new System.Drawing.Size(396, 35);
            this.noteLabel.TabIndex = 0;
            this.noteLabel.Text = "Leave a limit blank to use the master limit. Otherwise enter a value from 0 to 1" +
    "00.";
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.okButton.Location = new System.Drawing.Point(216, 510);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(90, 32);
            this.okButton.TabIndex = 2;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cancelButton.Location = new System.Drawing.Point(318, 510);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(90, 32);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // HourlyScheduleForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(420, 554);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.scheduleGrid);
            this.Controls.Add(this.noteLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "HourlyScheduleForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Time Schedule";
            ((System.ComponentModel.ISupportInitialize)(this.scheduleGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView scheduleGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn hourColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn limitColumn;
        private System.Windows.Forms.Label noteLabel;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
    }
}
