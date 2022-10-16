using System;
using System.Linq;
using HarmonyLib;
using Il2CppSystem.IO;
using MelonLoader;
using Sony.NP;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GrowableMoondewNectar.EntryPoint;
using Object = UnityEngine.Object;

namespace GrowableMoondewNectar
{
    [HarmonyPatch(typeof(GardenCatcher), nameof(GardenCatcher.Awake))]
    public static class PatchGardenCatcherAwake
    {
        public static void Prefix(GardenCatcher __instance)
        {
            __instance.plantable = __instance.plantable.AddItem(new GardenCatcher.PlantSlot()
            {
                plantedPrefab = EntryPoint.TreeMoonflower01.prefab,
                identType = EntryPoint.TreeMoonflower01.primaryResourceType,
                deluxePlantedPrefab = EntryPoint.TreeMoonflower01.prefab
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
            TreeMoonflower01.persistenceId = "patchNectar01Mod";
            resourceGrowerList.items.Add(TreeMoonflower01);
        }
    }

    public class EntryPoint : MelonMod
    {
        public static ResourceGrowerDefinition TreeMoonflower01;
        public static Transform DisabledGameObject;
        public static T GetOrDefault<T>(string name) where T : UnityEngine.Object => Resources.FindObjectsOfTypeAll<T>().FirstOrDefault(x => x.name == name);

        public override void OnInitializeMelon()
        {
            SystemContext.IsModded = true;
            DisabledGameObject = new GameObject("DisabledGameObject").transform;
            DisabledGameObject.gameObject.SetActive(false);
            Object.DontDestroyOnLoad(DisabledGameObject.gameObject);
            DisabledGameObject.gameObject.hideFlags |= HideFlags.HideAndDontSave;
            Action<Scene, LoadSceneMode> scene = (scene1, scene2) =>
            {
                if (!scene1.name.Equals("GameCore")) return;
                TreeMoonflower01.prefab = Object.Instantiate(GetOrDefault<ResourceGrowerDefinition>("TreeMoonflower01").prefab, EntryPoint.DisabledGameObject);
                TreeMoonflower01.primaryResourceType = GetOrDefault<IdentifiableType>("MoondewNectar");
                TreeMoonflower01.resources = new[]
                {
                    new ResourceSpawnerDefinition.WeightedResourceEntry()
                    {
                            weight = 1, minimumAmount = 5,
                            resourceIdentifiableType = TreeMoonflower01.primaryResourceType
                    }
                };
                var treeMoonflower01Prefab = TreeMoonflower01.prefab;
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
                spawnResource.resourceGrowerDefinition = TreeMoonflower01;
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
                spawnResource.resourceGrowerDefinition.maxResources = 5;
                spawnResource.resourceGrowerDefinition.minResources = 3;
            };
            SceneManager.add_sceneLoaded(scene);
        }
    }
}