using CodeGenerator.Domain.CodeElements;
using CodeGenerator.Domain.CodeElements.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodeGenerator.Domain.DotNet.Winforms
{
    public class ControlElement<T> where T : Control
    {
        public T Control { get; set; }

        public ControlElement(T control)
        {
            Control = control;
        }

        public virtual IEnumerable<StatementElement> CreateStatements(string variableName)
        {
            yield return new AssignmentStatement($"{variableName}.Location", $"new Point({Control.Location.X}, {Control.Location.Y})");
            yield return new AssignmentStatement($"{variableName}.Name", $"\"{Control.Name}\"");
            yield return new AssignmentStatement($"{variableName}.Size", $"new Size({Control.Size.Width}, {Control.Size.Height})");
            yield return new AssignmentStatement($"{variableName}.TabIndex", $"{Control.TabIndex}");
        }

        public IEnumerable<StatementElement> CreateStatementsUsingReflection(string variableName)
        {
            var properties = Control.GetType().GetProperties().Where(p => p.CanRead && p.CanWrite);

            // for strings -> take into account [AllowNull]
            // for bools -> use lowercase
            // for points and sizes -> use new Point(...) and new Size(...)
            // Exclude properties that are [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            // Only store value if different from [DefaultValue] attribute

            foreach (var property in properties)
            {
                var designerSerializationVisibilityAttribute = property.GetCustomAttributes(typeof(System.ComponentModel.DesignerSerializationVisibilityAttribute), false).FirstOrDefault() as System.ComponentModel.DesignerSerializationVisibilityAttribute;
                if (designerSerializationVisibilityAttribute != null && designerSerializationVisibilityAttribute.Visibility == System.ComponentModel.DesignerSerializationVisibility.Hidden)
                {
                    continue;
                }
                var defaultValueAttribute = property.GetCustomAttributes(typeof(System.ComponentModel.DefaultValueAttribute), false).FirstOrDefault() as System.ComponentModel.DefaultValueAttribute;
                var value = property.GetValue(Control);
                if (defaultValueAttribute != null && Equals(value, defaultValueAttribute.Value))
                {
                    continue;
                }
                if (value != null)
                {
                    string valueString;
                    if (value is string)
                    {
                        // Escape backslashes and double quotes in the string
                        var escapedValue = ((string)value).Replace("\\", "\\\\").Replace("\"", "\\\"");
                        valueString = $"\"{escapedValue}\"";
                    }
                    else if (value is bool)
                    {
                        valueString = value.ToString().ToLower();
                    }
                    else if (value is Point point)
                    {
                        valueString = $"new Point({point.X}, {point.Y})";
                    }
                    else if (value is Size size)
                    {
                        valueString = $"new Size({size.Width}, {size.Height})";
                    }
                    // Handle Flag Enums by checking if the value is an Enum and has the [Flags] attribute
                    else if (value.GetType().IsEnum && value.GetType().GetCustomAttributes(typeof(FlagsAttribute), false).Length > 0)
                    {
                        var enumType = value.GetType();
                        var enumValues = Enum.GetValues(enumType).Cast<Enum>();
                        var valueEnum = (Enum)value;
                        var selectedFlags = enumValues.Where(e => valueEnum.HasFlag(e)).Select(e => $"{enumType.Name}.{e}");
                        valueString = string.Join(" | ", selectedFlags);
                    }
                    // Handle regular Enums
                    else if (value.GetType().IsEnum)
                    {
                        var enumType = value.GetType();
                        valueString = $"{enumType.Name}.{value}";
                    }
                    // Handle Colors by using Color.FromArgb
                    else if (value is System.Drawing.Color color)
                    {
                        valueString = $"Color.FromArgb({color.A}, {color.R}, {color.G}, {color.B})";
                    }
                    // Handle Font by using new Font(...)
                    else if (value is System.Drawing.Font font)
                    {
                        valueString = $"new Font(\"{font.Name}\", {font.Size}f, {font.Style})";
                    }
                    // Handle Padding and Margin by using new Padding(...)
                    else if (value is System.Windows.Forms.Padding padding)
                    {
                        valueString = $"new Padding({padding.Left}, {padding.Top}, {padding.Right}, {padding.Bottom})";
                    }
                    // Handle Anchor and Dock by using AnchorStyles and DockStyle enums
                    else if (value is System.Windows.Forms.AnchorStyles anchor)
                    {
                        valueString = $"AnchorStyles.{anchor}";
                    }
                    else if (value is System.Windows.Forms.DockStyle dock)
                    {
                        valueString = $"DockStyle.{dock}";
                    }
                    // Handle FontStyle by using FontStyle enum
                    else if (value is System.Drawing.FontStyle fontStyle)
                    {
                        valueString = $"FontStyle.{fontStyle}";
                    }
                    // Handle ContentAlignment by using ContentAlignment enum
                    else if (value is System.Drawing.ContentAlignment contentAlignment)
                    {
                        valueString = $"ContentAlignment.{contentAlignment}";
                    }
                    // skip list of items and dictionaries for now, as they require more complex handling
                    else if (value is System.Collections.IEnumerable && !(value is string))
                    {
                        continue;
                    }
                    else
                    {
                        valueString = value.ToString();
                    }
                    yield return new AssignmentStatement($"{variableName}.{property.Name}", valueString);
                }

            }
        }
    }
}
