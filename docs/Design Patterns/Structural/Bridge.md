# Bridge Pattern

## Intentie
Ontkoppelt een **abstractie van zijn implementatie** zodat beide onafhankelijk kunnen variëren.

## Wanneer gebruiken?
- Wanneer je wilt voorkomen dat je een permanente binding hebt tussen abstractie en implementatie
- Wanneer abstractie én implementatie uitbreidbaar moeten zijn via subclassing
- Cross-platform applicaties
- Wanneer je runtime wilt wisselen van implementatie

## Structuur

```
???????????????????          ???????????????????
?   Abstraction   ??????????>?  Implementor    ?
???????????????????          ???????????????????
? -implementor    ?          ? +OperationImpl()?
? +Operation()    ?          ???????????????????
???????????????????                   ?
        ?                      ???????????????
        ?                      ?             ?
?????????????????       ????????????  ????????????
?RefinedAbstract?       ? ImplA    ?  ? ImplB    ?
?????????????????       ????????????  ????????????
```

## Implementatie in C#

### Basis Implementatie: Remote Controls & Devices

```csharp
// Implementor interface
public interface IDevice
{
    bool IsEnabled { get; }
    void Enable();
    void Disable();
    int Volume { get; set; }
    int Channel { get; set; }
    void PrintStatus();
}

// Concrete Implementors
public class Television : IDevice
{
    private bool _enabled;
    private int _volume = 30;
    private int _channel = 1;

    public bool IsEnabled => _enabled;

    public void Enable()
    {
        _enabled = true;
        Console.WriteLine("TV is now ON");
    }

    public void Disable()
    {
        _enabled = false;
        Console.WriteLine("TV is now OFF");
    }

    public int Volume
    {
        get => _volume;
        set
        {
            _volume = Math.Clamp(value, 0, 100);
            Console.WriteLine($"TV volume set to {_volume}");
        }
    }

    public int Channel
    {
        get => _channel;
        set
        {
            _channel = value;
            Console.WriteLine($"TV channel set to {_channel}");
        }
    }

    public void PrintStatus()
    {
        Console.WriteLine($"TV | Enabled: {_enabled} | Volume: {_volume} | Channel: {_channel}");
    }
}

public class Radio : IDevice
{
    private bool _enabled;
    private int _volume = 20;
    private int _channel = 88;

    public bool IsEnabled => _enabled;

    public void Enable()
    {
        _enabled = true;
        Console.WriteLine("Radio is now ON");
    }

    public void Disable()
    {
        _enabled = false;
        Console.WriteLine("Radio is now OFF");
    }

    public int Volume
    {
        get => _volume;
        set
        {
            _volume = Math.Clamp(value, 0, 100);
            Console.WriteLine($"Radio volume set to {_volume}");
        }
    }

    public int Channel
    {
        get => _channel;
        set
        {
            _channel = Math.Clamp(value, 88, 108);
            Console.WriteLine($"Radio frequency set to {_channel} FM");
        }
    }

    public void PrintStatus()
    {
        Console.WriteLine($"Radio | Enabled: {_enabled} | Volume: {_volume} | FM: {_channel}");
    }
}

// Abstraction
public class RemoteControl
{
    protected IDevice Device;

    public RemoteControl(IDevice device)
    {
        Device = device;
    }

    public void TogglePower()
    {
        if (Device.IsEnabled)
            Device.Disable();
        else
            Device.Enable();
    }

    public void VolumeUp()
    {
        Device.Volume += 10;
    }

    public void VolumeDown()
    {
        Device.Volume -= 10;
    }

    public void ChannelUp()
    {
        Device.Channel++;
    }

    public void ChannelDown()
    {
        Device.Channel--;
    }
}

// Refined Abstraction
public class AdvancedRemoteControl : RemoteControl
{
    public AdvancedRemoteControl(IDevice device) : base(device) { }

    public void Mute()
    {
        Device.Volume = 0;
        Console.WriteLine("Device muted");
    }

    public void SetChannel(int channel)
    {
        Device.Channel = channel;
    }

    public void PrintDeviceStatus()
    {
        Device.PrintStatus();
    }
}

// Gebruik
Console.WriteLine("--- TV with Basic Remote ---");
var tv = new Television();
var basicRemote = new RemoteControl(tv);
basicRemote.TogglePower();
basicRemote.VolumeUp();
basicRemote.ChannelUp();

Console.WriteLine("\n--- Radio with Advanced Remote ---");
var radio = new Radio();
var advancedRemote = new AdvancedRemoteControl(radio);
advancedRemote.TogglePower();
advancedRemote.SetChannel(101);
advancedRemote.Mute();
advancedRemote.PrintDeviceStatus();
```

### Praktisch Voorbeeld: Cross-Platform UI

```csharp
// Implementor - platform-specific rendering
public interface IRenderer
{
    void RenderButton(int x, int y, string label);
    void RenderTextBox(int x, int y, int width, string text);
    void RenderCheckBox(int x, int y, string label, bool isChecked);
}

// Concrete Implementors
public class WindowsRenderer : IRenderer
{
    public void RenderButton(int x, int y, string label)
    {
        Console.WriteLine($"[Windows] Drawing button '{label}' at ({x},{y}) with 3D border");
    }

    public void RenderTextBox(int x, int y, int width, string text)
    {
        Console.WriteLine($"[Windows] Drawing textbox at ({x},{y}), width={width}: '{text}'");
    }

    public void RenderCheckBox(int x, int y, string label, bool isChecked)
    {
        var check = isChecked ? "?" : "?";
        Console.WriteLine($"[Windows] Drawing checkbox {check} '{label}' at ({x},{y})");
    }
}

public class MacRenderer : IRenderer
{
    public void RenderButton(int x, int y, string label)
    {
        Console.WriteLine($"[macOS] Drawing rounded button '{label}' at ({x},{y})");
    }

    public void RenderTextBox(int x, int y, int width, string text)
    {
        Console.WriteLine($"[macOS] Drawing textbox at ({x},{y}), width={width}: '{text}'");
    }

    public void RenderCheckBox(int x, int y, string label, bool isChecked)
    {
        var check = isChecked ? "?" : "?";
        Console.WriteLine($"[macOS] Drawing checkbox {check} '{label}' at ({x},{y})");
    }
}

public class WebRenderer : IRenderer
{
    public void RenderButton(int x, int y, string label)
    {
        Console.WriteLine($"[Web] <button style='position:absolute;left:{x}px;top:{y}px'>{label}</button>");
    }

    public void RenderTextBox(int x, int y, int width, string text)
    {
        Console.WriteLine($"[Web] <input type='text' style='left:{x}px;top:{y}px;width:{width}px' value='{text}'/>");
    }

    public void RenderCheckBox(int x, int y, string label, bool isChecked)
    {
        var checkedAttr = isChecked ? "checked" : "";
        Console.WriteLine($"[Web] <label><input type='checkbox' {checkedAttr}/>{label}</label>");
    }
}

// Abstraction - UI Component
public abstract class UIComponent
{
    protected IRenderer Renderer;
    public int X { get; set; }
    public int Y { get; set; }

    protected UIComponent(IRenderer renderer, int x, int y)
    {
        Renderer = renderer;
        X = x;
        Y = y;
    }

    public abstract void Render();
}

// Refined Abstractions
public class Button : UIComponent
{
    public string Label { get; set; }

    public Button(IRenderer renderer, int x, int y, string label) : base(renderer, x, y)
    {
        Label = label;
    }

    public override void Render()
    {
        Renderer.RenderButton(X, Y, Label);
    }

    public void Click()
    {
        Console.WriteLine($"Button '{Label}' clicked!");
    }
}

public class TextBox : UIComponent
{
    public string Text { get; set; }
    public int Width { get; set; }

    public TextBox(IRenderer renderer, int x, int y, int width) : base(renderer, x, y)
    {
        Width = width;
        Text = string.Empty;
    }

    public override void Render()
    {
        Renderer.RenderTextBox(X, Y, Width, Text);
    }
}

public class CheckBox : UIComponent
{
    public string Label { get; set; }
    public bool IsChecked { get; set; }

    public CheckBox(IRenderer renderer, int x, int y, string label) : base(renderer, x, y)
    {
        Label = label;
    }

    public override void Render()
    {
        Renderer.RenderCheckBox(X, Y, Label, IsChecked);
    }

    public void Toggle()
    {
        IsChecked = !IsChecked;
    }
}

// Form that contains multiple components
public class Form
{
    private readonly List<UIComponent> _components = new();
    private readonly IRenderer _renderer;

    public Form(IRenderer renderer)
    {
        _renderer = renderer;
    }

    public Button AddButton(int x, int y, string label)
    {
        var button = new Button(_renderer, x, y, label);
        _components.Add(button);
        return button;
    }

    public TextBox AddTextBox(int x, int y, int width)
    {
        var textBox = new TextBox(_renderer, x, y, width);
        _components.Add(textBox);
        return textBox;
    }

    public CheckBox AddCheckBox(int x, int y, string label)
    {
        var checkBox = new CheckBox(_renderer, x, y, label);
        _components.Add(checkBox);
        return checkBox;
    }

    public void Render()
    {
        Console.WriteLine("Rendering form...");
        foreach (var component in _components)
        {
            component.Render();
        }
    }
}

// Gebruik
IRenderer renderer = Environment.OSVersion.Platform switch
{
    PlatformID.Win32NT => new WindowsRenderer(),
    PlatformID.MacOSX => new MacRenderer(),
    _ => new WebRenderer()
};

// Of via DI
// services.AddSingleton<IRenderer, WindowsRenderer>();

var form = new Form(renderer);
form.AddButton(10, 10, "Submit");
form.AddTextBox(10, 50, 200);
var rememberMe = form.AddCheckBox(10, 90, "Remember me");
rememberMe.IsChecked = true;
form.Render();
```

### Met Dependency Injection

```csharp
// Configuration-based renderer selection
public static class RendererFactory
{
    public static IRenderer Create(string platform)
    {
        return platform.ToLower() switch
        {
            "windows" => new WindowsRenderer(),
            "macos" => new MacRenderer(),
            "web" => new WebRenderer(),
            _ => throw new ArgumentException($"Unknown platform: {platform}")
        };
    }
}

// In Startup/Program.cs
var platform = builder.Configuration["UI:Platform"];
builder.Services.AddSingleton<IRenderer>(sp => RendererFactory.Create(platform));
builder.Services.AddScoped<Form>();
```

## Voordelen
- ? Ontkoppeling abstractie van implementatie
- ? Beide kunnen onafhankelijk uitbreiden
- ? Open/Closed Principle
- ? Single Responsibility
- ? Runtime implementatie switching

## Nadelen
- ? Verhoogt complexiteit
- ? Kan overkill zijn voor simpele hiërarchieën

## Bridge vs Adapter

| Aspect | Bridge | Adapter |
|--------|--------|---------|
| **Wanneer** | Design-time | Na implementatie |
| **Doel** | Voorkom class explosie | Compatibiliteit |
| **Ontwikkeling** | Parallel | Achteraf |

## Gerelateerde Patterns
- **Adapter**: Maakt incompatibele interfaces compatibel
- **Abstract Factory**: Kan Bridge implementaties creëren
- **Strategy**: Vergelijkbaar, maar Strategy focust op algoritmes
