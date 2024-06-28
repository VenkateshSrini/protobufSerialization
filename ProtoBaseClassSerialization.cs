using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.Collections.Generic;
using ProtoBuf;
using System.Collections;
// Ensure you have imported the ProtoBuf namespace

namespace Serialiation.PB.Console
{
    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    [ProtoInclude(100, typeof(ProtoChildClass))] // Tag 100 for ProtoChildClass
    [ProtoInclude(101, typeof(ProtoChildClass1))] // Tag 101 for ProtoChildClass1
    public abstract class ProtoBaseClass
    {
        protected string Name;
        protected Dictionary<short, string> Properties;


        protected ProtoBaseClass()
        {
            Name = "default";
            Properties = new Dictionary<short, string>();
        }
       
        protected ProtoBaseClass(string name)
        {
            Name = name;
            Properties = new Dictionary<short, string>();
        }
        public abstract void PrintValues();
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    public class ProtoChildClass : ProtoBaseClass
    {
        public int Age;
        public Decimal PriceMoney;
        
        protected ProtoChildClass() : base() { }

        public ProtoChildClass(string name, int age, decimal priceMoney) : base(name)
        {
            Age = age;
            Properties.Add(1, "childclass");
            PriceMoney = priceMoney;
            System.Console.WriteLine("Before Serialization");
            System.Console.WriteLine("=====================");
            PrintValues();
        }

        public override void PrintValues()
        {
            System.Console.WriteLine($"Name: {Name}, Age: {Age}, PriceMoney: {PriceMoney}");
        }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    public class ProtoChildClass1 : ProtoBaseClass
    {
        public int Age;
        public string Gender;

       
        protected ProtoChildClass1() : base() { }

        public ProtoChildClass1(string name, int age, string gender) : base(name)
        {
            Age = age;
            Gender = gender;
            Properties.Add(1, "childclass1");
        }

        public override void PrintValues()
        {
            System.Console.WriteLine($"Name: {Name}, Age: {Age}, Gender:- {Gender}");
        }
    }

    [ProtoContract]
    public class ProtoComposer
    {
        [ProtoMember(1, OverwriteList = true)]
        private Dictionary<short, ProtoBaseClass> baseClasses;

        [ProtoMember(2)]
        private string Name;

        [ProtoMember(3)]
        private int privateExample = 42; // This private field will now be serialized

        public Dictionary<short, ProtoBaseClass> Collection
        {
            get { return baseClasses; }
            set { baseClasses = value; }
        }

     
        public ProtoComposer()
        {
            Name = "abc";
            baseClasses = new Dictionary<short, ProtoBaseClass>();
        }
    }
    public class ProtoBaseClassSerialization
    {
        public static async Task Run()
        {
            var composer = new ProtoComposer();
            composer.Collection = new Dictionary<short, ProtoBaseClass>
            {
                { 1, new ProtoChildClass("Child1", 10,0.0m) },
                //{ 2, new ProtoChildClass1("Child2", 20,"male") }
            };
            var composerDict = new Dictionary<short, ProtoComposer>() {
                { 1, composer }
            };
            using var stream = await ProtobufSerializationHeper.SerializeAsync(composerDict);
            File.WriteAllBytes("proto.bin", ((MemoryStream)stream).ToArray());
            using var readStream = new MemoryStream(File.ReadAllBytes("proto.bin"));
            var deserializedComposer = await ProtobufSerializationHeper.DeserializeAsync<Dictionary<short, ProtoComposer>>(readStream);
            System.Console.WriteLine("After Serialization");
            System.Console.WriteLine("=====================");
            foreach (var item in deserializedComposer)
            {
                
                foreach (var baseClass in item.Value.Collection)
                {
                    baseClass.Value.PrintValues();
                }
            }
        }
    }
}

