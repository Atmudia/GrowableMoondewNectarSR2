using System.Linq;
using GrowableMoondewNectarMod;
using HarmonyLib;
using Il2Cpp;
using MelonLoader;
using UnityEngine;
using static GrowableMoondewNectarMod.EntryPoint;
using Object = UnityEngine.Object;

[assembly: MelonInfo(typeof(EntryPoint), "Growable Moondew Nectar", "1.0.2 ", "KomiksPL", "https://www.nexusmods.com/slimerancher2/mods/5")]
[assembly: MelonGame("MonomiPark", "SlimeRancher2")]
namespace GrowableMoondewNectarMod
{
    [HarmonyPatch(typeof(GardenCatcher), nameof(GardenCatcher.Awake))]
    public static class PatchGardenCatcherAwake
    {
        public static void Prefix(GardenCatcher __instance)
        {
            
            __instance.Plantable = __instance.Plantable.AddItem(new GardenCatcher.PlantSlot()
            {
                PlantedPrefab = EntryPoint.TreeMoonflower01.Prefab,
                IdentType = EntryPoint.TreeMoonflower01.PrimaryResourceType,
                DeluxePlantedPrefab = EntryPoint.TreeMoonflower01.Prefab
            }).ToArray();

        }
    }

    [HarmonyPatch(typeof(AutoSaveDirector), nameof(AutoSaveDirector.Awake))]
    public static class PatchAutoSaveDirectorAwake
    {
        public static void Prefix()
        {
            ResourceGrowerList resourceGrowerList = GetOrDefault<ResourceGrowerList>("ResourceGrowers");
            TreeMoonflower01 = ScriptableObject.CreateInstance<ResourceGrowerDefinition>();
            TreeMoonflower01.name = "PatchNectar01Mod";
            TreeMoonflower01._persistenceId = "patchNectar01Mod";
            resourceGrowerList.items.Add(TreeMoonflower01);
        }
    }

    public class EntryPoint : MelonMod
    {
        public static ResourceGrowerDefinition TreeMoonflower01;
        public static Transform RuntimeObject;
        public static T GetOrDefault<T>(string name) where T : UnityEngine.Object => Resources.FindObjectsOfTypeAll<T>().FirstOrDefault(x => x.name == name);

        public override void OnInitializeMelon()
        {
            SystemContext.IsModded = true;
            RuntimeObject = new GameObject("RuntimeObject").transform;
            RuntimeObject.gameObject.SetActive(false);
            Object.DontDestroyOnLoad(RuntimeObject.gameObject);
            RuntimeObject.gameObject.hideFlags |= HideFlags.HideAndDontSave;
        }

        public override void OnSceneWasLoaded(int buildIndex, string scene1)
        {
            if (!scene1.Equals("GameCore")) return;
            TreeMoonflower01._prefab = Object.Instantiate(GetOrDefault<ResourceGrowerDefinition>("TreeMoonflower01").Prefab, EntryPoint.RuntimeObject);
            TreeMoonflower01._primaryResourceType = GetOrDefault<IdentifiableType>("MoondewNectar");
            TreeMoonflower01._resources = new[]
            {
                new ResourceSpawnerDefinition.WeightedResourceEntry()
                {
                    Weight = 1, MinimumAmount = 5,
                    ResourceIdentifiableType = TreeMoonflower01.PrimaryResourceType
                }
            };
            var treeMoonflower01Prefab = TreeMoonflower01._prefab;
            var spawnResourceInChildren = treeMoonflower01Prefab.GetComponentInChildren<SpawnResource>();
            SlimeVarietyModulesStatic.GetCopyOf(treeMoonflower01Prefab.AddComponent<SpawnResource>(), spawnResourceInChildren);
            Object.Destroy(spawnResourceInChildren);
                    
            var treeMesh = treeMoonflower01Prefab.transform.Find("treeMesh").gameObject;
            var localPositionY = treeMesh.transform.localPosition.y;
            Object.Instantiate(treeMesh, treeMesh.transform.parent).transform.localPosition = new Vector3(3.75f, localPositionY, -3.75f);
            Object.Instantiate(treeMesh, treeMesh.transform.parent).transform.localPosition = new Vector3(-3.75f, localPositionY, 3.75f);
            Object.Instantiate(treeMesh, treeMesh.transform.parent).transform.localPosition = new Vector3(-3.75f, localPositionY, -3.75f);
                    
            Object.Instantiate(treeMesh, treeMesh.transform.parent).transform.localPosition = new Vector3(3.75f, localPositionY, 3.75f);
            var spawnResource = treeMoonflower01Prefab.GetComponent<SpawnResource>();
            spawnResource._resourceGrowerDefinition = TreeMoonflower01;
            var spawnJoint = spawnResource.SpawnJoints[0];
            var spawnJoint1 = Object.Instantiate(spawnJoint.gameObject, spawnJoint.transform.parent);
            spawnJoint1.transform.localPosition = new Vector3(3.75f, 0, -3.75f);
            var spawnJoint2 = Object.Instantiate(spawnJoint.gameObject, spawnJoint.transform.parent);
            spawnJoint2.transform.localPosition = new Vector3(-3.75f, 0, 3.75f);
            var spawnJoint3 = Object.Instantiate(spawnJoint.gameObject, spawnJoint.transform.parent);
            spawnJoint3.transform.localPosition = new Vector3(-3.75f, 0, -3.75f);
            var spawnJoint4 = Object.Instantiate(spawnJoint.gameObject, spawnJoint.transform.parent);
            spawnJoint4.transform.localPosition = new Vector3(3.75f, 0, 3.75f);

            Joint[] joints = {
                spawnJoint, spawnJoint1.GetComponent<Joint>(), spawnJoint2.GetComponent<Joint>(), spawnJoint3.GetComponent<Joint>(), spawnJoint4.GetComponent<Joint>()
            };
            spawnResource.SpawnJoints = joints;
            spawnResource._resourceGrowerDefinition._maxResources = 5;
            spawnResource._resourceGrowerDefinition._minResources = 3;
        }
    }
}