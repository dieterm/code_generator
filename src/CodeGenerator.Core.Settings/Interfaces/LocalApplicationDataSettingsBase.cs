using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Settings.Interfaces
{
    public abstract class LocalApplicationDataSettingsBase
    {
        /// <summary>
        /// Version of the settings format for migration purposes
        /// </summary>
        [JsonPropertyName("version")]
        public string Version { get; set; } = "1.0";

        /// <summary>
        /// Last modified timestamp
        /// </summary>
        [JsonPropertyName("lastModified")]
        public DateTime LastModified { get; set; } = DateTime.UtcNow;
    }
}
