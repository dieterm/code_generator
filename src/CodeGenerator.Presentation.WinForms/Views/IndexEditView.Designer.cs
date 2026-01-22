using CodeGenerator.UserControls.Views;

namespace CodeGenerator.Presentation.WinForms.Views
{
    partial class IndexEditView
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lblTitle = new Label();
            grpGeneral = new GroupBox();
            tableLayoutGeneral = new TableLayoutPanel();
            txtName = new SingleLineTextField();
            chkUnique = new BooleanField();
            chkClustered = new BooleanField();
            grpColumns = new GroupBox();
            splitContainerColumns = new SplitContainer();
            panelAvailable = new Panel();
            lstAvailableColumns = new ListView();
            colAvailableName = new ColumnHeader();
            colAvailableType = new ColumnHeader();
            lblAvailable = new Label();
            panelButtons = new Panel();
            btnAddColumn = new Button();
            btnRemoveColumn = new Button();
            btnMoveUp = new Button();
            btnMoveDown = new Button();
            panelSelected = new Panel();
            lstSelectedColumns = new ListView();
            colSelectedName = new ColumnHeader();
            colSelectedType = new ColumnHeader();
            lblSelected = new Label();
            grpGeneral.SuspendLayout();
            tableLayoutGeneral.SuspendLayout();
            grpColumns.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerColumns).BeginInit();
            splitContainerColumns.Panel1.SuspendLayout();
            splitContainerColumns.Panel2.SuspendLayout();
            splitContainerColumns.SuspendLayout();
            panelAvailable.SuspendLayout();
            panelButtons.SuspendLayout();
            panelSelected.SuspendLayout();
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Dock = DockStyle.Top;
            lblTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblTitle.Location = new Point(10, 10);
            lblTitle.Name = "lblTitle";
            lblTitle.Padding = new Padding(0, 0, 0, 10);
            lblTitle.Size = new Size(110, 31);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Index Details";
            // 
            // grpGeneral
            // 
            grpGeneral.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            grpGeneral.Controls.Add(tableLayoutGeneral);
            grpGeneral.Location = new Point(10, 44);
            grpGeneral.Name = "grpGeneral";
            grpGeneral.Size = new Size(598, 120);
            grpGeneral.TabIndex = 1;
            grpGeneral.TabStop = false;
            grpGeneral.Text = "General";
            // 
            // tableLayoutGeneral
            // 
            tableLayoutGeneral.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutGeneral.AutoSize = true;
            tableLayoutGeneral.ColumnCount = 1;
            tableLayoutGeneral.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutGeneral.Controls.Add(txtName, 0, 0);
            tableLayoutGeneral.Controls.Add(chkUnique, 0, 1);
            tableLayoutGeneral.Controls.Add(chkClustered, 0, 2);
            tableLayoutGeneral.Location = new Point(6, 22);
            tableLayoutGeneral.Name = "tableLayoutGeneral";
            tableLayoutGeneral.RowCount = 3;
            tableLayoutGeneral.RowStyles.Add(new RowStyle());
            tableLayoutGeneral.RowStyles.Add(new RowStyle());
            tableLayoutGeneral.RowStyles.Add(new RowStyle());
            tableLayoutGeneral.Size = new Size(586, 94);
            tableLayoutGeneral.TabIndex = 0;
            // 
            // txtName
            // 
            txtName.Dock = DockStyle.Top;
            txtName.ErrorMessageVisible = true;
            txtName.Label = "Index Name:";
            txtName.Location = new Point(3, 3);
            txtName.Name = "txtName";
            txtName.Size = new Size(580, 36);
            txtName.TabIndex = 0;
            // 
            // chkUnique
            // 
            chkUnique.Label = "Unique Index";
            chkUnique.Location = new Point(3, 45);
            chkUnique.Name = "chkUnique";
            chkUnique.Size = new Size(180, 20);
            chkUnique.TabIndex = 1;
            // 
            // chkClustered
            // 
            chkClustered.Label = "Clustered Index";
            chkClustered.Location = new Point(3, 71);
            chkClustered.Name = "chkClustered";
            chkClustered.Size = new Size(180, 20);
            chkClustered.TabIndex = 2;
            // 
            // grpColumns
            // 
            grpColumns.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            grpColumns.Controls.Add(splitContainerColumns);
            grpColumns.Location = new Point(10, 170);
            grpColumns.Name = "grpColumns";
            grpColumns.Size = new Size(598, 280);
            grpColumns.TabIndex = 2;
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
            // IndexEditView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoScroll = true;
            Controls.Add(grpColumns);
            Controls.Add(grpGeneral);
            Controls.Add(lblTitle);
            Name = "IndexEditView";
            Padding = new Padding(10);
            Size = new Size(618, 460);
            grpGeneral.ResumeLayout(false);
            grpGeneral.PerformLayout();
            tableLayoutGeneral.ResumeLayout(false);
            grpColumns.ResumeLayout(false);
            splitContainerColumns.Panel1.ResumeLayout(false);
            splitContainerColumns.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerColumns).EndInit();
            splitContainerColumns.ResumeLayout(false);
            panelAvailable.ResumeLayout(false);
            panelButtons.ResumeLayout(false);
            panelSelected.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblTitle;
        private GroupBox grpGeneral;
        private TableLayoutPanel tableLayoutGeneral;
        private SingleLineTextField txtName;
        private BooleanField chkUnique;
        private BooleanField chkClustered;
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
