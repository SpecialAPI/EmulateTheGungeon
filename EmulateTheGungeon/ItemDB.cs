using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ItemDB
{
    public object representedObject;

    public PickupObject this[int id]
    {
        get
        {
            return PickupObjectDatabase.Instance.InternalGetById(id);
        }
        set
        {
            PickupObject old = PickupObjectDatabase.Instance.Objects[id];
            if (old != null)
            {
                old.PickupObjectId = -1;
            }
            if (value != null)
            {
                value.PickupObjectId = id;
                value.gameObject.SetActive(true);
            }
            PickupObjectDatabase.Instance.Objects[id] = value;
        }
    }

    // Example name: "gshbd"
    // NOT "Test Gun"
    public PickupObject this[string name]
    {
        get
        {
            return PickupObjectDatabase.Instance.InternalGetByName(name);
        }
    }

    public int Count
    {
        get
        {
            return PickupObjectDatabase.Instance.Objects.Count;
        }
    }

    public List<PickupObject> ModItems;
    public Dictionary<string, PickupObject> ModItemMap;
    public Dictionary<string, List<WeightedGameObject>> ModLootPerFloor;
    public tk2dSpriteCollectionData WeaponCollection;
    public tk2dSpriteCollectionData WeaponCollection02;
    public tk2dSpriteCollectionData ProjectileCollection;
    public tk2dSpriteCollectionData ItemCollection;

    public ItemDB(object obj)
    {
        representedObject = obj;
        ModItems = (List<PickupObject>)obj.GetField("ModItems");
        ModItemMap = (Dictionary<string, PickupObject>)obj.GetField("ModItemMap");
        ModLootPerFloor = (Dictionary<string, List<WeightedGameObject>>)obj.GetField("ModLootPerFloor");
        WeaponCollection = (tk2dSpriteCollectionData)obj.GetField("WeaponCollection");
        WeaponCollection02 = (tk2dSpriteCollectionData)obj.GetField("WeaponCollection02");
        ProjectileCollection = (tk2dSpriteCollectionData)obj.GetField("ProjectileCollection");
        ItemCollection = (tk2dSpriteCollectionData)obj.GetField("ItemCollection");
    }

    public int Add(Gun value, tk2dSpriteCollectionData collection = null, string floor = "ANY")
    {
        return Add(value, false, floor);
    }
    public int Add(PickupObject value, bool updateSpriteCollections = false, string floor = "ANY")
    {
        return (int)representedObject.InvokeMethod("Add", value, updateSpriteCollections, floor);
    }

    public void DungeonStart()
    {
    }

    public Gun NewGun(string gunName, string gunNameShort = null)
    {
        return (Gun)representedObject.InvokeMethod("NewGun", gunName, gunNameShort);
    }
    public Gun NewGun(string gunName, Gun baseGun, string gunNameShort = null)
    {
        return (Gun)representedObject.InvokeMethod("NewGun", gunName, baseGun, gunNameShort);
    }
    //FIXME NewGun<> causing issues (MonoMod) //shut up zatherz
    /*
    public Gun NewGun<T>(string gunName, string gunNameShort = null) where T : GunBehaviour {
        Gun gun = NewGun(gunName, gunNameShort);
        gun.gameObject.AddComponent<T>();
        return gun;
    }
    public Gun NewGun<T>(string gunName, Gun baseGun, string gunNameShort = null) where T : GunBehaviour {
        Gun gun = NewGun(gunName, baseGun, gunNameShort);
        gun.gameObject.AddComponent<T>();
        return gun;
    }
    */

    public void SetupItem(PickupObject item, string name)
    {
        representedObject.InvokeMethod("SetupItem", item, name);
    }

    public PickupObject GetModItemByName(string name)
    {
        return (PickupObject)representedObject.InvokeMethod("GetModItemByName", name);
    }

}
