using CodeGenerator.UserControls.Views;

namespace CodeGenerator.Core.CodeElements.Views;

partial class TypeReferenceEditView
{
    private System.ComponentModel.IContainer? components = null;

    #region Component Designer generated code

    private void InitializeComponent()
    {
        lblTitle = new Label();
        txtTypeName = new SingleLineTextField();
        txtNamespace = new SingleLineTextField();
        chkIsNullable = new BooleanField();
        chkIsArray = new BooleanField();
        numArrayRank = new IntegerField();
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
        lblTitle.Size = new Size(150, 21);
        lblTitle.TabIndex = 0;
        lblTitle.Text = "Type Reference";
        // 
        // tableLayoutPanel
        // 
        tableLayoutPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        tableLayoutPanel.AutoSize = true;
        tableLayoutPanel.ColumnCount = 1;
        tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tableLayoutPanel.Controls.Add(txtTypeName, 0, 0);
        tableLayoutPanel.Controls.Add(txtNamespace, 0, 1);
        tableLayoutPanel.Controls.Add(chkIsNullable, 0, 2);
        tableLayoutPanel.Controls.Add(chkIsArray, 0, 3);
        tableLayoutPanel.Controls.Add(numArrayRank, 0, 4);
        tableLayoutPanel.Location = new Point(10, 40);
        tableLayoutPanel.Name = "tableLayoutPanel";
        tableLayoutPanel.RowCount = 5;
        tableLayoutPanel.RowStyles.Add(new RowStyle());
        tableLayoutPanel.RowStyles.Add(new RowStyle());
        tableLayoutPanel.RowStyles.Add(new RowStyle());
        tableLayoutPanel.RowStyles.Add(new RowStyle());
        tableLayoutPanel.RowStyles.Add(new RowStyle());
        tableLayoutPanel.Size = new Size(380, 230);
        tableLayoutPanel.TabIndex = 1;
        // 
        // txtTypeName
        // 
        txtTypeName.Dock = DockStyle.Top;
        txtTypeName.Label = "Type Name:";
        txtTypeName.Name = "txtTypeName";
        txtTypeName.Size = new Size(374, 50);
        txtTypeName.TabIndex = 0;
        // 
        // txtNamespace
        // 
        txtNamespace.Dock = DockStyle.Top;
        txtNamespace.Label = "Namespace:";
        txtNamespace.Name = "txtNamespace";
        txtNamespace.Size = new Size(374, 50);
        txtNamespace.TabIndex = 1;
        // 
        // chkIsNullable
        // 
        chkIsNullable.Dock = DockStyle.Top;
        chkIsNullable.Label = "Is Nullable:";
        chkIsNullable.Name = "chkIsNullable";
        chkIsNullable.Size = new Size(374, 30);
        chkIsNullable.TabIndex = 2;
        // 
        // chkIsArray
        // 
        chkIsArray.Dock = DockStyle.Top;
        chkIsArray.Label = "Is Array:";
        chkIsArray.Name = "chkIsArray";
        chkIsArray.Size = new Size(374, 30);
        chkIsArray.TabIndex = 3;
        // 
        // numArrayRank
        // 
        numArrayRank.Dock = DockStyle.Top;
        numArrayRank.Label = "Array Rank:";
        numArrayRank.Minimum = 1;
        numArrayRank.Maximum = 32;
        numArrayRank.Name = "numArrayRank";
        numArrayRank.Size = new Size(374, 50);
        numArrayRank.TabIndex = 4;
        // 
        // TypeReferenceEditView
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        AutoSize = true;
        Controls.Add(tableLayoutPanel);
        Controls.Add(lblTitle);
        Name = "TypeReferenceEditView";
        Padding = new Padding(10);
        Size = new Size(400, 280);
        tableLayoutPanel.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label lblTitle;
    private TableLayoutPanel tableLayoutPanel;
    private SingleLineTextField txtTypeName;
    private SingleLineTextField txtNamespace;
    private BooleanField chkIsNullable;
    private BooleanField chkIsArray;
    private IntegerField numArrayRank;
}
