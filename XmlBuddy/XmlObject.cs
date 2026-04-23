using System;
using System.Xml;

namespace XmlBuddy
{
    /// <summary>
    /// Abstract base class for objects that can be serialized to and from XML nodes.
    /// </summary>
    public abstract class XmlObject
    {
        /// <summary>
        /// Reads state from an XML node. Override to handle recognized node names.
        /// Throws <see cref="XmlException"/> for unknown nodes by default.
        /// </summary>
        public virtual void ParseXmlNode(XmlNode node)
        {
            NodeError(node);
        }

        protected void NodeError(XmlNode node)
        {
            throw new XmlException(string.Format("Unknown xml node passed to {0}: \"{1}\"", GetType(), node.Name));
        }

        /// <summary>
        /// Writes this object's state as XML nodes to the given writer.
        /// </summary>
        /// <param name="xmlWriter">The writer to serialize into.</param>
        public abstract void WriteXmlNodes(XmlTextWriter xmlWriter);
    }
}