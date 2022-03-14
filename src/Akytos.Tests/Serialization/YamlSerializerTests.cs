using System.Collections.Generic;
using Akytos.Serialization;
using Akytos.Serialization.Surrogates;
using Xunit;

namespace Akytos.Tests.Serialization;

public class YamlSerializerTests
{
    [Fact]
    public void Serialize_Should_WorkWithPrimitives()
    {
        var person = new Person("Matt", 28, 6.7f);

        var yamlSerializer = new YamlSerializer();

        const string expected = @"type: Akytos.Tests.Serialization.YamlSerializerTests+Person
value:
  m_name:
    type: System.String
    value: Matt
  m_age:
    type: System.Int32
    value: 28
  m_rating:
    type: System.Single
    value: 6.7
";

        string yaml = yamlSerializer.Serialize(person);

        Assert.Equal(expected, yaml);
    }

    [Fact]
    public void Serialize_Should_WorkWithNestedObjects()
    {
        var person = new Person("Matt", 28, 6.7f);
        var house = new House("Person Lane 54", person);

        var yamlSerializer = new YamlSerializer();

        string yaml = yamlSerializer.Serialize(house);

        const string expected = @"type: Akytos.Tests.Serialization.YamlSerializerTests+House
value:
  m_address:
    type: System.String
    value: Person Lane 54
  m_person:
    type: Akytos.Tests.Serialization.YamlSerializerTests+Person
    value:
      m_name:
        type: System.String
        value: Matt
      m_age:
        type: System.Int32
        value: 28
      m_rating:
        type: System.Single
        value: 6.7
";

        Assert.Equal(expected, yaml);
    }

    [Fact]
    public void Serialize_Should_WorkWithLists()
    {
        var registry = new Registry(
            new[] {"Address1", "Address2", "Address3"},
            new List<Person>
            {
                new("Matt", 28, 6.7f),
                new("Matt", 28, 6.7f),
                new("Matt", 28, 6.7f)
            });

        var serializer = new YamlSerializer();

        string yaml = serializer.Serialize(registry);
    }

    [Fact]
    public void SerializeNode_Should_Work()
    {
        var rootNode = new Node2D("RootNode");
        var node2D = new Node2D("Node2D");
        var spriteNode = new SpriteNode("SpriteNode");
        var childNode = new Node("ChildOfSpriteNode");

        rootNode.AddChild(node2D);
        rootNode.AddChild(spriteNode);
        spriteNode.AddChild(childNode);

        var serializer = new YamlSerializer();
        serializer.AddSurrogate(new Vector2SerializationSurrogate());

        string yaml = serializer.Serialize(rootNode);

        const string expected = @"type: Akytos.Node2D
value:
  m_children:
    type: System.Collections.Generic.List`1[[Akytos.Node, Akytos, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]
    value:
    - m_children:
        type: System.Collections.Generic.List`1[[Akytos.Node, Akytos, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]
        value: []
      m_name:
        type: System.String
        value: Node2D
      position:
        x: 0
        y: 0
      scale:
        x: 1
        y: 1
      rotation:
        type: System.Int32
        value: 0
    - m_children:
        type: System.Collections.Generic.List`1[[Akytos.Node, Akytos, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]
        value:
        - m_children:
            type: System.Collections.Generic.List`1[[Akytos.Node, Akytos, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]
            value: []
          m_name:
            type: System.String
            value: ChildOfSpriteNode
      m_name:
        type: System.String
        value: SpriteNode
      position:
        x: 0
        y: 0
      scale:
        x: 1
        y: 1
      rotation:
        type: System.Int32
        value: 0
      m_textureAsset: ''
  m_name:
    type: System.String
    value: RootNode
  position:
    x: 0
    y: 0
  scale:
    x: 1
    y: 1
  rotation:
    type: System.Int32
    value: 0
";

        Assert.Equal(expected, yaml);
    }

    private class Person
    {
        [SerializeField("Age")] private int m_age;
        [SerializeField("Name")] private string m_name;
        [SerializeField("Rating")] private float m_rating;

        public Person(string name, int age, float rating)
        {
            m_name = name;
            m_age = age;
            m_rating = rating;
        }
    }

    private class House
    {
        [SerializeField("Address")] private string m_address;
        [SerializeField("Person")] private Person m_person;

        public House(string address, Person person)
        {
            m_address = address;
            m_person = person;
        }
    }

    private class Registry
    {
        [SerializeField("Addresses")] private string[] m_addresses;
        [SerializeField("People")] private List<Person> m_people;


        public Registry(string[] addresses, List<Person> people)
        {
            m_addresses = addresses;
            m_people = people;
        }
    }
}