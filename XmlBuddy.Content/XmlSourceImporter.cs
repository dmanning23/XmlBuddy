using Microsoft.Xna.Framework.Content.Pipeline;
using System;
using TInput = System.String;

namespace XmlBuddy.Content
{
	/// <summary>
	/// MonoGame Content Pipeline importer that reads an XML file as raw text,
	/// passing it to <see cref="XmlDocumentProcessor"/> for further processing.
	/// </summary>
	[ContentImporter(".xml", DisplayName = "XmlBuddy Importer", DefaultProcessor = "XmlDocumentProcessor")]
	public class XmlSourceImporter : ContentImporter<TInput>
	{
		public override TInput Import(string filename, ContentImporterContext context)
		{
			try
			{
				return System.IO.File.ReadAllText(filename);
			}
			catch (Exception ex)
			{
				throw new Exception($"Error importing XML file: {filename}", ex);
			}
		}
	}
}
