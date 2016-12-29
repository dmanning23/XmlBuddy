using FilenameBuddy;
using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace XmlBuddy
{
	public delegate void XmlNodeFunc(XmlNode node);

	/// <summary>
	/// Stores all the states, messages, and state transitions
	/// </summary>
	public abstract class XmlFileBuddy : IDisposable
	{
		#region Fields

		/// <summary>
		/// The name this type of content is stored in the xml file
		/// </summary>
		public string ContentName { get; private set; }

		/// <summary>
		/// The file this dude was read/write from
		/// </summary>
		public Filename Filename { get; set; }

		public ContentManager Content { protected get; set; }

		#endregion //Fields

		#region Methods

		/// <summary>
		/// constructor
		/// </summary>
		protected XmlFileBuddy(string contentName)
		{
			ContentName = contentName;
			Filename = new Filename();
		}

		/// <summary>
		/// constructor
		/// </summary>
		protected XmlFileBuddy(string contentName, Filename file)
		{
			ContentName = contentName;
			Filename = new Filename(file);
		}

		protected XmlFileBuddy(string contentName, Filename file, ContentManager content) : this(contentName, file)
		{
			Content = content;
		}

		/// <summary>
		/// copy constructor
		/// </summary>
		protected XmlFileBuddy(XmlFileBuddy obj)
			: this(obj.ContentName, obj.Filename, obj.Content)
		{
		}

		public void ReadXmlFile(ContentManager content)
		{
			Content = content;
			ReadXmlFile();
		}

		/// <summary>
		/// Open the specified xml file and read it in
		/// </summary>
		public virtual void ReadXmlFile()
		{
			try
			{
				if (null == Content)
				{
					// Open the file.
#if ANDROID
					using (var stream = Game.Activity.Assets.Open(Filename.File))
#else
					using (var stream = File.Open(Filename.File, FileMode.Open, FileAccess.Read))
#endif
					{
						XmlDocument xmlDoc = new XmlDocument();
						xmlDoc.Load(stream);
						ParseRootNode(xmlDoc);
					}
				}
				else
				{
					var data = Content.Load<string>(Filename.GetPathFileNoExt());
					var xmlDoc = new XmlDocument();
					xmlDoc.LoadXml(data);
					ParseRootNode(xmlDoc);
				}
			}
			catch (Exception ex)
			{
				throw new Exception(string.Format("error in {0}", Filename.GetFile()), ex);
			}
		}

		private void ParseRootNode(XmlDocument xmlDoc)
		{
			XmlNode rootNode = xmlDoc.DocumentElement;
			if (rootNode.NodeType != XmlNodeType.Element)
			{
				//should be an xml node!!!
				throw new Exception("not an xml node: " + rootNode.NodeType.ToString());
			}

			//Read in child nodes
			ReadChildNodes(rootNode, ParseXmlNode);
		}

		/// <summary>
		/// write out this object to an xml file
		/// </summary>
		public virtual void WriteXml()
		{
#if !WINDOWS_UWP
			//open the file, create it if it doesnt exist yet
			using (XmlTextWriter xmlFile = new XmlTextWriter(Filename.File, null))
			{
				xmlFile.Formatting = Formatting.Indented;
				xmlFile.Indentation = 1;
				xmlFile.IndentChar = '\t';

				xmlFile.WriteStartDocument();

				//add the xml node
				xmlFile.WriteStartElement(ContentName);

				WriteXmlNodes(xmlFile);

				xmlFile.WriteEndElement();

				xmlFile.WriteEndDocument();

				// Close the file.
				xmlFile.Flush();
				xmlFile.Close();
			}
#endif
		}

		/// <summary>
		/// Parse a node from the xml file.
		/// </summary>
		/// <param name="node">the xml node to read from</param>
		public abstract void ParseXmlNode(XmlNode node);

		protected void NodeError(XmlNode node)
		{
			//what is in this node?
			throw new XmlException(string.Format("Unknown xml node passed to {0}: \"{1}\"", GetType(), node.Name));
		}

		/// <summary>
		/// Write out all the data for this object to xml nodes.
		/// </summary>
		/// <param name="xmlWriter"></param>
#if !WINDOWS_UWP
		public abstract void WriteXmlNodes(XmlTextWriter xmlWriter);
#endif

		/// <summary>
		/// Given an xml node, call the delegate on all its child nodes
		/// </summary>
		/// <param name="node"></param>
		/// <param name="func"></param>
		public static void ReadChildNodes(XmlNode node, XmlNodeFunc func)
		{
			if (node.HasChildNodes)
			{
				for (XmlNode childNode = node.FirstChild;
					null != childNode;
					childNode = childNode.NextSibling)
				{
					//ignore comment nodes
					if (childNode.NodeType != XmlNodeType.Comment)
					{
						func(childNode);
					}
				}
			}

			//Parse all the attributes too
			if (null != node.Attributes)
			{
				var attributes = node.Attributes;
				for (int i = 0; i < attributes.Count; i++)
				{
					func(attributes.Item(i));
				}
			}
		}

		public void Dispose()
		{
			Content = null;
		}

		#endregion //Methods
	}
}