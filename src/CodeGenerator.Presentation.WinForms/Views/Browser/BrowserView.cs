using CodeGenerator.Core.ViewModels.Browser;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using Microsoft.Web.WebView2.WinForms;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodeGenerator.Presentation.WinForms.Views.Browser
{
    public partial class BrowserView : UserControl, IView<BrowserViewModel>
    {
        private BrowserViewModel? _viewModel;
        private WebView2? webView;
        private bool _isWebViewInitialized = false;
        private string? _pendingUrl;
        private string? _pendingHtmlContent;

        public BrowserView()
        {
            InitializeComponent();
            
            webView = new WebView2
            {
                Dock = System.Windows.Forms.DockStyle.Fill
            };
            Controls.Add(webView);
            Load += BrowserView_Load;
            Disposed += BrowserView_Disposed;
        }

        private void BrowserView_Disposed(object? sender, EventArgs e)
        {
            if (_viewModel != null)
            {
                _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
            }
            webView?.Dispose();
        }

        private async void BrowserView_Load(object? sender, EventArgs e)
        {
            try
            {
                await webView!.EnsureCoreWebView2Async(null);
                _isWebViewInitialized = true;
                
                // Process any pending navigation
                if (!string.IsNullOrEmpty(_pendingUrl))
                {
                    NavigateToUrl(_pendingUrl);
                    _pendingUrl = null;
                }
                else if (!string.IsNullOrEmpty(_pendingHtmlContent))
                {
                    NavigateToHtmlContent(_pendingHtmlContent);
                    _pendingHtmlContent = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to initialize WebView2: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void BindViewModel(BrowserViewModel viewModel)
        {
            if (_viewModel != null)
            {
                _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
            }

            _viewModel = viewModel;
            _viewModel.PropertyChanged += ViewModel_PropertyChanged;

            // Navigate to initial URL or HTML content if set
            if (!string.IsNullOrEmpty(_viewModel.Url))
            {
                NavigateToUrl(_viewModel.Url);
            } 
            else if(!string.IsNullOrEmpty(_viewModel.HtmlContent))
            {
                NavigateToHtmlContent(_viewModel.HtmlContent);
            }
        }

        private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(BrowserViewModel.Url))
            {
                NavigateToUrl(_viewModel?.Url);
            } 
            else if(e.PropertyName == nameof(BrowserViewModel.HtmlContent))
            {
                NavigateToHtmlContent(_viewModel?.HtmlContent);
            }
        }

        private void NavigateToUrl(string? url)
        {
            if (string.IsNullOrEmpty(url))
                return;

            if (_isWebViewInitialized && webView?.CoreWebView2 != null)
            {
                webView.CoreWebView2.Navigate(url);
            }
            else
            {
                // Store URL to navigate to once WebView2 is initialized
                _pendingUrl = url;
                _pendingHtmlContent = null;
            }
        }

        private void NavigateToHtmlContent(string? htmlContent)
        {
            if (string.IsNullOrEmpty(htmlContent))
                return;

            if (_isWebViewInitialized && webView?.CoreWebView2 != null)
            {
                webView.CoreWebView2.NavigateToString(htmlContent);
            }
            else
            {
                // Store HTML content to navigate to once WebView2 is initialized
                _pendingHtmlContent = htmlContent;
                _pendingUrl = null;
            }
        }

        public void BindViewModel<TModel>(TModel viewModel) where TModel : IViewModel
        {
            BindViewModel((BrowserViewModel)(object)viewModel);
        }
    }
}
