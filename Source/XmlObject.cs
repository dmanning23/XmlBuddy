using System;
using System.Xml;

namespace XmlBuddy
{
	public abstract class XmlObject
	{
		public virtual void ParseXmlNode(XmlNode node)
		{
			//what is in this node?
			throw new XmlException(string.Format("Unknown xml node passed to {0}: \"{1}\"", GetType(), node.Name));
		}

		/// <summary>
		/// write this dude out to file
		/// </summary>
		/// <param name="xmlWriter"></param>
#if WINDOWS_UWP
		public abstract void WriteXmlNodes();
#else
		public abstract void WriteXmlNodes(XmlTextWriter xmlWriter);
#endif
	}
}