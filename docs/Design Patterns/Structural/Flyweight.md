# Flyweight Pattern

## Intentie
Gebruikt **delen** om grote aantallen fijnkorrelige objecten efficiënt te ondersteunen door **gemeenschappelijke state te delen**.

## Wanneer gebruiken?
- Wanneer een applicatie een groot aantal vergelijkbare objecten gebruikt
- Wanneer geheugengebruik een probleem is
- Wanneer objecten intrinsieke (gedeelde) en extrinsieke (unieke) state hebben
- Game development (particles, tiles), text editors (characters)

## Structuur

```
???????????????????????
?  FlyweightFactory   ?
???????????????????????
? -flyweights: Map    ?
? +GetFlyweight()     ?
???????????????????????
          ?
          ?
???????????????????????
?     Flyweight       ?
???????????????????????
? -intrinsicState     ?  ??? Shared (immutable)
? +Operation(extState)?  ??? extrinsic passed in
???????????????????????
```

## Implementatie in C#

### Basis Implementatie: Text Characters

```csharp
// Flyweight - shared character formatting
public class CharacterFormat
{
    public string FontFamily { get; }
    public int FontSize { get; }
    public bool IsBold { get; }
    public bool IsItalic { get; }
    public string Color { get; }

    public CharacterFormat(string fontFamily, int fontSize, bool isBold, bool isItalic, string color)
    {
        FontFamily = fontFamily;
        FontSize = fontSize;
        IsBold = isBold;
        IsItalic = isItalic;
        Color = color;
    }

    public void Render(char character, int x, int y)
    {
        Console.WriteLine($"Rendering '{character}' at ({x},{y}) " +
                         $"[{FontFamily}, {FontSize}pt, Bold:{IsBold}, Italic:{IsItalic}, {Color}]");
    }
}

// Flyweight Factory
public class CharacterFormatFactory
{
    private readonly Dictionary<string, CharacterFormat> _formats = new();

    public CharacterFormat GetFormat(string fontFamily, int fontSize, bool isBold, bool isItalic, string color)
    {
        var key = $"{fontFamily}_{fontSize}_{isBold}_{isItalic}_{color}";

        if (!_formats.TryGetValue(key, out var format))
        {
            format = new CharacterFormat(fontFamily, fontSize, isBold, isItalic, color);
            _formats[key] = format;
            Console.WriteLine($"Created new format: {key}");
        }

        return format;
    }

    public int FormatCount => _formats.Count;
}

// Character with extrinsic state
public class Character
{
    public char Char { get; }
    public int X { get; set; }
    public int Y { get; set; }
    public CharacterFormat Format { get; } // Flyweight reference

    public Character(char c, int x, int y, CharacterFormat format)
    {
        Char = c;
        X = x;
        Y = y;
        Format = format;
    }

    public void Render()
    {
        Format.Render(Char, X, Y);
    }
}

// Document using flyweights
public class TextDocument
{
    private readonly List<Character> _characters = new();
    private readonly CharacterFormatFactory _formatFactory = new();

    public void AddCharacter(char c, int x, int y, string font, int size, bool bold, bool italic, string color)
    {
        var format = _formatFactory.GetFormat(font, size, bold, italic, color);
        _characters.Add(new Character(c, x, y, format));
    }

    public void Render()
    {
        foreach (var character in _characters)
        {
            character.Render();
        }
    }

    public void PrintStats()
    {
        Console.WriteLine($"\nCharacters: {_characters.Count}");
        Console.WriteLine($"Unique formats: {_formatFactory.FormatCount}");
        Console.WriteLine($"Memory saved: ~{(_characters.Count - _formatFactory.FormatCount) * 50} bytes");
    }
}

// Gebruik
var document = new TextDocument();

// Add "Hello" in Arial 12pt
foreach (var (c, i) in "Hello".Select((c, i) => (c, i)))
{
    document.AddCharacter(c, i * 10, 0, "Arial", 12, false, false, "Black");
}

// Add "World" in Arial 12pt Bold
foreach (var (c, i) in "World".Select((c, i) => (c, i)))
{
    document.AddCharacter(c, i * 10, 20, "Arial", 12, true, false, "Black");
}

// Add more text with same formatting
foreach (var (c, i) in "!!!!".Select((c, i) => (c, i)))
{
    document.AddCharacter(c, i * 10, 40, "Arial", 12, false, false, "Black");
}

document.Render();
document.PrintStats();
```

### Game Development: Particle System

```csharp
// Flyweight - particle type (intrinsic state)
public class ParticleType
{
    public string TexturePath { get; }
    public int[] TextureData { get; } // Simulated texture data
    public string BlendMode { get; }
    public float BaseSpeed { get; }

    public ParticleType(string texturePath, string blendMode, float baseSpeed)
    {
        TexturePath = texturePath;
        BlendMode = blendMode;
        BaseSpeed = baseSpeed;
        // Simulate loading texture (expensive)
        TextureData = new int[1024 * 1024]; // 4MB per texture
        Console.WriteLine($"Loaded texture: {texturePath}");
    }
}

// Particle with extrinsic state
public class Particle
{
    public float X { get; set; }
    public float Y { get; set; }
    public float VelocityX { get; set; }
    public float VelocityY { get; set; }
    public float Lifetime { get; set; }
    public float Scale { get; set; }
    public float Rotation { get; set; }
    public ParticleType Type { get; } // Flyweight

    public Particle(ParticleType type, float x, float y)
    {
        Type = type;
        X = x;
        Y = y;
        Scale = 1.0f;
        Lifetime = 1.0f;
    }

    public void Update(float deltaTime)
    {
        X += VelocityX * Type.BaseSpeed * deltaTime;
        Y += VelocityY * Type.BaseSpeed * deltaTime;
        Lifetime -= deltaTime;
    }

    public void Render()
    {
        // Use flyweight's texture data
        // Console.WriteLine($"Rendering {Type.TexturePath} at ({X:F1}, {Y:F1})");
    }
}

// Flyweight Factory
public class ParticleTypeFactory
{
    private readonly Dictionary<string, ParticleType> _types = new();

    public ParticleType GetParticleType(string texturePath, string blendMode, float baseSpeed)
    {
        var key = $"{texturePath}_{blendMode}_{baseSpeed}";

        if (!_types.TryGetValue(key, out var type))
        {
            type = new ParticleType(texturePath, blendMode, baseSpeed);
            _types[key] = type;
        }

        return type;
    }

    public int TypeCount => _types.Count;
    public long EstimatedMemorySaved(int particleCount) =>
        (particleCount - _types.Count) * 4 * 1024 * 1024L; // 4MB per texture
}

// Particle System
public class ParticleSystem
{
    private readonly List<Particle> _particles = new();
    private readonly ParticleTypeFactory _typeFactory = new();

    public void EmitFire(float x, float y, int count)
    {
        var fireType = _typeFactory.GetParticleType("fire.png", "Additive", 100f);
        
        for (int i = 0; i < count; i++)
        {
            var particle = new Particle(fireType, x, y)
            {
                VelocityX = (Random.Shared.NextSingle() - 0.5f) * 2,
                VelocityY = -Random.Shared.NextSingle() * 2,
                Scale = 0.5f + Random.Shared.NextSingle() * 0.5f,
                Lifetime = 1f + Random.Shared.NextSingle()
            };
            _particles.Add(particle);
        }
    }

    public void EmitSmoke(float x, float y, int count)
    {
        var smokeType = _typeFactory.GetParticleType("smoke.png", "Alpha", 50f);
        
        for (int i = 0; i < count; i++)
        {
            var particle = new Particle(smokeType, x, y)
            {
                VelocityX = (Random.Shared.NextSingle() - 0.5f),
                VelocityY = -Random.Shared.NextSingle(),
                Scale = 1f + Random.Shared.NextSingle(),
                Lifetime = 2f + Random.Shared.NextSingle() * 2
            };
            _particles.Add(particle);
        }
    }

    public void EmitSparks(float x, float y, int count)
    {
        var sparkType = _typeFactory.GetParticleType("spark.png", "Additive", 200f);
        
        for (int i = 0; i < count; i++)
        {
            var particle = new Particle(sparkType, x, y)
            {
                VelocityX = (Random.Shared.NextSingle() - 0.5f) * 4,
                VelocityY = (Random.Shared.NextSingle() - 0.5f) * 4,
                Scale = 0.2f + Random.Shared.NextSingle() * 0.3f,
                Lifetime = 0.5f + Random.Shared.NextSingle() * 0.5f
            };
            _particles.Add(particle);
        }
    }

    public void Update(float deltaTime)
    {
        foreach (var particle in _particles)
        {
            particle.Update(deltaTime);
        }
        _particles.RemoveAll(p => p.Lifetime <= 0);
    }

    public void PrintStats()
    {
        Console.WriteLine($"\nParticle Statistics:");
        Console.WriteLine($"  Active particles: {_particles.Count}");
        Console.WriteLine($"  Unique particle types: {_typeFactory.TypeCount}");
        Console.WriteLine($"  Memory saved: ~{_typeFactory.EstimatedMemorySaved(_particles.Count) / 1024 / 1024} MB");
    }
}

// Gebruik
var particleSystem = new ParticleSystem();

// Emit many particles
particleSystem.EmitFire(100, 100, 1000);
particleSystem.EmitSmoke(100, 100, 500);
particleSystem.EmitSparks(100, 100, 2000);
particleSystem.EmitFire(200, 100, 1000); // Reuses fire type
particleSystem.EmitSmoke(200, 100, 500); // Reuses smoke type

particleSystem.PrintStats();
```

### With String Interning (Built-in Flyweight)

```csharp
// C# has built-in flyweight for strings
string a = "Hello";
string b = "Hello";
string c = string.Intern(new string(new[] { 'H', 'e', 'l', 'l', 'o' }));

Console.WriteLine(ReferenceEquals(a, b)); // True - same object
Console.WriteLine(ReferenceEquals(a, c)); // True - interned

// Custom string flyweight for specific domain
public class CountryCodeFlyweight
{
    private static readonly Dictionary<string, string> _codes = new();

    public static string GetCode(string code)
    {
        var normalized = code.ToUpperInvariant();
        
        if (!_codes.TryGetValue(normalized, out var cached))
        {
            cached = normalized;
            _codes[normalized] = cached;
        }

        return cached;
    }
}

// All references to "NL" will be the same object
var country1 = CountryCodeFlyweight.GetCode("nl");
var country2 = CountryCodeFlyweight.GetCode("NL");
Console.WriteLine(ReferenceEquals(country1, country2)); // True
```

## Intrinsic vs Extrinsic State

| State Type | Beschrijving | Opslag |
|------------|--------------|--------|
| **Intrinsic** | Gedeeld, onveranderlijk | In Flyweight |
| **Extrinsic** | Uniek per context, veranderlijk | Extern doorgegeven |

## Voordelen
- ? Significant geheugen besparing
- ? Minder objecten in memory
- ? Betere cache performance

## Nadelen
- ? Complexiteit verhoogd
- ? Runtime overhead (lookups)
- ? Threading complications (shared state)

## Gerelateerde Patterns
- **Factory**: Flyweight Factory beheert instances
- **Singleton**: Extreme vorm van Flyweight
- **Composite**: Leaf nodes kunnen flyweights zijn
- **State/Strategy**: State/Strategy objecten kunnen flyweights zijn
