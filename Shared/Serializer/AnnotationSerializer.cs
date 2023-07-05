using DocsWASM.Shared.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocsWASM.Shared.Serializer
{
	public class AnnotationSerializer
	{
		public static byte[] Serialize(Annotation annotation)
		{
			using (var ms = new MemoryStream())
			using (var writer = new BinaryWriter(ms))
			{
				Common.WriteNullableString(writer, annotation.Text);
				writer.Write(annotation.Point.X);
				writer.Write(annotation.Point.Y);
				writer.Write(annotation.PageId);
				writer.Write(annotation.Id);

				writer.Write(annotation.UserId);
				Common.WriteNullableString(writer, annotation.UserName);
				writer.Write(annotation.ModifiedDate.ToBinary());

				writer.Flush();
				return ms.ToArray();
			}
		}

		public static Annotation Deserialize(byte[] data)
		{
			using (var ms = new MemoryStream(data))
			using (var reader = new BinaryReader(ms))
			{
				var annotation = new Annotation();
				annotation.Text = Common.ReadNullableString(reader);
				annotation.Point = new Point(reader.ReadDouble(), reader.ReadDouble());
				annotation.PageId = reader.ReadUInt32();
				annotation.Id = reader.ReadUInt32();

				annotation.UserId = reader.ReadUInt32();
				annotation.UserName = Common.ReadNullableString(reader);
				annotation.ModifiedDate = DateTime.FromBinary(reader.ReadInt64());

				return annotation;
			}
		}

	}
}
