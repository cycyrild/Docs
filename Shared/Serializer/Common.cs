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

				// escape control characters
				value = value.Replace("\0", "\\0");

				// convert string to bytes using a specific encoding
				byte[] bytes = Encoding.UTF8.GetBytes(value);

				// write length of the string first, then the bytes
				writer.Write(bytes.Length);
				writer.Write(bytes);
			}
		}

		public static string ReadNullableString(BinaryReader reader)
		{
			if (reader.ReadBoolean())
			{
				// read length of the string first
				int length = reader.ReadInt32();

				// read bytes
				byte[] bytes = reader.ReadBytes(length);

				// convert bytes back to string using the same encoding
				string value = Encoding.UTF8.GetString(bytes);

				// unescape control characters
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
