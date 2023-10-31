namespace Serialization;

using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.Json;
using System.Xml.Serialization;

public class Program
{

    private const int MaxGreetings = 100000;
    private const string MyName = "Joseph";

    public static void Main(string[] args)
    {

        do
        {
            // Create 100,000 random greetings, timing the process.
            // Report it to the user.
            var sw = new Stopwatch();
            sw.Start();
            List<Greeting> greetings = GreetingsGenerate();
            sw.Stop();

            Console.WriteLine($"Total Messages Created: {greetings.Count()}. Total Time {sw.Elapsed.TotalMilliseconds} (ms), Average Time (per item) {sw.Elapsed.TotalMilliseconds / MaxGreetings}");

            
            // DataContractJsonSerializer
            Console.WriteLine("Json (using DataContractJsonSerializer)...");
            
            var dataContractSerialize  = LogFunctionAndReturn( "Serialize", 
                                        ()=> greetings.ToJsonDataContract());
     

            var dataContractDeserialize = LogFunctionAndReturn("Deserialize", 
                                        ()=> dataContractSerialize.FromJsonDataContract<List<Greeting>>());
        

            var dataContractSerializeCompress = LogFunctionAndReturn("Serialize and Compress",
                                        ()=> greetings.ToCompressedJsonContract());
                
            var dataContractDecompressDeserialize = LogFunctionAndReturn("Decompress and Deserialize", 
                                        ()=> dataContractSerializeCompress.ToDecompressedJsonContract<List<Greeting>>());




            // System.Text.Json DONE!
            Console.WriteLine("Json (using System.Text.Json)...");

            var JsonSerialize = LogFunctionAndReturn("Serialize",
                                    () => greetings.ToJson());


            var JsonDeserialize = LogFunctionAndReturn("Deserialize",
                                        () => JsonSerialize.FromJson<List<Greeting>>());


            var JsonSerializeCompress = LogFunctionAndReturn("Serialize and Compress",
                                        () => greetings.ToCompressedJson());

            var JsonDecompressDeserialize = LogFunctionAndReturn("Decompress and Deserialize",
                                        () => JsonSerializeCompress.ToDecompressedJson<List<Greeting>>());


            //// XML
            //Console.WriteLine("XML...");

            //var xmlSerialize = TimeSerializeFunction(greetings, XMLSerialize, sw);
            //Console.WriteLine($"\t Serialize \t \t \t {xmlSerialize.Item2} (ms)");

            //var xmlDeserialize = TimeDeserializeFunction(xmlSerialize.Item1, XMLDeserialize, sw);
            //Console.WriteLine($"\t Deserialize \t \t \t {xmlDeserialize.Elapsed} (ms)");

            //var xmlSerializeCompress = TimeSerializeFunction(greetings, XMLSerializeCompress, sw);
            //Console.WriteLine($"\t Serialize and Compress \t {xmlSerializeCompress.Item2} (ms)");

            //var xmlDecompressDeserialize = TimeDeserializeFunction(xmlSerializeCompress.Item1, XMLDecompressDeserialize, sw);
            //Console.WriteLine($"\t Decompress and Deserialize      {xmlDecompressDeserialize.Item2} (ms)");

            Console.WriteLine("Done. (any key to go again.");
        } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
    }





    private static string XMLSerialize(IEnumerable<Greeting> greetings)
    {
        var xmlSerializer = new XmlSerializer(typeof(List<Greeting>));
        var stringBuilder = new StringBuilder();
        using (var writer = new StringWriter(stringBuilder))
        {
            xmlSerializer.Serialize(writer, greetings);
            return stringBuilder.ToString();
        }     
    }

    private static IEnumerable<Greeting> XMLDeserialize(string xmlText)
    {
        var xmlSerializer = new XmlSerializer(typeof(List<Greeting>));
        using (var reader = new StringReader(xmlText))
        {
            return (List< Greeting>)xmlSerializer.Deserialize(reader);      
                
        } 
       
    }

    // With Compression/Decompression.

    private static string XMLSerializeCompress(IEnumerable<Greeting> greetings)
    {
        var xmlSerializer = new XmlSerializer(typeof(List<Greeting>));    
        using (var memoryStream = new MemoryStream())
        {
            using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress))
            {
                xmlSerializer.Serialize(gZipStream, greetings);
            }
            return Convert.ToBase64String(memoryStream.ToArray());
        }
    }

    private static IEnumerable<Greeting> XMLDecompressDeserialize(string xmlText)
    {
        var xmlSerializer = new XmlSerializer(typeof(List<Greeting>));

        using (var memoryStream = new MemoryStream(Convert.FromBase64String(xmlText)))
        {
            using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
            {
                return (List<Greeting>)xmlSerializer.Deserialize(gZipStream);
            }
        }

    }

    static (T Result, double Elapsed) TimeFunction<T>(Func<T> func)
    {
        var stopwatch = Stopwatch.StartNew();
        var returnArray = func();
        stopwatch.Stop();
        return (returnArray, stopwatch.Elapsed.TotalMilliseconds);
    }

    private static List<Greeting> GreetingsGenerate()
    {
        List<Greeting> greetings = new List<Greeting>();

        var people = PersonPopulate();
        var messages = MessagePopulate();
        var toPeople = people.Where(x => x.Name != MyName);

        for (int i = 0; i < MaxGreetings; i++ )
        {
            var currGreet = new Greeting()
            {
                From = people.FindRandom(),
                To = toPeople.FindRandom(),
                Message = messages.FindRandom()
            };
            
            greetings.Add(currGreet);
        }

        return greetings;
    }
    private static TResult LogFunctionAndReturn<TResult>(string name, Func<TResult> func)
    {
        var ( Result, Elapsed) = TimeFunction(func);
        Console.WriteLine($"    {name,-40} {Elapsed} (ms)");
        return Result;
    }

    private static List<string> MessagePopulate()
    {
        return new List<string>{
            "a",
            "aaa",
            "bang",
            "bing",
            "eee",
            "oo",
            "ooo",
            "tang",
            "ting",
            "walla",
            "welcome"
        };
    }

    private static IEnumerable<Person> PersonPopulate()
    {
        return new List<Person>()
        {
            new Person(){Name ="Dad" },
            new Person(){Name ="David Blaine" },
            new Person(){Name ="Joseph"},
            new Person(){Name ="Mike Oxsmaul" },
            new Person(){Name ="Moe Lester" },
            new Person(){Name ="Mom" },
            new Person(){Name ="Scatman" },
            new Person(){Name ="Sean" },
            new Person(){Name ="Walter White" },
        };
    }

}