using CodeGenerator.UserControls.Views;

namespace CodeGenerator.Presentation.WinForms.Views
{
    partial class ForeignKeyEditView
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
            cmbReferencedTable = new ComboboxField();
            cmbOnDeleteAction = new ComboboxField();
            cmbOnUpdateAction = new ComboboxField();
            grpColumnMappings = new GroupBox();
            dgvColumnMappings = new DataGridView();
            colSourceColumn = new DataGridViewComboBoxColumn();
            colSourceDataType = new DataGridViewTextBoxColumn();
            colReferencedColumn = new DataGridViewComboBoxColumn();
            colReferencedDataType = new DataGridViewTextBoxColumn();
            panelMappingsButtons = new Panel();
            btnRemoveMapping = new Button();
            btnAddMapping = new Button();
            grpGeneral.SuspendLayout();
            tableLayoutGeneral.SuspendLayout();
            grpColumnMappings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvColumnMappings).BeginInit();
            panelMappingsButtons.SuspendLayout();
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
            lblTitle.Size = new Size(157, 31);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Foreign Key Details";
            // 
            // grpGeneral
            // 
            grpGeneral.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            grpGeneral.Controls.Add(tableLayoutGeneral);
            grpGeneral.Location = new Point(10, 44);
            grpGeneral.Name = "grpGeneral";
            grpGeneral.Size = new Size(580, 165);
            grpGeneral.TabIndex = 1;
            grpGeneral.TabStop = false;
            grpGeneral.Text = "General";
            // 
            // tableLayoutGeneral
            // 
            tableLayoutGeneral.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutGeneral.AutoSize = true;
            tableLayoutGeneral.ColumnCount = 2;
            tableLayoutGeneral.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutGeneral.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutGeneral.Controls.Add(txtName, 0, 0);
            tableLayoutGeneral.Controls.Add(cmbReferencedTable, 0, 1);
            tableLayoutGeneral.Controls.Add(cmbOnDeleteAction, 0, 2);
            tableLayoutGeneral.Controls.Add(cmbOnUpdateAction, 1, 2);
            tableLayoutGeneral.Location = new Point(6, 22);
            tableLayoutGeneral.Name = "tableLayoutGeneral";
            tableLayoutGeneral.RowCount = 3;
            tableLayoutGeneral.RowStyles.Add(new RowStyle());
            tableLayoutGeneral.RowStyles.Add(new RowStyle());
            tableLayoutGeneral.RowStyles.Add(new RowStyle());
            tableLayoutGeneral.Size = new Size(568, 136);
            tableLayoutGeneral.TabIndex = 0;
            // 
            // txtName
            // 
            tableLayoutGeneral.SetColumnSpan(txtName, 2);
            txtName.Dock = DockStyle.Top;
            txtName.ErrorMessageVisible = true;
            txtName.Label = "Foreign Key Name:";
            txtName.Location = new Point(3, 3);
            txtName.Name = "txtName";
            txtName.Size = new Size(562, 36);
            txtName.TabIndex = 0;
            // 
            // cmbReferencedTable
            // 
            tableLayoutGeneral.SetColumnSpan(cmbReferencedTable, 2);
            cmbReferencedTable.Dock = DockStyle.Top;
            cmbReferencedTable.ErrorMessageVisible = true;
            cmbReferencedTable.Label = "Referenced Table:";
            cmbReferencedTable.Location = new Point(3, 45);
            cmbReferencedTable.Name = "cmbReferencedTable";
            cmbReferencedTable.Size = new Size(562, 50);
            cmbReferencedTable.TabIndex = 1;
            // 
            // cmbOnDeleteAction
            // 
            cmbOnDeleteAction.Dock = DockStyle.Top;
            cmbOnDeleteAction.ErrorMessageVisible = false;
            cmbOnDeleteAction.Label = "On Delete:";
            cmbOnDeleteAction.Location = new Point(3, 101);
            cmbOnDeleteAction.Name = "cmbOnDeleteAction";
            cmbOnDeleteAction.Size = new Size(278, 30);
            cmbOnDeleteAction.TabIndex = 2;
            // 
            // cmbOnUpdateAction
            // 
            cmbOnUpdateAction.Dock = DockStyle.Top;
            cmbOnUpdateAction.ErrorMessageVisible = false;
            cmbOnUpdateAction.Label = "On Update:";
            cmbOnUpdateAction.Location = new Point(287, 101);
            cmbOnUpdateAction.Name = "cmbOnUpdateAction";
            cmbOnUpdateAction.Size = new Size(278, 30);
            cmbOnUpdateAction.TabIndex = 3;
            // 
            // grpColumnMappings
            // 
            grpColumnMappings.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            grpColumnMappings.Controls.Add(dgvColumnMappings);
            grpColumnMappings.Controls.Add(panelMappingsButtons);
            grpColumnMappings.Location = new Point(10, 215);
            grpColumnMappings.Name = "grpColumnMappings";
            grpColumnMappings.Size = new Size(580, 235);
            grpColumnMappings.TabIndex = 2;
            grpColumnMappings.TabStop = false;
            grpColumnMappings.Text = "Column Mappings";
            // 
            // dgvColumnMappings
            // 
            dgvColumnMappings.AllowUserToAddRows = false;
            dgvColumnMappings.AllowUserToDeleteRows = false;
            dgvColumnMappings.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvColumnMappings.Columns.AddRange(new DataGridViewColumn[] { colSourceColumn, colSourceDataType, colReferencedColumn, colReferencedDataType });
            dgvColumnMappings.Dock = DockStyle.Fill;
            dgvColumnMappings.Location = new Point(3, 49);
            dgvColumnMappings.MultiSelect = false;
            dgvColumnMappings.Name = "dgvColumnMappings";
            dgvColumnMappings.RowHeadersVisible = false;
            dgvColumnMappings.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dgvColumnMappings.Size = new Size(574, 183);
            dgvColumnMappings.TabIndex = 1;
            dgvColumnMappings.CellValueChanged += dgvColumnMappings_CellValueChanged;
            dgvColumnMappings.CurrentCellDirtyStateChanged += dgvColumnMappings_CurrentCellDirtyStateChanged;
            // 
            // colSourceColumn
            // 
            colSourceColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colSourceColumn.HeaderText = "Source Column";
            colSourceColumn.Name = "colSourceColumn";
            // 
            // colSourceDataType
            // 
            colSourceDataType.HeaderText = "Source Type";
            colSourceDataType.Name = "colSourceDataType";
            colSourceDataType.ReadOnly = true;
            // 
            // colReferencedColumn
            // 
            colReferencedColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colReferencedColumn.HeaderText = "Referenced Column";
            colReferencedColumn.Name = "colReferencedColumn";
            // 
            // colReferencedDataType
            // 
            colReferencedDataType.HeaderText = "Referenced Type";
            colReferencedDataType.Name = "colReferencedDataType";
            colReferencedDataType.ReadOnly = true;
            // 
            // panelMappingsButtons
            // 
            panelMappingsButtons.Controls.Add(btnRemoveMapping);
            panelMappingsButtons.Controls.Add(btnAddMapping);
            panelMappingsButtons.Dock = DockStyle.Top;
            panelMappingsButtons.Location = new Point(3, 19);
            panelMappingsButtons.Name = "panelMappingsButtons";
            panelMappingsButtons.Size = new Size(574, 30);
            panelMappingsButtons.TabIndex = 0;
            // 
            // btnRemoveMapping
            // 
            btnRemoveMapping.Location = new Point(84, 3);
            btnRemoveMapping.Name = "btnRemoveMapping";
            btnRemoveMapping.Size = new Size(75, 23);
            btnRemoveMapping.TabIndex = 1;
            btnRemoveMapping.Text = "Remove";
            btnRemoveMapping.UseVisualStyleBackColor = true;
            btnRemoveMapping.Click += btnRemoveMapping_Click;
            // 
            // btnAddMapping
            // 
            btnAddMapping.Location = new Point(3, 3);
            btnAddMapping.Name = "btnAddMapping";
            btnAddMapping.Size = new Size(75, 23);
            btnAddMapping.TabIndex = 0;
            btnAddMapping.Text = "Add";
            btnAddMapping.UseVisualStyleBackColor = true;
            btnAddMapping.Click += btnAddMapping_Click;
            // 
            // ForeignKeyEditView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoScroll = true;
            Controls.Add(grpColumnMappings);
            Controls.Add(grpGeneral);
            Controls.Add(lblTitle);
            Name = "ForeignKeyEditView";
            Padding = new Padding(10);
            Size = new Size(600, 470);
            grpGeneral.ResumeLayout(false);
            grpGeneral.PerformLayout();
            tableLayoutGeneral.ResumeLayout(false);
            grpColumnMappings.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvColumnMappings).EndInit();
            panelMappingsButtons.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblTitle;
        private GroupBox grpGeneral;
        private TableLayoutPanel tableLayoutGeneral;
        private SingleLineTextField txtName;
        private ComboboxField cmbReferencedTable;
        private ComboboxField cmbOnDeleteAction;
        private ComboboxField cmbOnUpdateAction;
        private GroupBox grpColumnMappings;
        private Panel panelMappingsButtons;
        private Button btnAddMapping;
        private Button btnRemoveMapping;
        private DataGridView dgvColumnMappings;
        private DataGridViewComboBoxColumn colSourceColumn;
        private DataGridViewTextBoxColumn colSourceDataType;
        private DataGridViewComboBoxColumn colReferencedColumn;
        private DataGridViewTextBoxColumn colReferencedDataType;
    }
}
