namespace CodeGenerator.Presentation.WinForms.Views
{
    partial class ArtifactPreviewView
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
            Syncfusion.Windows.Forms.Edit.Implementation.Config.Config config1 = new Syncfusion.Windows.Forms.Edit.Implementation.Config.Config();
            editControl = new Syncfusion.Windows.Forms.Edit.EditControl();
            ((System.ComponentModel.ISupportInitialize)editControl).BeginInit();
            SuspendLayout();
            // 
            // editControl
            // 
            editControl.AllowZoom = false;
            editControl.ChangedLinesMarkingLineColor = Color.FromArgb(255, 238, 98);
            editControl.CodeSnipptSize = new Size(100, 100);
            editControl.Configurator = config1;
            editControl.ContextChoiceBackColor = Color.FromArgb(255, 255, 255);
            editControl.ContextChoiceBorderColor = Color.FromArgb(233, 166, 50);
            editControl.ContextChoiceForeColor = SystemColors.InfoText;
            editControl.ContextPromptBackgroundBrush = new Syncfusion.Drawing.BrushInfo(Color.FromArgb(255, 255, 255));
            editControl.ContextTooltipBackgroundBrush = new Syncfusion.Drawing.BrushInfo(Color.FromArgb(231, 232, 236));
            editControl.Dock = DockStyle.Fill;
            editControl.IndicatorMarginBackColor = Color.Empty;
            editControl.LineNumbersColor = Color.FromArgb(0, 128, 128);
            editControl.Location = new Point(0, 0);
            editControl.Name = "editControl";
            editControl.RenderRightToLeft = false;
            editControl.ScrollPosition = new Point(0, 0);
            editControl.SelectionTextColor = Color.FromArgb(173, 214, 255);
            editControl.ShowEndOfLine = false;
            editControl.Size = new Size(586, 454);
            editControl.StatusBarSettings.CoordsPanel.Width = 150;
            editControl.StatusBarSettings.EncodingPanel.Width = 100;
            editControl.StatusBarSettings.FileNamePanel.Width = 100;
            editControl.StatusBarSettings.InsertPanel.Width = 33;
            editControl.StatusBarSettings.Offcie2007ColorScheme = Syncfusion.Windows.Forms.Office2007Theme.Blue;
            editControl.StatusBarSettings.Offcie2010ColorScheme = Syncfusion.Windows.Forms.Office2010Theme.Blue;
            editControl.StatusBarSettings.StatusPanel.Width = 70;
            editControl.StatusBarSettings.TextPanel.Width = 214;
            editControl.StatusBarSettings.VisualStyle = Syncfusion.Windows.Forms.Tools.Controls.StatusBar.VisualStyle.Default;
            editControl.TabIndex = 0;
            editControl.Text = "";
            editControl.UseXPStyleBorder = true;
            editControl.VisualColumn = 1;
            editControl.VScrollMode = Syncfusion.Windows.Forms.Edit.ScrollMode.Immediate;
            editControl.ZoomFactor = 1F;
            // 
            // ArtifactPreviewView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(editControl);
            Name = "ArtifactPreviewView";
            Size = new Size(586, 454);
            ((System.ComponentModel.ISupportInitialize)editControl).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Syncfusion.Windows.Forms.Edit.EditControl editControl;
    }
}
