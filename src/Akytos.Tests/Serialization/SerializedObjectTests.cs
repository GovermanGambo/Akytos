using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Akytos.Tests.Serialization;

public class SerializedObjectTests
{
    private class ExampleSerializedObjectA
    {
        [SerializeField("String Field")] private string stringField;
        [SerializeField("Int Field")] private int intField;
        [SerializeField("Float Field")] private float floatField;

        public ExampleSerializedObjectA()
        {
        }

        public ExampleSerializedObjectA(string stringField, int intField, float floatField)
        {
            this.stringField = stringField;
            this.intField = intField;
            this.floatField = floatField;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not ExampleSerializedObjectA exampleSerializedObjectA)
            {
                return false;
            }

            return stringField == exampleSerializedObjectA.stringField &&
                   intField == exampleSerializedObjectA.intField && floatField == exampleSerializedObjectA.floatField;
        }
    }

    private class ExampleSerializedObjectB
    {
        [SerializeField("Field")] private ExampleSerializedObjectA serializedObjectA;

        public ExampleSerializedObjectB()
        {
        }

        public ExampleSerializedObjectB(ExampleSerializedObjectA serializedObjectA)
        {
            this.serializedObjectA = serializedObjectA;
        }
    }

    private class ExampleSerializedObjectC
    {
        [SerializeField("StringList")] private string[] stringList;
        [SerializeField("CustomList")] private ExampleSerializedObjectB[] customList;

        public ExampleSerializedObjectC(string[] stringList, ExampleSerializedObjectB[] customList)
        {
            this.stringList = stringList;
            this.customList = customList;
        }
    }

    [Fact]
    public void SerializedObjectCreate_ShouldCreateSerializedObjectWithSimpleFields()
    {
        var testObject = new ExampleSerializedObjectA("Test", 20, 43.97f);

        var serializedObject = SerializedObject.Create(testObject);
        
        Assert.NotNull(serializedObject);
        Assert.Equal(typeof(ExampleSerializedObjectA), Type.GetType(serializedObject.Type));
        Assert.Equal(3, serializedObject.Fields.Length);
        var stringField = serializedObject.Fields[0];
        var intField = serializedObject.Fields[1];
        var floatField = serializedObject.Fields[2];
        
        Assert.NotNull(stringField);
        Assert.Equal("stringField", stringField.Key);
        Assert.Equal(typeof(string), Type.GetType(stringField.Type));
        Assert.Equal("Test", stringField.Value);
        
        Assert.NotNull(intField);
        Assert.Equal("intField", intField.Key);
        Assert.Equal(typeof(int), Type.GetType(intField.Type));
        Assert.Equal(20, intField.Value);
        
        Assert.NotNull(floatField);
        Assert.Equal("floatField", floatField.Key);
        Assert.Equal(typeof(float), Type.GetType(floatField.Type));
        Assert.Equal(43.97f, floatField.Value);
    }

    [Fact]
    public void SerializedObjectCreate_ShouldCreateSerializedObjectWithSerializedObjectAsField()
    {
        var serializedObjectA = new ExampleSerializedObjectA("Test", 20, 43.97f);

        var testObject = new ExampleSerializedObjectB(serializedObjectA);

        var serializedObject = SerializedObject.Create(testObject);
        
        Assert.NotNull(serializedObject);
        Assert.Equal(typeof(ExampleSerializedObjectB), Type.GetType(serializedObject.Type));
        Assert.Single(serializedObject.Fields);

        var serializedObjectField = serializedObject.Fields[0];
        Assert.NotNull(serializedObjectField);
        Assert.Equal(typeof(ExampleSerializedObjectA), Type.GetType(serializedObjectField.Type));
        Assert.Equal("serializedObjectA", serializedObjectField.Key);
        Assert.IsType<SerializedObject>(serializedObjectField.Value);

        var obj = serializedObjectField.Value as SerializedObject;
        Assert.NotNull(obj);
        Assert.Equal(3, obj.Fields.Length);
        
        var stringField = obj.Fields[0];
        var intField = obj.Fields[1];
        var floatField = obj.Fields[2];
        
        Assert.NotNull(stringField);
        Assert.Equal("stringField", stringField.Key);
        Assert.Equal(typeof(string), Type.GetType(stringField.Type));
        Assert.Equal("Test", stringField.Value);
        
        Assert.NotNull(intField);
        Assert.Equal("intField", intField.Key);
        Assert.Equal(typeof(int), Type.GetType(intField.Type));
        Assert.Equal(20, intField.Value);
        
        Assert.NotNull(floatField);
        Assert.Equal("floatField", floatField.Key);
        Assert.Equal(typeof(float), Type.GetType(floatField.Type));
        Assert.Equal(43.97f, floatField.Value);
    }

    [Fact]
    public void SerializedObjectCreate_ShouldCreateSerializedObjectWithLists()
    {
        string[] stringList = {"A", "B", "C"};
        var customList = new[]
        {
            new ExampleSerializedObjectB(new ExampleSerializedObjectA("A", 1, 0.65f)),
            new ExampleSerializedObjectB(new ExampleSerializedObjectA("B", 2, 0.65f)),
            new ExampleSerializedObjectB(new ExampleSerializedObjectA("C", 3, 0.65f))
        };

        var exampleSerializedObjectC = new ExampleSerializedObjectC(stringList, customList);

        var serializedObject = SerializedObject.Create(exampleSerializedObjectC);
        
        Assert.NotNull(serializedObject);
        Assert.Equal(typeof(ExampleSerializedObjectC), Type.GetType(serializedObject.Type));
        Assert.Equal(2, serializedObject.Fields.Length);

        var serializedStringList = serializedObject.Fields[0];
        var serializedCustomList = serializedObject.Fields[1];
        
        Assert.NotNull(serializedStringList);
        Assert.Equal("stringList", serializedStringList.Key);
        Assert.Equal(typeof(string[]), Type.GetType(serializedStringList.Type));
        Assert.True(serializedStringList.Value is IEnumerable);
        
        Assert.NotNull(serializedCustomList);
        Assert.Equal("customList", serializedCustomList.Key);
        Assert.Equal(typeof(ExampleSerializedObjectB[]), Type.GetType(serializedCustomList.Type));
        Assert.True(serializedCustomList.Value is IEnumerable);

        var serializedObjectList = ((IEnumerable<object>) serializedCustomList.Value!).OfType<SerializedObject>();
        Assert.NotNull(serializedObjectList);
        Assert.NotEmpty(serializedObjectList);
        Assert.Equal(typeof(ExampleSerializedObjectB), Type.GetType(serializedObjectList.ElementAt(0).Type));
    }
}