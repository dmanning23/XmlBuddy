using Microsoft.Xna.Framework.Content.Pipeline;
using System;
using TInput = System.String;

namespace XmlBuddy.Content
{
	/// <summary>
	/// MonoGame Content Pipeline importer that reads a JSON file as raw text,
	/// passing it to <see cref="XmlDocumentProcessor"/> for further processing.
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
				throw new Exception($"Error importing JSON file: {filename}", ex);
			}
		}
	}
}
