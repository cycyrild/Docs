using System.Text.RegularExpressions;
using System.Text;
using System.Globalization;

namespace DocsWASM.Shared.Helpers
{
	public class Text
	{
		public static string CleanString(string value)
		{
			var stringBuilder = new StringBuilder();

			// Remove control, surrogate, private use, and non-assigned characters
			foreach (var c in value)
			{
				UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
				if (!char.IsControl(c) && !char.IsSurrogate(c) &&
					unicodeCategory != UnicodeCategory.PrivateUse &&
					unicodeCategory != UnicodeCategory.OtherNotAssigned)
				{
					stringBuilder.Append(c);
				}
			}

			// Replace multiple spaces with a single space
			string singleSpaced = Regex.Replace(stringBuilder.ToString(), @"\s+", " ");

			// Trim the string
			return singleSpaced.Trim();
		}
	}
}
