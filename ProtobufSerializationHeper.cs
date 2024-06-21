using System.IO;
using System.Threading;
using ProtoBuf;
using ProtoBuf.Meta;
namespace Serialiation.PB.Console;
public static class ProtobufSerializationHeper
{
    private static SemaphoreSlim semaphoreSlimforSerialization = new SemaphoreSlim(1, 1);
    private static SemaphoreSlim semaphoreSlimforDeserialization = new SemaphoreSlim(1, 1);
    public static async Task<Stream> SerializeAsync<T>(T obj)
    {

        await semaphoreSlimforSerialization.WaitAsync();
        var memoryStream = new MemoryStream();
        try
        {
            Serializer.Serialize(memoryStream, obj);
            memoryStream.Position = 0;
            return memoryStream;
        }
        finally
        {
            semaphoreSlimforSerialization.Release();
        }
    }
    public static async Task<Stream> SerializeAsync<T>(T obj, RuntimeTypeModel runtimeTypeModel)
    {

        await semaphoreSlimforSerialization.WaitAsync();
        var memoryStream = new MemoryStream();
        try
        {
            runtimeTypeModel.Serialize(memoryStream, obj);

            memoryStream.Position = 0;
            return memoryStream;
        }
        finally
        {
            semaphoreSlimforSerialization.Release();
        }
    }
    public static async Task<T> DeserializeAsync<T>(Stream stream)
    {
        await semaphoreSlimforDeserialization.WaitAsync();
        try
        {
            return Serializer.Deserialize<T>(stream);
        }
        finally
        {
            semaphoreSlimforDeserialization.Release();
        }
    }
    public static async Task<T> DeserializeAsync<T>(Stream stream, RuntimeTypeModel runtimeTypeModel)
    {
        await semaphoreSlimforDeserialization.WaitAsync();
        try
        {
            return runtimeTypeModel.Deserialize<T>(stream);
        }
        finally
        {
            semaphoreSlimforDeserialization.Release();
        }
    }
}
