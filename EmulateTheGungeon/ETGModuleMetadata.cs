using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class ETGModuleMetadata
{
    public virtual string Archive
    {
        get
        {
            return "";
        }
        set
        {
        }
    }

    private string _Directory = "";

    public virtual string Directory
    {
        get
        {
            return _Directory;
        }
        set
        {
        }
    }

    private string _Name;

    public virtual string Name
    {
        get
        {
            return _Name;
        }
        set
        {
            if (_Name != null)
            {
                throw new InvalidOperationException("The ETGModuleMetadata name is read-only!");
            }
            _Name = value;
        }
    }

    public virtual Texture2D Icon
    {
        get
        {
            return null;
        }
        set
        {
        }
    }

    private Version _Version;
    
    public virtual Version Version
    {
        get
        {
            return _Version;
        }
        set
        {
            if (_Version != null)
            {
                throw new InvalidOperationException("The ETGModuleMetadata version is read-only!");
            }
            _Version = value;
        }
    }

    private string _DLL;

    public virtual string DLL
    {
        get
        {
            return _DLL;
        }
        set
        {
            if (_DLL != null)
            {
                throw new InvalidOperationException("The ETGModuleMetadata DLL path is read-only!");
            }
            _DLL = value;
        }
    }

    private bool _Prelinked = true;

    public virtual bool Prelinked
    {
        get
        {
            return _Prelinked;
        }
        set
        {
            throw new InvalidOperationException("The ETGModuleMetadata Prelinked flag is read-only!");
        }
    }

    //private ETGMod.Profile _Profile;

    /*
    public virtual ETGMod.Profile Profile
    {
        get
        {
            return _Profile;
        }
        set
        {
            if (_Profile != null)
            {
                throw new InvalidOperationException("The ETGModuleMetadata profile is read-only!");
            }
            _Profile = value;
        }
    }
    */ // the profile is dead

    private List<ETGModuleMetadata> _Dependencies;
    /// <summary>
    /// The dependencies of the mod. In case of backends, this will return null.
    /// 
    /// Can only be set by ETGMod itself by default, unless you're having your own ETGModuleMetadata - extending type.
    /// </summary>
    public virtual ICollection<ETGModuleMetadata> Dependencies
    {
        get
        {
            if (_Dependencies == null)
            {
                return null;
            }
            return _Dependencies.AsReadOnly();
        }
        set
        {
            throw new InvalidOperationException("The ETGModuleMetadata dependency list is read-only!");
        }
    }

    public override string ToString()
    {
        return Name + " " + Version;
    }

    internal static ETGModuleMetadata Parse(string archive, string directory, Stream stream)
    {
        ETGModuleMetadata metadata = new ETGModuleMetadata();
        metadata._Directory = directory;
        metadata._Prelinked = false;
        //metadata._Profile = ETGMod.BaseProfile; // Works as if it were set to Release
        metadata._Dependencies = new List<ETGModuleMetadata>();

        using (StreamReader reader = new StreamReader(stream))
        {
            int lineN = -1;
            while (!reader.EndOfStream)
            {
                ++lineN;
                string line = reader.ReadLine();
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }
                line = line.Trim();
                if (line[0] == '#')
                {
                    continue;
                }
                if (!line.Contains(":"))
                {
                    Debug.LogWarning("INVALID METADATA LINE #" + lineN);
                    continue;
                }
                string[] data = line.Split(':');
                if (data.Length < 2)
                {
                    Debug.LogWarning("INVALID METADATA LINE #" + lineN);
                    continue;
                }
                if (2 < data.Length)
                {
                    StringBuilder newData = new StringBuilder();
                    for (int i = 1; i < data.Length; i++)
                    {
                        newData.Append(data[i]);
                        if (i < data.Length - 1)
                        {
                            newData.Append(':');
                        }
                    }
                    data = new string[] { data[0], newData.ToString() };
                }
                string prop = data[0].Trim();
                data[1] = data[1].Trim();

                if (prop == "Name")
                {
                    metadata._Name = data[1];

                }
                else if (prop == "Version")
                {
                    metadata._Version = new Version(data[1]);

                }
                else if (prop == "DLL")
                {
                    metadata._DLL = data[1].Replace("\\", "/");

                }
                else if (prop == "Prelinked")
                {
                    metadata._Prelinked = data[1].ToLowerInvariant() == "true";

                }
                else if (prop == "Depends" || prop == "Dependency")
                {
                    ETGModuleMetadata dep = new ETGModuleMetadata();
                    dep._Name = data[1];
                    dep._Version = new Version(0, 0);
                    if (data[1].Contains(" "))
                    {
                        string[] depData = data[1].Split(' ');
                        dep._Name = depData[0].Trim();
                        dep._Version = new Version(depData[1].Trim());
                    }
                    metadata._Dependencies.Add(dep);

                }
            }
        }

        // Set the DLL path to be absolute in folder mods if not already absolute
        if (!string.IsNullOrEmpty(directory) && !File.Exists(metadata._DLL))
        {
            metadata._DLL = Path.Combine(directory, metadata._DLL.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar));
        }

        // Add dependency to Base 1.0 if missing.
        bool dependsOnBase = false;
        foreach (ETGModuleMetadata dependency in metadata.Dependencies)
        {
            if (dependency.Name == "Base")
            {
                dependsOnBase = true;
                break;
            }
        }
        if (!dependsOnBase)
        {
            Debug.Log("WARNING: No dependency to Base found in " + metadata + "! Adding dependency to Base 1.0...");
            metadata._Dependencies.Insert(0, new ETGModuleMetadata()
            {
                _Name = "Base",
                _Version = new Version(1, 0)
            });
        }

        return metadata;
    }

}
