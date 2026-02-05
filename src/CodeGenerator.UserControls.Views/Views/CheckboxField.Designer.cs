namespace CodeGenerator.UserControls.Views;

partial class CheckboxField
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
        chkValue = new CheckBox();
        SuspendLayout();
        // 
        // chkValue
        // 
        chkValue.AutoSize = true;
        chkValue.Dock = DockStyle.Fill;
        chkValue.Location = new Point(0, 0);
        chkValue.Name = "chkValue";
        chkValue.Size = new Size(200, 25);
        chkValue.TabIndex = 0;
        chkValue.Text = "Checkbox";
        chkValue.UseVisualStyleBackColor = true;
        // 
        // CheckboxField
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(chkValue);
        Name = "CheckboxField";
        Size = new Size(200, 25);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private CheckBox chkValue;
}
