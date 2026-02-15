using CodeGenerator.Shared.Operations;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace CodeGenerator.Core.LLM.Tools
{
    /// <summary>
    /// Bridges workspace operations to LLM AI tools.
    /// Generates AIFunction wrappers with full parameter metadata from the TParams POCO.
    /// </summary>
    public class LlmOperationBridge
    {
        private readonly OperationExecutor _executor;
        private readonly Func<Func<string>, string> _uiInvoker;
        private readonly ILogger<LlmOperationBridge> _logger;

        /// <summary>
        /// JsonSerializerOptions that only includes properties annotated with [Description] or [Required],
        /// effectively excluding internal undo/redo state fields (EntityArtifact, PropertyArtifact, etc.)
        /// that cause deep/circular JSON schema generation failures.
        /// </summary>
        private static readonly JsonSerializerOptions _toolJsonOptions = new(JsonSerializerOptions.Default)
        {
            TypeInfoResolver = new DefaultJsonTypeInfoResolver
            {
                Modifiers =
                {
                    static typeInfo =>
                    {
                        if (typeInfo.Kind != JsonTypeInfoKind.Object) return;

                        foreach (var prop in typeInfo.Properties)
                        {
                            // Only include properties that have a [Description] or [Required] attribute
                            var memberInfo = prop.AttributeProvider;
                            if (memberInfo == null)
                            {
                                prop.ShouldSerialize = (_, _) => false;
                                continue;
                            }

                            var hasDescription = memberInfo.GetCustomAttributes(typeof(DescriptionAttribute), true).Length > 0;
                            var hasRequired = memberInfo.GetCustomAttributes(typeof(RequiredAttribute), true).Length > 0;

                            if (!hasDescription && !hasRequired)
                            {
                                prop.ShouldSerialize = (_, _) => false;
                            }
                        }
                    }
                }
            }
        };

        public LlmOperationBridge(OperationExecutor executor, Func<Func<string>, string> uiInvoker, ILogger<LlmOperationBridge> logger)
        {
            _executor = executor;
            _uiInvoker = uiInvoker;
            _logger = logger;
        }

        /// <summary>
        /// Generate AIFunction tools from all registered operations.
        /// The [Description] attributes on TParams properties become
        /// the tool parameter descriptions automatically.
        /// </summary>
        public List<AIFunction> GenerateTools()
        {
            var tools = new List<AIFunction>();

            foreach (var operation in _executor.GetRegisteredOperations())
            {
                try
                {
                    var tool = CreateToolForOperation(operation);
                    if (tool != null)
                        tools.Add(tool);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating tool for operation {OperationId}", operation.OperationId);
                    throw;
                }
                
            }

            tools.Add(AIFunctionFactory.Create(
                ListAvailableOperations,
                "ListAvailableOperations",
                "List all available workspace operations with their parameter schemas"));

            return tools;
        }

        private string ListAvailableOperations()
        {
            var lines = new List<string>();
            foreach (var op in _executor.GetRegisteredOperations())
            {
                var paramsType = op.ParameterType;
                var paramDescriptions = paramsType.GetProperties()
                    .Where(p => p.GetCustomAttribute<DescriptionAttribute>() != null
                             || p.GetCustomAttribute<RequiredAttribute>() != null)
                    .Select(p =>
                    {
                        var desc = p.GetCustomAttribute<DescriptionAttribute>()?.Description ?? "";
                        var required = p.GetCustomAttribute<RequiredAttribute>() != null;
                        return $"    - {p.Name} ({p.PropertyType.Name}{(required ? ", required" : "")}): {desc}";
                    });

                lines.Add($"[{op.OperationId}] {op.Description}");
                lines.AddRange(paramDescriptions);
            }
            return string.Join("\n", lines);
        }

        private AIFunction? CreateToolForOperation(IOperation operation)
        {
            var opType = operation.GetType();
            var iface = opType.GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType
                    && i.GetGenericTypeDefinition() == typeof(IOperation<>));

            if (iface == null) return null;

            var paramsType = iface.GetGenericArguments()[0];
            var method = typeof(LlmOperationBridge)
                .GetMethod(nameof(CreateTypedTool), BindingFlags.NonPublic | BindingFlags.Instance)!
                .MakeGenericMethod(paramsType);

            return method.Invoke(this, [operation]) as AIFunction;
        }

        private AIFunction CreateTypedTool<TParams>(IOperation<TParams> operation)
            where TParams : class, new()
        {
            return AIFunctionFactory.Create(
                (TParams parameters) => _uiInvoker(() =>
                {
                    var result = _executor.Execute(operation, parameters);
                    return result.Message;
                }),
                operation.OperationId,
                operation.Description,
                _toolJsonOptions);
        }
    }
}
