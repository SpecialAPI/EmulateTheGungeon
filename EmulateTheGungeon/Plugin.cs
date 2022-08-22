using BepInEx;
using BepInEx.Bootstrap;
using Gungeon;
using HarmonyLib;
using Mono.Cecil;
using MonoMod.RuntimeDetour;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace EmulateTheGungeon
{
    [BepInPlugin(GUID, NAME, VERSION)]
    [BepInDependency("etgmodding.etg.mtgapi")]
    [HarmonyPatch]
    public class Plugin : BaseUnityPlugin
    {
        public const string GUID = "spapi.etg.emtg";
        public const string NAME = "Emulate the Gungeon";
        public const string VERSION = "1.0.0";
        public static Harmony harmony;

        public void Awake()
        {
            (harmony = new Harmony(GUID)).PatchAll();
        }

        [HarmonyPatch(typeof(Type), nameof(Type.GetMethod), typeof(string), typeof(BindingFlags))]
        [HarmonyPostfix]
        public static void FixOrig(Type __instance, ref MethodInfo __result, string name, BindingFlags bindingAttr)
        {
            if(__result == null && name.Contains("orig_"))
            {
                __result = __instance.GetMethod(name.Replace("orig_", ""), bindingAttr); //sure hope this doesnt break everything
            }
        }

        public void Start()
        {
            var ionic = Path.Combine(Path.Combine(Info.Location, ".."), "Ionic.Zip.dll");
            if (File.Exists(ionic))
            {
                try
                {
                    Assembly.LoadFile(ionic);
                }
                catch
                {
                }
            }
            ETGModMainBehaviour.Instance = Chainloader.ManagerObject.AddComponent<ETGModMainBehaviour>();
            ETGModConsole.Commands = new(GetStaticMTGField("ETGModConsole", "Commands"));
            ETGMod.StartGlobalCoroutine = (Func<IEnumerator, Coroutine>)GetStaticMTGField("ETGMod", "StartGlobalCoroutine");
            ETGMod.StopGlobalCoroutine = (Action<Coroutine>)GetStaticMTGField("ETGMod", "StopGlobalCoroutine");
            Game.Items = new(InvokeStaticMTGMethod("Gungeon.Game", "get_Items"));
            Game.Enemies = new(InvokeStaticMTGMethod("Gungeon.Game", "get_Enemies"));
            ETGMod.Databases.Items = new(GetStaticMTGField("ETGMod+Databases", "Items"));
            ETGMod.Databases.Strings = new(GetStaticMTGField("ETGMod+Databases", "Strings"));
            ETGMod.Assets.Packer = new(GetStaticMTGField("ETGMod+Assets", "Packer"));
            ETGMod.Assets.TextureMap = (Dictionary<string, Texture2D>)GetStaticMTGField("ETGMod+Assets", "TextureMap");
            ETGMod.Assets.HookUnity();
            var plinfos = Chainloader.PluginInfos.Values.ToList();
            List<string> validFiles = new();
            Logger.LogInfo("EMTG: Finding mods...");
            var execAsmb = Assembly.GetExecutingAssembly();
            foreach (var file in Directory.GetFiles(Paths.PluginPath, "*.dll", SearchOption.AllDirectories))
            {
                var fileName = file.Substring(file.LastIndexOf(Path.DirectorySeparatorChar) + 1);
                try
                {
                    if (plinfos.Exists(x => x.Location == file))
                    {
                        continue;
                    }
                    AssemblyDefinition def = AssemblyDefinition.ReadAssembly(file, TypeLoader.ReaderParameters);
                    if (def.MainModule.GetTypeReferences().All(r => r.FullName != typeof(ETGModule).FullName))
                    {
                        def.Dispose();
                        continue;
                    }
                    if(def.MainModule.AssemblyReferences.All(r => r.FullName != execAsmb.FullName))
                    {
                        def.Dispose();
                        continue;
                    }
                    def.MainModule.GetTypeReferences().Do(x =>
                    {
                        if(x == null)
                        {
                            return;
                        }
                        var assembly = AppDomain.CurrentDomain.GetAssemblies().ToList();
                        assembly.Remove(mtgAssembly);
                        assembly.RemoveAll(x => x.FullName == mtgAssembly.FullName);
                        var type = assembly.Select(x => x.GetTypes()).SelectMany(x => x).ToList().Find(x2 => x2 != null && x2.FullName.Replace("+", "/") == x.FullName);
                        if (type == null)
                        {
                            Logger.LogError("NONEXISTANT TYPE REFERENCE " + x.FullName);
                        }
                    });
                    Logger.LogInfo("Found valid file: " + fileName);
                    validFiles.Add(file);
                    def.Dispose();
                }
                catch(Exception ex)
                {
                    Logger.LogError("Skipping " + fileName + " because exception: " + ex);
                }
            }
            Logger.LogInfo("Done! " + validFiles.Count + " mods found.");
            Logger.LogInfo("Initializing mods...");
            int loadedmods = 0;
            foreach(var file in validFiles)
            {
                var fileName = file.Substring(file.LastIndexOf(Path.DirectorySeparatorChar) + 1);
                try
                {
                    var folder = Path.Combine(file, "..");
                    var metadataLocation = Path.Combine(folder, "metadata.txt");
                    if (!File.Exists(metadataLocation))
                    {
                        continue;
                    }
                    var spritesFolderLocation = Path.Combine(folder, "sprites");
                    var spritesFolderExists = Directory.Exists(spritesFolderLocation);
                    ETGModuleMetadata metadata;
                    using (var s = File.OpenRead(metadataLocation))
                    {
                        metadata = ETGModuleMetadata.Parse("", folder, s);
                    }
                    var asmb = Assembly.LoadFile(file);
                    var types = asmb.GetTypes();
                    var typeNames = types.Select(x => x.FullName).ToList();
                    typeNames.Sort();
                    types = typeNames.Select(x => types.Where(x2 => x2.FullName == x).First()).ToArray();
                    if (types.Length > 0)
                    {
                        foreach (var type in types)
                        {
                            if(type.Name == "MultiActiveReloadManager")
                            {
                                MethodInfo method = null;
                                MethodInfo method1 = null;
                                MethodInfo method2 = null;
                                MethodInfo method3 = null;
                                try
                                {
                                    method = type.GetMethod("SetupHooks", new Type[] { });
                                    method1 = type.GetMethod("TriggerReloadHook");
                                    method2 = type.GetMethod("OnActiveReloadPressedHook", new Type[] { typeof(Action<Gun, PlayerController, Gun, bool>), typeof(Gun), typeof(PlayerController), typeof(Gun), typeof(bool) });
                                    method3 = type.GetMethod("ReloadHook", new Type[] { typeof(Func<Gun, bool>), typeof(Gun) });
                                }
                                catch { }
                                if(method != null && method1 != null && method2 != null && method3 != null)
                                {
                                    multiactiveReloadTypes.Add(type);
                                    new Hook(method, typeof(Plugin).GetMethod(nameof(Plugin.SetupHooks)));
                                }
                            }
                            if (!type.IsSubclassOf(typeof(ETGModule)))
                            {
                                continue;
                            }
                            Logger.LogInfo("Found module: " + type.FullName);
                            var guid = $"emtg.etg.emulatedmod_{loadedmods}";
                            loadedmods++;
                            var module = Activator.CreateInstance(type) as ETGModule;
                            module.Metadata = metadata;
                            PluginInfo plinfo = new();
                            var t = typeof(PluginInfo);
                            var f = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
                            t.GetProperty(nameof(PluginInfo.Metadata), f).SetValue(plinfo, new BepInPlugin(guid, metadata.Name, metadata.Version.ToString()), new object[0]);
                            t.GetProperty(nameof(PluginInfo.Processes), f).SetValue(plinfo, new List<BepInProcess>(), new object[0]);
                            t.GetProperty(nameof(PluginInfo.Dependencies), f).SetValue(plinfo, new List<BepInDependency>(), new object[0]);
                            t.GetProperty(nameof(PluginInfo.Incompatibilities), f).SetValue(plinfo, new List<BepInIncompatibility>(), new object[0]);
                            t.GetProperty(nameof(PluginInfo.Location), f).SetValue(plinfo, file, new object[0]);
                            t.GetProperty(nameof(PluginInfo.Instance), f).SetValue(plinfo, this, new object[0]);
                            Chainloader.PluginInfos[guid] = plinfo;
                            InvokeStaticMTGMethod("ETGModMainBehaviour", "WaitForGameManagerAwake", (GameManager x) =>
                            {
                                try
                                {
                                    module?.Init();
                                }
                                catch (Exception ex)
                                {
                                    InvokeStaticMTGMethod("ETGModConsole", "Log", $"Failed initializing module {module?.GetType().Name ?? "Null"}. Error: " + ex, false);
                                }
                            });
                            InvokeStaticMTGMethod("ETGModMainBehaviour", "WaitForGameManagerAwake", (GameManager x) =>
                            {
                                try
                                {
                                    module?.Start();
                                    if (spritesFolderExists) // mtg api "broke" projectile sprites (by not being absolute garbage) so i need to do this to fix them
                                    {
                                        ETGMod.Databases.Items.ProjectileCollection.spriteNameLookupDict = null;
                                        ((List<string>)GetMTGType("ETGMod+Assets").GetField("processedFolders", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null)).Remove(spritesFolderLocation);
                                        InvokeStaticMTGMethod("ETGMod+Assets", "SetupSpritesFromFolder", spritesFolderLocation);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    InvokeStaticMTGMethod("ETGModConsole", "Log", $"Failed starting module {module?.GetType().Name ?? "Null"}. Error: " + ex, false);
                                }
                            });
                            Logger.LogInfo("Module " + type.FullName + " loaded successfully");
                        }
                    }
                    else
                    {
                        Logger.LogError("No ETGModule type found.");
                    }
                    Logger.LogInfo("Successfully loaded file " + fileName);
                }
                catch(Exception ex)
                {
                    Logger.LogError("An error occured while loading file " + fileName + ": " + ex);
                }
            }
            SetStaticMTGField("ETGMod+AIActor", "OnPreStart", Delegate.Combine((Delegate)GetStaticMTGField("ETGMod+AIActor", "OnPreStart"), (AIActor x) => ETGMod.AIActor.OnPreStart?.Invoke(x)));
            SetStaticMTGField("ETGMod+AIActor", "OnPostStart", Delegate.Combine((Delegate)GetStaticMTGField("ETGMod+AIActor", "OnPostStart"), (AIActor x) => ETGMod.AIActor.OnPostStart?.Invoke(x)));
            SetStaticMTGField("ETGMod+AIActor", "OnBlackPhantomnessCheck", Delegate.Combine((Delegate)GetStaticMTGField("ETGMod+AIActor", "OnBlackPhantomnessCheck"), (AIActor x) => ETGMod.AIActor.OnBlackPhantomnessCheck?.Invoke(x)));
            SetStaticMTGField("ETGMod+Chest", "OnPostSpawn", Delegate.Combine((Delegate)GetStaticMTGField("ETGMod+Chest", "OnPostSpawn"), (Chest x) => ETGMod.Chest.OnPostSpawn?.Invoke(x)));
            SetStaticMTGField("ETGMod+Chest", "OnPreOpen", Activator.CreateInstance(GetMTGType("ETGMod+Chest+DOnPreOpen"), Delegate.Combine((Delegate)GetStaticMTGField("ETGMod+Chest", "OnPreOpen"),
                (bool shouldOpen, Chest chest, PlayerController player) => ETGMod.RunHook(ETGMod.Chest.OnPreOpen, shouldOpen, shouldOpen, chest, player))));
            SetStaticMTGField("ETGMod+Chest", "OnPostOpen", Delegate.Combine((Delegate)GetStaticMTGField("ETGMod+Chest", "OnPostOpen"), (Chest x, PlayerController x2) => ETGMod.Chest.OnPostOpen?.Invoke(x, x2)));
            Logger.LogInfo("Done! " + loadedmods + " mods loaded.");
        }

        public static void SetupHooks(Action orig)
        {
            var type = multiactiveReloadTypes.FirstOrDefault();
            if (type == null)
            {
                return;
            }
            multiactiveReloadTypes.RemoveAt(0);
            var method1 = type.GetMethod("TriggerReloadHook");
            var method3 = type.GetMethod("ReloadHook", new Type[] { typeof(Func<Gun, bool>), typeof(Gun) });
            Hook hook = new Hook(typeof(GameUIReloadBarController).GetMethod("TriggerReload", BindingFlags.Instance | BindingFlags.Public), method1);
            Hook hook3 = new Hook(typeof(Gun).GetMethod("Reload", BindingFlags.Instance | BindingFlags.Public), method3);
        }

        public static List<Type> multiactiveReloadTypes = new();
        public delegate void Action<T, T2, T3, T4, T5>(T arg1, T2 arg2, T3 arg3, T4 arg4);
    }
}
