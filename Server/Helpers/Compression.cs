using System.IO.Compression;

namespace DocsWASM.Server.Helpers
{
	public class Compression
	{
		public static async Task<byte[]> CompressBytesAsync(byte[] bytes, CancellationToken cancel = default(CancellationToken))
		{
			using (var outputStream = new MemoryStream())
			{
				using (var compressionStream = new BrotliStream(outputStream, CompressionLevel.Optimal))
				{
					await compressionStream.WriteAsync(bytes, 0, bytes.Length, cancel);
				}
				return outputStream.ToArray();
			}
		}
	}
}


