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
			public Dictionary<uint, List<string>> pageMatchs { get; set; }
			public DocumentHeader documentHeader { get; set;}
		}
	}
}
