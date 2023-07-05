using DocsWASM.Shared.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocsWASM.Shared.Serializer
{
	public class AnnotationListSerializer
	{
		public static byte[] Serialize(List<Annotation> annotations)
		{
			using (var ms = new MemoryStream())
			using (var writer = new BinaryWriter(ms))
			{
				writer.Write(annotations.Count);
				foreach (var annotation in annotations)
				{
					var annotationBytes = AnnotationSerializer.Serialize(annotation);
					writer.Write(annotationBytes.Length);
					writer.Write(annotationBytes);
				}

				writer.Flush();
				return ms.ToArray();
			}
		}

		public static List<Annotation> Deserialize(byte[] data)
		{
			using (var ms = new MemoryStream(data))
			using (var reader = new BinaryReader(ms))
			{
				var annotations = new List<Annotation>();
				var annotationCount = reader.ReadInt32();

				for (var i = 0; i < annotationCount; i++)
				{
					var annotationLength = reader.ReadInt32();
					var annotationBytes = reader.ReadBytes(annotationLength);
					var annotation = AnnotationSerializer.Deserialize(annotationBytes);
					annotations.Add(annotation);
				}

				return annotations;
			}
		}

	}
}
