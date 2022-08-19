using System;
using System.Collections.Generic;

public sealed class StringDB
{
    public object representedObject;

    internal StringDB(object obj)
    {
        representedObject = obj;
        Core = new(obj.GetField("Core"));
        Items = new(obj.GetField("Items"));
        Enemies = new(obj.GetField("Enemies"));
        Intro = new(obj.GetField("Intro"));
    }

    public StringTableManager.GungeonSupportedLanguages CurrentLanguage
    {
        get
        {
            return GameManager.Options.CurrentLanguage;
        }
        set
        {
            StringTableManager.SetNewLanguage(value, true);
        }
    }

    public StringDBTable Core;
    public StringDBTable Items;
    public StringDBTable Enemies;
    public StringDBTable Intro;

    public Action<StringTableManager.GungeonSupportedLanguages> OnLanguageChanged;

    public void LanguageChanged()
    {
        Core.LanguageChanged();
        Items.LanguageChanged();
        Enemies.LanguageChanged();
        Intro.LanguageChanged();
        OnLanguageChanged?.Invoke(CurrentLanguage);
    }

}

public sealed class StringDBTable
{
    public object representedObject;

    internal StringDBTable(object obj)
    {
        representedObject = obj;
    }

    public Dictionary<string, StringTableManager.StringCollection> Table
    {
        get
        {
            return (Dictionary<string, StringTableManager.StringCollection>)representedObject.InvokeMethod("get_Table");
        }
    }

    public StringTableManager.StringCollection this[string key]
    {
        get
        {
            return Table[key];
        }
        set
        {
            representedObject.InvokeMethod("set_Item", key, value);
        }
    }

    public bool ContainsKey(string key)
    {
        return Table.ContainsKey(key);
    }

    public void Set(string key, string value)
    {
        representedObject.InvokeMethod("Set", key, value);
    }

    public string Get(string key)
    {
        return StringTableManager.GetString(key);
    }

    public void LanguageChanged()
    {
        representedObject.InvokeMethod("LanguageChanged");
    }

}