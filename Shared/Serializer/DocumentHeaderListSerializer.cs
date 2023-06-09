using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DocsWASM.Shared.DocumentModels;
using static DocsWASM.Shared.Serializer.Common;
namespace DocsWASM.Shared.Serializer
{
	public static class DocumentHeaderListSerializer
	{

		public static byte[] Serialize(List<DocumentHeader> headers)
		{
			using (MemoryStream ms = new MemoryStream())
			using (BinaryWriter writer = new BinaryWriter(ms))
			{
				// Write the count of DocumentHeaders first
				writer.Write(headers.Count);

				// Then serialize each DocumentHeader
				foreach (var header in headers)
				{
					var headerBytes = DocumentHeaderSerializer.Serialize(header);
					writer.Write(headerBytes.Length);
					writer.Write(headerBytes);
				}

				return ms.ToArray();
			}
		}

		public static List<DocumentHeader> Deserialize(byte[] data)
		{
			using (MemoryStream ms = new MemoryStream(data))
			using (BinaryReader reader = new BinaryReader(ms))
			{
				var headers = new List<DocumentHeader>();

				// Read the count of DocumentHeaders first
				int count = reader.ReadInt32();

				// Then deserialize each DocumentHeader
				for (int i = 0; i < count; i++)
				{
					int headerLength = reader.ReadInt32();
					var headerBytes = reader.ReadBytes(headerLength);
					var header = DocumentHeaderSerializer.Deserialize(headerBytes);
					headers.Add(header);
				}

				return headers;
			}
		}
	}
}


