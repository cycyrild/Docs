using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DocsWASM.Shared.DocumentModels;
using static DocsWASM.Shared.Serializer.Common;

namespace DocsWASM.Shared.Serializer
{
	public class DocumentHeaderSerializer
	{
		public static byte[] Serialize(DocumentHeader header)
		{
			using (MemoryStream ms = new MemoryStream())
			using (BinaryWriter writer = new BinaryWriter(ms))
			{

				writer.Write(header.DocumentId);
				WriteNullableString(writer, header.DocumentName);
				WriteNullableString(writer, header.Description);
				writer.Write(header.SubjectType);
				WriteNullableString(writer, header.SubjectTypeName);
				writer.Write(header.OwnerUserId);
				WriteNullableString(writer, header.OwnerUserName);

				// Handle byte array
				if (header.ImgPreview == null)
				{
					writer.Write(-1);
				}
				else
				{
					writer.Write(header.ImgPreview.Length);
					writer.Write(header.ImgPreview);
				}

				writer.Write(header.DocType);
				WriteNullableString(writer, header.DocTypeName);
				WriteNullableString(writer, header.YearGroup);
				WriteNullableString(writer, header.SchoolName);
				writer.Write(header.ChapterId);
				WriteNullableString(writer, header.ChapterName);
				writer.Write(header.CreatedDate.ToBinary());

				// Handle IEnumerable<uint> Pages
				if (header.Pages == null)
				{
					writer.Write(-1);
				}
				else
				{
					writer.Write(header.Pages.Count());
					foreach (var page in header.Pages)
					{
						writer.Write(page);
					}
				}

				writer.Write(header.Approved);

				return ms.ToArray();
			}
		}

		public static DocumentHeader Deserialize(byte[] data)
		{
			using (MemoryStream ms = new MemoryStream(data))
			using (BinaryReader reader = new BinaryReader(ms))
			{


				var header = new DocumentHeader
				{
					DocumentId = reader.ReadUInt32(),
					DocumentName = ReadNullableString(reader),
					Description = ReadNullableString(reader),
					SubjectType = reader.ReadUInt32(),
					SubjectTypeName = ReadNullableString(reader),
					OwnerUserId = reader.ReadUInt32(),
					OwnerUserName = ReadNullableString(reader),
				};
				// Handle byte array
				int imgPreviewLength = reader.ReadInt32();
				if (imgPreviewLength == -1)
				{
					header.ImgPreview = null;
				}
				else
				{
					header.ImgPreview = reader.ReadBytes(imgPreviewLength);
				}

				header.DocType = reader.ReadByte();
				header.DocTypeName = ReadNullableString(reader);
				header.YearGroup = ReadNullableString(reader);
				header.SchoolName = ReadNullableString(reader);
				header.ChapterId = reader.ReadUInt32();
				header.ChapterName = ReadNullableString(reader);
				header.CreatedDate = DateTime.FromBinary(reader.ReadInt64());

				// Handle IEnumerable<uint> Pages
				int pagesCount = reader.ReadInt32();
				if (pagesCount == -1)
				{
					header.Pages = null;
				}
				else
				{
					var pages = new List<uint>();
					for (int j = 0; j < pagesCount; j++)
					{
						pages.Add(reader.ReadUInt32());
					}
					header.Pages = pages;
				}

				header.Approved = reader.ReadByte();

				return header;
			}
		}
	}
}

