using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using ProtoBuf;
using ProtoBuf.Data;
using ProtoBuf.Meta;
namespace Serialiation.PB.Console;

public class DataTableSerializer
{
    public async Task SerializeDataTable(DataTable dataTable, string filePath)
    {
        
        using var stream = await ProtobufSerializationHeper.SerializeDataTable(dataTable);
        File.WriteAllBytes(filePath, ((MemoryStream)stream).ToArray());
    }

    public async Task<DataTable> DeserializeDataTable(string filePath)
    {
        var dataTable = await ProtobufSerializationHeper.DeserializeDataTable(new MemoryStream(File.ReadAllBytes(filePath)));
        return dataTable;
    }

}
public class DatatableSerialization
{
    public static async Task Run()
    {
        // Create a sample DataTable to serialize
        DataTable dataTable = new DataTable();
        dataTable.Columns.Add("Id", typeof(int));
        dataTable.Columns.Add("Name", typeof(string));

        // Add some sample data
        dataTable.Rows.Add(1, "Alice");
        dataTable.Rows.Add(2, "Bob");
        
        // Specify the file path to save the serialized data
        string filePath = "serializedDataTable.bin";

        // Serialize the DataTable
        System.Console.WriteLine("Serializing DataTable...");
        var serializer = new DataTableSerializer();
        await serializer.SerializeDataTable(dataTable, filePath);

        // Deserialize the DataTable
        System.Console.WriteLine("Deserializing DataTable...");
        DataTable deserializedDataTable = await serializer.DeserializeDataTable(filePath);

        // Display the deserialized data
        System.Console.WriteLine("Deserialized DataTable content:");
        foreach (DataRow row in deserializedDataTable.Rows)
        {
            System.Console.WriteLine($"Id: {row["Id"]}, Name: {row["Name"]}");
        }
    }
}
