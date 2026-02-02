using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Shared.Models
{
    public class ParameterizedString
    {
        public const string DefaultParameterFormat = "{$$VALUE$$}";
        public const string ParameterTemplatePlaceholder = "$$VALUE$$";
        public ParameterizedString()
            : this(string.Empty, DefaultParameterFormat)
        {
            
        }
        public ParameterizedString(string initialTemplate)
            : this(initialTemplate, DefaultParameterFormat)
        {
            
        }
        public ParameterizedString(string initialTemplate, string parameterFormat)
        {
            Template = initialTemplate;
            ParameterFormat = parameterFormat;
            AssertParameterFormatRequired();
        }
        /// <summary>
        /// This template is used to specify the parameter-format.<br />
        /// Use "$$VALUE$$" as a placeholder for the parameter format. <br />
        /// For example: "{$$VALUE$$}" will result in parameters like "{ParameterName}"<br />
        /// </summary>
        public string ParameterFormat { get; }
        public string Template { get; set; }
        public List<ParameterizedStringParameter> Parameters { get; set; } = new List<ParameterizedStringParameter>();

        private void AssertParameterFormatRequired()
        {
            if(string.IsNullOrWhiteSpace(ParameterFormat))
                throw new InvalidOperationException("ParameterFormat cannot be null or whitespace.");
        }
        /// <summary>
        /// eg. $"{Layer}.{Scope}" -> "Application.Shared"
        /// </summary>
        public string GetOutput(ReadOnlyDictionary<string, string> parameterValues)
        {
            AssertParameterFormatRequired();
            if (Template == null) return null;
            var output = Template;
            foreach (var (key, value) in parameterValues)
            {
                var parmeterPattern = ParameterFormat.Replace(ParameterTemplatePlaceholder, key);
                output = output.Replace(parmeterPattern, parameterValues[key]);
            }
            return output;
        }

        /// <summary>
        /// eg. $"{Layer}.{Scope}" -> "Application.Shared"
        /// </summary>
        public string GetOutput(Dictionary<string, string> parameterValues)
        {
            AssertParameterFormatRequired();
            if (Template == null) return null;
            var output = Template;
            foreach (var (key, value) in parameterValues)
            {
                var parmeterPattern = ParameterFormat.Replace(ParameterTemplatePlaceholder, key);
                output = output.Replace(parmeterPattern, parameterValues[key]);
            }
            return output;
        }

        public string GetPreview()
        {
            AssertParameterFormatRequired();
            var preview = Template;
            foreach (var parameter in Parameters)
            {
                var parmeterPattern = ParameterFormat.Replace(ParameterTemplatePlaceholder, parameter.Parameter);
                preview = preview.Replace(parmeterPattern, parameter.ExampleValue); 
            }
            return preview;
        }
    }
}
