using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocsWASM.Shared.Views
{
    public class ChapterModel
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public string Subject { get; set; }
        public string? Description { get; set; }
        public ulong Count { get; set; }
		public List<PathModel> Paths { get; set; }
	}
}
