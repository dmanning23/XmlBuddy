using Microsoft.Xna.Framework.Content.Pipeline;
using System;
using TInput = System.String;

namespace XmlBuddy.Content
{
	/// <summary>
	/// This class will be instantiated by the XNA Framework Content Pipeline
	/// to import a file from disk into the specified type, TImport.
	///
	/// This should be part of a Content Pipeline Extension Library project.
	///
	/// TODO: change the ContentImporter attribute to specify the correct file
	/// extension, display name, and default processor for this importer.
	/// </summary>
	[ContentImporter(".json", DisplayName = "XmlBuddy Json Importer", DefaultProcessor = "XmlDocumentProcessor")]
	public class JsonSourceImporter : ContentImporter<TInput>
	{
		public override TInput Import(string filename, ContentImporterContext context)
		{
			try
			{
				return System.IO.File.ReadAllText(filename);
			}
			catch (Exception ex)
			{
				throw new Exception("There was an error importing the thing", ex);
			}
		}
	}
}
