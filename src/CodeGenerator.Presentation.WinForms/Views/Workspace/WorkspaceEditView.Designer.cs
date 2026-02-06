using CodeGenerator.UserControls.Views;

namespace CodeGenerator.Presentation.WinForms.Views
{
    partial class WorkspaceEditView
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
            txtName = new SingleLineTextField();
            txtRootNamespace = new SingleLineTextField();
            folderOutputDirectory = new FolderField();
            cbxTargetFramework = new ComboboxField();
            cbxLanguage = new ComboboxField();
            cbxCodeArchitecture = new ComboboxField();
            cbxDependencyInjectionFramework = new ComboboxField();
            tableLayoutPanel = new TableLayoutPanel();
            tableLayoutPanel.SuspendLayout();
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            lblTitle.Location = new Point(10, 10);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(166, 21);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Workspace Properties";
            // 
            // tableLayoutPanel
            // 
            tableLayoutPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel.AutoSize = true;
            tableLayoutPanel.ColumnCount = 1;
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel.Controls.Add(txtName, 0, 0);
            tableLayoutPanel.Controls.Add(txtRootNamespace, 0, 1);
            tableLayoutPanel.Controls.Add(folderOutputDirectory, 0, 2);
            tableLayoutPanel.Controls.Add(cbxTargetFramework, 0, 3);
            tableLayoutPanel.Controls.Add(cbxLanguage, 0, 4);
            tableLayoutPanel.Controls.Add(cbxCodeArchitecture, 0, 5);
            tableLayoutPanel.Controls.Add(cbxDependencyInjectionFramework, 0, 6);
            tableLayoutPanel.Location = new Point(10, 40);
            tableLayoutPanel.Name = "tableLayoutPanel";
            tableLayoutPanel.RowCount = 7;
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel.Size = new Size(380, 400);
            tableLayoutPanel.TabIndex = 1;
            // 
            // txtName
            // 
            txtName.Dock = DockStyle.Top;
            txtName.Label = "Workspace Name:";
            txtName.Location = new Point(3, 3);
            txtName.Name = "txtName";
            txtName.Size = new Size(374, 50);
            txtName.TabIndex = 0;
            // 
            // txtRootNamespace
            // 
            txtRootNamespace.Dock = DockStyle.Top;
            txtRootNamespace.Label = "Root Namespace:";
            txtRootNamespace.Location = new Point(3, 59);
            txtRootNamespace.Name = "txtRootNamespace";
            txtRootNamespace.Size = new Size(374, 50);
            txtRootNamespace.TabIndex = 1;
            // 
            // folderOutputDirectory
            // 
            folderOutputDirectory.Dock = DockStyle.Top;
            folderOutputDirectory.Label = "Default Output Directory:";
            folderOutputDirectory.Location = new Point(3, 115);
            folderOutputDirectory.Name = "folderOutputDirectory";
            folderOutputDirectory.Size = new Size(374, 50);
            folderOutputDirectory.TabIndex = 2;
            // 
            // cbxTargetFramework
            // 
            cbxTargetFramework.Dock = DockStyle.Top;
            cbxTargetFramework.Label = "Default Target Framework:";
            cbxTargetFramework.Location = new Point(3, 171);
            cbxTargetFramework.Name = "cbxTargetFramework";
            cbxTargetFramework.Size = new Size(374, 50);
            cbxTargetFramework.TabIndex = 3;
            // 
            // cbxLanguage
            // 
            cbxLanguage.Dock = DockStyle.Top;
            cbxLanguage.Label = "Default Language:";
            cbxLanguage.Location = new Point(3, 227);
            cbxLanguage.Name = "cbxLanguage";
            cbxLanguage.Size = new Size(374, 50);
            cbxLanguage.TabIndex = 4;
            // 
            // cbxCodeArchitecture
            // 
            cbxCodeArchitecture.Dock = DockStyle.Top;
            cbxCodeArchitecture.Label = "Code Architecture:";
            cbxCodeArchitecture.Location = new Point(3, 283);
            cbxCodeArchitecture.Name = "cbxCodeArchitecture";
            cbxCodeArchitecture.Size = new Size(374, 50);
            cbxCodeArchitecture.TabIndex = 5;
            // 
            // cbxDependencyInjectionFramework
            // 
            cbxDependencyInjectionFramework.Dock = DockStyle.Top;
            cbxDependencyInjectionFramework.Label = "Dependency Injection Framework:";
            cbxDependencyInjectionFramework.Location = new Point(3, 339);
            cbxDependencyInjectionFramework.Name = "cbxDependencyInjectionFramework";
            cbxDependencyInjectionFramework.Size = new Size(374, 50);
            cbxDependencyInjectionFramework.TabIndex = 6;
            // 
            // WorkspaceEditView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tableLayoutPanel);
            Controls.Add(lblTitle);
            Name = "WorkspaceEditView";
            Padding = new Padding(10);
            Size = new Size(400, 460);
            tableLayoutPanel.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblTitle;
        private TableLayoutPanel tableLayoutPanel;
        private SingleLineTextField txtName;
        private SingleLineTextField txtRootNamespace;
        private FolderField folderOutputDirectory;
        private ComboboxField cbxTargetFramework;
        private ComboboxField cbxLanguage;
        private ComboboxField cbxCodeArchitecture;
        private ComboboxField cbxDependencyInjectionFramework;
    }
}
