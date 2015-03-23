using System.Collections.Generic;
using System.IO;

namespace ZendIni
{
	public class ZendIniParser
	{
		private string filePath;

		public ZendIniParser ()
		{
		}

		public ZendIniParser (string filePath)
		{
			this.filePath = filePath;
		}

		public Dictionary<string, Dictionary<string, List<string>>> Parse ()
		{
			string configData = this.ReadFile ();

			return this.ParseString (configData);
		}

		public Dictionary<string, Dictionary<string, List<string>>> ParseString (string data)
		{
			var ConfigData = new Dictionary<string, Dictionary<string, List<string>>> ();

			string[] dataLines = data.Split ('\n');
			string currentKey = "";

			foreach (string current in dataLines) {
				string local = current.Trim ();
				if (local.Length == 0) {
					continue;
				}

				// skip comments
				if (local.Substring (0, 1) == ";") {
					continue;
				}

				// We have hit a top level key, create a new record to store it
				// in and a new dictionary to store it's children in.
				if (local.Substring (0, 1) == "[") {					
					currentKey = local.Substring (1, local.Length - 2);
					ConfigData.Add (currentKey, new Dictionary<string, List<string>> ());
					continue;
				}

				// bail if we got to data and aren't under a key.
				if (currentKey.Length == 0) {
					continue;
				}

				// this is an actual array of data so we need to do some cleaning before
				// can drop our values in
				if (local.Contains ("[]")) {
					string[] localValues = local.Split ('=');
					localValues [0] = localValues [0].Replace ("[]", "").Trim ();
					localValues [1] = localValues [1].Trim ();

					// since this is a real array we are likely to encounter multiple entries.
					// make sure that if we already have this key/val in our top level key
					// we add instead wiping it out.
					List<string> valueList;
					if (!ConfigData [currentKey].ContainsKey (localValues [0])) {
						// no key add.
						valueList = new List<string> ();
					} else {
						// we alrdya have this, get our current list so we can append.
						valueList = ConfigData [currentKey] [localValues [0]] as List<string>;
					}

					valueList.Add (localValues [1]);
					ConfigData [currentKey] [localValues [0]] = valueList;
					continue;
				}

				// should be a normal key/val
				KeyValuePair<string, string> localValue = this.ParseValue (local);
				ConfigData [currentKey].Add (localValue.Key, new List<string> ());
				ConfigData [currentKey] [localValue.Key].Add (localValue.Value);
			}

			return ConfigData;
		}

		private KeyValuePair<string, string> ParseValue (string inputData)
		{
			string[] inputArray = inputData.Trim ().Split ('=');

			if (inputArray.Length != 2) {
				throw new InvalidDataException ("Got a key/val pair with too many equals: " + inputData);
			}

			return new KeyValuePair<string, string> (inputArray [0].Trim (), inputArray [1].Trim ());
		}

		private string ReadFile ()
		{
			StreamReader reader = new StreamReader (this.filePath);
			return reader.ReadToEnd ();
		}
	}
}

