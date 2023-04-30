using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DocsWASM.Shared.AccountModels;

namespace DocsWASM.Shared.Views
{
	public class MembersModel
	{
		public ulong? id { get; set; }
		public string? email { get; set; }
		public string? username { get; set; }
		public DateTime? createdDate { get; set; }
		public DateTime? modifiedDate { get; set; }
		public string? createdIp { get; set; }
		public string? lastIp { get; set; }
		public DateTime? lastLogin { get; set; }
		public string? bio { get; set; }
		public string? firstName { get; set; }
		public string? lastName { get; set; }
		public bool? fullNamePrivacy { get; set; }
		public byte? userType { get; set; }
		public uint? uploadedDocuments { get; set; }
	}

	public class RankingModele
	{
		public MembersModel Member { get; set; }
		public int UploadCount { get; set;}
	}

	public class RankingsModele
	{
		public List<RankingModele> Rankings { get; set; }
	}
}
