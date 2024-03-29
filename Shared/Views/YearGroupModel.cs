﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocsWASM.Shared.Views
{
	public class YearGroupModel
	{
		public string School { get; set; }
		public string Name { get; set; }
		public string? Description { get; set; }
		public List<PathModels> Paths { get; set; }
	}
}
