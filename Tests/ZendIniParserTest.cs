using System.Collections.Generic;
using NUnit.Framework;
using ZendIni;

namespace Tests
{
	[TestFixture ()]
	public class ZendIniParserTest
	{

		[Test ()]
		public void TestParseStringWithSingleElementSingleValueAndComments ()
		{
			const string testString = @"
			; beep
			[hello]
			; boop
			world = blarg!
			";

			var parser = new ZendIniParser ();
			var config = parser.ParseString (testString);

			Assert.IsInstanceOf<Dictionary<string, Dictionary<string, object>>> (config);
			Assert.IsInstanceOf<Dictionary<string, object>> (config["hello"]);
			Assert.IsInstanceOf<string> (config["hello"]["world"]);
			Assert.AreEqual ("blarg!", config["hello"]["world"].ToString());
		}

		[Test ()]
		public void TestParseStringWithSingleElementArrayOfValues ()
		{
			const string testString = @"
				[data_here]
				host[] = test1
				host[] = test2
			";

			var parser = new ZendIniParser ();
			var config = parser.ParseString (testString);

			Assert.IsInstanceOf<Dictionary<string, Dictionary<string, object>>> (config);
			Assert.IsInstanceOf<Dictionary<string, object>> (config["data_here"]);
			Assert.IsInstanceOf<List<string>> (config ["data_here"] ["host"]);

			var hosts = config ["data_here"] ["host"] as List<string>;
			Assert.AreEqual (2, hosts.Count);
			Assert.AreEqual ("test1", hosts [0]);
			Assert.AreEqual ("test2", hosts [1]);
		}
	}
}

