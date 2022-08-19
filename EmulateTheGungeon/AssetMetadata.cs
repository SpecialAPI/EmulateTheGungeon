using System;
using System.IO;
using System.Reflection;
using IOFile = System.IO.File;

/// <summary>
/// ETGMod asset metadata.
/// </summary>
public class AssetMetadata
{
    public ContainerType Container;
    public Type AssetType = null;

    public string File;

    public string Zip;

    public byte[] RawData;

    public Assembly Assembly;
    public string AssemblyName;

    public long Offset;
    public int Length;

    /// <summary>
    /// Returns a new stream to read the data from.
    /// In case of limited data (Length is set), LimitedStream is used.
    /// </summary>
    public Stream Stream => null;

    /// <summary>
    /// Returns the files contents.
    /// </summary>
    public byte[] Data => null;

    public bool HasData => false;

    public AssetMetadata()
    {
        Container = ContainerType.Filesystem;
    }

    public AssetMetadata(string file)
        : this(file, 0, 0)
    {
    }

    public AssetMetadata(string file, long offset, int length)
        : this()
    {
    }

    public AssetMetadata(string zip, string file, byte[] rawData)
        : this(file)
    {
    }

    public AssetMetadata(Assembly assembly, string file)
        : this(file)
    {
    }

    public enum ContainerType
    {
        Filesystem,
        Zip,
        Assembly
    }
}