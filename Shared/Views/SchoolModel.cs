using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocsWASM.Shared.Views
{
	public class SchoolModel
	{
		public string Name { get; set; }
		public string? Description { get; set; }
		public string? SvgLogo { get; set; }
		public ulong DocumentCount { get; set; }
		public List<PathModels> Paths { get; set; }
	}
}
