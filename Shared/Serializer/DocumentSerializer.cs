using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DocsWASM.Shared.DocumentModels;
using static DocsWASM.Shared.UploadModels;

namespace DocsWASM.Shared.Serializer
{
	public class DocumentSerializer
	{
		public static byte[] Serialize(Document document)
		{
			using (var ms = new MemoryStream())
			using (var writer = new BinaryWriter(ms))
			{
				// Serialize DocumentHeader using DocumentHeaderSerializer
				var headerBytes = DocumentHeaderSerializer.Serialize(document.DocumentHeader);
				writer.Write(headerBytes.Length);
				writer.Write(headerBytes);

				writer.Write(document.Page.Count);
				foreach (var page in document.Page)
				{
					writer.Write(page.Id);
					writer.Write(page.PageNo);
					writer.Write(page.DocumentId);
					Common.WriteNullableString(writer, page.Paragraphs);
					Common.WriteNullableString(writer, page.Name);
					Common.WriteNullableString(writer, page.YearGroup);
					Common.WriteNullableString(writer, page.School);
					writer.Write(page.ChapterId);
					writer.Write(page.DocType);
					writer.Write((byte)page.DocBinType);
					writer.Write(page.SubjectType);
					writer.Write(page.IsCorrection);
					Common.WriteNullableByteArray(writer, page.Bin);
					Common.WriteNullableByteArray(writer, page.PlaceHolder);
				}
				writer.Flush();
				return ms.ToArray();
			}
		}

		public static Document Deserialize(byte[] data)
		{
			using (var ms = new MemoryStream(data))
			using (var reader = new BinaryReader(ms))
			{
				var document = new Document();

				// Deserialize DocumentHeader using DocumentHeaderSerializer
				int headerLength = reader.ReadInt32();
				var headerBytes = reader.ReadBytes(headerLength);
				document.DocumentHeader = DocumentHeaderSerializer.Deserialize(headerBytes);

				var pages = new List<Page>();
				var pagesCount = reader.ReadInt32();
				for (var i = 0; i < pagesCount; i++)
				{
					var page = new Page();

					page.Id = reader.ReadUInt32();
					page.PageNo = reader.ReadUInt32();
					page.DocumentId = reader.ReadUInt32();
					page.Paragraphs = Common.ReadNullableString(reader);
					page.Name = Common.ReadNullableString(reader);
					page.YearGroup = Common.ReadNullableString(reader);
					page.School = Common.ReadNullableString(reader);
					page.ChapterId = reader.ReadUInt32();
					page.DocType = reader.ReadByte();
					page.DocBinType = (dataBinTypesEnum)reader.ReadByte();
					page.SubjectType = reader.ReadUInt32();
					page.IsCorrection = reader.ReadBoolean();
					page.Bin = Common.ReadNullableByteArray(reader);
					page.PlaceHolder = Common.ReadNullableByteArray(reader);

					pages.Add(page);
				}
				document.Page = pages;

				return document;
			}
		}

	}
}
