using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DocsWASM.Shared.UploadModels;

namespace DocsWASM.Shared
{
	public class SubjectsChapters
	{
		public class chapter
		{
			public uint id { get; set; }
			public string name { get; set; }
		}
		public Dictionary<uint, string> subjects { get; set; }
		public Dictionary<uint, List<chapter>> chapters { get; set; }

	}
	public class UploadModels
	{
		public enum dataBinTypesEnum { pdf = 0, png = 1, jpg = 2, svg = 3, webp = 4 };

		public static Dictionary<string, byte> dataBinTypes = new Dictionary<string, byte>()
		{
			//{ ".svg", (byte)dataBinTypesEnum.svg},
			{ ".pdf", (byte)dataBinTypesEnum.pdf},
            { ".png" , (byte)dataBinTypesEnum.png},
            { ".jpg", (byte)dataBinTypesEnum.jpg },
			{ ".jpeg", (byte)dataBinTypesEnum.jpg },
			//{ ".webp", (byte)dataBinTypesEnum.webp }
		};

		public class UploadSendModel
		{
			public UploadModel Upload { get; set; }
			public List<PageModel> Pages{ get; set; }
		}

		public class UploadStatus
		{
			public bool success { get; set; }
			public uint? documenId { get; set; }
			public string? errorMessage { get; set; }
		}

		public class UploadModel
		{
			public uint OwnerUserId { get; set; }

			[Required(ErrorMessage = "Name is required.")]
			[StringLength(60, ErrorMessage = "Must be less than 60 characters.")]
			public string Name { get; set; }

			[StringLength(200, ErrorMessage = "Must be less than 200 characters.")]
			public string Description { get; set; }

			[Required(ErrorMessage = "Document type is required.")]
			public uint? DocumentTypeId { get; set; }

			[Required(ErrorMessage = "Year group is required.")]
			public string YearGroupName { get; set; }

			[Required(ErrorMessage = "School name group is required.")]
			public string SchoolName { get; set; }

			[Required(ErrorMessage = "Chapter id is required.")]
			public uint? ChapterId { get; set; }

			[Required(ErrorMessage = "Subject id is required.")]
			public uint? SubjectId { get; set; }

			[Range(typeof(bool), "true", "true", ErrorMessage = "You must select at least one file")]
			public bool FilePresent { get; set; }

		}

		public class PageModel
		{
			public int orginalDocumentPage { get; set; }
			public byte[] bin { get; set; }
			public dataBinTypesEnum fileType { get; set; }
			public string fileName { get; set; }
			public bool isCorrection { get; set; }
			public string paragraphsString { get; set; }
			public PageModel(int orginalDocumentPage,  byte[] bin, string fileName, dataBinTypesEnum fileType, bool isCorrection, string paragraphsString)
			{
				this.orginalDocumentPage = orginalDocumentPage;
				this.bin = bin;
				this.fileName = fileName;
				this.fileType = fileType;
				this.isCorrection = isCorrection;
				this.paragraphsString = paragraphsString;
			}
		}
	}

}
