using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DocsWASM.Shared.SearchModels.SearchResult;
using static DocsWASM.Shared.SearchModels;
using static DocsWASM.Shared.Serializer.Common;
namespace DocsWASM.Shared.Serializer
{
	public class SearchResultListSerializer
	{
		/*public static byte[] Serialize(List<SearchResult> searchResults)
		{
			using (MemoryStream ms = new MemoryStream())
			using (BinaryWriter writer = new BinaryWriter(ms))
			{
				// Write the count of search results first
				writer.Write(searchResults.Count);

				// Then serialize each SearchResult
				foreach (var searchResult in searchResults)
				{
					// Serialize DocumentHeader
					var headerBytes = DocumentHeaderSerializer.Serialize(searchResult.documentHeader);
					writer.Write(headerBytes.Length);
					writer.Write(headerBytes);

					// Serialize List<PageMatch>
					if (searchResult.pageMatchs == null)
					{
						writer.Write(-1);
					}
					else
					{
						writer.Write(searchResult.pageMatchs.Count);
						foreach (var match in searchResult.pageMatchs)
						{
							WriteNullableString(writer, match.match);
							writer.Write(match.page);
						}
					}
				}

				return ms.ToArray();
			}
		}

		public static List<SearchResult> Deserialize(byte[] data)
		{
			using (MemoryStream ms = new MemoryStream(data))
			using (BinaryReader reader = new BinaryReader(ms))
			{
				var searchResults = new List<SearchResult>();

				// Read the count of search results first
				int count = reader.ReadInt32();

				// Then deserialize each SearchResult
				for (int i = 0; i < count; i++)
				{
					var searchResult = new SearchResult();

					// Deserialize DocumentHeader
					int headerLength = reader.ReadInt32();
					var headerBytes = reader.ReadBytes(headerLength);
					searchResult.documentHeader = DocumentHeaderSerializer.Deserialize(headerBytes);

					// Deserialize List<PageMatch>
					int matchesCount = reader.ReadInt32();
					if (matchesCount == -1)
					{
						searchResult.pageMatchs = null;
					}
					else
					{
						var matches = new List<PageMatch>();
						for (int j = 0; j < matchesCount; j++)
						{
							var match = new PageMatch
							{
								match = ReadNullableString(reader),
								page = reader.ReadUInt32()
							};
							matches.Add(match);
						}
						searchResult.pageMatchs = matches;
					}

					searchResults.Add(searchResult);
				}

				return searchResults;
			}
		}*/
		public static byte[] Serialize(List<SearchResult> searchResults)
		{
			using (MemoryStream ms = new MemoryStream())
			{
				using (BinaryWriter writer = new BinaryWriter(ms))
				{
					writer.Write(searchResults.Count);

					foreach (var result in searchResults)
					{
						// Check if pageMatchs is null before trying to use it
						if (result.pageMatchs != null)
						{
							writer.Write(result.pageMatchs.Count);
							foreach (var pair in result.pageMatchs)
							{
								writer.Write(pair.Key);
								writer.Write(pair.Value.Count);
								foreach (var s in pair.Value)
								{
									Common.WriteNullableString(writer, s);
								}
							}
						}
						else
						{
							writer.Write(0); // Write 0 count for null pageMatchs
						}

						byte[] headerData = DocumentHeaderSerializer.Serialize(result.documentHeader); // serialize DocumentHeader to byte array here
						Common.WriteNullableByteArray(writer, headerData);
					}
				}

				return ms.ToArray();
			}
		}

		public static List<SearchResult> Deserialize(byte[] data)
		{
			using (MemoryStream ms = new MemoryStream(data))
			{
				using (BinaryReader reader = new BinaryReader(ms))
				{
					int count = reader.ReadInt32();

					List<SearchResult> searchResults = new List<SearchResult>(count);

					for (int i = 0; i < count; i++)
					{
						SearchResult result = new SearchResult();

						int pageMatchCount = reader.ReadInt32();
						result.pageMatchs = new Dictionary<uint, List<string>>(pageMatchCount);
						for (int j = 0; j < pageMatchCount; j++)
						{
							uint key = reader.ReadUInt32();
							int listCount = reader.ReadInt32();
							var list = new List<string>(listCount);
							for (int k = 0; k < listCount; k++)
							{
								list.Add(Common.ReadNullableString(reader));
							}
							result.pageMatchs.Add(key, list);
						}

						byte[] headerData = Common.ReadNullableByteArray(reader);
						result.documentHeader = DocumentHeaderSerializer.Deserialize(headerData); // deserialize DocumentHeader from byte array here

						searchResults.Add(result);
					}

					return searchResults;
				}
			}
		}
	}
}
