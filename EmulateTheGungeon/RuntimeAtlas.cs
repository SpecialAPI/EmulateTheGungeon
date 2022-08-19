using System;
using UnityEngine;
using System.Collections.Generic;

public class RuntimeAtlasPacker
{
    public object representedObject;

    public List<RuntimeAtlasPage> Pages = new List<RuntimeAtlasPage>();

    public int Width;
    public int Height;
    public TextureFormat Format;
    public int Padding;

    public RuntimeAtlasPacker(object obj)
    {
        representedObject = obj;

        Width = (int)obj.GetField("Width");
        Height = (int)obj.GetField("Height");
        Format = (TextureFormat)obj.GetField("Format");
        Padding = (int)obj.GetField("Padding");
    }

    public RuntimeAtlasSegment Pack(Texture2D tex, bool apply = false)
    {
        return new(representedObject.InvokeMethod("Pack", tex, apply));
    }

    public Action<RuntimeAtlasPage> OnNewPage;

    public RuntimeAtlasPage NewPage()
    {
        return new(representedObject.InvokeMethod("NewPage"));
    }

    public void Apply()
    {
        representedObject.InvokeMethod("Apply");
    }

    public bool IsPageTexture(Texture2D tex) => (bool)representedObject.InvokeMethod("IsPageTexture", tex);
}

public class RuntimeAtlasPage
{
    public object representedObject;
    public static int DefaultSize = Math.Min(SystemInfo.maxTextureSize, 4096);

    public List<RuntimeAtlasSegment> Segments = new List<RuntimeAtlasSegment>();
    public Texture2D Texture;

    public int Padding;

    public RuntimeAtlasPage(object obj)
    {
        representedObject = obj;
        Segments = (List<RuntimeAtlasSegment>)obj.GetField("Segments");
        Texture = (Texture2D)obj.GetField("Texture");
        Padding = (int)obj.GetField("Padding");

    }

    public RuntimeAtlasSegment Pack(Texture2D tex, bool apply = false)
    {
        return new(representedObject.InvokeMethod("Pack", tex, apply));
    }

    public void Apply()
    {
        representedObject.InvokeMethod("Apply");
    }
}

public class RuntimeAtlasSegment
{
    public object representedObject;
    public RuntimeAtlasSegment(object obj)
    {
        representedObject = obj;
        texture = (Texture2D)obj.GetField("texture");
        x = (int)obj.GetField("x");
        y = (int)obj.GetField("y");
        width = (int)obj.GetField("width");
        height = (int)obj.GetField("height");
    }

    public Texture2D texture;

    public int x;
    public int y;
    public int width;
    public int height;

    public Vector2[] uvs => (Vector2[])InvokeStaticMTGMethod("ETGMod+Assets", "GenerateUVs", texture, x, y, width, height);
}