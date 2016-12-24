using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using System.Xml;

namespace XmlBuddy.Content
{
	/// <summary>
	/// This class will be instantiated by the XNA Content Pipeline to write the specified data type into binary .xnb format.
	///
	/// This should be part of a Content Pipeline Extension Library project.
	/// </summary>
	[ContentTypeWriter]
	public class XmlSourceWriter : ContentTypeWriter<XmlSource>
	{
		protected override void Write(ContentWriter output, XmlSource value)
		{
			output.Write(value.XmlCode);
		}

		public override string GetRuntimeType(TargetPlatform targetPlatform)
		{
			return typeof(XmlDocument).AssemblyQualifiedName;
		}

		public override string GetRuntimeReader(TargetPlatform targetPlatform)
		{
			return typeof(XmlDocumentReader).AssemblyQualifiedName;
		}
	}
}
