using FilenameBuddy;
using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Newtonsoft.Json;

namespace XmlBuddy
{
    public delegate void XmlNodeFunc(XmlNode node);

    /// <summary>
    /// Abstract base class for objects that serialize to and from XML or JSON files.
    /// </summary>
    public abstract class XmlFileBuddy : IDisposable
    {
        #region Fields

        /// <summary>
        /// The root XML element name used when reading and writing this object's file.
        /// </summary>
        [JsonIgnore]
        public string ContentName { get; private set; }

        /// <summary>
        /// The file path this object reads from and writes to.
        /// </summary>
        [JsonIgnore]
        public Filename Filename { get; set; }

        [JsonIgnore]
        public ContentManager Content { protected get; set; }

        #endregion //Fields

        #region Methods

        protected XmlFileBuddy(string contentName)
        {
            ContentName = contentName;
            Filename = new Filename();
        }

        protected XmlFileBuddy(string contentName, Filename file)
        {
            ContentName = contentName;
            Filename = new Filename(file);
        }

        protected XmlFileBuddy(XmlFileBuddy obj)
            : this(obj.ContentName, obj.Filename)
        {
            Content = obj.Content;
        }

        /// <summary>
        /// Open the specified xml file and read it in
        /// </summary>
        public virtual void ReadXmlFile(ContentManager content = null)
        {
            Content = content;

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
                    var data = Content.Load<string>(Filename.GetRelPathFileNoExt());
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

        /// <summary>
        /// Open the specified JSON file and populate this object from it.
        /// </summary>
        public virtual void ReadJsonFile(ContentManager content = null)
        {
            Content = content;
            try
            {
                if (null == Content)
                {
#if ANDROID
					using (var stream = Game.Activity.Assets.Open(Filename.File))
#else
                    using (var stream = File.Open(Filename.File, FileMode.Open, FileAccess.Read))
#endif
                    {
                        using (var sr = new StreamReader(stream))
                        {
                            JsonConvert.PopulateObject(sr.ReadToEnd(), this);
                        }
                    }
                }
                else
                {
                    var data = content.Load<string>(Filename.GetRelPathFileNoExt());
                    JsonConvert.PopulateObject(data, this);
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
                throw new Exception("not an xml node: " + rootNode.NodeType.ToString());
            }

            ReadChildNodes(rootNode, ParseXmlNode);
        }

        /// <summary>
        /// Write this object to the XML file at <see cref="Filename"/>.
        /// </summary>
        public virtual void WriteXml()
        {
            using (XmlTextWriter xmlFile = new XmlTextWriter(Filename.File, null))
            {
                xmlFile.Formatting = System.Xml.Formatting.Indented;
                xmlFile.Indentation = 1;
                xmlFile.IndentChar = '\t';

                xmlFile.WriteStartDocument();
                xmlFile.WriteStartElement(ContentName);

                WriteXmlNodes(xmlFile);

                xmlFile.WriteEndElement();
                xmlFile.WriteEndDocument();

                xmlFile.Flush();
                xmlFile.Close();
            }
        }

        /// <summary>
        /// Write this object to the JSON file at <see cref="Filename"/>.
        /// </summary>
        public virtual void WriteJson()
        {
            using (var file = File.CreateText(Filename.File))
            {
                var serializer = new JsonSerializer()
                {
                    DefaultValueHandling = DefaultValueHandling.Ignore,
                };
                serializer.Serialize(file, this);
            }
        }

        /// <summary>
        /// Parse a node from the xml file.
        /// </summary>
        /// <param name="node">the xml node to read from</param>
        public abstract void ParseXmlNode(XmlNode node);

        protected void NodeError(XmlNode node)
        {
            throw new XmlException(string.Format("Unknown xml node passed to {0}: \"{1}\"", GetType(), node.Name));
        }

        /// <summary>
        /// Write this object's state as XML nodes to the given writer.
        /// </summary>
        /// <param name="xmlWriter">The writer to serialize into.</param>
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
                    if (childNode.NodeType != XmlNodeType.Comment)
                    {
                        func(childNode);
                    }
                }
            }

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