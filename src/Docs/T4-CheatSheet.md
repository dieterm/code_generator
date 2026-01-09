# T4 Template Cheat Sheet

Een complete gids voor beginners om te werken met T4 (Text Template Transformation Toolkit) templates.

## ?? Inhoudsopgave

- [Basis Syntax](#basis-syntax)
- [Directives](#directives)
- [Code Blocks](#code-blocks)
- [Variabelen en Data Types](#variabelen-en-data-types)
- [Loops en Conditionals](#loops-en-conditionals)
- [Helper Methods (Class Features)](#helper-methods-class-features)
- [Bestanden Inlezen](#bestanden-inlezen)
- [JSON Verwerken](#json-verwerken)
- [Tips & Best Practices](#tips--best-practices)
- [Veelvoorkomende Fouten](#veelvoorkomende-fouten)

---

## Basis Syntax

T4 templates gebruiken 3 soorten code blocks:

| Syntax | Naam | Doel | Voorbeeld |
|--------|------|------|-----------|
| `<# ... #>` | **Statement Block** | Voor code die geen output genereert | `<# int x = 5; #>` |
| `<#= ... #>` | **Expression Block** | Voor output van een waarde | `<#= x #>` |
| `<#+ ... #>` | **Class Feature Block** | Voor helper methods en properties | `<#+ void MyMethod() { } #>` |

### Simpel Voorbeeld

```csharp
<#@ template language="C#" #>
<#@ output extension=".txt" #>
<#
    string name = "John";
    int age = 30;
#>
Hallo, mijn naam is <#= name #> en ik ben <#= age #> jaar oud.
```

**Output:**
```
Hallo, mijn naam is John en ik ben 30 jaar oud.
```

---

## Directives

Directives staan altijd bovenaan de template en configureren het gedrag.

### Template Directive

```csharp
<#@ template debug="false" hostspecific="true" language="C#" #>
```

- `debug="true"` - Maakt debugging mogelijk
- `hostspecific="true"` - Geeft toegang tot `Host` object (nodig voor bestandspaden)
- `language="C#"` - Programmeertaal (C# of VB.NET)

### Assembly Directive

Refereer naar externe assemblies:

```csharp
<#@ assembly name="System.Core" #>
<#@ assembly name="Newtonsoft.Json" #>
<#@ assembly name="C:\Path\To\MyLibrary.dll" #>
```

### Import Directive

Importeer namespaces:

```csharp
<#@ import namespace="System.Linq" #>
<#@ import namespace="System.Collections.Generic" #>
<#@ import namespace="System.IO" #>
<#@ import namespace="Newtonsoft.Json.Linq" #>
```

### Output Directive

Bepaal de extensie van het output bestand:

```csharp
<#@ output extension=".txt" #>
<#@ output extension=".cs" #>
<#@ output extension=".sql" #>
```

---

## Code Blocks

### Statement Block - Geen Output

Gebruik voor variabelen, loops, conditionals:

```csharp
<#
    // Variabelen
    string projectName = "MyProject";
    int version = 1;
    
    // Arrays
    string[] items = new string[] { "Apple", "Banana", "Orange" };
    
    // Lists
    var numbers = new List<int> { 1, 2, 3, 4, 5 };
#>
```

### Expression Block - Output Genereren

Gebruik `<#= ... #>` om waarden te printen:

```csharp
<#
    string name = "Alice";
    int count = 42;
#>
Name: <#= name #>
Count: <#= count #>
Total: <#= count * 2 #>
```

---

## Variabelen en Data Types

### Basis Types

```csharp
<#
    // String
    string text = "Hello World";
    
    // Numbers
    int number = 42;
    double price = 99.99;
    decimal amount = 1000.50m;
    
    // Boolean
    bool isActive = true;
    
    // DateTime
    DateTime now = DateTime.Now;
#>
```

### Arrays

```csharp
<#
    // String array
    string[] colors = new string[] { "Red", "Green", "Blue" };
    
    // Int array
    int[] numbers = { 1, 2, 3, 4, 5 };
#>
```

### Lists en Collections

```csharp
<#
    // List
    var names = new List<string> { "John", "Jane", "Bob" };
    
    // Dictionary
    var settings = new Dictionary<string, string>
    {
        { "Host", "localhost" },
        { "Port", "5432" }
    };
#>
```

---

## Loops en Conditionals

### Foreach Loop

```csharp
<#
    string[] items = { "Apple", "Banana", "Orange" };
#>
Fruit List:
<#
    foreach (var item in items)
    {
#>
- <#= item #>
<#
    }
#>
```

**Output:**
```
Fruit List:
- Apple
- Banana
- Orange
```

### For Loop met Index

```csharp
<#
    string[] items = { "First", "Second", "Third" };
#>
<#
    for (int i = 0; i < items.Length; i++)
    {
#>
<#= i + 1 #>. <#= items[i] #>
<#
    }
#>
```

**Output:**
```
1. First
2. Second
3. Third
```

### If-Else Statements

```csharp
<#
    bool isProduction = true;
    string environment = isProduction ? "Production" : "Development";
#>
<#
    if (isProduction)
    {
#>
// WARNING: This is PRODUCTION code!
<#
    }
    else
    {
#>
// This is development code
<#
    }
#>
Environment: <#= environment #>
```

### Switch Statement

```csharp
<#
    string type = "Class";
#>
<#
    switch (type)
    {
        case "Class":
#>
public class MyClass { }
<#
            break;
        case "Interface":
#>
public interface IMyInterface { }
<#
            break;
        default:
#>
// Unknown type
<#
            break;
    }
#>
```

---

## Helper Methods (Class Features)

Class feature blocks (`<#+ ... #>`) moeten **altijd aan het einde** van de template staan.

### Basic Helper Method

```csharp
<#@ template language="C#" #>
<#@ output extension=".txt" #>
<#
    string result = ToUpperCase("hello world");
#>
Result: <#= result #>

<#+
    // Helper method
    string ToUpperCase(string input)
    {
        return input.ToUpper();
    }
#>
```

### Meerdere Helper Methods

```csharp
<#
    string name = "john_doe";
#>
PascalCase: <#= ToPascalCase(name) #>
CamelCase: <#= ToCamelCase(name) #>

<#+
    string ToPascalCase(string input)
    {
        var parts = input.Split('_');
        return string.Join("", parts.Select(p => 
            char.ToUpper(p[0]) + p.Substring(1).ToLower()));
    }
    
    string ToCamelCase(string input)
    {
        string pascal = ToPascalCase(input);
        return char.ToLower(pascal[0]) + pascal.Substring(1);
    }
#>
```

---

## Bestanden Inlezen

### Relatieve Paden met Host.ResolvePath

```csharp
<#@ template hostspecific="true" #>
<#@ import namespace="System.IO" #>
<#
    // Leest een bestand in dezelfde directory
    string content = File.ReadAllText(this.Host.ResolvePath("myfile.txt"));
#>
File content:
<#= content #>
```

### Absolute Paden

```csharp
<#
    string path = @"C:\Data\config.txt";
    string[] lines = File.ReadAllLines(path);
#>
<#
    foreach (var line in lines)
    {
#>
- <#= line #>
<#
    }
#>
```

---

## JSON Verwerken

### JSON Inlezen met Newtonsoft.Json

**Setup:**

```csharp
<#@ template hostspecific="true" #>
<#@ assembly name="C:\Users\[USERNAME]\.nuget\packages\newtonsoft.json\13.0.3\lib\netstandard2.0\Newtonsoft.Json.dll" #>
<#@ import namespace="System.IO" #>
<#@ import namespace="Newtonsoft.Json.Linq" #>
```

**JSON Bestand (mydata.json):**

```json
{
  "entities": [
    {
      "name": "Country",
      "properties": [
        { "name": "id", "type": "integer" },
        { "name": "name", "type": "string" }
      ]
    }
  ]
}
```

**Template:**

```csharp
<#
    var json = LoadJson(this.Host.ResolvePath("mydata.json"));
    var entities = json["entities"];
#>
// Generated Entities
<#
    foreach (var entity in entities)
    {
#>
public class <#= entity["name"] #>
{
<#
        foreach (var prop in entity["properties"])
        {
            string csharpType = MapToCSharpType(prop["type"].ToString());
#>
    public <#= csharpType #> <#= ToPascalCase(prop["name"].ToString()) #> { get; set; }
<#
        }
#>
}

<#
    }
#>

<#+
    JObject LoadJson(string filePath)
    {
        string jsonContent = File.ReadAllText(filePath);
        return JObject.Parse(jsonContent);
    }
    
    string MapToCSharpType(string jsonType)
    {
        switch (jsonType.ToLower())
        {
            case "integer": return "int";
            case "string": return "string";
            case "boolean": return "bool";
            case "decimal": return "decimal";
            default: return "object";
        }
    }
    
    string ToPascalCase(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;
        return char.ToUpper(input[0]) + input.Substring(1);
    }
#>
```

**Output:**

```csharp
// Generated Entities
public class Country
{
    public int Id { get; set; }
    public string Name { get; set; }
}
```

---

## Tips & Best Practices

### ? DO's

1. **Gebruik duidelijke variabele namen**
   ```csharp
   string entityName = "User";  // ? Goed
   string e = "User";           // ? Slecht
   ```

2. **Groepeer gerelateerde directives**
   ```csharp
   <#@ template language="C#" hostspecific="true" #>
   <#@ assembly name="System.Core" #>
   <#@ assembly name="Newtonsoft.Json" #>
   <#@ import namespace="System.IO" #>
   <#@ import namespace="System.Linq" #>
   ```

3. **Gebruik helper methods voor herbruikbare logic**
   ```csharp
   <#+
       string FormatPropertyName(string name)
       {
           // Herbruikbare formatting logic
           return name.Replace("_", "");
       }
   #>
   ```

4. **Valideer input data**
   ```csharp
   <#
       if (entities == null || !entities.Any())
       {
           throw new Exception("No entities found!");
       }
   #>
   ```

### ? DON'Ts

1. **Geen class features voor statements**
   ```csharp
   <#+
       string name = "Test";  // ? Fout - gebruik <# #> hiervoor
   #>
   ```

2. **Class features niet in het midden**
   ```csharp
   <# string x = "test"; #>
   <#+ void Method() { } #>    // ? Fout
   <# string y = "test2"; #>   // ? Zal error geven
   ```

3. **Geen complexe logic in expression blocks**
   ```csharp
   <#= DoComplexCalculation() #>  // ? Moeilijk te debuggen
   
   // ? Beter:
   <# 
       var result = DoComplexCalculation();
   #>
   Result: <#= result #>
   ```

---

## Veelvoorkomende Fouten

### 1. Class Feature Block niet op het einde

**? Fout:**
```csharp
<#+ 
    void MyMethod() { }
#>
<#
    string name = "Test";  // Error!
#>
```

**? Correct:**
```csharp
<#
    string name = "Test";
#>
<#+ 
    void MyMethod() { }
#>
```

### 2. hostspecific niet ingesteld

**? Fout:**
```csharp
<#@ template language="C#" #>
<#
    // Error: Host is not available
    var path = this.Host.ResolvePath("file.txt");
#>
```

**? Correct:**
```csharp
<#@ template language="C#" hostspecific="true" #>
<#
    var path = this.Host.ResolvePath("file.txt");
#>
```

### 3. Assembly niet gerefereerd

**? Fout:**
```csharp
<#@ import namespace="Newtonsoft.Json" #>
<#
    // Error: Assembly not referenced
    var json = JsonConvert.DeserializeObject("{}");
#>
```

**? Correct:**
```csharp
<#@ assembly name="C:\...\Newtonsoft.Json.dll" #>
<#@ import namespace="Newtonsoft.Json" #>
<#
    var json = JsonConvert.DeserializeObject("{}");
#>
```

### 4. Vergeten output directive

**? Fout:**
```csharp
<#@ template language="C#" #>
// Output bestand krijgt verkeerde extensie
```

**? Correct:**
```csharp
<#@ template language="C#" #>
<#@ output extension=".cs" #>
```

---

## Template Transformeren

### Via Visual Studio
- Sla het `.tt` bestand op
- Visual Studio transformeert automatisch (als Custom Tool is ingesteld)

### Via Command Line (dotnet-t4)

**Installatie:**
```bash
dotnet tool install --global dotnet-t4
```

**Gebruik:**
```bash
# Basis transformatie
t4 MyTemplate.tt -o Output.txt

# Met parameters
t4 MyTemplate.tt -o Output.cs -p:Name=MyClass
```

### Via MSBuild

In `.csproj`:
```xml
<ItemGroup>
  <None Update="MyTemplate.tt">
    <Generator>TextTemplatingFileGenerator</Generator>
    <LastGenOutput>MyTemplate.cs</LastGenOutput>
  </None>
</ItemGroup>
```

---

## Voorbeelden

### Voorbeeld 1: C# Class Generator

```csharp
<#@ template language="C#" #>
<#@ output extension=".cs" #>
<#
    string className = "Product";
    var properties = new[] 
    {
        new { Name = "Id", Type = "int" },
        new { Name = "Name", Type = "string" },
        new { Name = "Price", Type = "decimal" }
    };
#>
namespace MyProject.Models
{
    public class <#= className #>
    {
<#
    foreach (var prop in properties)
    {
#>
        public <#= prop.Type #> <#= prop.Name #> { get; set; }
<#
    }
#>
    }
}
```

### Voorbeeld 2: SQL Table Generator

```csharp
<#@ template language="C#" #>
<#@ output extension=".sql" #>
<#
    string tableName = "Users";
    var columns = new[]
    {
        new { Name = "Id", Type = "INT", Constraints = "PRIMARY KEY IDENTITY(1,1)" },
        new { Name = "Username", Type = "NVARCHAR(100)", Constraints = "NOT NULL" },
        new { Name = "Email", Type = "NVARCHAR(255)", Constraints = "NOT NULL UNIQUE" },
        new { Name = "CreatedAt", Type = "DATETIME", Constraints = "DEFAULT GETDATE()" }
    };
#>
CREATE TABLE [dbo].[<#= tableName #>]
(
<#
    for (int i = 0; i < columns.Length; i++)
    {
        var col = columns[i];
#>
    [<#= col.Name #>] <#= col.Type #> <#= col.Constraints #><#= i < columns.Length - 1 ? "," : "" #>
<#
    }
#>
);
```

---

## Nuttige Links

- [Microsoft T4 Documentatie](https://learn.microsoft.com/en-us/visualstudio/modeling/code-generation-and-t4-text-templates)
- [T4 Syntax Reference](https://learn.microsoft.com/en-us/visualstudio/modeling/writing-a-t4-text-template)
- [dotnet-t4 Tool](https://github.com/mono/t4)

---

**Happy Templating! ??**
