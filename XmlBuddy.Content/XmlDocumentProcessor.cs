using Microsoft.Xna.Framework.Content.Pipeline;
using TInput = System.String;
using TOutput = System.String;

namespace XmlBuddy.Content
{
	/// <summary>
	/// MonoGame Content Pipeline processor for XML and JSON source files.
	/// Passes raw text through unchanged; deserialization happens at runtime.
	/// </summary>
	[ContentProcessor(DisplayName = "XmlBuddy")]
	public class XmlDocumentProcessor : ContentProcessor<TInput, TOutput>
	{
		public override TOutput Process(TInput input, ContentProcessorContext context)
		{
			return input;
		}
	}
}