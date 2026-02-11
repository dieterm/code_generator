using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.CodeElements.Artifacts;
using CodeGenerator.Domain.CodeElements;
using CodeGenerator.Shared.Operations;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.CodeElements.Controllers
{
    public class AttributesContainerArtifactController : CodeElementArtifactControllerBase<AttributesContainerArtifact>
    {
        public AttributesContainerArtifactController(OperationExecutor operationExecutor, CodeElementsTreeViewController treeViewController, ILogger<AttributesContainerArtifactController> logger) 
            : base(operationExecutor, treeViewController, logger)
        {
        }

        protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(AttributesContainerArtifact artifact)
        {
            yield return new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
            {
                Id = "AddAttribute",
                Text = "Add Attribute",
                IconKey = "AddAttributeIcon",
                Execute = async (artifact) =>
                {
                    var newAttributeElement = new AttributeElement("NewAttribute");
                    (artifact as AttributesContainerArtifact)!.AddAttributeElement(newAttributeElement);
                }
            };

            // AttributeElement.Common.XmlElement("Example");
            // AttributeElement.Common.Serializable
            // AttributeElement.Common.DataMember(name: "Example", order: 1);
            // AttributeElement.Common.Flags
            // AttributeElement.Common.JsonPropertyName("Example");
            // AttributeElement.Common.Obsolete(message: "This is obsolete", isError: true);
            // AttributeElement.Common.Required

            var commonAttributes = new List<(string Name, Func<AttributeElement> CreateElement)>
            {
                ("Serializable", () => AttributeElement.Common.Serializable),
                ("Flags", () => AttributeElement.Common.Flags),
                ("Required", () => AttributeElement.Common.Required),
                ("JsonPropertyName", () => AttributeElement.Common.JsonPropertyName("Example")),
                ("XmlElement", () => AttributeElement.Common.XmlElement("Example")),
                ("DataMember", () => AttributeElement.Common.DataMember(name: "Example", order: 1)),
                ("Obsolete", () => AttributeElement.Common.Obsolete(message: "This is obsolete", isError: true))
            };

            yield return new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
            {
                Id = "AddCommonAttribute",
                Text = "Add Common Attribute",
                IconKey = "AddAttributeIcon",
                SubCommands = commonAttributes.Select(attr => new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
                {
                    Id = $"Add{attr.Name}Attribute",
                    Text = attr.Name,
                    IconKey = "AddAttributeIcon",
                    Execute = async (artifact) =>
                    {
                        var newAttributeElement = attr.CreateElement();
                        (artifact as AttributesContainerArtifact)!.AddAttributeElement(newAttributeElement);
                    }
                }).ToList()
            };
        }
    }
}
