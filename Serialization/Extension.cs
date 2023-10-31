using System.IO.Compression;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.Json;

namespace Serialization;

public static class Extension
{
    private readonly static Random _random = new();

    public static T FindRandom<T>(this IEnumerable<T> collection)
    {
        return collection.ElementAt(_random.Next(collection.Count()));
    }

    // Extension Methods for DataContract.
    public static string ToJsonDataContract<T>(this T obj)
    {
        var serializer = new DataContractJsonSerializer(typeof(T));
        using (var memoryStream = new MemoryStream())
        {

            serializer.WriteObject(memoryStream, obj);
            memoryStream.Position = 0;
            StreamReader stream = new StreamReader(memoryStream);

            return stream.ReadToEnd();
        }
    }

    public static T FromJsonDataContract<T>(this string json)
    {
        var deserializer = new DataContractJsonSerializer(typeof(T));
        using (var memorystream = new MemoryStream(Encoding.Unicode.GetBytes(json)))
        {
            var greetingList = deserializer.ReadObject(memorystream);
            return (T)greetingList;
        }
    }

    public static string ToCompressedJsonContract<T>(this T obj)
    {
        var serializer = new DataContractJsonSerializer(typeof(T));
        using (var memoryStream = new MemoryStream())
        {
            using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress))
            {
                serializer.WriteObject(gZipStream, obj);
            }
            return Convert.ToBase64String(memoryStream.ToArray());
        }

    }

    public static T ToDecompressedJsonContract<T>(this string json)
    {
        var deserializer = new DataContractJsonSerializer(typeof(T));

        using (var memoryStream = new MemoryStream(Convert.FromBase64String(json)))
        {
            using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
            {
                var greetingList = deserializer.ReadObject(gZipStream);
                return (T)greetingList;
            }

        }
    }

    // Extension Methods for System.Json
    public static string ToJson<T>(this T obj)
    {
        return JsonSerializer.Serialize(obj);
    }

    public static T FromJson<T>(this string json)
    {
        return JsonSerializer.Deserialize<T>(json);
    }

    public static string ToCompressedJson<T>(this T obj)
    {
        using (var memoryStream = new MemoryStream())
        {

            using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress))
            {
                JsonSerializer.Serialize(gZipStream, obj);
            }
            return Convert.ToBase64String(memoryStream.ToArray());
        }
    }

    public static T ToDecompressedJson<T>(this string json)
    {
        byte[] decodedData = Convert.FromBase64String(json);

        using (var memoryStream = new MemoryStream(decodedData))
        {
            using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
            {
                var returnArray = JsonSerializer.Deserialize<T>(gZipStream);

                return returnArray;
            }
        }
    }
}