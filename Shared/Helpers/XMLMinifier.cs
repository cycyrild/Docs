using System.Text;
using System.Text.RegularExpressions;

namespace XMLMinifier
{
	/*/// <summary>
	/// Config object for the XML minifier.
	/// </summary>
	public class XMLMinifierSettings
	{
		public bool RemoveEmptyLines { get; set; }
		public bool RemoveWhitespaceBetweenElements { get; set; }
		public bool CloseEmptyTags { get; set; }
		public bool RemoveComments { get; set; }
		public bool TrimDecimals { get; set; }

		public static XMLMinifierSettings Aggressive
		{
			get
			{
				return new XMLMinifierSettings
				{
					RemoveEmptyLines = true,
					RemoveWhitespaceBetweenElements = true,
					CloseEmptyTags = true,
					RemoveComments = true,
					TrimDecimals = true
				};
			}
		}

		public static XMLMinifierSettings NoMinification
		{
			get
			{
				return new XMLMinifierSettings
				{
					RemoveEmptyLines = false,
					RemoveWhitespaceBetweenElements = false,
					CloseEmptyTags = false,
					RemoveComments = false,
					TrimDecimals = false
				};
			}
		}
	}

	/// <summary>
	/// XML minifier. No Regex allowed! ;-)
	/// 
	/// Example)
	///     var sampleXml = File.ReadAllText("somefile.xml");
	///     var minifiedXml = new XMLMinifier(XMLMinifierSettings.Aggressive).Minify(sampleXml);
	/// </summary>
	public class XMLMinifier
	{
		private XMLMinifierSettings _minifierSettings;

		public XMLMinifier(XMLMinifierSettings minifierSettings)
		{
			_minifierSettings = minifierSettings;
		}

		public byte[] Minify(byte[] data)
		{
			var txt = Encoding.UTF8.GetString(data);
			return Encoding.UTF8.GetBytes(Minify(txt));
		}

		public string Minify(string xml)
		{
			if (_minifierSettings.TrimDecimals == true)
				xml = Regex.Replace(xml, @"(?<=\.\d\d)\d*", "", RegexOptions.Compiled);


			var originalXmlDocument = new XmlDocument();
			originalXmlDocument.PreserveWhitespace = !(_minifierSettings.RemoveWhitespaceBetweenElements || _minifierSettings.RemoveEmptyLines);
			originalXmlDocument.Load(new MemoryStream(Encoding.UTF8.GetBytes(xml)));

			//remove comments first so we have less to compress later
			if (_minifierSettings.RemoveComments)
			{
				foreach (XmlNode comment in originalXmlDocument.SelectNodes("//comment()"))
				{
					comment.ParentNode.RemoveChild(comment);
				}
			}

			if (_minifierSettings.CloseEmptyTags)
			{
				foreach (XmlElement el in originalXmlDocument.SelectNodes("descendant::*[not(*) and not(normalize-space())]"))
				{
					el.IsEmpty = true;
				}
			}

			if (_minifierSettings.RemoveWhitespaceBetweenElements)
			{
				return originalXmlDocument.InnerXml;
			}
			else
			{
				var minified = new MemoryStream();
				originalXmlDocument.Save(minified);

				return Encoding.UTF8.GetString(minified.ToArray());
			}
		}
	}*/
	public class XMLMinifier
	{
		public static string Clean(string svg)
		{
			svg = Regex.Replace(svg, @"<!DOCTYPE[^>[]*(\[[^]]*\])?>", "", RegexOptions.Compiled);
			svg = Regex.Replace(svg, @"<\?xml.*\?>", "", RegexOptions.Compiled);
			svg = Regex.Replace(svg, @"(?<=\.\d\d)\d*", "", RegexOptions.Compiled);
			svg = Regex.Replace(svg, @"\n", "", RegexOptions.Compiled);
			return svg;
		}
	}
}