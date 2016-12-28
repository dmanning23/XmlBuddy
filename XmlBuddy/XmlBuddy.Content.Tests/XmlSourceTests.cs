using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlBuddy.Content.Tests
{
	[TestFixture]
    public class XmlSourceTests
    {
		[Test]
		public void XmlCodeTest()
		{
			var test = new XmlSource("test");

			Assert.AreEqual("test", test.XmlCode);
		}
    }
}
