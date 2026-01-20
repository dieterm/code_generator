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
            grpColumnMappings = new GroupBox();
            panelMappingsButtons = new Panel();
            btnRemoveMapping = new Button();
            btnAddMapping = new Button();
            dgvColumnMappings = new DataGridView();
            colSourceColumn = new DataGridViewComboBoxColumn();
            colReferencedColumn = new DataGridViewComboBoxColumn();
            grpGeneral.SuspendLayout();
            tableLayoutGeneral.SuspendLayout();
            grpColumnMappings.SuspendLayout();
            panelMappingsButtons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvColumnMappings).BeginInit();
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
            lblTitle.Size = new Size(142, 31);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Foreign Key Details";
            // 
            // grpGeneral
            // 
            grpGeneral.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            grpGeneral.Controls.Add(tableLayoutGeneral);
            grpGeneral.Location = new Point(10, 44);
            grpGeneral.Name = "grpGeneral";
            grpGeneral.Size = new Size(380, 130);
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
            tableLayoutGeneral.Controls.Add(cmbReferencedTable, 0, 1);
            tableLayoutGeneral.Location = new Point(6, 22);
            tableLayoutGeneral.Name = "tableLayoutGeneral";
            tableLayoutGeneral.RowCount = 2;
            tableLayoutGeneral.RowStyles.Add(new RowStyle());
            tableLayoutGeneral.RowStyles.Add(new RowStyle());
            tableLayoutGeneral.Size = new Size(368, 100);
            tableLayoutGeneral.TabIndex = 0;
            // 
            // txtName
            // 
            txtName.Dock = DockStyle.Top;
            txtName.ErrorMessageVisible = true;
            txtName.Label = "Foreign Key Name:";
            txtName.Location = new Point(3, 3);
            txtName.Name = "txtName";
            txtName.Size = new Size(362, 36);
            txtName.TabIndex = 0;
            // 
            // cmbReferencedTable
            // 
            cmbReferencedTable.Dock = DockStyle.Top;
            cmbReferencedTable.ErrorMessageVisible = true;
            cmbReferencedTable.Label = "Referenced Table:";
            cmbReferencedTable.Location = new Point(3, 45);
            cmbReferencedTable.Name = "cmbReferencedTable";
            cmbReferencedTable.Size = new Size(362, 50);
            cmbReferencedTable.TabIndex = 1;
            // 
            // grpColumnMappings
            // 
            grpColumnMappings.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            grpColumnMappings.Controls.Add(dgvColumnMappings);
            grpColumnMappings.Controls.Add(panelMappingsButtons);
            grpColumnMappings.Location = new Point(10, 180);
            grpColumnMappings.Name = "grpColumnMappings";
            grpColumnMappings.Size = new Size(380, 200);
            grpColumnMappings.TabIndex = 2;
            grpColumnMappings.TabStop = false;
            grpColumnMappings.Text = "Column Mappings";
            // 
            // panelMappingsButtons
            // 
            panelMappingsButtons.Controls.Add(btnRemoveMapping);
            panelMappingsButtons.Controls.Add(btnAddMapping);
            panelMappingsButtons.Dock = DockStyle.Top;
            panelMappingsButtons.Location = new Point(3, 19);
            panelMappingsButtons.Name = "panelMappingsButtons";
            panelMappingsButtons.Size = new Size(374, 30);
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
            // dgvColumnMappings
            // 
            dgvColumnMappings.AllowUserToAddRows = false;
            dgvColumnMappings.AllowUserToDeleteRows = false;
            dgvColumnMappings.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvColumnMappings.Columns.AddRange(new DataGridViewColumn[] { colSourceColumn, colReferencedColumn });
            dgvColumnMappings.Dock = DockStyle.Fill;
            dgvColumnMappings.Location = new Point(3, 49);
            dgvColumnMappings.MultiSelect = false;
            dgvColumnMappings.Name = "dgvColumnMappings";
            dgvColumnMappings.RowHeadersVisible = false;
            dgvColumnMappings.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvColumnMappings.Size = new Size(374, 148);
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
            // colReferencedColumn
            // 
            colReferencedColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colReferencedColumn.HeaderText = "Referenced Column";
            colReferencedColumn.Name = "colReferencedColumn";
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
            Size = new Size(400, 400);
            grpGeneral.ResumeLayout(false);
            grpGeneral.PerformLayout();
            tableLayoutGeneral.ResumeLayout(false);
            grpColumnMappings.ResumeLayout(false);
            panelMappingsButtons.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvColumnMappings).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblTitle;
        private GroupBox grpGeneral;
        private TableLayoutPanel tableLayoutGeneral;
        private SingleLineTextField txtName;
        private ComboboxField cmbReferencedTable;
        private GroupBox grpColumnMappings;
        private Panel panelMappingsButtons;
        private Button btnAddMapping;
        private Button btnRemoveMapping;
        private DataGridView dgvColumnMappings;
        private DataGridViewComboBoxColumn colSourceColumn;
        private DataGridViewComboBoxColumn colReferencedColumn;
    }
}
