using ProtoBuf;
using ProtoBuf.Meta;
using System;
using System.Collections.Generic;
using System.IO;


using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serialiation.PB.Console
{
    public interface IAnimal
    {
        string Name { get; set; }
    }

    [ProtoContract]
    public class Dog : IAnimal
    {
        [ProtoMember(1)]
        public string Name { get; set; }
    }

    [ProtoContract]
    public class Cat : IAnimal
    {
        [ProtoMember(1)]
        public string Name { get; set; }
    }
    [ProtoContract]
    public class AnimalListWrapper
    {
        [ProtoMember(1)]
        public List<IAnimal> Animals { get; set; }

        public AnimalListWrapper()
        {
            Animals = new List<IAnimal>();
        }
    }


    public class ProtoInterfaceSerialization
    {
        public static async Task Run()
        {
            // Configure the custom RuntimeTypeModel
            var model = RuntimeTypeModel.Create();
            model.Add(typeof(IAnimal), false).AddSubType(101, typeof(Dog)).AddSubType(102, typeof(Cat));
            model.Add(typeof(AnimalListWrapper), true);

            // Create an instance of AnimalListWrapper and populate it
            var animals = new AnimalListWrapper();
            animals.Animals.Add(new Dog { Name = "Buddy" });
            animals.Animals.Add(new Cat { Name = "Whiskers" });
            using var stream = await ProtobufSerializationHeper.SerializeAsync(animals,model);
            File.WriteAllBytes("protoInterface.bin", ((MemoryStream)stream).ToArray());
            using var readStream = new MemoryStream(File.ReadAllBytes("protoInterface.bin"));
            var deserializedAnimals = await ProtobufSerializationHeper.DeserializeAsync<AnimalListWrapper>(readStream, model);
            foreach (var animal in deserializedAnimals.Animals)
            {
                System.Console.WriteLine(animal.Name);
            }
            
           
        }
    }


}
