/*using Newtonsoft.Json;
using Newtonsoft.Json.Bson;*/
using static DocsWASM.Shared.UploadModels;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.IO;
using System.IO;
using MongoDB.Bson;
using System.IO.Compression;
using System.Threading.Channels;

namespace DocsWASM.Shared.Helpers
{
	public class Bson
	{
		public static T FromBson<T>(byte[] bytes)
		{

			return BsonSerializer.Deserialize<T>(bytes);
		}

		public static byte[] ToBson<T>(T obj)
		{
			return BsonExtensionMethods.ToBson<T>(obj);
		}

	}
}
