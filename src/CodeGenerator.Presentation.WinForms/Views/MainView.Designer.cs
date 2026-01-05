namespace CodeGenerator.Presentation.WinForms
{
    partial class MainView
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainView));
            ribbonControl = new Syncfusion.Windows.Forms.Tools.RibbonControlAdv();
            backStageView = new Syncfusion.Windows.Forms.BackStageView(components);
            backStage1 = new Syncfusion.Windows.Forms.BackStage();
            back_tab_home = new Syncfusion.Windows.Forms.BackStageTab();
            button1 = new Button();
            back_tab_new = new Syncfusion.Windows.Forms.BackStageTab();
            btnNew = new Button();
            back_tab_open = new Syncfusion.Windows.Forms.BackStageTab();
            btnOpen = new Button();
            btnSave = new Syncfusion.Windows.Forms.BackStageButton();
            backStageSeparator1 = new Syncfusion.Windows.Forms.BackStageSeparator();
            btnExit = new Syncfusion.Windows.Forms.BackStageButton();
            toolStripTabItem1 = new Syncfusion.Windows.Forms.Tools.ToolStripTabItem();
            toolStripTabItem2 = new Syncfusion.Windows.Forms.Tools.ToolStripTabItem();
            statusStrip = new Syncfusion.Windows.Forms.Tools.StatusStripEx();
            lblProgress = new ToolStripStatusLabel();
            pbProgress = new ToolStripProgressBar();
            lblStatus = new Syncfusion.Windows.Forms.Tools.StatusStripLabel();
            dockingManager = new Syncfusion.Windows.Forms.Tools.DockingManager(components);
            ((System.ComponentModel.ISupportInitialize)ribbonControl).BeginInit();
            ribbonControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)backStage1).BeginInit();
            backStage1.SuspendLayout();
            back_tab_home.SuspendLayout();
            back_tab_new.SuspendLayout();
            back_tab_open.SuspendLayout();
            statusStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dockingManager).BeginInit();
            SuspendLayout();
            // 
            // ribbonControl
            // 
            ribbonControl.AutoLayoutToolStrip = true;
            ribbonControl.BackStageView = backStageView;
            ribbonControl.Font = new Font("Segoe UI", 8.25F);
            ribbonControl.Header.AddMainItem(toolStripTabItem1);
            ribbonControl.Header.AddMainItem(toolStripTabItem2);
            ribbonControl.LauncherStyle = Syncfusion.Windows.Forms.Tools.LauncherStyle.Metro;
            ribbonControl.Location = new Point(1, 0);
            ribbonControl.MenuButtonFont = new Font("Segoe UI", 8.25F);
            ribbonControl.MenuButtonText = "File";
            ribbonControl.MenuButtonWidth = 56;
            ribbonControl.MenuColor = Color.FromArgb(0, 114, 198);
            ribbonControl.Name = "ribbonControl";
            ribbonControl.OfficeColorScheme = Syncfusion.Windows.Forms.Tools.ToolStripEx.ColorScheme.Managed;
            // 
            // ribbonControl.OfficeMenu
            // 
            ribbonControl.OfficeMenu.Name = "OfficeMenu";
            ribbonControl.OfficeMenu.ShowItemToolTips = true;
            ribbonControl.OfficeMenu.Size = new Size(12, 65);
            ribbonControl.QuickPanelImageLayout = PictureBoxSizeMode.StretchImage;
            ribbonControl.QuickPanelVisible = false;
            ribbonControl.RibbonHeaderImage = Syncfusion.Windows.Forms.Tools.RibbonHeaderImage.None;
            ribbonControl.RibbonStyle = Syncfusion.Windows.Forms.Tools.RibbonStyle.Office2016;
            ribbonControl.SelectedTab = toolStripTabItem1;
            ribbonControl.ShowRibbonDisplayOptionButton = true;
            ribbonControl.Size = new Size(802, 160);
            ribbonControl.SystemText.QuickAccessDialogDropDownName = "Start menu";
            ribbonControl.SystemText.RenameDisplayLabelText = "&Display Name:";
            ribbonControl.TabIndex = 0;
            ribbonControl.Text = "ribbonControl";
            ribbonControl.ThemeName = "Office2016";
            ribbonControl.TitleColor = Color.Black;
            // 
            // backStageView
            // 
            backStageView.BackStage = backStage1;
            backStageView.HostControl = null;
            backStageView.HostForm = this;
            // 
            // backStage1
            // 
            backStage1.AllowDrop = true;
            backStage1.BackStagePanelWidth = 130;
            backStage1.BeforeTouchSize = new Size(798, 398);
            backStage1.BorderStyle = BorderStyle.None;
            backStage1.ChildItemSize = new Size(80, 140);
            backStage1.Controls.Add(back_tab_home);
            backStage1.Controls.Add(back_tab_new);
            backStage1.Controls.Add(back_tab_open);
            backStage1.Controls.Add(btnSave);
            backStage1.Controls.Add(backStageSeparator1);
            backStage1.Controls.Add(btnExit);
            backStage1.Font = new Font("Segoe UI", 8.25F);
            backStage1.ItemSize = new Size(130, 40);
            backStage1.Location = new Point(0, 0);
            backStage1.MinimumSize = new Size(100, 143);
            backStage1.Name = "backStage1";
            backStage1.OfficeColorScheme = Syncfusion.Windows.Forms.Tools.ToolStripEx.ColorScheme.Managed;
            backStage1.Size = new Size(798, 398);
            backStage1.TabIndex = 1;
            backStage1.ThemeName = "BackStage2016Renderer";
            backStage1.Visible = false;
            // 
            // back_tab_home
            // 
            back_tab_home.Accelerator = "";
            back_tab_home.Controls.Add(button1);
            back_tab_home.Image = Resources.LucideIcons__ffffff.house;
            back_tab_home.ImageSize = new Size(16, 16);
            back_tab_home.Location = new Point(129, 0);
            back_tab_home.Name = "back_tab_home";
            back_tab_home.Placement = Syncfusion.Windows.Forms.BackStageItemPlacement.Top;
            back_tab_home.Position = new Point(11, 51);
            back_tab_home.ShowCloseButton = true;
            back_tab_home.Size = new Size(669, 398);
            back_tab_home.TabIndex = 1;
            back_tab_home.Text = "Home";
            back_tab_home.ThemesEnabled = false;
            // 
            // button1
            // 
            button1.Location = new Point(50, 27);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 0;
            button1.Text = "button1";
            button1.UseVisualStyleBackColor = true;
            // 
            // back_tab_new
            // 
            back_tab_new.Accelerator = "";
            back_tab_new.Controls.Add(btnNew);
            back_tab_new.Image = Resources.LucideIcons__ffffff.file;
            back_tab_new.ImageSize = new Size(16, 16);
            back_tab_new.Location = new Point(129, 0);
            back_tab_new.Name = "back_tab_new";
            back_tab_new.Placement = Syncfusion.Windows.Forms.BackStageItemPlacement.Top;
            back_tab_new.Position = new Point(52, 92);
            back_tab_new.ShowCloseButton = true;
            back_tab_new.Size = new Size(669, 398);
            back_tab_new.TabIndex = 2;
            back_tab_new.Text = "Nieuw";
            back_tab_new.ThemesEnabled = false;
            // 
            // btnNew
            // 
            btnNew.Location = new Point(45, 40);
            btnNew.Name = "btnNew";
            btnNew.Size = new Size(75, 23);
            btnNew.TabIndex = 0;
            btnNew.Text = "Nieuw";
            btnNew.UseVisualStyleBackColor = true;
            // 
            // back_tab_open
            // 
            back_tab_open.Accelerator = "";
            back_tab_open.Controls.Add(btnOpen);
            back_tab_open.Image = Resources.LucideIcons__ffffff.folder_open;
            back_tab_open.ImageSize = new Size(16, 16);
            back_tab_open.Location = new Point(129, 0);
            back_tab_open.Name = "back_tab_open";
            back_tab_open.Placement = Syncfusion.Windows.Forms.BackStageItemPlacement.Top;
            back_tab_open.Position = new Point(93, 133);
            back_tab_open.ShowCloseButton = true;
            back_tab_open.Size = new Size(669, 398);
            back_tab_open.TabIndex = 3;
            back_tab_open.Text = "&Openen";
            back_tab_open.ThemesEnabled = false;
            // 
            // btnOpen
            // 
            btnOpen.Location = new Point(59, 31);
            btnOpen.Name = "btnOpen";
            btnOpen.Size = new Size(75, 23);
            btnOpen.TabIndex = 0;
            btnOpen.Text = "Open";
            btnOpen.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            btnSave.Accelerator = "";
            btnSave.BackColor = Color.Transparent;
            btnSave.BeforeTouchSize = new Size(110, 25);
            btnSave.Image = Resources.LucideIcons__ffffff.save;
            btnSave.Location = new Point(10, 133);
            btnSave.Name = "btnSave";
            btnSave.Placement = Syncfusion.Windows.Forms.BackStageItemPlacement.Top;
            btnSave.Size = new Size(110, 25);
            btnSave.TabIndex = 6;
            btnSave.Text = "Op&slaan";
            // 
            // backStageSeparator1
            // 
            backStageSeparator1.BackColor = Color.FromArgb(100, 189, 255);
            backStageSeparator1.Location = new Point(20, 196);
            backStageSeparator1.Name = "backStageSeparator1";
            backStageSeparator1.Placement = Syncfusion.Windows.Forms.BackStageItemPlacement.Top;
            backStageSeparator1.Size = new Size(90, 1);
            backStageSeparator1.TabIndex = 7;
            backStageSeparator1.Text = "backStageSeparator1";
            // 
            // btnExit
            // 
            btnExit.Accelerator = "";
            btnExit.BackColor = Color.Transparent;
            btnExit.BeforeTouchSize = new Size(110, 25);
            btnExit.Image = Resources.LucideIcons__ffffff.square_x;
            btnExit.Location = new Point(10, 158);
            btnExit.Name = "btnExit";
            btnExit.Placement = Syncfusion.Windows.Forms.BackStageItemPlacement.Top;
            btnExit.Size = new Size(110, 25);
            btnExit.TabIndex = 5;
            btnExit.Text = "Afsluiten";
            // 
            // toolStripTabItem1
            // 
            toolStripTabItem1.Name = "toolStripTabItem1";
            // 
            // ribbonControl.ribbonPanel1
            // 
            toolStripTabItem1.Panel.Name = "ribbonPanel1";
            toolStripTabItem1.Panel.ScrollPosition = 0;
            toolStripTabItem1.Panel.TabIndex = 2;
            toolStripTabItem1.Panel.Text = "toolStripTabItem1";
            toolStripTabItem1.Position = 0;
            toolStripTabItem1.Size = new Size(119, 31);
            ribbonControl.TabGroups.SetTabGroup(toolStripTabItem1, null);
            toolStripTabItem1.Tag = "1";
            toolStripTabItem1.Text = "toolStripTabItem1";
            // 
            // toolStripTabItem2
            // 
            toolStripTabItem2.Name = "toolStripTabItem2";
            // 
            // ribbonControl.ribbonPanel2
            // 
            toolStripTabItem2.Panel.Name = "ribbonPanel2";
            toolStripTabItem2.Panel.ScrollPosition = 0;
            toolStripTabItem2.Panel.TabIndex = 3;
            toolStripTabItem2.Panel.Text = "toolStripTabItem2";
            toolStripTabItem2.Position = 1;
            toolStripTabItem2.Size = new Size(119, 31);
            ribbonControl.TabGroups.SetTabGroup(toolStripTabItem2, null);
            toolStripTabItem2.Tag = "2";
            toolStripTabItem2.Text = "toolStripTabItem2";
            // 
            // statusStrip
            // 
            statusStrip.BackColor = Color.FromArgb(241, 241, 241);
            statusStrip.BeforeTouchSize = new Size(798, 22);
            statusStrip.Items.AddRange(new ToolStripItem[] { lblProgress, pbProgress, lblStatus });
            statusStrip.Location = new Point(2, 427);
            statusStrip.MetroColor = Color.FromArgb(135, 206, 255);
            statusStrip.Name = "statusStrip";
            statusStrip.Size = new Size(798, 22);
            statusStrip.TabIndex = 2;
            statusStrip.Text = "statusStripEx1";
            statusStrip.ThemeName = "Office2016Colorful";
            // 
            // lblProgress
            // 
            lblProgress.Name = "lblProgress";
            lblProgress.Size = new Size(68, 15);
            lblProgress.Text = "<progress>";
            // 
            // pbProgress
            // 
            pbProgress.Name = "pbProgress";
            pbProgress.Size = new Size(100, 15);
            // 
            // lblStatus
            // 
            lblStatus.Margin = new Padding(0, 3, 0, 2);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(54, 15);
            lblStatus.Text = "<status>";
            // 
            // dockingManager
            // 
            dockingManager.AnimateAutoHiddenWindow = true;
            dockingManager.AutoHideTabForeColor = Color.Empty;
            dockingManager.CloseTabOnMiddleClick = false;
            dockingManager.HostControl = this;
            dockingManager.MetroButtonColor = Color.FromArgb(255, 255, 255);
            dockingManager.MetroColor = Color.FromArgb(17, 158, 218);
            dockingManager.MetroSplitterBackColor = Color.FromArgb(155, 159, 183);
            dockingManager.ReduceFlickeringInRtl = false;
            dockingManager.CaptionButtons.Add(new Syncfusion.Windows.Forms.Tools.CaptionButton(Syncfusion.Windows.Forms.Tools.CaptionButtonType.Close, "CloseButton"));
            dockingManager.CaptionButtons.Add(new Syncfusion.Windows.Forms.Tools.CaptionButton(Syncfusion.Windows.Forms.Tools.CaptionButtonType.Pin, "PinButton"));
            dockingManager.CaptionButtons.Add(new Syncfusion.Windows.Forms.Tools.CaptionButton(Syncfusion.Windows.Forms.Tools.CaptionButtonType.Maximize, "MaximizeButton"));
            dockingManager.CaptionButtons.Add(new Syncfusion.Windows.Forms.Tools.CaptionButton(Syncfusion.Windows.Forms.Tools.CaptionButtonType.Restore, "RestoreButton"));
            dockingManager.CaptionButtons.Add(new Syncfusion.Windows.Forms.Tools.CaptionButton(Syncfusion.Windows.Forms.Tools.CaptionButtonType.Menu, "MenuButton"));
            // 
            // MainView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(statusStrip);
            Controls.Add(backStage1);
            Controls.Add(ribbonControl);
            Icon = (Icon)resources.GetObject("$this.Icon");
            IsMdiContainer = true;
            Name = "MainView";
            Padding = new Padding(1, 0, 1, 1);
            Text = "Code Generator";
            ((System.ComponentModel.ISupportInitialize)ribbonControl).EndInit();
            ribbonControl.ResumeLayout(false);
            ribbonControl.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)backStage1).EndInit();
            backStage1.ResumeLayout(false);
            back_tab_home.ResumeLayout(false);
            back_tab_new.ResumeLayout(false);
            back_tab_open.ResumeLayout(false);
            statusStrip.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dockingManager).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Syncfusion.Windows.Forms.Tools.RibbonControlAdv ribbonControl;
        private Syncfusion.Windows.Forms.Tools.ToolStripTabItem toolStripTabItem1;
        private Syncfusion.Windows.Forms.BackStageView backStageView;
        private Syncfusion.Windows.Forms.BackStage backStage1;
        private Syncfusion.Windows.Forms.BackStageTab back_tab_home;
        private Syncfusion.Windows.Forms.BackStageTab back_tab_new;
        private Syncfusion.Windows.Forms.BackStageTab back_tab_open;
        private Syncfusion.Windows.Forms.BackStageButton btnExit;
        private Syncfusion.Windows.Forms.Tools.ToolStripTabItem toolStripTabItem2;
        private Button btnOpen;
        private Button button1;
        private Button btnNew;
        private Syncfusion.Windows.Forms.BackStageButton btnSave;
        private Syncfusion.Windows.Forms.BackStageSeparator backStageSeparator1;
        private Syncfusion.Windows.Forms.Tools.StatusStripEx statusStrip;
        private ToolStripStatusLabel lblProgress;
        private ToolStripProgressBar pbProgress;
        private Syncfusion.Windows.Forms.Tools.StatusStripLabel lblStatus;
        private Syncfusion.Windows.Forms.Tools.DockingManager dockingManager;
    }
}
