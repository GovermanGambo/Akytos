using Akytos.Assets;
using Akytos.Serialization;
using Akytos.Serialization.Surrogates;

namespace Akytos;

/// <summary>
///     Responsible for saving and loading scenes (or Nodes) to/from disk.
/// </summary>
internal class SceneLoader
{
    private readonly YamlSerializer m_serializer;
    private readonly YamlDeserializer m_deserializer;

    /// <summary>
    ///     Creates a new <see cref="SceneLoader"/>
    /// </summary>
    public SceneLoader(AssetManager assetManager)
    {
        m_serializer = new YamlSerializer();
        m_deserializer = new YamlDeserializer();
        var vector2Serializer = new Vector2SerializationSurrogate();
        var texture2DAssetSerializer = new Texture2DAssetSerializationSurrogate(assetManager);
        m_serializer.AddSurrogate(texture2DAssetSerializer);
        m_serializer.AddSurrogate(vector2Serializer);
        m_deserializer.AddSurrogate(texture2DAssetSerializer);
        m_deserializer.AddSurrogate(vector2Serializer);
    }

    /// <summary>
    ///     Loads a scene from the specified filepath.
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns>The parsed root node of the scene.</returns>
    /// <exception cref="DeserializationException">If the deserialization of the file contents failed.</exception>
    /// <exception cref="IOException">If there was an issue with loading the file from disk.</exception>
    public Node LoadScene(string filePath)
    {
        string yaml = File.ReadAllText(filePath);

        var node = m_deserializer.Deserialize(yaml) as Node;

        return node;
    }

    /// <summary>
    ///     Saves a scene to the specified filepath.
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="scene"></param>
    /// <exception cref="IOException">If there was an issue with saving the file to disk.</exception>
    public void SaveScene(string filePath, Node scene)
    {
        string yaml = m_serializer.Serialize(scene);
        
        File.WriteAllText(filePath, yaml);
    }
}