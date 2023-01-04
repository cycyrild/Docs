using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocsWASM.Shared;
using static DocsWASM.Shared.DocumentModele;

namespace DocsWASM.Shared.Views
{
	public class DocumentTypeModel
	{
		public Byte Id { get; set; }
		public string Name { get; set; }
		public ulong Count { get; set; }
	}
}