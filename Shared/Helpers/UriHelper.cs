using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DocsWASM.Shared.Helpers
{
	public class UriHelper
	{
		public static Uri AddParameter(Uri url, string paramName, string paramValue)
		{
			var uriBuilder = new UriBuilder(url);
			var query = HttpUtility.ParseQueryString(uriBuilder.Query);
			query[paramName] = paramValue;
			uriBuilder.Query = query.ToString();

			return uriBuilder.Uri;
		}

		public static string ToQueryString(NameValueCollection nvc)
		{
			if (nvc == null) return string.Empty;

			StringBuilder sb = new StringBuilder();

			foreach (string key in nvc.Keys)
			{
				if (string.IsNullOrWhiteSpace(key)) continue;

				string[] values = nvc.GetValues(key);
				if (values == null) continue;

				foreach (string value in values)
				{
					sb.Append(sb.Length == 0 ? "?" : "&");
					sb.AppendFormat("{0}={1}", Uri.EscapeDataString(key), Uri.EscapeDataString(value));
				}
			}

			return sb.ToString();
		}
	}
}
