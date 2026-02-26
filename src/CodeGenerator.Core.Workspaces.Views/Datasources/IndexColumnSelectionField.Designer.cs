namespace CodeGenerator.Presentation.WinForms.Views
{
    partial class IndexColumnSelectionField
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            grpColumns = new GroupBox();
            splitContainerColumns = new SplitContainer();
            panelAvailable = new Panel();
            lstAvailableColumns = new ListView();
            colAvailableName = new ColumnHeader();
            colAvailableType = new ColumnHeader();
            lblAvailable = new Label();
            panelSelected = new Panel();
            lstSelectedColumns = new ListView();
            colSelectedName = new ColumnHeader();
            colSelectedType = new ColumnHeader();
            lblSelected = new Label();
            panelButtons = new Panel();
            btnAddColumn = new Button();
            btnRemoveColumn = new Button();
            btnMoveUp = new Button();
            btnMoveDown = new Button();
            grpColumns.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerColumns).BeginInit();
            splitContainerColumns.Panel1.SuspendLayout();
            splitContainerColumns.Panel2.SuspendLayout();
            splitContainerColumns.SuspendLayout();
            panelAvailable.SuspendLayout();
            panelSelected.SuspendLayout();
            panelButtons.SuspendLayout();
            SuspendLayout();
            // 
            // grpColumns
            // 
            grpColumns.Controls.Add(splitContainerColumns);
            grpColumns.Dock = DockStyle.Fill;
            grpColumns.Location = new Point(0, 0);
            grpColumns.Name = "grpColumns";
            grpColumns.Size = new Size(598, 280);
            grpColumns.TabIndex = 0;
            grpColumns.TabStop = false;
            grpColumns.Text = "Index Columns";
            // 
            // splitContainerColumns
            // 
            splitContainerColumns.Dock = DockStyle.Fill;
            splitContainerColumns.Location = new Point(3, 19);
            splitContainerColumns.Name = "splitContainerColumns";
            // 
            // splitContainerColumns.Panel1
            // 
            splitContainerColumns.Panel1.Controls.Add(panelAvailable);
            // 
            // splitContainerColumns.Panel2
            // 
            splitContainerColumns.Panel2.Controls.Add(panelSelected);
            splitContainerColumns.Panel2.Controls.Add(panelButtons);
            splitContainerColumns.Size = new Size(592, 258);
            splitContainerColumns.SplitterDistance = 253;
            splitContainerColumns.TabIndex = 0;
            // 
            // panelAvailable
            // 
            panelAvailable.Controls.Add(lstAvailableColumns);
            panelAvailable.Controls.Add(lblAvailable);
            panelAvailable.Dock = DockStyle.Fill;
            panelAvailable.Location = new Point(0, 0);
            panelAvailable.Name = "panelAvailable";
            panelAvailable.Size = new Size(253, 258);
            panelAvailable.TabIndex = 0;
            // 
            // lstAvailableColumns
            // 
            lstAvailableColumns.Columns.AddRange(new ColumnHeader[] { colAvailableName, colAvailableType });
            lstAvailableColumns.Dock = DockStyle.Fill;
            lstAvailableColumns.FullRowSelect = true;
            lstAvailableColumns.Location = new Point(0, 20);
            lstAvailableColumns.MultiSelect = false;
            lstAvailableColumns.Name = "lstAvailableColumns";
            lstAvailableColumns.Size = new Size(253, 238);
            lstAvailableColumns.TabIndex = 1;
            lstAvailableColumns.UseCompatibleStateImageBehavior = false;
            lstAvailableColumns.View = View.Details;
            // 
            // colAvailableName
            // 
            colAvailableName.Text = "Column";
            colAvailableName.Width = 80;
            // 
            // colAvailableType
            // 
            colAvailableType.Text = "Type";
            colAvailableType.Width = 70;
            // 
            // lblAvailable
            // 
            lblAvailable.Dock = DockStyle.Top;
            lblAvailable.Location = new Point(0, 0);
            lblAvailable.Name = "lblAvailable";
            lblAvailable.Size = new Size(253, 20);
            lblAvailable.TabIndex = 0;
            lblAvailable.Text = "Available Columns:";
            // 
            // panelSelected
            // 
            panelSelected.Controls.Add(lstSelectedColumns);
            panelSelected.Controls.Add(lblSelected);
            panelSelected.Dock = DockStyle.Fill;
            panelSelected.Location = new Point(35, 0);
            panelSelected.Name = "panelSelected";
            panelSelected.Size = new Size(300, 258);
            panelSelected.TabIndex = 1;
            // 
            // lstSelectedColumns
            // 
            lstSelectedColumns.Columns.AddRange(new ColumnHeader[] { colSelectedName, colSelectedType });
            lstSelectedColumns.Dock = DockStyle.Top;
            lstSelectedColumns.FullRowSelect = true;
            lstSelectedColumns.Location = new Point(0, 20);
            lstSelectedColumns.MultiSelect = false;
            lstSelectedColumns.Name = "lstSelectedColumns";
            lstSelectedColumns.Size = new Size(300, 238);
            lstSelectedColumns.TabIndex = 1;
            lstSelectedColumns.UseCompatibleStateImageBehavior = false;
            lstSelectedColumns.View = View.Details;
            // 
            // colSelectedName
            // 
            colSelectedName.Text = "Column";
            colSelectedName.Width = 90;
            // 
            // colSelectedType
            // 
            colSelectedType.Text = "Type";
            colSelectedType.Width = 70;
            // 
            // lblSelected
            // 
            lblSelected.Dock = DockStyle.Top;
            lblSelected.Location = new Point(0, 0);
            lblSelected.Name = "lblSelected";
            lblSelected.Size = new Size(300, 20);
            lblSelected.TabIndex = 0;
            lblSelected.Text = "Index Columns:";
            // 
            // panelButtons
            // 
            panelButtons.Controls.Add(btnAddColumn);
            panelButtons.Controls.Add(btnRemoveColumn);
            panelButtons.Controls.Add(btnMoveUp);
            panelButtons.Controls.Add(btnMoveDown);
            panelButtons.Dock = DockStyle.Left;
            panelButtons.Location = new Point(0, 0);
            panelButtons.Name = "panelButtons";
            panelButtons.Size = new Size(35, 258);
            panelButtons.TabIndex = 0;
            // 
            // btnAddColumn
            // 
            btnAddColumn.Enabled = false;
            btnAddColumn.Location = new Point(3, 23);
            btnAddColumn.Name = "btnAddColumn";
            btnAddColumn.Size = new Size(28, 28);
            btnAddColumn.TabIndex = 0;
            btnAddColumn.Text = ">";
            btnAddColumn.UseVisualStyleBackColor = true;
            // 
            // btnRemoveColumn
            // 
            btnRemoveColumn.Enabled = false;
            btnRemoveColumn.Location = new Point(3, 57);
            btnRemoveColumn.Name = "btnRemoveColumn";
            btnRemoveColumn.Size = new Size(28, 28);
            btnRemoveColumn.TabIndex = 1;
            btnRemoveColumn.Text = "<";
            btnRemoveColumn.UseVisualStyleBackColor = true;
            // 
            // btnMoveUp
            // 
            btnMoveUp.Enabled = false;
            btnMoveUp.Location = new Point(3, 100);
            btnMoveUp.Name = "btnMoveUp";
            btnMoveUp.Size = new Size(28, 28);
            btnMoveUp.TabIndex = 2;
            btnMoveUp.Text = "?";
            btnMoveUp.UseVisualStyleBackColor = true;
            // 
            // btnMoveDown
            // 
            btnMoveDown.Enabled = false;
            btnMoveDown.Location = new Point(3, 134);
            btnMoveDown.Name = "btnMoveDown";
            btnMoveDown.Size = new Size(28, 28);
            btnMoveDown.TabIndex = 3;
            btnMoveDown.Text = "?";
            btnMoveDown.UseVisualStyleBackColor = true;
            // 
            // IndexColumnSelectionField
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(grpColumns);
            Name = "IndexColumnSelectionField";
            Size = new Size(598, 280);
            grpColumns.ResumeLayout(false);
            splitContainerColumns.Panel1.ResumeLayout(false);
            splitContainerColumns.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerColumns).EndInit();
            splitContainerColumns.ResumeLayout(false);
            panelAvailable.ResumeLayout(false);
            panelSelected.ResumeLayout(false);
            panelButtons.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private GroupBox grpColumns;
        private SplitContainer splitContainerColumns;
        private Panel panelAvailable;
        private ListView lstAvailableColumns;
        private ColumnHeader colAvailableName;
        private ColumnHeader colAvailableType;
        private Label lblAvailable;
        private Panel panelButtons;
        private Button btnAddColumn;
        private Button btnRemoveColumn;
        private Button btnMoveUp;
        private Button btnMoveDown;
        private Panel panelSelected;
        private ListView lstSelectedColumns;
        private ColumnHeader colSelectedName;
        private ColumnHeader colSelectedType;
        private Label lblSelected;
    }
}
