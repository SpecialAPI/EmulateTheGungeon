using BepInEx;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

public static partial class ETGMod
{
    public readonly static Version BaseVersion = new Version(0, 3, 3);
    // The following line will be replaced by Travis. travis? travis who?
    public readonly static int BaseTravisBuild = 0;
    public static string BaseUIVersion
    {
        get
        {
            string v = BaseVersion.ToString(3);
            return v;
        }
    }

    public static T RunHook<T>(this MulticastDelegate md, T val, params object[] args)
    {
        if (md == null)
        {
            return val;
        }
        object[] array = new object[args.Length + 1];
        array[0] = val;
        Array.Copy(args, 0, array, 1, args.Length);
        Delegate[] invocationList = md.GetInvocationList();
        for (int i = 0; i < invocationList.Length; i++)
        {
            array[0] = invocationList[i].DynamicInvoke(array);
        }
        return (T)((object)array[0]);
    }


    public static string GameFolder = Paths.GameRootPath;
    public static string ModsDirectory = Paths.PluginPath;
    public static string ModsListFile = Path.Combine(Paths.PluginPath, "mods.txt");
    public static string RelinkCacheDirectory = Path.Combine(Paths.PluginPath, "RelinkCache");
    public static string ResourcesDirectory = Path.Combine(Paths.GameRootPath, "Resources");
    public static List<ETGModule> AllMods = new();
    public static List<ETGModule> GameMods = new();
    public static Func<IEnumerator, Coroutine> StartGlobalCoroutine;
    public static Action<Coroutine> StopGlobalCoroutine;
    public static bool Initialized => true;

    public class Databases
    {
        public static ItemDB Items;
        public static StringDB Strings;
    }

	public static class AIActor
    {
        public static Action<global::AIActor> OnPreStart;
        public static Action<global::AIActor> OnPostStart;
        public static Action<global::AIActor> OnBlackPhantomnessCheck;
    }

    public static class Chest
    {
        public static Action<global::Chest> OnPostSpawn;
        public static DOnPreOpen OnPreOpen;
        public static Action<global::Chest, PlayerController> OnPostOpen;
        public delegate bool DOnPreOpen(bool shouldOpen, global::Chest chest, PlayerController player);
    }

    public class Assets
    {
        public readonly static Type t_Object = typeof(UnityEngine.Object);
        public readonly static Type t_AssetDirectory = typeof(AssetDirectory);
        public readonly static Type t_Texture = typeof(Texture);
        public readonly static Type t_Texture2D = typeof(Texture2D);
        public readonly static Type t_tk2dSpriteCollectionData = typeof(tk2dSpriteCollectionData);
        public readonly static Type t_tk2dSpriteDefinition = typeof(tk2dSpriteDefinition);

        /// <summary>
        /// Asset map. All string - AssetMetadata entries here will cause an asset to be remapped. Use ETGMod.Assets.AddMapping to add an entry.
        /// </summary>
        public readonly static Dictionary<string, AssetMetadata> Map = new(StringComparer.InvariantCultureIgnoreCase);
        /// <summary>
        /// Directories that would not fit into Map due to conflicts.
        /// </summary>
        public readonly static Dictionary<string, AssetMetadata> MapDirs = new(StringComparer.InvariantCultureIgnoreCase);
        /// <summary>
        /// Texture remappings. This dictionary starts empty and will be filled as sprites get replaced. Feel free to add your own remapping here.
        /// </summary>
        public static Dictionary<string, Texture2D> TextureMap;

        public static bool DumpResources = false;

        public static bool DumpSprites = false;

        public static bool DumpSpritesMetadata = false;

        public static bool EnabledLegacyFileSystemTextureMapping = false;
        public static Shader DefaultSpriteShader;
        public static RuntimeAtlasPacker Packer;

        public static Vector2[] GenerateUVs(Texture2D texture, int x, int y, int width, int height)
        {
            return new Vector2[] {
                new Vector2((x        ) / (float) texture.width, (y         ) / (float) texture.height),
                new Vector2((x + width) / (float) texture.width, (y         ) / (float) texture.height),
                new Vector2((x        ) / (float) texture.width, (y + height) / (float) texture.height),
                new Vector2((x + width) / (float) texture.width, (y + height) / (float) texture.height),
            };
        }

        public static bool TryGetMapped(string path, out AssetMetadata metadata, bool includeDirs = false)
        {
            if (includeDirs)
            {
                if (MapDirs.TryGetValue(path, out metadata))
                    return true;
            }

            if (Map.TryGetValue(path, out metadata))
                return true;

            return false;
        }
        public static AssetMetadata GetMapped(string path)
        {
            AssetMetadata metadata;
            TryGetMapped(path, out metadata);
            return metadata;
        }

        public static AssetMetadata AddMapping(string path, AssetMetadata metadata)
        {
            return Map[path] = metadata;
        }

        public static string RemoveExtension(string file, out Type type)
        {
            type = t_Object;

            if (EndsWithInvariant(file, ".png"))
            {
                type = t_Texture2D;
                file = file.Substring(0, file.Length - 4);
            }

            return file;
        }

        public static void Crawl(string dir, string root = null)
        {
        }

        public static void Crawl(Assembly asm)
        {
        }

        public static void HookUnity()
        {
            if (!Directory.Exists(ResourcesDirectory))
            {
                Debug.Log("Resources directory not existing, creating...");
                Directory.CreateDirectory(ResourcesDirectory);
            }

            string spritesDir = Path.Combine(ResourcesDirectory, "sprites");
            if (!Directory.Exists(spritesDir))
            {
                Debug.Log("Sprites directory not existing, creating...");
                Directory.CreateDirectory(spritesDir);
            }

            DefaultSpriteShader = Shader.Find("tk2d/BlendVertexColor");
        }

        public static UnityEngine.Object Load(string path, Type type)
        {
            return null;
        }

        public static void HandleSprites(tk2dSpriteCollectionData sprites)
        {
        }

        public static void HandleDfAtlas(dfAtlas atlas)
        {
        }

        public static void ReplaceTexture(tk2dSpriteDefinition frame, Texture2D replacement, bool pack = true)
        {
            InvokeStaticMTGMethod("ETGMod+Assets", "ReplaceTexture", frame, replacement);
        }
    }

    public static void Handle(this tk2dBaseSprite sprite)
    {
        Assets.HandleSprites(sprite.Collection);
    }

    public static void HandleAuto(this tk2dBaseSprite sprite)
    {
    }

    public static void Handle(this tk2dSpriteCollectionData sprites)
    {
        Assets.HandleSprites(sprites);
    }

    public static void Handle(this dfAtlas atlas)
    {
        Assets.HandleDfAtlas(atlas);
    }
    public static void HandleAuto(this dfAtlas atlas)
    {
    }

    public static void MapAssets(this Assembly asm)
    {
        Assets.Crawl(asm);
    }

    public static void ReplaceTexture(this tk2dSpriteDefinition frame, Texture2D replacement, bool pack = true)
    {
        Assets.ReplaceTexture(frame, replacement, pack);
    }
}
