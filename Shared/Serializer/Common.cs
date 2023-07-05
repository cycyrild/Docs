using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocsWASM.Shared.Serializer
{
	public class Common
	{
		public static void WriteNullableString(BinaryWriter writer, string value)
		{
			if (value == null)
			{
				writer.Write(false);
			}
			else
			{
				writer.Write(true);

				value = value.Replace("\0", "\\0");

				byte[] bytes = Encoding.UTF8.GetBytes(value);

				writer.Write(bytes.Length);
				writer.Write(bytes);
			}
		}

		public static string ReadNullableString(BinaryReader reader)
		{
			if (reader.ReadBoolean())
			{
				int length = reader.ReadInt32();

				byte[] bytes = reader.ReadBytes(length);

				string value = Encoding.UTF8.GetString(bytes);

				value = value.Replace("\\0", "\0");

				return value;
			}
			else
			{
				return null;
			}
		}

		public static void WriteNullableByteArray(BinaryWriter writer, byte[] value)
		{
			if (value != null)
			{
				writer.Write(true);
				writer.Write((long)value.Length);
				writer.Write(value);
			}
			else
			{
				writer.Write(false);
			}
		}

		public static byte[] ReadNullableByteArray(BinaryReader reader)
		{
			if (reader.ReadBoolean())
			{
				long length = reader.ReadInt64();
				return reader.ReadBytes((int)length);
			}
			else
			{
				return null;
			}
		}
	}
}
