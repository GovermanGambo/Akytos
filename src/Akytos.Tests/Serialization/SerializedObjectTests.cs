using System;
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
}