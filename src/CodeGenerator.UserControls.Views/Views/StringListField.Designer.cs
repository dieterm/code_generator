namespace CodeGenerator.UserControls.Views
{
    partial class StringListField
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            grpBox = new GroupBox();
            lstItems = new ListBox();
            pnlButtons = new FlowLayoutPanel();
            btnAdd = new Button();
            btnEdit = new Button();
            btnRemove = new Button();
            grpBox.SuspendLayout();
            pnlButtons.SuspendLayout();
            SuspendLayout();
            // 
            // grpBox
            // 
            grpBox.Controls.Add(lstItems);
            grpBox.Controls.Add(pnlButtons);
            grpBox.Dock = DockStyle.Fill;
            grpBox.Location = new Point(0, 0);
            grpBox.Name = "grpBox";
            grpBox.Size = new Size(300, 180);
            grpBox.TabIndex = 0;
            grpBox.TabStop = false;
            grpBox.Text = "Items:";
            // 
            // lstItems
            // 
            lstItems.Dock = DockStyle.Fill;
            lstItems.ItemHeight = 15;
            lstItems.Location = new Point(3, 19);
            lstItems.Name = "lstItems";
            lstItems.Size = new Size(294, 127);
            lstItems.TabIndex = 0;
            // 
            // pnlButtons
            // 
            pnlButtons.AutoSize = true;
            pnlButtons.Controls.Add(btnAdd);
            pnlButtons.Controls.Add(btnEdit);
            pnlButtons.Controls.Add(btnRemove);
            pnlButtons.Dock = DockStyle.Bottom;
            pnlButtons.Location = new Point(3, 146);
            pnlButtons.Name = "pnlButtons";
            pnlButtons.Size = new Size(294, 31);
            pnlButtons.TabIndex = 1;
            // 
            // btnAdd
            // 
            btnAdd.Location = new Point(3, 3);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(75, 25);
            btnAdd.TabIndex = 0;
            btnAdd.Text = "Add";
            // 
            // btnEdit
            // 
            btnEdit.Enabled = false;
            btnEdit.Location = new Point(84, 3);
            btnEdit.Name = "btnEdit";
            btnEdit.Size = new Size(75, 25);
            btnEdit.TabIndex = 1;
            btnEdit.Text = "Edit";
            // 
            // btnRemove
            // 
            btnRemove.Enabled = false;
            btnRemove.Location = new Point(165, 3);
            btnRemove.Name = "btnRemove";
            btnRemove.Size = new Size(75, 25);
            btnRemove.TabIndex = 2;
            btnRemove.Text = "Remove";
            // 
            // StringListField
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(grpBox);
            Name = "StringListField";
            Size = new Size(300, 180);
            grpBox.ResumeLayout(false);
            grpBox.PerformLayout();
            pnlButtons.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private GroupBox grpBox;
        private ListBox lstItems;
        private FlowLayoutPanel pnlButtons;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnRemove;
    }
}
