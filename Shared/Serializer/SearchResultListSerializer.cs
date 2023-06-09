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
		public static byte[] Serialize(List<SearchResult> searchResults)
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
		}
	}
}
