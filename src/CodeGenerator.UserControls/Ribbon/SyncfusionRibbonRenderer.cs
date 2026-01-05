using CodeGenerator.Shared.Ribbon;
using Syncfusion.Windows.Forms.Tools;
using System.Drawing;
using System.IO;

namespace CodeGenerator.UserControls.Ribbon;

/// <summary>
/// Renders RibbonViewModel to Syncfusion RibbonControlAdv
/// </summary>
public class SyncfusionRibbonRenderer : IRibbonRenderer
{
    /// <inheritdoc/>
    public void Render(RibbonViewModel viewModel, object ribbonControl)
    {
        if (ribbonControl is not RibbonControlAdv ribbon)
            throw new ArgumentException("ribbonControl must be a RibbonControlAdv", nameof(ribbonControl));

        foreach (var tabViewModel in viewModel.Tabs)
        {
            AddTab(tabViewModel, ribbon);
        }
    }

    /// <inheritdoc/>
    public void AddTab(RibbonTabViewModel tabViewModel, object ribbonControl)
    {
        if (ribbonControl is not RibbonControlAdv ribbon)
            throw new ArgumentException("ribbonControl must be a RibbonControlAdv", nameof(ribbonControl));

        var tab = CreateTab(tabViewModel);
        
        foreach (var toolStripViewModel in tabViewModel.ToolStrips)
        {
            var toolStrip = CreateToolStrip(toolStripViewModel);
            tab.Panel.Controls.Add(toolStrip);
        }

        ribbon.Header.AddMainItem(tab);
    }

    /// <inheritdoc/>
    public void RemoveTab(string tabName, object ribbonControl)
    {
        if (ribbonControl is not RibbonControlAdv ribbon)
            throw new ArgumentException("ribbonControl must be a RibbonControlAdv", nameof(ribbonControl));

        ToolStripTabItem? tabToRemove = null;
        var mainItems = ribbon.Header.MainItems.Cast<object>().ToList();
        
        foreach (var item in mainItems)
        {
            if (item is ToolStripTabItem tab && tab.Name == tabName)
            {
                tabToRemove = tab;
                break;
            }
        }

        if (tabToRemove != null)
        {
            // Dispose and remove the tab panel controls
            tabToRemove.Panel.Controls.Clear();
            ribbon.Controls.Remove(tabToRemove.Panel);
            tabToRemove.Dispose();
        }
    }

    private ToolStripTabItem CreateTab(RibbonTabViewModel viewModel)
    {
        var tab = new ToolStripTabItem
        {
            Name = viewModel.Name,
            Text = viewModel.Text,
            Position = viewModel.Position,
            Visible = viewModel.Visible
        };

        if (!string.IsNullOrEmpty(viewModel.Tag))
        {
            tab.Tag = viewModel.Tag;
        }

        tab.Panel.Name = $"ribbonPanel{viewModel.Name}";
        tab.Panel.Text = viewModel.Text;

        return tab;
    }

    private ToolStripEx CreateToolStrip(RibbonToolStripViewModel viewModel)
    {
        var toolStrip = new ToolStripEx
        {
            Name = viewModel.Name,
            Text = viewModel.Text,
            Dock = System.Windows.Forms.DockStyle.None,
            Font = new Font("Segoe UI", 8.25F),
            ForeColor = Color.MidnightBlue,
            GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden,
            Office12Mode = false,
            Padding = new System.Windows.Forms.Padding(3, 0, 0, 0),
            RightToLeft = System.Windows.Forms.RightToLeft.No
        };

        if (viewModel.ImageData != null)
        {
            if(viewModel.ImageData is byte[] byteData)
            {
                using var ms = new MemoryStream(byteData);
                toolStrip.Image = Image.FromStream(ms);
            } 
            else if(viewModel.ImageData is Image img)
            {
                toolStrip.Image = img;
            } else
            {
                throw new InvalidOperationException($"Unsupported image data type for toolstrip {viewModel.Name}");
            }
            //using var ms = new MemoryStream(viewModel.ImageData);
            //toolStrip.Image = Image.FromStream(ms);
        }

        foreach (var itemViewModel in viewModel.Items)
        {
            var item = CreateToolStripItem(itemViewModel);
            if (item != null)
            {
                toolStrip.Items.Add(item);
            }
        }

        return toolStrip;
    }

    private System.Windows.Forms.ToolStripItem? CreateToolStripItem(RibbonItemViewModel viewModel)
    {
        return viewModel switch
        {
            RibbonButtonViewModel buttonVm => CreateButton(buttonVm),
            RibbonSeparatorViewModel => new System.Windows.Forms.ToolStripSeparator(),
            _ => null
        };
    }

    private System.Windows.Forms.ToolStripButton CreateButton(RibbonButtonViewModel viewModel)
    {
        var button = new System.Windows.Forms.ToolStripButton
        {
            Name = viewModel.Name,
            Text = viewModel.Text,
            Enabled = viewModel.Enabled,
            Visible = viewModel.Visible,
            DisplayStyle = ConvertDisplayStyle(viewModel.DisplayStyle),
            ImageTransparentColor = Color.Magenta
        };

        if (viewModel.ImageData != null)
        {
            if(viewModel.ImageData is byte[] byteData)
            {
                using var ms = new MemoryStream(byteData);
                button.Image = Image.FromStream(ms);
            } 
            else if(viewModel.ImageData is Image img)
            {
                button.Image = img;
            } else
            {
                throw new InvalidOperationException($"Unsupported image data type for button {viewModel.Name}");
            }
        }

        if (!string.IsNullOrEmpty(viewModel.ToolTipText))
        {
            button.ToolTipText = viewModel.ToolTipText;
        }

        // Apply size
        if (viewModel.Size == RibbonButtonSize.Large)
        {
            button.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            button.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
        }

        // Wire up click handler
        if (viewModel.ClickHandler != null)
        {
            button.Click += (sender, e) => viewModel.ClickHandler(e);
        }

        return button;
    }

    private System.Windows.Forms.ToolStripItemDisplayStyle ConvertDisplayStyle(RibbonButtonDisplayStyle style)
    {
        return style switch
        {
            RibbonButtonDisplayStyle.Image => System.Windows.Forms.ToolStripItemDisplayStyle.Image,
            RibbonButtonDisplayStyle.Text => System.Windows.Forms.ToolStripItemDisplayStyle.Text,
            RibbonButtonDisplayStyle.ImageAndText => System.Windows.Forms.ToolStripItemDisplayStyle.ImageAndText,
            _ => System.Windows.Forms.ToolStripItemDisplayStyle.ImageAndText
        };
    }
}
