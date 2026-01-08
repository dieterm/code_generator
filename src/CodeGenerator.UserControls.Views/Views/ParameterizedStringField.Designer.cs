
using System.ComponentModel;

namespace CodeGenerator.UserControls.Views
{
    partial class ParameterizedStringField
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
            components = new Container();
            ComponentResourceManager resources = new ComponentResourceManager(typeof(ParameterizedStringField));
            lblLabel = new Label();
            txtValue = new Syncfusion.Windows.Forms.Tools.TextBoxExt();
            lblPreview = new Label();
            btnShowParameters = new Button();
            ctxMenuParameters = new ContextMenuStrip(components);
            lblErrorMessage = new Label();
            ((ISupportInitialize)txtValue).BeginInit();
            SuspendLayout();
            // 
            // lblLabel
            // 
            lblLabel.AutoSize = true;
            lblLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblLabel.Location = new Point(3, 4);
            lblLabel.Name = "lblLabel";
            lblLabel.Size = new Size(68, 15);
            lblLabel.TabIndex = 1;
            lblLabel.Text = "Field Label:";
            // 
            // txtValue
            // 
            txtValue.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtValue.BeforeTouchSize = new Size(181, 23);
            txtValue.Location = new Point(115, 0);
            txtValue.Name = "txtValue";
            txtValue.Size = new Size(181, 23);
            txtValue.TabIndex = 2;
            // 
            // lblPreview
            // 
            lblPreview.AutoSize = true;
            lblPreview.Font = new Font("Segoe UI", 9F, FontStyle.Italic, GraphicsUnit.Point, 0);
            lblPreview.Location = new Point(115, 26);
            lblPreview.Name = "lblPreview";
            lblPreview.Size = new Size(188, 15);
            lblPreview.TabIndex = 3;
            lblPreview.Text = "VzwWijzer.Geoservice.Presentation";
            // 
            // btnShowParameters
            // 
            btnShowParameters.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnShowParameters.ContextMenuStrip = ctxMenuParameters;
            btnShowParameters.Image = (Image)resources.GetObject("btnShowParameters.Image");
            btnShowParameters.Location = new Point(300, 0);
            btnShowParameters.Name = "btnShowParameters";
            btnShowParameters.Size = new Size(26, 23);
            btnShowParameters.TabIndex = 4;
            btnShowParameters.UseVisualStyleBackColor = true;
            btnShowParameters.Click += btnShowParameters_Click;
            // 
            // ctxMenuParameters
            // 
            ctxMenuParameters.Name = "ctxMenuParameters";
            ctxMenuParameters.Size = new Size(61, 4);
            ctxMenuParameters.Opening += ctxMenuParameters_Opening;
            // 
            // lblErrorMessage
            // 
            lblErrorMessage.AutoSize = true;
            lblErrorMessage.ForeColor = Color.Red;
            lblErrorMessage.Location = new Point(115, 41);
            lblErrorMessage.Name = "lblErrorMessage";
            lblErrorMessage.Size = new Size(81, 15);
            lblErrorMessage.TabIndex = 6;
            lblErrorMessage.Text = "Error message";
            // 
            // ParameterizedStringField
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(lblErrorMessage);
            Controls.Add(btnShowParameters);
            Controls.Add(lblPreview);
            Controls.Add(txtValue);
            Controls.Add(lblLabel);
            Name = "ParameterizedStringField";
            Size = new Size(329, 59);
            ((ISupportInitialize)txtValue).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }


        #endregion

        private Label lblLabel;
        private Syncfusion.Windows.Forms.Tools.TextBoxExt txtValue;
        private Label lblPreview;
        private Button btnShowParameters;
        private ContextMenuStrip ctxMenuParameters;
        private Label lblErrorMessage;
    }
}
