using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocsWASM.Shared
{
	public class PathModels
	{

		public class Path
		{
			public string? title { get; set; }
			public paths path { get; set; }
			public Path(string title, paths path)
			{
				this.title= title;
				this.path = path;
			}
		}
		public enum paths { schools, yearGroups, subjects, chapters, docTypes, documents, document };
	}
}
