using CodeGenerator.Shared.Operations;
using Microsoft.Extensions.AI;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace CodeGenerator.Core.Copilot.Tools
{
    /// <summary>
    /// Bridges workspace operations to Copilot AI tools.
    /// Generates AIFunction wrappers with full parameter metadata from the TParams POCO.
    /// </summary>
    public class CopilotOperationBridge
    {
        private readonly OperationExecutor _executor;
        private readonly Func<Func<string>, string> _uiInvoker;

        public CopilotOperationBridge(OperationExecutor executor, Func<Func<string>, string> uiInvoker)
        {
            _executor = executor;
            _uiInvoker = uiInvoker;
        }

        /// <summary>
        /// Generate AIFunction tools from all registered operations.
        /// The [Description] attributes on TParams properties become
        /// the Copilot tool parameter descriptions automatically.
        /// </summary>
        public List<AIFunction> GenerateTools()
        {
            var tools = new List<AIFunction>();

            foreach (var operation in _executor.GetRegisteredOperations())
            {
                var tool = CreateToolForOperation(operation);
                if (tool != null)
                    tools.Add(tool);
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
            var method = typeof(CopilotOperationBridge)
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
                operation.Description);
        }
    }
}
