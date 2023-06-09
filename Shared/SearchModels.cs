using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static DocsWASM.Shared.DocumentModels;

namespace DocsWASM.Shared
{
	public class SearchModels
	{

		public class SearchResult
		{
			public class PageMatch
			{
				public string match { get; set; }
				public uint page { get; set; }
			}
			public List<PageMatch> pageMatchs { get; set; }
			public DocumentHeader documentHeader { get; set;}
		}
	}
}
