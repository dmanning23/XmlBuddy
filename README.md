XmlBuddy
=================

MonoGame content pipeline project for reading/writing XML files.

To use this library:
1. Add the Nuget package from <a href="https://www.nuget.org/packages/XmlBuddy/">https://www.nuget.org/packages/XmlBuddy/</a>
2. In the MonoGame Pipeline tool,  open References -> Collection to open the Reference Editor.
3. Click "Add", navigate to your project's Content folder, and select XmlBuddy.Content.dll
4. Save the project and restart the Pipeline tool.
5. Select your XML files in the navigation pane, and change the Importer to "Xml Source Importer" and the Processor to "XML Document Processor"
6. Load the xml data into your game using the following recipe:

```
protected override void LoadContent()
{
	var data = Content.Load<string>(filename);
	var xmlDoc = new XmlDocument();
	xmlDoc.LoadXml(data);
	
	...
}
```

