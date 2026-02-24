using CodeGenerator.Domain.CodeElements;
using CodeGenerator.Domain.CodeElements.Statements;
using CodeGenerator.Domain.ProgrammingLanguages;
using CodeGenerator.Domain.ProgrammingLanguages.CSharp;

namespace CodeGenerator.Domain.DotNet.Winforms
{
    public class FormElement
    {
        public CodeFileElement CreateCodeFile(string formName, string classNamespace, ProgrammingLanguage? programmingLanguage = null)
        {
            var codeFileElement = new CodeFileElement(formName, programmingLanguage??CSharpLanguage.Instance);
            
            codeFileElement
                .AddUsing("System")
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.ComponentModel")
                .AddUsing("System.Data")
                .AddUsing("System.Drawing")
                .AddUsing("System.Linq")
                .AddUsing("System.Text")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("System.Windows.Forms");

            var cls_Form = codeFileElement.AddNamespace(classNamespace, new ClassElement(formName) { 
                Modifiers = ElementModifiers.Partial,
                BaseTypes = [ TypeReference.Winforms.Form ],
            });

            cls_Form.AddConstructor(new RawStatementElement("InitializeComponent();"));
            
            return codeFileElement;
        }

        public CodeFileElement CreateDesignerFile(string formName, string classNamespace, ProgrammingLanguage? programmingLanguage = null)
        {
            var codeFileElement = new CodeFileElement($"{formName}.Designer", programmingLanguage ?? CSharpLanguage.Instance);

            var ns_CodeGeneratorPresentationWinFormsViews = new NamespaceElement(classNamespace) { IsFileScoped = false };

            var cls_Form = new ClassElement(formName) { Modifiers = ElementModifiers.Partial };
            cls_Form.Fields.Add(new FieldElement("components", TypeReference.Winforms.IContainer)
            {
                InitialValue = "null",
                Documentation = "Required designer variable."
            });
            var method_Dispose = new MethodElement("Dispose", TypeReference.Common.Void);
            method_Dispose.AccessModifier = AccessModifier.Protected;
            method_Dispose.Modifiers = (ElementModifiers)8;
            method_Dispose.Documentation = "Clean up any resources being used.";
            var param_disposing_5 = new ParameterElement("disposing", TypeReference.Common.Bool);
            method_Dispose.Parameters.Add(param_disposing_5);
            var ifStmt = new IfStatementElement("disposing && (components != null)");
            ifStmt.ThenStatements.Statements.Add(new RawStatementElement("components.Dispose();"));
            method_Dispose.Body.Statements.Add(ifStmt);
            method_Dispose.Body.Statements.Add(new RawStatementElement("base.Dispose(disposing);"));
            cls_Form.Methods.Add(method_Dispose);
            var method_InitializeComponent = new MethodElement("InitializeComponent", TypeReference.Common.Void);
            method_InitializeComponent.AccessModifier = AccessModifier.Private;
            method_InitializeComponent.Documentation = "Required method for Designer support - do not modify the contents of this method with the code editor.";
            method_InitializeComponent.Body.Statements.Add(new AssignmentStatement("this.components", "new System.ComponentModel.Container()"));
            method_InitializeComponent.Body.Statements.Add(new AssignmentStatement("this.AutoScaleMode", "System.Windows.Forms.AutoScaleMode.Font"));
            method_InitializeComponent.Body.Statements.Add(new AssignmentStatement("this.ClientSize", "new System.Drawing.Size(800, 450)"));
            method_InitializeComponent.Body.Statements.Add(new AssignmentStatement("this.Text", "\"Form1\""));
            cls_Form.Methods.Add(method_InitializeComponent);
            ns_CodeGeneratorPresentationWinFormsViews.Types.Add(cls_Form);
            codeFileElement.Namespaces.Add(ns_CodeGeneratorPresentationWinFormsViews);
            return codeFileElement;
        }
    }
}
