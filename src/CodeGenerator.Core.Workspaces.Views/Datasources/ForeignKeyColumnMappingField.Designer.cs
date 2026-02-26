namespace CodeGenerator.Presentation.WinForms.Views
{
    partial class ForeignKeyColumnMappingField
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
            grpColumnMappings = new GroupBox();
            dgvColumnMappings = new DataGridView();
            colSourceColumn = new DataGridViewComboBoxColumn();
            colSourceDataType = new DataGridViewTextBoxColumn();
            colReferencedColumn = new DataGridViewComboBoxColumn();
            colReferencedDataType = new DataGridViewTextBoxColumn();
            panelMappingsButtons = new Panel();
            btnRemoveMapping = new Button();
            btnAddMapping = new Button();
            grpColumnMappings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvColumnMappings).BeginInit();
            panelMappingsButtons.SuspendLayout();
            SuspendLayout();
            // 
            // grpColumnMappings
            // 
            grpColumnMappings.Controls.Add(dgvColumnMappings);
            grpColumnMappings.Controls.Add(panelMappingsButtons);
            grpColumnMappings.Dock = DockStyle.Fill;
            grpColumnMappings.Location = new Point(0, 0);
            grpColumnMappings.Name = "grpColumnMappings";
            grpColumnMappings.Size = new Size(580, 235);
            grpColumnMappings.TabIndex = 0;
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
            // ForeignKeyColumnMappingField
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(grpColumnMappings);
            Name = "ForeignKeyColumnMappingField";
            Size = new Size(580, 235);
            grpColumnMappings.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvColumnMappings).EndInit();
            panelMappingsButtons.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

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
