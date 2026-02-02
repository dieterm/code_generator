using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Generators.MessageBus
{
    public class RequestingPlaceholderContentEventArgs : GeneratorContextEventArgs
    {
        public string PlaceHolderName { get; }
        /// <summary>
        /// All subscribers can add content lines to this list to provide content for the placeholder
        /// The key is the generatorID of the generator providing the content
        /// </summary>
        public ReadOnlyDictionary<string, string> Content { get { return new ReadOnlyDictionary<string, string>(_content); } } 
        private readonly Dictionary<string, string> _content = new Dictionary<string, string>();
        public RequestingPlaceholderContentEventArgs(string placeHolderName, GenerationResult result) 
            : base(result)
        {
            PlaceHolderName = placeHolderName;
        }
        /// <summary>
        /// Adds content for the specified generator ID
        /// </summary>
        /// <param name="generatorId">The ID of the generator providing the content</param>
        /// <param name="content">The content to add</param>
        public void AddContent(string generatorId, string content)
        {
            _content[generatorId] = content;
        }

        public void AddContent(IMessageBusAwareGenerator generator, string content)
        {
            _content[generator.SettingsDescription.Id] = content;
        }
    }
}
