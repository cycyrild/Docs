using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DocsWASM.Shared.UploadModels;
using static DocsWASM.Shared.Serializer.Common;

namespace DocsWASM.Shared.Serializer
{
    public static class UploadSendModelSerializer
    {
		public static byte[] Serialize(UploadSendModel model)
		{
			using (MemoryStream ms = new MemoryStream())
			using (BinaryWriter writer = new BinaryWriter(ms))
			{
				writer.Write(model.Upload.OwnerUserId);
				Common.WriteNullableString(writer, model.Upload.Name);
				Common.WriteNullableString(writer, model.Upload.Description);
				writer.Write(model.Upload.DocumentTypeId.HasValue);

				if (model.Upload.DocumentTypeId.HasValue)
					writer.Write(model.Upload.DocumentTypeId.Value);

				Common.WriteNullableString(writer, model.Upload.YearGroupName);
				Common.WriteNullableString(writer, model.Upload.SchoolName);
				writer.Write(model.Upload.ChapterId.HasValue);
				if (model.Upload.ChapterId.HasValue) writer.Write(model.Upload.ChapterId.Value);
				writer.Write(model.Upload.SubjectId.HasValue);
				if (model.Upload.SubjectId.HasValue) writer.Write(model.Upload.SubjectId.Value);
				writer.Write(model.Upload.FilePresent);

				writer.Write(model.Pages.Count);
				foreach (var page in model.Pages)
				{
					Common.WriteNullableByteArray(writer, page.bin);
					writer.Write(page.orginalDocumentPage);
					writer.Write((byte)page.fileType);
					Common.WriteNullableString(writer, page.fileName);
					writer.Write(page.isCorrection);
					Common.WriteNullableString(writer, page.paragraphsString);
				}

				writer.Flush();
				return ms.ToArray();
			}
		}

		public static UploadSendModel Deserialize(byte[] data)
		{
			using (MemoryStream ms = new MemoryStream(data))
			using (BinaryReader reader = new BinaryReader(ms))
			{
				var upload = new UploadModel
				{
					OwnerUserId = reader.ReadUInt32(),
					Name = Common.ReadNullableString(reader),
					Description = Common.ReadNullableString(reader),
					DocumentTypeId = reader.ReadBoolean() ? reader.ReadUInt32() : (uint?)null,
					YearGroupName = Common.ReadNullableString(reader),
					SchoolName = Common.ReadNullableString(reader),
					ChapterId = reader.ReadBoolean() ? reader.ReadUInt32() : (uint?)null,
					SubjectId = reader.ReadBoolean() ? reader.ReadUInt32() : (uint?)null,
					FilePresent = reader.ReadBoolean()
				};

				var pagesCount = reader.ReadInt32();
				var pages = new List<PageModel>(pagesCount);

				for (int i = 0; i < pagesCount; i++)
				{
					var bin = Common.ReadNullableByteArray(reader);

					var orginalDocumentPage = reader.ReadInt32();
					var fileType = (dataBinTypesEnum)reader.ReadByte();
					var fileName = Common.ReadNullableString(reader);
					var isCorrection = reader.ReadBoolean();
					var paragraphsString = Common.ReadNullableString(reader);
					var page = new PageModel(orginalDocumentPage, bin, fileName, fileType, isCorrection, paragraphsString);
					pages.Add(page);
				}

				return new UploadSendModel { Upload = upload, Pages = pages };
			}
		}

	}
}
