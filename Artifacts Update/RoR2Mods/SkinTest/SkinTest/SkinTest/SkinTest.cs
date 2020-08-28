using BepInEx;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.Skills;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace SkinTest
{
    [R2APISubmoduleDependency(nameof(PrefabAPI), nameof(LoadoutAPI), nameof(SurvivorAPI), nameof(LanguageAPI), nameof(ResourcesAPI))]
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.KingEnderBrine.SkinTest", "SkinTest", "1.0.0")]

    public class SkinTest : BaseUnityPlugin
    {
        private void Awake()
        {

            using (var assetStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("SkinTest.skintest"))
            {
                var MainAssetBundle = AssetBundle.LoadFromStream(assetStream);
                ResourcesAPI.AddProvider(new AssetBundleResourcesProvider("@SkinTest", MainAssetBundle));
            }

            var commandoPrefab = Resources.Load<GameObject>("prefabs/characterbodies/CommandoBody").InstantiateClone("CommandoCopy");
            var commandoBody = commandoPrefab.GetComponent<CharacterBody>();
            commandoBody.baseNameToken = "test";

            Debug.Log(string.Join(", ", commandoPrefab.GetComponentInChildren<SkinnedMeshRenderer>().bones.Select(el => "'" + el.name + "'")));
            BodyCatalog.getAdditionalEntries += (list) => list.Add(commandoPrefab);

            var mySurvivorDef = new SurvivorDef
            {
                name = "Test commando" + Environment.NewLine,
                bodyPrefab = commandoPrefab,
                descriptionToken = "Test commando",
                displayPrefab = Resources.Load<GameObject>("prefabs/characterdisplays/CommandoDisplay").InstantiateClone("CommandoTestDisplay", false),
                primaryColor = new Color(0.8039216f, 0.482352942f, 0.843137264f),
                unlockableName = "",
            };
            SurvivorAPI.AddSurvivor(mySurvivorDef);

            var skinController = commandoPrefab.GetComponentInChildren<ModelSkinController>();
            var mdl = skinController.gameObject;
            var renderer = mdl.GetComponentInChildren<SkinnedMeshRenderer>();
            var renderers = mdl.transform.GetChild(0).GetComponentsInChildren<Renderer>();

            var altSkin = new LoadoutAPI.SkinDefInfo();
            altSkin.Name = "ComandoCustomAlt";
            altSkin.NameToken = "TEST_SKIN";
            altSkin.RootObject = mdl;
            altSkin.BaseSkins = new SkinDef[0];// { skinController.skins[0] };
            altSkin.UnlockableName = "";
            altSkin.RendererInfos = new CharacterModel.RendererInfo[]
            {
                new CharacterModel.RendererInfo()
                {
                    defaultMaterial = Resources.Load<Material>("@SkinTest:Assets/Resources/matMercAlt.mat"),
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    ignoreOverlays = false,
                    renderer = renderer
                },
                new CharacterModel.RendererInfo()
                {
                    defaultMaterial = Resources.Load<Material>("@SkinTest:Assets/Resources/matMercAlt.mat"),
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    ignoreOverlays = false,
                    renderer = renderers[0]
                },
                new CharacterModel.RendererInfo()
                {
                    defaultMaterial = Resources.Load<Material>("@SkinTest:Assets/Resources/matMercAlt.mat"),
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    ignoreOverlays = false,
                    renderer = renderers[1]
                }
            };
            altSkin.GameObjectActivations = new SkinDef.GameObjectActivation[0];
            //altSkin.MinionSkinReplacements = new SkinDef.MinionSkinReplacement[0];
            //altSkin.ProjectileGhostReplacements = new SkinDef.ProjectileGhostReplacement[0];
            altSkin.MeshReplacements = new SkinDef.MeshReplacement[]
            {
                new SkinDef.MeshReplacement()
                {
                    mesh = Resources.Load<Mesh>("@SkinTest:Assets/Resources/skintest.blend"),
                    renderer = renderer
                }
            }; 

            Array.Resize(ref skinController.skins, skinController.skins.Length + 1);
            skinController.skins[skinController.skins.Length - 1] = LoadoutAPI.CreateNewSkinDef(altSkin);
        }
    }
}