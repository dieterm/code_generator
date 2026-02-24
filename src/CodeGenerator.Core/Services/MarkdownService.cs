using Markdig;
using System;
using System.Text;

namespace CodeGenerator.Core.Services
{
    /// <summary>
    /// Service for converting Markdown content to HTML with embedded CSS styling
    /// </summary>
    public class MarkdownService
    {
        private readonly MarkdownPipeline _pipeline;

        public MarkdownService()
        {
            // Configure Markdig pipeline with advanced features
            _pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions() // Enables tables, task lists, definition lists, etc.
                .UsePipeTables() // Support for pipe tables
                .UseEmphasisExtras() // Support for strikethrough, subscript, superscript
                .UseGridTables() // Support for grid tables
                .UseAutoLinks() // Automatically convert URLs to links
                .UseTaskLists() // Support for GitHub-style task lists
                .UseAutoIdentifiers() // Auto-generate IDs for headers
                .Build();
        }

        /// <summary>
        /// Convert Markdown content to a complete HTML page with embedded CSS
        /// </summary>
        /// <param name="markdownContent">The Markdown content to convert</param>
        /// <param name="title">Optional title for the HTML page (defaults to "Document")</param>
        /// <returns>Complete HTML page as string</returns>
        public string ConvertToHtml(string markdownContent, string title = "Document")
        {
            if (string.IsNullOrEmpty(markdownContent))
            {
                return GenerateEmptyHtml(title);
            }

            // Convert markdown to HTML
            var htmlContent = Markdown.ToHtml(markdownContent, _pipeline);

            // Build complete HTML page with embedded CSS
            return BuildCompleteHtmlPage(htmlContent, title);
        }

        /// <summary>
        /// Convert Markdown content to HTML body only (without HTML wrapper and CSS)
        /// </summary>
        /// <param name="markdownContent">The Markdown content to convert</param>
        /// <returns>HTML body content as string</returns>
        public string ConvertToHtmlBody(string markdownContent)
        {
            if (string.IsNullOrEmpty(markdownContent))
            {
                return string.Empty;
            }

            return Markdown.ToHtml(markdownContent, _pipeline);
        }

        private string BuildCompleteHtmlPage(string htmlBody, string title)
        {
            var sb = new StringBuilder();
            
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html lang=\"en\">");
            sb.AppendLine("<head>");
            sb.AppendLine("    <meta charset=\"UTF-8\">");
            sb.AppendLine("    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
            sb.AppendLine($"    <title>{EscapeHtml(title)}</title>");
            sb.AppendLine("    <style>");
            sb.AppendLine(GetEmbeddedCss());
            sb.AppendLine("    </style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("    <div class=\"markdown-body\">");
            sb.AppendLine(htmlBody);
            sb.AppendLine("    </div>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            return sb.ToString();
        }

        private string GenerateEmptyHtml(string title)
        {
            return BuildCompleteHtmlPage("<p>No content available.</p>", title);
        }

        private string EscapeHtml(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            return text
                .Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\"", "&quot;")
                .Replace("'", "&#39;");
        }

        private string GetEmbeddedCss()
        {
            // GitHub-inspired markdown styling
            return @"
body {
    font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', 'Roboto', 'Oxygen', 'Ubuntu', 'Cantarell', 'Fira Sans', 'Droid Sans', 'Helvetica Neue', sans-serif;
    line-height: 1.6;
    color: #24292e;
    background-color: #ffffff;
    margin: 0;
    padding: 0;
}

.markdown-body {
    max-width: 980px;
    margin: 0 auto;
    padding: 45px;
    box-sizing: border-box;
}

.markdown-body h1,
.markdown-body h2,
.markdown-body h3,
.markdown-body h4,
.markdown-body h5,
.markdown-body h6 {
    margin-top: 24px;
    margin-bottom: 16px;
    font-weight: 600;
    line-height: 1.25;
    padding-bottom: 0.3em;
}

.markdown-body h1 {
    font-size: 2em;
    border-bottom: 1px solid #eaecef;
}

.markdown-body h2 {
    font-size: 1.5em;
    border-bottom: 1px solid #eaecef;
}

.markdown-body h3 {
    font-size: 1.25em;
}

.markdown-body h4 {
    font-size: 1em;
}

.markdown-body h5 {
    font-size: 0.875em;
}

.markdown-body h6 {
    font-size: 0.85em;
    color: #6a737d;
}

.markdown-body p {
    margin-top: 0;
    margin-bottom: 16px;
}

.markdown-body a {
    color: #0366d6;
    text-decoration: none;
}

.markdown-body a:hover {
    text-decoration: underline;
}

.markdown-body blockquote {
    padding: 0 1em;
    color: #6a737d;
    border-left: 0.25em solid #dfe2e5;
    margin: 0 0 16px 0;
}

.markdown-body blockquote > :first-child {
    margin-top: 0;
}

.markdown-body blockquote > :last-child {
    margin-bottom: 0;
}

.markdown-body code {
    padding: 0.2em 0.4em;
    margin: 0;
    font-size: 85%;
    background-color: rgba(27, 31, 35, 0.05);
    border-radius: 3px;
    font-family: 'SFMono-Regular', Consolas, 'Liberation Mono', Menlo, Courier, monospace;
}

.markdown-body pre {
    padding: 16px;
    overflow: auto;
    font-size: 85%;
    line-height: 1.45;
    background-color: #f6f8fa;
    border-radius: 3px;
    margin-bottom: 16px;
}

.markdown-body pre code {
    display: inline;
    padding: 0;
    margin: 0;
    overflow: visible;
    line-height: inherit;
    background-color: transparent;
    border: 0;
}

.markdown-body table {
    border-spacing: 0;
    border-collapse: collapse;
    margin-top: 0;
    margin-bottom: 16px;
    display: block;
    width: 100%;
    overflow: auto;
}

.markdown-body table th {
    font-weight: 600;
    padding: 6px 13px;
    border: 1px solid #dfe2e5;
    background-color: #f6f8fa;
}

.markdown-body table td {
    padding: 6px 13px;
    border: 1px solid #dfe2e5;
}

.markdown-body table tr {
    background-color: #ffffff;
    border-top: 1px solid #c6cbd1;
}

.markdown-body table tr:nth-child(2n) {
    background-color: #f6f8fa;
}

.markdown-body ul,
.markdown-body ol {
    padding-left: 2em;
    margin-top: 0;
    margin-bottom: 16px;
}

.markdown-body li {
    margin-top: 0.25em;
}

.markdown-body li + li {
    margin-top: 0.25em;
}

.markdown-body img {
    max-width: 100%;
    box-sizing: content-box;
    background-color: #ffffff;
}

.markdown-body hr {
    height: 0.25em;
    padding: 0;
    margin: 24px 0;
    background-color: #e1e4e8;
    border: 0;
}

/* Task lists */
.markdown-body input[type='checkbox'] {
    margin-right: 0.5em;
}

.markdown-body .task-list-item {
    list-style-type: none;
}

.markdown-body .task-list-item input[type='checkbox'] {
    margin-left: -1.6em;
}

/* Strikethrough */
.markdown-body del {
    text-decoration: line-through;
}

/* Definition lists */
.markdown-body dl {
    padding: 0;
}

.markdown-body dl dt {
    padding: 0;
    margin-top: 16px;
    font-size: 1em;
    font-style: italic;
    font-weight: 600;
}

.markdown-body dl dd {
    padding: 0 16px;
    margin-bottom: 16px;
}

/* Dark mode support (optional) */
@media (prefers-color-scheme: dark) {
    body {
        color: #c9d1d9;
        background-color: #0d1117;
    }

    .markdown-body h6 {
        color: #8b949e;
    }

    .markdown-body a {
        color: #58a6ff;
    }

    .markdown-body blockquote {
        color: #8b949e;
        border-left-color: #3b434b;
    }

    .markdown-body code {
        background-color: rgba(110, 118, 129, 0.4);
    }

    .markdown-body pre {
        background-color: #161b22;
    }

    .markdown-body table th {
        background-color: #161b22;
        border-color: #30363d;
    }

    .markdown-body table td {
        border-color: #30363d;
    }

    .markdown-body table tr {
        background-color: #0d1117;
        border-top-color: #21262d;
    }

    .markdown-body table tr:nth-child(2n) {
        background-color: #161b22;
    }

    .markdown-body hr {
        background-color: #21262d;
    }
}
";
        }
    }
}
