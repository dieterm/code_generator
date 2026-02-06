using CodeGenerator.Shared.Ribbon;
using Microsoft.DotNet.DesignTools.ViewModels;
using Syncfusion.Windows.Forms.Tools;
using System.Drawing;
using System.IO;
using System.Resources;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace CodeGenerator.UserControls.Ribbon;

/// <summary>
/// Renders RibbonViewModel to Syncfusion RibbonControlAdv
/// </summary>
public class SyncfusionRibbonRenderer : IRibbonRenderer
{
    private global::System.Resources.ResourceManager? _resourceManager;

    public IRibbonRenderer SetResourceManager(global::System.Resources.ResourceManager resourceManager)
    {
        _resourceManager = resourceManager;
        return this;
    }

    /// <summary>
    /// try to get Image from ResourceManager.
    /// if not found, returns null.
    /// </summary>
    public Image? GetResourceImage(string resourceKey)
    {
        if(_resourceManager==null)
            throw new InvalidOperationException("ResourceManager is not set. Call SyncfusionRibbonRenderer.SetResourceManager(...) first.");
        
        object? obj = _resourceManager.GetObject(resourceKey);
        if (obj == null)
            return null;

        return (Image)obj;
    }

    public void SetImageFromModel(Func<object> imageDataGetter, Action<Image> imageSetter, string modelName)
    {
        var imageData = imageDataGetter();
        if (imageData is byte[] byteData)
        {
            using var ms = new MemoryStream(byteData);
            imageSetter(Image.FromStream(ms));
        }
        else if (imageData is Image img)
        {
            imageSetter( img);
        }
        else if (imageData is string resourceName)
        {
            var imgFromResource = GetResourceImage(resourceName);
            if (imgFromResource != null)
                imageSetter(imgFromResource);
            else
            {
                if(resourceName.Contains("_"))
                {
                    resourceName = resourceName.Replace("_", "-");
                    imgFromResource = GetResourceImage(resourceName);
                    if (imgFromResource != null)
                    {
                        imageSetter(imgFromResource);
                        return;
                    }
                }
                throw new InvalidOperationException($"Image resource '{resourceName}' not found for toolstrip {modelName}");
            }

        }
        else
        {
            throw new InvalidOperationException($"Unsupported image data type for button {modelName}");
        }
    }

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
            SetImageFromModel(() => viewModel.ImageData, img => toolStrip.Image = img, viewModel.Name);
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
            RibbonDropDownButtonViewModel dropDownVm => CreateDropDownButton(dropDownVm),
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
       
        if (viewModel.HiddenWhenDisabled) { 
            button.EnabledChanged += (s, e) =>
            {
                // When enabled state changes, update visibility if needed
                button.Visible = button.Enabled;
            };
        }

        if (viewModel.Command != null)
        {
            // set this after the EnabledChanged event to avoid overwriting visibility
            button.Command = viewModel.Command;
            button.CommandParameter = viewModel.CommandParameter;
        }

        if (viewModel.ImageData != null)
        {
            SetImageFromModel(() => viewModel.ImageData, img => button.Image = img, viewModel.Name);
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

    private System.Windows.Forms.ToolStripSplitButton CreateDropDownButton(RibbonDropDownButtonViewModel viewModel)
    {
        var button = new System.Windows.Forms.ToolStripSplitButton
        {
            Name = viewModel.Name,
            Text = viewModel.Text,
            Enabled = viewModel.Enabled,
            Visible = viewModel.Visible,
            DisplayStyle = ConvertDisplayStyle(viewModel.DisplayStyle),
            ImageTransparentColor = Color.Magenta
        };

        if (viewModel.HiddenWhenDisabled)
        {
            button.EnabledChanged += (s, e) =>
            {
                button.Visible = button.Enabled;
            };
        }

        if (viewModel.Command != null)
        {
            // Use ButtonClick so only the main button area triggers the command,
            // not the dropdown arrow
            button.ButtonClick += (s, e) =>
            {
                if (viewModel.Command.CanExecute(viewModel.CommandParameter))
                    viewModel.Command.Execute(viewModel.CommandParameter);
            };

            // Sync enabled state with command
            viewModel.Command.CanExecuteChanged += (s, e) =>
            {
                if (button.IsDisposed) return;
                var canExecute = viewModel.Command.CanExecute(viewModel.CommandParameter);
                if (button.Owner?.InvokeRequired == true)
                    button.Owner.Invoke(() => button.Enabled = canExecute);
                else
                    button.Enabled = canExecute;
            };

            button.Enabled = viewModel.Command.CanExecute(viewModel.CommandParameter);
        }

        if (viewModel.ImageData != null)
        {
            SetImageFromModel(() => viewModel.ImageData, img => button.Image = img, viewModel.Name);
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

        // Wire up click handler (main button area only)
        if (viewModel.ClickHandler != null)
        {
            button.ButtonClick += (sender, e) => viewModel.ClickHandler(e);
        }

        // Populate static dropdown items
        PopulateDropDownItems(button, viewModel.DropDownItems);

        // If a dynamic provider is set, repopulate on each dropdown opening
        if (viewModel.DropDownItemsProvider != null)
        {
            button.DropDownOpening += (s, e) =>
            {
                button.DropDownItems.Clear();
                var items = viewModel.DropDownItemsProvider();
                foreach (var item in items)
                {
                    button.DropDownItems.Add(CreateDropDownMenuItem(item));
                }
            };
        }

        return button;
    }

    private void PopulateDropDownItems(System.Windows.Forms.ToolStripSplitButton button, IEnumerable<RibbonDropDownItemViewModel> items)
    {
        foreach (var item in items)
        {
            button.DropDownItems.Add(CreateDropDownMenuItem(item));
        }
    }

    private System.Windows.Forms.ToolStripItem CreateDropDownMenuItem(RibbonDropDownItemViewModel viewModel)
    {
        if (viewModel.IsSeparator)
        {
            return new System.Windows.Forms.ToolStripSeparator();
        }

        var menuItem = new System.Windows.Forms.ToolStripMenuItem
        {
            Name = viewModel.Name,
            Text = viewModel.Text,
            Enabled = viewModel.Enabled,
            Visible = viewModel.Visible,
            Tag = viewModel.Tag
        };

        if (viewModel.ImageData != null)
        {
            SetImageFromModel(() => viewModel.ImageData, img => menuItem.Image = img, viewModel.Name);
        }

        if (!string.IsNullOrEmpty(viewModel.ToolTipText))
        {
            menuItem.ToolTipText = viewModel.ToolTipText;
        }

        if (viewModel.ClickHandler != null)
        {
            menuItem.Click += (sender, e) => viewModel.ClickHandler(viewModel);
        }

        return menuItem;
    }
}
