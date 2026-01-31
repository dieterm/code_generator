namespace CodeGenerator.Domain.CodeElements
{
    /// <summary>
    /// Represents an event declaration
    /// </summary>
    public class EventElement : CodeElement
    {
        /// <summary>
        /// Type of the event (typically a delegate type like EventHandler)
        /// </summary>
        public TypeReference Type { get; set; } = new();

        /// <summary>
        /// Whether this is a field-like event (auto-implemented)
        /// </summary>
        public bool IsFieldLike { get; set; } = true;

        /// <summary>
        /// Custom add accessor body (for non-field-like events)
        /// </summary>
        public string? AddAccessorBody { get; set; }

        /// <summary>
        /// Custom remove accessor body (for non-field-like events)
        /// </summary>
        public string? RemoveAccessorBody { get; set; }

        public EventElement() { }

        public EventElement(string name, TypeReference type)
        {
            Name = name;
            Type = type;
        }

        /// <summary>
        /// Create an event with EventHandler type
        /// </summary>
        public static EventElement EventHandler(string name)
        {
            return new EventElement(name, new TypeReference("EventHandler"));
        }

        /// <summary>
        /// Create an event with generic EventHandler type
        /// </summary>
        public static EventElement EventHandler(string name, TypeReference eventArgsType)
        {
            return new EventElement(name, TypeReference.Generic("EventHandler", eventArgsType));
        }
    }
}
