using System;
#if !BRIDGE
using System.Xml;
#endif

namespace XmlBuddy
{
	public abstract class XmlObject
	{
#if !BRIDGE
		public virtual void ParseXmlNode(XmlNode node)
		{
			//what is in this node?
			throw new XmlException(string.Format("Unknown xml node passed to {0}: \"{1}\"", GetType(), node.Name));
		}

		/// <summary>
		/// write this dude out to file
		/// </summary>
		/// <param name="xmlWriter"></param>
		public abstract void WriteXmlNodes(XmlTextWriter xmlWriter);
#endif
	}
}