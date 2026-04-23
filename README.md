# XmlBuddy

[![NuGet](https://img.shields.io/nuget/v/XmlBuddy.svg)](https://www.nuget.org/packages/XmlBuddy/)

A MonoGame library for reading and writing XML and JSON data files, with optional MonoGame Content Pipeline integration.

## Features

- Abstract base classes for objects that serialize to/from XML or JSON
- Works with both the MonoGame `ContentManager` and direct file I/O
- MonoGame Content Pipeline importers for `.xml` and `.json` files
- Android asset support
- JSON support via Newtonsoft.Json

## Installation

Add the NuGet package to your game project:

```
dotnet add package XmlBuddy
```

Or search for `XmlBuddy` in the NuGet package manager.

## Content Pipeline Setup

To use XmlBuddy with the MonoGame Content Pipeline (MGCB Editor):

1. Add a reference to `XmlBuddy.Content.dll` in the MGCB Editor under **References**.
2. Select your `.xml` or `.json` content files.
3. Set the **Importer** to `XmlBuddy Importer` (XML) or `XmlBuddy Json Importer` (JSON).
4. Set the **Processor** to `XmlBuddy`.
5. Rebuild the content project.

## Usage

### Implementing a serializable object

Inherit from `XmlFileBuddy` and implement `ParseXmlNode` and `WriteXmlNodes`:

```csharp
// Nested object — inherits XmlObject rather than XmlFileBuddy
public class MyItem : XmlObject
{
    public string Label { get; set; }

    public override void ParseXmlNode(XmlNode node)
    {
        switch (node.Name)
        {
            case "Label": Label = node.InnerText; break;
            default: NodeError(node); break;
        }
    }

    public override void WriteXmlNodes(XmlTextWriter writer)
    {
        writer.WriteElementString("Label", Label);
    }
}

// Top-level file object
public class MyData : XmlFileBuddy
{
    public string Name { get; set; }
    public int Value { get; set; }
    public List<MyItem> Items { get; set; } = new();

    public MyData() : base("MyData") { }

    public override void ParseXmlNode(XmlNode node)
    {
        switch (node.Name)
        {
            case "Name":  Name = node.InnerText; break;
            case "Value": Value = int.Parse(node.InnerText); break;
            case "Items":
                // ReadChildNodes iterates every child of the <Items> element
                // and calls the delegate once per child node
                ReadChildNodes(node, itemNode =>
                {
                    var item = new MyItem();
                    ReadChildNodes(itemNode, item.ParseXmlNode);
                    Items.Add(item);
                });
                break;
            default: NodeError(node); break;
        }
    }

    public override void WriteXmlNodes(XmlTextWriter writer)
    {
        writer.WriteElementString("Name", Name);
        writer.WriteElementString("Value", Value.ToString());

        writer.WriteStartElement("Items");
        foreach (var item in Items)
        {
            writer.WriteStartElement("Item");
            item.WriteXmlNodes(writer);
            writer.WriteEndElement();
        }
        writer.WriteEndElement();
    }
}
```

The corresponding XML file looks like this:

```xml
<?xml version="1.0"?>
<MyData>
	<Name>example</Name>
	<Value>42</Value>
	<Items>
		<Item>
			<Label>first</Label>
		</Item>
		<Item>
			<Label>second</Label>
		</Item>
	</Items>
</MyData>
```

### Loading with ContentManager (Content Pipeline)

```csharp
protected override void LoadContent()
{
    var myData = new MyData();
    myData.Filename = new Filename("Content/myfile");
    myData.ReadXmlFile(Content);
}
```

### Loading without ContentManager (direct file I/O)

```csharp
var myData = new MyData();
myData.Filename = new Filename("path/to/myfile.xml");
myData.ReadXmlFile();
```

### Writing XML

```csharp
myData.Filename = new Filename("path/to/output.xml");
myData.WriteXml();
```

### Reading and writing JSON

```csharp
// Read
myData.Filename = new Filename("path/to/myfile.json");
myData.ReadJsonFile();

// Write
myData.WriteJson();
```

## API Overview

### `XmlFileBuddy`

Abstract base class for top-level data objects that map to a file.

| Member | Description |
|---|---|
| `ContentName` | Root XML element name used when writing |
| `Filename` | File path to read from / write to |
| `ReadXmlFile(ContentManager?)` | Load state from an XML file |
| `ReadJsonFile(ContentManager?)` | Load state from a JSON file |
| `WriteXml()` | Serialize state to an XML file |
| `WriteJson()` | Serialize state to a JSON file |
| `ParseXmlNode(XmlNode)` | **Abstract.** Handle a single XML node during read |
| `WriteXmlNodes(XmlTextWriter)` | **Abstract.** Write child nodes during serialization |
| `ReadChildNodes(XmlNode, XmlNodeFunc)` | Static helper to iterate child nodes and attributes |

### `XmlObject`

Lighter abstract base for nested objects that are serialized as part of a parent document rather than a standalone file.

| Member | Description |
|---|---|
| `ParseXmlNode(XmlNode)` | Override to handle recognized node names |
| `WriteXmlNodes(XmlTextWriter)` | **Abstract.** Write this object's XML representation |

## Dependencies

- [FilenameBuddy](https://www.nuget.org/packages/FilenameBuddy/) — cross-platform file path handling
- [Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json/) — JSON serialization
- MonoGame 3.8+

## License

MIT
