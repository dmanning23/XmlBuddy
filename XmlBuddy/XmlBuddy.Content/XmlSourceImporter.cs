using Microsoft.Xna.Framework.Content.Pipeline;

namespace XmlBuddy.Content
{
	/// <summary>
	/// This class will be instantiated by the XNA Content Pipeline to import a file from disk into the specified type, TImport.
	/// 
	/// This should be part of a Content Pipeline Extension Library project.
	/// </summary>
	[ContentImporter(".xml", DefaultProcessor = "XmlDocumentProcessor", DisplayName = "Xml Source Importer")]
	public class XmlSourceImporter : ContentImporter<XmlSource>
	{
		public override XmlSource Import(string filename, ContentImporterContext context)
		{
			return new XmlSource(System.IO.File.ReadAllText(filename));
		}
	}
}
