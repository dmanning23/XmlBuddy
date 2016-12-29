using Microsoft.Xna.Framework.Content.Pipeline;
using System;
using TInput = System.String;
using TOutput = System.String;

namespace XmlBuddy.Content
{
	/// <summary>
	/// This class will be instantiated by the XNA Framework Content Pipeline
	/// to apply custom processing to content data, converting an object of
	/// type TInput to TOutput. The input and output types may be the same if
	/// the processor wishes to alter data without changing its type.
	///
	/// This should be part of a Content Pipeline Extension Library project.
	///
	/// TODO: change the ContentProcessor attribute to specify the correct
	/// display name for this processor.
	/// </summary>
	[ContentProcessor(DisplayName = "Xml - XmlBuddy")]
	public class XmlDocumentProcessor : ContentProcessor<TInput, TOutput>
	{
		public override TOutput Process(TInput input, ContentProcessorContext context)
		{
			try
			{
				return input;
			}
			catch (Exception ex)
			{
				throw new Exception("There was an error processing the xml", ex);
			}
		}
	}
}