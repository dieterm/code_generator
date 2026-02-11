using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.CodeElements.Artifacts;
using CodeGenerator.Domain.CodeElements;
using CodeGenerator.Shared.Operations;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Core.CodeElements.Controllers;

public class TypesContainerArtifactController : CodeElementArtifactControllerBase<TypesContainerArtifact>
{
    public TypesContainerArtifactController(OperationExecutor operationExecutor, CodeElementsTreeViewController treeViewController, ILogger<TypesContainerArtifactController> logger)
        : base(operationExecutor, treeViewController, logger) { }

    protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(TypesContainerArtifact typesContainerArtifact)
    {
        yield return new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
        {
            Id = "AddClass",
            Text = "Add Class",
            IconKey = "AddClassIcon",
            Execute = async (artifact) =>
            {
                var newClassElement = new ClassElement("NewClass");
                (artifact as TypesContainerArtifact)!.CodeElement.Types.Add(newClassElement);
                var newClassArtifact = new ClassElementArtifact(newClassElement);
                artifact.AddChild(newClassArtifact);
            }
        };

        yield return new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
        {
            Id = "AddInterface",
            Text = "Add Interface",
            IconKey = "AddInterfaceIcon",
            Execute = async (artifact) =>
            {
                var newInterfaceElement = new InterfaceElement("INewInterface");
                (artifact as TypesContainerArtifact)!.CodeElement.Types.Add(newInterfaceElement);
                var newInterfaceArtifact = new InterfaceElementArtifact(newInterfaceElement);
                artifact.AddChild(newInterfaceArtifact);
            }
        };

        yield return new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
        {
            Id = "AddStruct",
            Text = "Add Struct",
            IconKey = "AddStructIcon",
            Execute = async (artifact) =>
            {
                var newStructElement = new StructElement("NewStruct");
                (artifact as TypesContainerArtifact)!.CodeElement.Types.Add(newStructElement);
                var newStructArtifact = new StructElementArtifact(newStructElement);
                artifact.AddChild(newStructArtifact);
            }
        };

        yield return new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
        {
            Id = "AddEnum",
            Text = "Add Enum",
            IconKey = "AddEnumIcon",
            Execute = async (artifact) =>
            {
                var newEnumElement = new EnumElement("NewEnum");
                (artifact as TypesContainerArtifact)!.CodeElement.Types.Add(newEnumElement);
                var newEnumArtifact = new EnumElementArtifact(newEnumElement);
                artifact.AddChild(newEnumArtifact);
            }
        };

        yield return new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
        {
            Id = "AddDelegate",
            Text = "Add Delegate",
            IconKey = "AddDelegateIcon",
            Execute = async (artifact) =>
            {
                var newDelegateElement = new DelegateElement("NewDelegate", new TypeReference("PropertyChangedEventArgs","SomeFunkyNamespace"));
                (artifact as TypesContainerArtifact)!.CodeElement.Types.Add(newDelegateElement);
                var newDelegateArtifact = new DelegateElementArtifact(newDelegateElement);
                artifact.AddChild(newDelegateArtifact);
            }
        };
    }
}
