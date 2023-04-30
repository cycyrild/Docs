using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocsWASM.Shared.Views
{
	public class SubjectModel
	{
		public uint Id { get; set; }
		public string Name { get; set; }
		public string? SvgLogo { get; set; }
        public ulong Count { get; set; }
		public List<PathModels> Paths { get; set; }
	}
}
