using FilenameBuddy;
using System;
using System.Diagnostics;
using System.IO;
#if !BRIDGE
using System.Xml;
#endif
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Newtonsoft.Json;

namespace XmlBuddy
{
#if !BRIDGE
	public delegate void XmlNodeFunc(XmlNode node);
#endif

	/// <summary>
	/// Stores all the states, messages, and state transitions
	/// </summary>
	public abstract class XmlFileBuddy : IDisposable
	{
		#region Fields

		/// <summary>
		/// The name this type of content is stored in the xml file
		/// </summary>
		[JsonIgnore]
		public string ContentName { get; private set; }

		/// <summary>
		/// The file this dude was read/write from
		/// </summary>
		[JsonIgnore]
		public Filename Filename { get; set; }

		[JsonIgnore]
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
#if BRIDGE
			throw new NotImplementedException();
#endif

			try
			{
				if (null == Content)
				{
					// Open the file.
#if ANDROID
					using (var stream = Game.Activity.Assets.Open(Filename.File))
#elif !BRIDGE
					using (var stream = File.Open(Filename.File, FileMode.Open, FileAccess.Read))
#endif
					{
#if !BRIDGE
						XmlDocument xmlDoc = new XmlDocument();
						xmlDoc.Load(stream);
						ParseRootNode(xmlDoc);
#endif
					}
				}
				else
				{
#if !BRIDGE
					var data = Content.Load<string>(Filename.GetRelPathFileNoExt());
					var xmlDoc = new XmlDocument();
					xmlDoc.LoadXml(data);
					ParseRootNode(xmlDoc);
#endif
				}
			}
			catch (Exception ex)
			{
				throw new Exception(string.Format("error in {0}", Filename.GetFile()), ex);
			}
		}

		public static T ReadJsonFile<T>(Filename file, ContentManager content = null) where T : XmlFileBuddy
		{
			try
			{
				if (null == content)
				{
#if BRIDGE
					throw new NotSupportedException();
#endif

#if ANDROID
					using (var stream = Game.Activity.Assets.Open(file.File))
#elif !BRIDGE
					using (var stream = File.Open(file.File, FileMode.Open, FileAccess.Read))
#endif
					{
#if !BRIDGE
						JsonSerializer serializer = new JsonSerializer();
						using (var sr = new StreamReader(stream))
						{
							using (var jsonTextReader = new JsonTextReader(sr))
							{
								return serializer.Deserialize<T>(jsonTextReader);
							}
						}
#endif
					}
				}
				else
				{
					var data = content.Load<string>(file.GetRelPathFileNoExt());
					return JsonConvert.DeserializeObject<T>(data);
				}
			}
			catch (Exception ex)
			{
				throw new Exception(string.Format("error in {0}", file.GetFile()), ex);
			}
		}

#if !BRIDGE
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
#endif

		/// <summary>
		/// write out this object to an xml file
		/// </summary>
		public virtual void WriteXml()
		{
#if !BRIDGE
			//open the file, create it if it doesnt exist yet
			using (XmlTextWriter xmlFile = new XmlTextWriter(Filename.File, null))
			{
				xmlFile.Formatting = System.Xml.Formatting.Indented;
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

		public void WriteJson()
		{
#if !BRIDGE
			// serialize JSON directly to a file
			using (var file = File.CreateText(Filename.File))
			{
				JsonSerializer serializer = new JsonSerializer();
				serializer.Serialize(file, this);
			}
#endif
		}

#if !BRIDGE
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

		public abstract void WriteXmlNodes(XmlTextWriter xmlWriter);


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
#endif

		public void Dispose()
		{
			Content = null;
		}

#endregion //Methods
	}
}