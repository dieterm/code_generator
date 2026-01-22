namespace CodeGenerator.Presentation.WinForms.Views
{
    partial class ScribanTemplateEditView
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
            if (disposing)
            {
                // Clean up temp config file
                CleanupTempConfigFile();
                
                if (_viewModel != null)
                {
                    _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
                }
                
                components?.Dispose();
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
            editControl = new Syncfusion.Windows.Forms.Edit.EditControl();
            ((System.ComponentModel.ISupportInitialize)(editControl)).BeginInit();
            SuspendLayout();
            // 
            // editControl
            // 
            editControl.CodeSnipptSize = new Size(100, 100);
            editControl.Dock = DockStyle.Fill;
            editControl.IndicatorMarginBackColor = SystemColors.ControlLight;
            editControl.Location = new Point(0, 0);
            editControl.Name = "editControl";
            editControl.RenderRightToLeft = false;
            editControl.ScrollPosition = new Point(0, 0);
            editControl.ScrollVisualStyle = Syncfusion.Windows.Forms.ScrollBarCustomDrawStyles.Metro;
            editControl.ShowHorizontalSplitters = false;
            editControl.ShowIndicatorMargin = false;
            editControl.ShowLineNumbers = true;
            editControl.ShowOutliningCollapsers = false;
            editControl.ShowVerticalSplitters = false;
            editControl.Size = new Size(600, 400);
            editControl.TabIndex = 0;
            editControl.Text = "";
            editControl.UseXPStyleBorder = true;
            editControl.VisualColumn = 1;
            editControl.WordWrap = false;
            // 
            // ScribanTemplateEditView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(editControl);
            Name = "ScribanTemplateEditView";
            Size = new Size(600, 400);
            ((System.ComponentModel.ISupportInitialize)(editControl)).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Syncfusion.Windows.Forms.Edit.EditControl editControl;
    }
}
