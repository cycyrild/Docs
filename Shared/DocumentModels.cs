using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DocsWASM.Shared.UploadModels;

namespace DocsWASM.Shared
{
	public class DocumentModels
	{
		public static Dictionary<dataBinTypesEnum, string> dataBinTypesMime = new Dictionary<dataBinTypesEnum, string>()
		{
			{dataBinTypesEnum.svg, "image/svg+xml" },
			{dataBinTypesEnum.webp, "image/webp" }
		};

		public class Document
		{
			public DocumentHeader DocumentHeader { get; set; }
			public List<Page> Page { get; set;}
		}

		public class DocumentHeader
		{
			public uint DocumentId { get; set; }
			public string DocumentName { get; set; }
			public string? Description { get; set; }
			public uint SubjectType { get; set; }
			public string SubjectTypeName { get; set; }
			public uint OwnerUserId { get; set; }
			public string OwnerUserName { get; set; }
			public byte[] ImgPreview { get; set; }
			public Byte DocType { get; set; }
			public string DocTypeName { get; set; }
			public string YearGroup { get; set; }
			public string SchoolName { get; set; }
			public uint ChapterId { get; set; }
			public string ChapterName { get; set; }
			public DateTime CreatedDate { get; set; }
			public IEnumerable<uint> Pages { get; set; }
			public Byte Approved { get; set; }
		}

		public class Page
		{
			public uint Id { get; set; }
			public uint PageNo { get; set; }
			public uint DocumentId { get; set; }
			public string Paragraphs { get; set; }
			public string Name { get; set; }
			public string YearGroup { get; set; }
			public string School { get; set; }
			public uint ChapterId { get; set; }
			public Byte DocType { get; set; }
			public dataBinTypesEnum DocBinType { get; set; }
			public uint SubjectType { get; set; }
			public bool IsCorrection { get; set; }
			public byte[] Bin { get; set; }
			public byte[] PlaceHolder { get; set; }
		}

		public class PreviewDocumentHeaders
		{
			public List<DocumentHeader> Headers { get; set; }
		}
	}


}
