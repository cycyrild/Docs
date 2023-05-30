using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;

namespace XMLHelper
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
	public class XMLHelper
	{
        public static string ExtractTextStringsFromSvg(string svgString)
        {
            // Pattern pour extraire le contenu des balises HTML contenant du texte
            string pattern = @"<[^>]*>([^<]*)<\/[^>]*>";

            // Pattern pour ignorer les balises <style>
            string ignorePattern = @"<style\b[^<]*(?:(?!<\/style>)<[^<]*)*<\/style>";

            // Appliquer le pattern pour ignorer les balises <style>
            svgString = Regex.Replace(svgString, ignorePattern, "",  RegexOptions.Singleline);

            // Liste pour stocker les chaînes de caractères extraites
            var textStrings = new List<string>();

            // Recherche des correspondances dans la chaîne SVG
            var matches = Regex.Matches(svgString, pattern, RegexOptions.Singleline);

            // Parcours des correspondances et extraction des chaînes de caractères
            foreach (Match match in matches)
            {
                // Extraction du contenu des balises HTML
                string htmlContent = match.Groups[1].Value;

                // Décodage de l'échappement HTML
                string decodedContent = HttpUtility.HtmlDecode(htmlContent);

                // Ajout de la chaîne de caractères à la liste
                textStrings.Add(decodedContent);
            }

            // Retourne le tableau des chaînes de caractères extraites
            return String.Join(" ", textStrings);
        }

        public static string Clean(string svg)
		{
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(svg);

            ValidateNoJavascript(doc.DocumentElement);

            RemoveComments(doc);

            // Remove XML declaration
            if (doc.FirstChild.NodeType == XmlNodeType.XmlDeclaration)
                doc.RemoveChild(doc.FirstChild);

            // Remove DOCTYPE (if present)
            if (doc.FirstChild.NodeType == XmlNodeType.DocumentType)
                doc.RemoveChild(doc.FirstChild);

            var settings = new XmlWriterSettings
            {
                Indent = false,
                NewLineHandling = NewLineHandling.None
            };

            using (var stringWriter = new StringWriter())
            using (var xmlTextWriter = XmlWriter.Create(stringWriter, settings))
            {
                doc.Save(xmlTextWriter);
                return stringWriter.GetStringBuilder().ToString();
            }
        }

        static void ValidateNoJavascript(XmlNode node)
        {
            if (node.NodeType == XmlNodeType.Element)
            {
                if (node.Name.ToLower() == "script")
                    throw new Exception("Le document SVG contient du JavaScript.");

                foreach (XmlAttribute attr in node.Attributes)
                    if (attr.Name.StartsWith("on"))
                        throw new Exception("Le document SVG contient des événements JavaScript.");

                foreach (XmlNode child in node.ChildNodes)
                    ValidateNoJavascript(child);
            }
        }

        static void RemoveComments(XmlNode node)
        {
            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.NodeType == XmlNodeType.Comment)
                    node.RemoveChild(child);
                else
                    RemoveComments(child);
            }
        }


        static string RotateSvg(string svgContent)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(svgContent);

            XmlNode svgNode = doc.DocumentElement;
            if (svgNode.Name != "svg")
            {
                throw new Exception("Le document XML n'est pas un document SVG valide.");
            }

            // Assuming the width and height attributes are specified and are integers.
            int width = int.Parse(svgNode.Attributes["width"].Value);
            int height = int.Parse(svgNode.Attributes["height"].Value);

            // Determine the existing rotation, if any.
            int existingRotation = 0;
            if (svgNode.Attributes["transform"] != null)
            {
                var match = Regex.Match(svgNode.Attributes["transform"].Value, @"rotate\((\d+)");
                if (match.Success)
                {
                    existingRotation = int.Parse(match.Groups[1].Value);
                }
            }

            // Calculate the new rotation.
            int newRotation = (existingRotation + 90) % 360;

            // Create a transformation that rotates around the center of the SVG.
            string transform = $"rotate({newRotation}, {width / 2}, {height / 2})";

            // Set the transformation attribute.
            if (svgNode.Attributes["transform"] == null)
            {
                XmlAttribute transformAttr = doc.CreateAttribute("transform");
                transformAttr.Value = transform;
                svgNode.Attributes.Append(transformAttr);
            }
            else
            {
                svgNode.Attributes["transform"].Value = transform;
            }

            return doc.OuterXml;
        }
    }
}