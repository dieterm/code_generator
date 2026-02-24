using CodeGenerator.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.ViewModels.Browser
{
    public class BrowserViewModel : ViewModelBase
    {
        private string? _url;
        public string? Url
        {
            get { return _url; }
            set { SetProperty(ref _url, value); }
        }

        private string? _htmlContent;
        public string? HtmlContent
        {
            get { return _htmlContent; }
            set { SetProperty(ref _htmlContent, value); }
        }
        private string _title;
        /// <summary>
        /// Tab title
        /// </summary>
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

    }
}
