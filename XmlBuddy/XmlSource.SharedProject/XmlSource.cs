using System;

namespace XmlBuddy
{
	/// <summary>
	/// This is the importer for an xml file.
	/// </summary>
	public class XmlSource : IDisposable
	{
		public string XmlCode { get; private set; }

		public XmlSource(string xmlCode)
		{
			XmlCode = xmlCode;
		}

		public void Dispose()
		{
			XmlCode = null;
		}
	}
}
