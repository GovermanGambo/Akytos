using System;
using System.Collections.Generic;
using Akytos.Assets;
using Akytos.Graphics;
using Akytos.SceneSystems;
using Akytos.Serialization;
using Akytos.Serialization.Surrogates;
using Moq;
using Xunit;

namespace Akytos.Tests.Serialization;

public class YamlSerializerTests
{
    [Fact]
    public void Serialize_Should_WorkWithPrimitives()
    {
        var person = new Person("Matt", 28, 6.7f);

        var yamlSerializer = new YamlSerializer();

        const string expected = @"type: Akytos.Tests.Serialization.Person
value:
  m_age:
    type: System.Int32
    value: 28
  m_name:
    type: System.String
    value: Matt
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

        const string expected = @"type: Akytos.Tests.Serialization.House
value:
  m_address:
    type: System.String
    value: Person Lane 54
  m_person:
    type: Akytos.Tests.Serialization.Person
    value:
      m_age:
        type: System.Int32
        value: 28
      m_name:
        type: System.String
        value: Matt
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

        const string expected = @"type: Akytos.Tests.Serialization.Registry
value:
  m_addresses:
    type: System.String[]
    value:
      length: 3
      elements:
      - type: System.String
        value: Address1
      - type: System.String
        value: Address2
      - type: System.String
        value: Address3
  m_people:
    type: System.Collections.Generic.List`1[[Akytos.Tests.Serialization.Person, Akytos.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]
    value:
      length: 3
      elements:
      - type: Akytos.Tests.Serialization.Person
        value:
          m_age:
            type: System.Int32
            value: 28
          m_name:
            type: System.String
            value: Matt
          m_rating:
            type: System.Single
            value: 6.7
      - type: Akytos.Tests.Serialization.Person
        value:
          m_age:
            type: System.Int32
            value: 28
          m_name:
            type: System.String
            value: Matt
          m_rating:
            type: System.Single
            value: 6.7
      - type: Akytos.Tests.Serialization.Person
        value:
          m_age:
            type: System.Int32
            value: 28
          m_name:
            type: System.String
            value: Matt
          m_rating:
            type: System.Single
            value: 6.7
";
        
        Assert.Equal(expected, yaml);
    }

    [Theory]
    [InlineData(TestEnum.OneValue, "type: Akytos.Tests.Serialization.TestEnum\nvalue: 0\n")]
    [InlineData(TestEnum.TwoValue, "type: Akytos.Tests.Serialization.TestEnum\nvalue: 1\n")]
    public void Serialize_Should_WorkWithEnums(TestEnum testEnum, string expected)
    {
        var serializer = new YamlSerializer();
        string yaml = serializer.Serialize(testEnum);
        
        Assert.Equal(expected, yaml);
    }
    
    [Theory]
    [InlineData("type: Akytos.Tests.Serialization.TestEnum\nvalue: 0\n", TestEnum.OneValue)]
    [InlineData("type: Akytos.Tests.Serialization.TestEnum\nvalue: 1\n", TestEnum.TwoValue)]
    public void Deserialize_Should_WorkWithEnums(string yaml, TestEnum expected)
    {
        var serializer = new YamlDeserializer();
        var result = serializer.Deserialize(yaml);
        
        Assert.Equal(expected, result);
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
        serializer.AddSurrogate(new ColorSerializationSurrogate());

        string yaml = serializer.Serialize(rootNode);

        const string expected = @"type: Akytos.SceneSystems.Node2D
value:
  m_children:
    type: System.Collections.Generic.List`1[[Akytos.SceneSystems.Node, Akytos, Version=0.1.0.0, Culture=neutral, PublicKeyToken=null]]
    value:
      length: 2
      elements:
      - type: Akytos.SceneSystems.Node2D
        value:
          m_children:
            type: System.Collections.Generic.List`1[[Akytos.SceneSystems.Node, Akytos, Version=0.1.0.0, Culture=neutral, PublicKeyToken=null]]
            value:
              length: 0
              elements: []
          m_name:
            type: System.String
            value: Node2D
          m_modulate:
            type: Akytos.Color
            value:
              r: 1
              g: 1
              b: 1
              a: 1
          m_isVisible:
            type: System.Boolean
            value: True
          position:
            type: System.Numerics.Vector2
            value:
              x: 0
              y: 0
          scale:
            type: System.Numerics.Vector2
            value:
              x: 1
              y: 1
          rotation:
            type: System.Int32
            value: 0
      - type: Akytos.SceneSystems.SpriteNode
        value:
          m_children:
            type: System.Collections.Generic.List`1[[Akytos.SceneSystems.Node, Akytos, Version=0.1.0.0, Culture=neutral, PublicKeyToken=null]]
            value:
              length: 1
              elements:
              - type: Akytos.SceneSystems.Node
                value:
                  m_children:
                    type: System.Collections.Generic.List`1[[Akytos.SceneSystems.Node, Akytos, Version=0.1.0.0, Culture=neutral, PublicKeyToken=null]]
                    value:
                      length: 0
                      elements: []
                  m_name:
                    type: System.String
                    value: ChildOfSpriteNode
          m_name:
            type: System.String
            value: SpriteNode
          m_modulate:
            type: Akytos.Color
            value:
              r: 1
              g: 1
              b: 1
              a: 1
          m_isVisible:
            type: System.Boolean
            value: True
          position:
            type: System.Numerics.Vector2
            value:
              x: 0
              y: 0
          scale:
            type: System.Numerics.Vector2
            value:
              x: 1
              y: 1
          rotation:
            type: System.Int32
            value: 0
          m_textureAsset: ''
          m_isCentered:
            type: System.Boolean
            value: False
  m_name:
    type: System.String
    value: RootNode
  m_modulate:
    type: Akytos.Color
    value:
      r: 1
      g: 1
      b: 1
      a: 1
  m_isVisible:
    type: System.Boolean
    value: True
  position:
    type: System.Numerics.Vector2
    value:
      x: 0
      y: 0
  scale:
    type: System.Numerics.Vector2
    value:
      x: 1
      y: 1
  rotation:
    type: System.Int32
    value: 0
";

        Assert.Equal(expected, yaml);
    }

    [Fact]
    public void Deserialize_Should_WorkWithPrimitives()
    {
        var person = new Person("Matt", 28, 6.7f);

        var yamlSerializer = new YamlSerializer();
        var yamlDeserializer = new YamlDeserializer();

        string yaml = yamlSerializer.Serialize(person);

        var obj = yamlDeserializer.Deserialize(yaml) as Person;

        Assert.Equal(person, obj);
    }

    [Fact]
    public void Deserialize_Should_WorkWithLists()
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
        var deserializer = new YamlDeserializer();
        
        string yaml = serializer.Serialize(registry);

        object? obj = deserializer.Deserialize(yaml) as Registry;
        
        Assert.Equal(registry, obj);
    }

    [Fact]
    public void DeserializeNode_Should_Work()
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

        var deserializer = new YamlDeserializer();
        deserializer.AddSurrogate(new Vector2SerializationSurrogate());

        var node = deserializer.Deserialize(yaml) as Node2D;
        
        Assert.Equal(rootNode, node);
    }

    [Fact]
    public void SerializingSpriteNode_Should_Work()
    {
        const string filePath = "textures/Test.png";
        var textureMock = new Mock<ITexture2D>();
        var asset = new Texture2DAsset(textureMock.Object, filePath);
        var assetManagerMock = new Mock<IAssetManager>();
        assetManagerMock.Setup(am => am.Load<ITexture2D>(filePath)).Returns(asset);

        var spriteNode = new SpriteNode();
    }
}

public enum TestEnum
{
    OneValue,
    TwoValue
}

public class Person
{
    [SerializeField("Age")] private readonly int m_age;
    [SerializeField("Name")] private readonly string m_name;
    [SerializeField("Rating")] private readonly float m_rating;

    public Person()
    {
        m_name = "";
    }

    public Person(string name, int age, float rating)
    {
        m_name = name;
        m_age = age;
        m_rating = rating;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Person person) return false;

        return person.m_name == m_name && person.m_age == m_age &&
               System.Math.Abs(person.m_rating - m_rating) < 0.0001f;
    }
}

public class House
{
    [SerializeField("Address")] private string m_address;
    [SerializeField("Person")] private Person m_person;
    
    public House(string address, Person person)
    {
        m_address = address;
        m_person = person;
    }
}

public class Registry
{
    [SerializeField("Addresses")] private string[] m_addresses;
    [SerializeField("People")] private List<Person> m_people;

    public Registry()
    {
        m_addresses = Array.Empty<string>();
        m_people = new List<Person>();
    }
    
    public Registry(string[] addresses, List<Person> people)
    {
        m_addresses = addresses;
        m_people = people;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Registry registry)
        {
            return false;
        }

        if (m_addresses.Length != registry.m_addresses.Length || m_people.Count != registry.m_people.Count)
        {
            return false;
        }

        for (int i = 0; i < m_addresses.Length; i++)
        {
            if (m_addresses[i] != registry.m_addresses[i])
            {
                return false;
            }
        }
        
        for (int i = 0; i < m_people.Count; i++)
        {
            if (!m_people[i].Equals(registry.m_people[i]))
            {
                return false;
            }
        }

        return true;
    }
}