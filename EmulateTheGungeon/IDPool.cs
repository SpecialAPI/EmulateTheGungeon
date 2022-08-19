using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class IDPool<T>
{
    public object representedObject;

    public IDPool(object obj)
    {
        representedObject = obj;
    }

    public T this[string id]
    {
        set => Set(id, value, false);
        get => Get(id);
    }

    public int Count => (int)representedObject.InvokeMethod("get_Count");

    public class NonExistantIDException : Exception
    {
        public NonExistantIDException(string id) : base($"Object with ID {id} doesn't exist") { }
    }

    public class BadIDElementException : Exception
    {
        public BadIDElementException(string name) : base($"The ID's {name} can not contain any colons or whitespace") { }
    }

    public class LockedNamespaceException : Exception
    {
        public LockedNamespaceException(string namesp) : base($"The ID namespace {namesp} is locked") { }
    }

    public class ItemIDExistsException : Exception
    {
        public ItemIDExistsException(string id) : base($"Item {id} already exists") { }
    }

    public class BadlyFormattedIDException : Exception
    {
        public BadlyFormattedIDException(string id) : base($"ID was improperly formatted: {id}") { }
    }

    public void LockNamespace(string namesp) { }

    private void Set(string id, T obj, bool throwOnExists)
    {
        representedObject.InvokeMethod("Set", id, obj);
    }

    public void Add(string id, T obj) => representedObject.InvokeMethod("Add", id, obj);

    public T Get(string id)
    {
        return (T)representedObject.InvokeMethod("Get", id);
    }

    public void Remove(string id, bool destroy = true)
    {
        representedObject.InvokeMethod("Remove", id, destroy);
    }

    public void Rename(string source, string target)
    {
        try
        {
            representedObject.InvokeMethod("Rename", source, target);
        }
        catch
        {
            representedObject.InvokeMethod("Rename", source.ToID(), target);
        }
    }

    public static void VerifyID(string id)
    {
        if (ETGMod.Count(id, ':') > 1)
            throw new BadlyFormattedIDException(id);
    }

    public static string Resolve(string id)
    {
        id = id.Trim();
        if (id.Contains(":"))
        {
            VerifyID(id);
            return id;
        }
        else
        {
            return $"gungeon:{id}";
        }
    }

    public static Entry Split(string id)
    {
        VerifyID(id);
        string[] split = id.Split(':');
        if (split.Length != 2)
            throw new BadlyFormattedIDException(id);
        return new Entry(split[0], split[1]);
    }

    public bool ContainsID(string id)
    {
        return (bool)representedObject.InvokeMethod("ContainsID", id);
    }

    public bool NamespaceIsLocked(string namesp)
    {
        return false;
    }

    public string[] AllIDs => (string[])representedObject.InvokeMethod("get_AllIDs");

    public IEnumerable<T> Entries => (IEnumerable<T>)representedObject.InvokeMethod("get_Entries");

    public IEnumerable<string> IDs => (IEnumerable<string>)representedObject.InvokeMethod("get_IDs");

    public IEnumerable<KeyValuePair<string, T>> Pairs => (IEnumerable<KeyValuePair<string, T>>)representedObject.InvokeMethod("get_Pairs");

    public struct Entry
    {
        public string Namespace;
        public string Name;

        public Entry(string namesp, string name)
        {
            Namespace = namesp;
            Name = name;
        }
    }
}
