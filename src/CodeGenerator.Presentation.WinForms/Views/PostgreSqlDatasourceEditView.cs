using CodeGenerator.Core.Workspaces.Datasources.PostgreSql.ViewModels;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using CodeGenerator.UserControls.Views;
using System.ComponentModel;

namespace CodeGenerator.Presentation.WinForms.Views;

/// <summary>
/// View for editing PostgreSQL datasource properties
/// </summary>
public partial class PostgreSqlDatasourceEditView : UserControl, IView<PostgreSqlDatasourceEditViewModel>
{
    private PostgreSqlDatasourceEditViewModel? _viewModel;
    private CancellationTokenSource? _loadingCts;

    public PostgreSqlDatasourceEditView()
    {
        InitializeComponent();

        btnLoadObjects.Click += BtnLoadObjects_Click;
        btnAddObject.Click += BtnAddObject_Click;
        lstObjects.SelectedIndexChanged += LstObjects_SelectedIndexChanged;
    }

    public void BindViewModel(PostgreSqlDatasourceEditViewModel viewModel)
    {
        if (_viewModel != null)
        {
            _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
            _viewModel.AvailableObjects.CollectionChanged -= AvailableObjects_CollectionChanged;
        }

        _viewModel = viewModel;

        if (_viewModel == null) return;

        // Bind fields
        txtName.BindViewModel(_viewModel.NameField);
        txtServer.BindViewModel(_viewModel.ServerField);
        txtPort.BindViewModel(_viewModel.PortField);
        txtDatabase.BindViewModel(_viewModel.DatabaseField);
        txtUsername.BindViewModel(_viewModel.UsernameField);
        txtPassword.BindViewModel(_viewModel.PasswordField);
        cboSslMode.BindViewModel(_viewModel.SslModeField);

        // Subscribe to events
        _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        _viewModel.AvailableObjects.CollectionChanged += AvailableObjects_CollectionChanged;

        UpdateUI();
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (InvokeRequired)
        {
            Invoke(() => ViewModel_PropertyChanged(sender, e));
            return;
        }

        if (e.PropertyName == nameof(PostgreSqlDatasourceEditViewModel.IsConnecting))
        {
            UpdateUI();
        }
        else if (e.PropertyName == nameof(PostgreSqlDatasourceEditViewModel.ConnectionStatus))
        {
            lblStatus.Text = _viewModel?.ConnectionStatus ?? string.Empty;
        }
        else if (e.PropertyName == nameof(PostgreSqlDatasourceEditViewModel.ErrorMessage))
        {
            lblError.Text = _viewModel?.ErrorMessage ?? string.Empty;
            lblError.Visible = !string.IsNullOrEmpty(_viewModel?.ErrorMessage);
        }
        else if (e.PropertyName == nameof(PostgreSqlDatasourceEditViewModel.SelectedObject))
        {
            btnAddObject.Enabled = _viewModel?.SelectedObject != null;
        }
    }

    private void AvailableObjects_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (InvokeRequired)
        {
            Invoke(() => AvailableObjects_CollectionChanged(sender, e));
            return;
        }

        RefreshObjectsList();
    }

    private void RefreshObjectsList()
    {
        lstObjects.Items.Clear();

        if (_viewModel == null) return;

        foreach (var obj in _viewModel.AvailableObjects)
        {
            var item = new ListViewItem(obj.Name)
            {
                Tag = obj,
                ImageKey = obj.TypeIcon
            };
            item.SubItems.Add(obj.Schema);
            item.SubItems.Add(obj.ObjectType.ToString());
            lstObjects.Items.Add(item);
        }
    }

    private void UpdateUI()
    {
        var isConnecting = _viewModel?.IsConnecting ?? false;

        btnLoadObjects.Enabled = !isConnecting;
        btnLoadObjects.Text = isConnecting ? "Loading..." : "Load Tables/Views";

        grpConnection.Enabled = !isConnecting;
        btnAddObject.Enabled = !isConnecting && _viewModel?.SelectedObject != null;
    }

    private async void BtnLoadObjects_Click(object? sender, EventArgs e)
    {
        if (_viewModel == null) return;

        _loadingCts?.Cancel();
        _loadingCts = new CancellationTokenSource();

        try
        {
            await _viewModel.LoadDatabaseObjectsAsync(_loadingCts.Token);
        }
        catch (OperationCanceledException)
        {
            // Cancelled, ignore
        }
    }

    private async void BtnAddObject_Click(object? sender, EventArgs e)
    {
        if (_viewModel == null) return;

        try
        {
            await _viewModel.AddSelectedObjectAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error adding object: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void LstObjects_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (_viewModel == null) return;

        _viewModel.SelectedObject = lstObjects.SelectedItems.Count > 0
            ? lstObjects.SelectedItems[0].Tag as DatabaseObjectViewModel
            : null;
    }

    public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
    {
        BindViewModel((PostgreSqlDatasourceEditViewModel)(object)viewModel);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _loadingCts?.Cancel();
            _loadingCts?.Dispose();

            if (_viewModel != null)
            {
                _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
                _viewModel.AvailableObjects.CollectionChanged -= AvailableObjects_CollectionChanged;
            }

            components?.Dispose();
        }
        base.Dispose(disposing);
    }
}
