using FilenameBuddy;
using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
#if ANDROID
using Microsoft.Xna.Framework;
#endif
#if OUYA
using Ouya.Console.Api;
#endif

namespace XmlBuddy
{
	public delegate void XmlNodeFunc(XmlNode xmlNode);

	/// <summary>
	/// Stores all the states, messages, and state transitions
	/// </summary>
	public abstract class XmlFileBuddy
	{
		#region Fields

		/// <summary>
		/// The name this type of content is stored in the xml file
		/// </summary>
		public string ContentName { get; private set; }

		/// <summary>
		/// The file this dude was read/write from
		/// </summary>
		public Filename XmlFilename { get; set; }

		#endregion //Fields

		#region Methods

		/// <summary>
		/// constructor
		/// </summary>
		public XmlFileBuddy(string contentName)
		{
			ContentName = contentName;
			XmlFilename = new Filename();
		}

		/// <summary>
		/// constructor
		/// </summary>
		public XmlFileBuddy(string contentName, Filename file)
		{
			ContentName = contentName;
			XmlFilename = new Filename(file);
		}

		/// <summary>
		/// copy constructor
		/// </summary>
		protected XmlFileBuddy(XmlFileBuddy obj)
			: this(obj.ContentName, obj.XmlFilename)
		{
		}

		/// <summary>
		/// read in serialized xna state machine from XML
		/// </summary>
		/// <param name="strFilename">file to open</param>
		/// <returns>whether or not it was able to open it</returns>
		public virtual void ReadXmlFile()
		{
			// Open the file.
#if ANDROID
			Stream stream = Game.Activity.Assets.Open(XmlFilename.File);
#else
			FileStream stream = File.Open(XmlFilename.File, FileMode.Open, FileAccess.Read);
#endif
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(stream);
			XmlNode rootNode = xmlDoc.DocumentElement;

#if DEBUG
			if (rootNode.NodeType != XmlNodeType.Element)
			{
				//should be an xml node!!!
				throw new Exception("not an xml node: " + rootNode.NodeType.ToString());
			}

			//eat up the name of that xml node
			if (("XnaContent" != rootNode.Name) || !rootNode.HasChildNodes)
			{
				throw new Exception("invalid XnaContent node: " + rootNode.Name);
			}
#endif
			//next node is "<Asset Type="SPFSettings.StateMachineXML">"
			XmlNode assetNode = rootNode.FirstChild;
#if DEBUG
			if (!assetNode.HasChildNodes)
			{
				throw new Exception("invalid Asset node: no child nodes");
			}
			if ("Asset" != assetNode.Name)
			{
				throw new Exception("invalid Asset node: " + assetNode.Name);
			}

			//should have an attribute Type
			XmlNamedNodeMap mapAttributes = assetNode.Attributes;
			for (int i = 0; i < mapAttributes.Count; i++)
			{
				//will only have the name attribute
				string strName = mapAttributes.Item(i).Name;
				string strValue = mapAttributes.Item(i).Value;
				if ("Type" == strName)
				{
					if (strValue != ContentName)
					{
						throw new Exception("invalid Type node: " + strValue);
					}
				}
				else
				{
					throw new Exception("invalid Type node: " + strName);
				}
			}
#endif

			//Read in child nodes
			ReadChildNodes(assetNode, ParseXmlNode);

			// Close the file.
			stream.Close();
		}

		/// <summary>
		/// write out this object to an xml file
		/// </summary>
		/// <param name="strFilename">teh file to write out to</param>
		public void WriteXml()
		{
			//open the file, create it if it doesnt exist yet
			XmlTextWriter xmlFile = new XmlTextWriter(XmlFilename.File, null);
			xmlFile.Formatting = Formatting.Indented;
			xmlFile.Indentation = 1;
			xmlFile.IndentChar = '\t';

			xmlFile.WriteStartDocument();

			//add the xml node
			xmlFile.WriteStartElement("XnaContent");
			xmlFile.WriteStartElement("Asset");
			xmlFile.WriteAttributeString("Type", ContentName);

			WriteXmlNodes(xmlFile);

			xmlFile.WriteEndElement();
			xmlFile.WriteEndElement();

			xmlFile.WriteEndDocument();

			// Close the file.
			xmlFile.Flush();
			xmlFile.Close();
		}

		/// <summary>
		/// Parse a node from the xml file.
		/// </summary>
		/// <param name="xmlNode">the xml node to read from</param>
		/// <returns></returns>
		public abstract void ParseXmlNode(XmlNode xmlNode);

		/// <summary>
		/// Write out all the data for this object to xml nodes.
		/// </summary>
		/// <param name="xmlFile"></param>
		public abstract void WriteXmlNodes(XmlTextWriter xmlFile);

		/// <summary>
		/// Given an xml node, call the delegate on all its child nodes
		/// </summary>
		/// <param name="xmlNode"></param>
		/// <param name="func"></param>
		public static void ReadChildNodes(XmlNode xmlNode, XmlNodeFunc func)
		{
			if (xmlNode.HasChildNodes)
			{
				for (XmlNode childNode = xmlNode.FirstChild;
					null != childNode;
					childNode = childNode.NextSibling)
				{
					func(childNode);
				}
			}
		}

		#endregion //Methods
	}
}