using BepInEx;
using BepInEx.Configuration;
using R2API;
using R2API.Utils;
using RoR2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace PallesenProductions
{
    [R2APISubmoduleDependency(nameof(R2API.Utils.CommandHelper))]
    [R2APISubmoduleDependency("LoadoutAPI")]
    [R2APISubmoduleDependency("EntityAPI")]
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.PallesenProductions.RyanSkinAPI", "RyanSkinAPI", "3.4.1")]

    public class RyanSkinAPI : BaseUnityPlugin
    {
        public static RyanSkinAPI instance;

        public class Skin
        {

            public string bodyName;//name of the character to apply this to
            public int meshReplacement;//default skin to use as base
            public string skinName;//name of this skin

            public Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();//list of textures and where they go to
            public SkinDef.MeshReplacement[] meshReplacements;//list of mesh replacements
        }

        public struct PendingSkin
        {
            public string bodyName;//name of the character to apply this to
            public string SkinFolder;//path to the Skin.CFG for this skin
            public ConfigFile config;//the skin.cfg for this skin
        }

        public Dictionary<string, List<PendingSkin>> pendingSkins = new Dictionary<string, List<PendingSkin>>();

        private List<string> floatProperties = new List<string>();
        private List<string> colorProperties = new List<string>();

        public GameObject displayBody;
        public void Awake()
        {
            instance = this;
            SetMaterialPropertyList();

            On.RoR2.Loadout.BodyLoadoutManager.Init += (orig) =>
            {


                //Now that all characters should have been added, including custom characters.

                SkinSetup();

                LoadAllSkins();

                ApplySkins();

                typeof(SkinCatalog).GetMethod("Init", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[0]);

                orig();
            };

            On.RoR2.Util.BuildPrefabTransformPath += (orig, root, transform, includeclone) =>
            {
                if (transform.parent == null)
                {
                    return transform.name;
                }

                return orig(root, transform, includeclone);

            };

            R2API.Utils.CommandHelper.AddToConsoleWhenReady();


        }

        [ConCommand(commandName = "DumpSurvivorSkins", flags = ConVarFlags.None, helpText = "Dumps all default skins for all survivors.")]
        private static void DumpSurvivorSkins(ConCommandArgs args)
        {
            RyanSkinAPI.instance.DumpDefaultSkins();
        }
        [ConCommand(commandName = "DumpAllSkins", flags = ConVarFlags.None, helpText = "Dumps all default skins for all characters.")]
        private static void DumpAllSkins(ConCommandArgs args)
        {
            RyanSkinAPI.instance.DumpAllSkins();
        }
        public void SetMaterialPropertyList()
        {
            floatProperties.Add("_Cull");
            floatProperties.Add("_BlueChannelBias");
            floatProperties.Add("_BlueChannelSmoothness");
            floatProperties.Add("_ColorsOn");
            floatProperties.Add("_DecalLayer");
            floatProperties.Add("_Depth");
            floatProperties.Add("_DitherOn");
            floatProperties.Add("_EliteBrightnessMax");
            floatProperties.Add("_EliteBrightnessMin");
            floatProperties.Add("_EliteIndex");
            floatProperties.Add("_EmPower");
            floatProperties.Add("_EnableCutout");
            floatProperties.Add("_Fade");
            floatProperties.Add("_FadeBias");
            floatProperties.Add("_FlowEmissionStrength");
            floatProperties.Add("_FlowHeightBias");
            floatProperties.Add("_FlowHeightPower");
            floatProperties.Add("_FlowMaskStrength");
            floatProperties.Add("_FlowNormalStrength");
            floatProperties.Add("_FlowSpeed");
            floatProperties.Add("_FlowTextureScaleFactor");
            floatProperties.Add("_FlowmapOn");
            floatProperties.Add("_ForceSpecOn");
            floatProperties.Add("_FresnelBoost");
            floatProperties.Add("_FresnelPower");
            floatProperties.Add("_GreenChannelBias");
            floatProperties.Add("_GreenChannelSmoothness");
            floatProperties.Add("_LimbPrimeMask");
            floatProperties.Add("_LimbRemovalOn");
            floatProperties.Add("_NormalStrength");
            floatProperties.Add("_PrintBias");
            floatProperties.Add("_PrintBoost");
            floatProperties.Add("_PrintDirection");
            floatProperties.Add("_PrintEmissionToAlbedoLerp");
            floatProperties.Add("_PrintOn");
            floatProperties.Add("_RampInfo");
            floatProperties.Add("_SliceAlphaDepth");
            floatProperties.Add("_SliceBandHeight");
            floatProperties.Add("_SliceHeight");
            floatProperties.Add("_Smoothness");
            floatProperties.Add("_SpecularExponent");
            floatProperties.Add("_SpecularStrength");
            floatProperties.Add("_SplatmapOn");
            floatProperties.Add("_SplatmapTileScale");
            floatProperties.Add("_FEON");
            floatProperties.Add("_SrcBlend");
            floatProperties.Add("_DstBlend");
            floatProperties.Add("_InvFade");
            floatProperties.Add("_AlphaBias");
            floatProperties.Add("_AlphaBoost");
            floatProperties.Add("_Boost");
            floatProperties.Add("_CalcTextureAlphaOn");
            floatProperties.Add("_CloudOffsetOn");
            floatProperties.Add("_CloudsOn");
            floatProperties.Add("_DisableRemapOn");
            floatProperties.Add("_ExternalAlpha");
            floatProperties.Add("_FadeCloseDistance");
            floatProperties.Add("_FadeCloseOn");
            floatProperties.Add("_FresnelOn");
            floatProperties.Add("_InternalSimpleBlendMode");
            floatProperties.Add("_OffsetAmount");
            floatProperties.Add("_UseUV1On");
            floatProperties.Add("_VertexAlphaOn");
            floatProperties.Add("_VertexColorOn");
            floatProperties.Add("_VertexOffsetOn");
            colorProperties.Add("_EmColor");
        }

        public void SkinSetup()
        {
            //Iterate through all characters and add default skins.
            foreach (SurvivorDef def in SurvivorCatalog.allSurvivorDefs)
            {
                if (SkinsAreApplicable(def.bodyPrefab))
                {
                    //Add default skins to characters without skins
                    DefaultSetup(def.bodyPrefab);
                }

            }

        }

        public bool SkinsAreApplicable(GameObject body)
        {
            if (SurvivorCatalog.FindSurvivorDefFromBody(body).survivorIndex > SurvivorIndex.Count)
            {
                //is a custom survivor.

                if (!body.CompareTag("Player"))
                {
                    return false;
                }
            }

            return true;
        }

        //Find all Skin.CFG files in the plugins folder and add them as pending skins.
        private void LoadAllSkins()
        {
            foreach (string SkinConfig in Directory.EnumerateFiles(BepInEx.Paths.PluginPath, "Skin.cfg", SearchOption.AllDirectories))
            {
                if (SkinConfig.EndsWith("Skin.cfg"))
                {
                    string[] newpath = SkinConfig.Split(new string[] { @"\" }, StringSplitOptions.RemoveEmptyEntries);

                    ConfigFile config = new ConfigFile(SkinConfig, false);
                    string currentBodyName = config.Bind<string>("", "CharacterBodyName", "null").Value;

                    if (!pendingSkins.ContainsKey(currentBodyName))
                    {
                        pendingSkins.Add(currentBodyName, new List<PendingSkin>());
                    }

                    pendingSkins[currentBodyName].Add(new PendingSkin() { config = config, bodyName = currentBodyName, SkinFolder = Directory.GetParent(SkinConfig).FullName });
                }
            }
        }

        List<PendingSkin> minionSkins = new List<PendingSkin>();
        //TODO: Attempt thread or make async
        private void ApplySkins()
        {

            foreach (SurvivorDef def in SurvivorCatalog.allSurvivorDefs)
            {
                GameObject body = def.bodyPrefab;

                //Compare all pending skins
                if (pendingSkins.ContainsKey(body.name))
                {
                    if (SkinsAreApplicable(body))
                    {
                        List<string> skinsToRemove = new List<string>();

                        foreach (PendingSkin pendingSkin in pendingSkins[body.name])
                        {
                            AddSkin(body, pendingSkin.SkinFolder, pendingSkin.config);
                            skinsToRemove.Add(body.name);
                        }

                        skinsToRemove.ForEach(x => pendingSkins.Remove(x));
                    }
                    else
                    {
                        base.Logger.LogError("Error: " + body.name + " Has not been tagged with SkinReady by it's creator, and cannot have skins.");
                    }
                }
            }

            foreach (GameObject def in BodyCatalog.allBodyPrefabs)
            {
                GameObject body = def;

                //Compare all pending skins
                if (pendingSkins.ContainsKey(body.name))
                {
                    //if (SkinsAreApplicable(body))
                    //{

                    foreach (PendingSkin pendingSkin in pendingSkins[body.name])
                    {
                        try
                        {
                            AddSkin(body, pendingSkin.SkinFolder, pendingSkin.config);
                        }
                        catch
                        {
                            base.Logger.LogInfo("Error applying skin to " + body.name);
                        }
                    }
                    //}
                    //else
                    //{
                    //    base.Logger.LogError("Error: " + body.name + " Has not been tagged with SkinReady by it's creator, and cannot have skins.");
                    //}
                }
            }
        }
        public void DefaultSetup(GameObject body)
        {

            ModelLocator modelLocator = body.GetComponentInChildren<ModelLocator>();
            if (modelLocator)
            {
                ModelSkinController component = modelLocator.modelTransform.GetComponentInChildren<ModelSkinController>();

                //If the character does not have a modelskincontroller, or no skins, add one with a default skin
                if (!component || component.skins.Length == 0)
                {
                    AddDefaultSkin(body);
                }

            }
            else
            {
                base.Logger.LogError("ERROR: " + body.name + " Does not contain a model locator and cannot have skins.");
            }
        }

        private ModelSkinController AddDefaultSkin(GameObject def)
        {
            base.Logger.LogInfo("Adding default skin to " + def);

            ModelSkinController component = def.GetComponentInChildren<ModelLocator>().modelTransform.gameObject.AddComponent<ModelSkinController>();
            CharacterModel model = def.GetComponentInChildren<ModelLocator>().modelTransform.GetComponentInChildren<CharacterModel>();

            LoadoutAPI.SkinDefInfo skinDefInfo = default(LoadoutAPI.SkinDefInfo);
            skinDefInfo.BaseSkins = new SkinDef[0];
            skinDefInfo.Icon = LoadoutAPI.CreateSkinIcon(Color.black, Color.white, Color.black, Color.white);
            skinDefInfo.NameToken = "Default";
            skinDefInfo.UnlockableName = "";
            skinDefInfo.RootObject = def.GetComponentInChildren<ModelLocator>().modelTransform.gameObject;
            skinDefInfo.RendererInfos = model.baseRendererInfos;
            skinDefInfo.MeshReplacements = new SkinDef.MeshReplacement[0];
            skinDefInfo.GameObjectActivations = new SkinDef.GameObjectActivation[1] { new SkinDef.GameObjectActivation() { gameObject = def, shouldActivate = true } };
            skinDefInfo.Name = "DEFAULT_" + def.name + "_SKIN";

            if (model)
            {
                skinDefInfo.RendererInfos = model.baseRendererInfos;

                for (int i = 0; i < skinDefInfo.RendererInfos.Length; i++)
                {
                    skinDefInfo.RendererInfos[i].defaultMaterial.enableInstancing = true;
                    skinDefInfo.RendererInfos[i].renderer.material.enableInstancing = true;
                    skinDefInfo.RendererInfos[i].renderer.sharedMaterial.enableInstancing = true;
                }

                SkinDef skinDef3 = LoadoutAPI.CreateNewSkinDef(skinDefInfo);
                component.skins = new SkinDef[1] { skinDef3 };

                LoadoutAPI.AddSkinToCharacter(def, skinDef3);

                SkinDef[] skins = component.skins;

                SkinDef[][] newSkins = typeof(BodyCatalog).GetFieldValue<SkinDef[][]>("skins");
                newSkins[BodyCatalog.FindBodyIndex(def)] = skins;
                typeof(BodyCatalog).SetFieldValue<SkinDef[][]>("skins", newSkins);
            }
            else
            {
                base.Logger.LogError("Unable to create new skin for " + def);
            }

            return component;
        }

        public void AddSkin(GameObject gameObject, string SkinFolder, ConfigFile config)
        {
            LoadoutAPI.SkinDefInfo skinDefInfo = new LoadoutAPI.SkinDefInfo();
            skinDefInfo.BaseSkins = new SkinDef[0];
            skinDefInfo.Icon = LoadoutAPI.CreateSkinIcon(config.Bind<Color>("", "IconColorTop", Color.magenta).Value, config.Bind<Color>("", "IconColorRight", Color.black).Value, config.Bind<Color>("", "IconColorBottom", Color.magenta).Value, config.Bind<Color>("", "IconColorLeft", Color.black).Value);
            skinDefInfo.NameToken = config.Bind<string>("", "SkinName", "DefaultName").Value;
            skinDefInfo.UnlockableName = config.Bind<string>("", "UnlockableName", "").Value;
            skinDefInfo.RootObject = gameObject.GetComponentInChildren<ModelLocator>().modelTransform.gameObject;
            skinDefInfo.RendererInfos = new CharacterModel.RendererInfo[0];
            skinDefInfo.MeshReplacements = new SkinDef.MeshReplacement[0];
            skinDefInfo.GameObjectActivations = new SkinDef.GameObjectActivation[0];
            skinDefInfo.Name = "CUSTOM_" + gameObject.name + "_SKIN";

            base.Logger.LogInfo("Adding new skin to " + gameObject.name + " with name " + skinDefInfo.NameToken);

            int MeshReplacementIndex = config.Bind<int>("", "MeshReplacementIndex", -1).Value;
            string MeshReplacementKey = config.Bind<string>("", "SkinMeshToUse", "Default").Value;

            string minionSkinForBody = config.Bind<string>("", "MinionReplacementForBody", "NONE", new ConfigDescription("If this is a minion skin, determines the characterbody for it")).Value;
            string minionSkinForSkin = config.Bind<string>("", "MinionReplacementForSkin", "NONE", new ConfigDescription("If this is a minion skin, determines the characterbody for it")).Value;


            bool isMinionSkin = false;
            if (minionSkinForBody != null)
             isMinionSkin = minionSkinForBody != "NONE";


            if (MeshReplacementKey != "Default" && MeshReplacementKey != "Custom" && MeshReplacementKey != null)
            {
                SkinDef def1 = gameObject.GetComponentInChildren<ModelLocator>().modelTransform.GetComponentInChildren<ModelSkinController>().skins.First(x => x.nameToken == MeshReplacementKey);
                MeshReplacementIndex = Array.IndexOf(gameObject.GetComponentInChildren<ModelLocator>().modelTransform.GetComponentInChildren<ModelSkinController>().skins, def1);

            }

            SkinDef def = gameObject.GetComponentInChildren<ModelLocator>().modelTransform.GetComponentInChildren<ModelSkinController>().skins[MeshReplacementIndex];

            if (def)
            {
                skinDefInfo.MeshReplacements = def.meshReplacements;
                skinDefInfo.RendererInfos = def.rendererInfos;
            }

            CharacterModel.RendererInfo[] rendererInfos = skinDefInfo.RendererInfos;
            CharacterModel.RendererInfo[] array = new CharacterModel.RendererInfo[rendererInfos.Length];
            rendererInfos.CopyTo(array, 0);



            foreach (string MaterialFolder in Directory.EnumerateDirectories(SkinFolder))
            {
                //Get name of render from folder name
                string[] MaterialIndexVs = MaterialFolder.Split(new string[] { @"\" }, StringSplitOptions.None);
                string RendererName = MaterialIndexVs[MaterialIndexVs.Length - 1];

                //Find the renderer based on the name
                int index = Array.IndexOf(array, array.FirstOrDefault(x => x.renderer.name == RendererName));

                //Get the material of the renderer
                Material defaultMaterial = array[index].defaultMaterial;


                if (defaultMaterial)
                {
                    //Create a clone of the material
                    defaultMaterial = UnityEngine.Object.Instantiate<Material>(defaultMaterial);

                    //Iterate through all files related to this material from the skin folder
                    foreach (string FilePath in Directory.EnumerateFiles(MaterialFolder))
                    {
                        //Loading and applying textures from .png
                        if (FilePath.EndsWith(".PNG") || FilePath.EndsWith(".png"))
                        {
                            //Use File name to get texture name
                            string[] FilePathVs = FilePath.Split(new string[] { @"\" }, StringSplitOptions.None);
                            string FileName = FilePathVs[FilePathVs.Length - 1].Replace(".PNG", "");

                            //Create new Texture2d and load image in
                            Texture2D savedTex = new Texture2D(1, 1, TextureFormat.RGBAFloat, false, true);
                            ImageConversion.LoadImage(savedTex, System.IO.File.ReadAllBytes(FilePath));

                            //Apply the loaded image
                            savedTex.Apply();

                            //Attempt to place this image onto the material in the correct property
                            if (defaultMaterial.HasProperty(FileName))
                            {
                                defaultMaterial.enableInstancing = true;
                                defaultMaterial.SetTexture(FileName, GetReadableTextureCopy(savedTex));
                            }

                        }
                        else if (FilePath.EndsWith(".cfg"))
                        {

                            //Load the config at this path
                            BepInEx.Configuration.ConfigFile perMatConfig = new ConfigFile(FilePath, true);

                            //iterate through material properties and apply via config
                            foreach (string value in floatProperties)
                            {

                                if (defaultMaterial.HasProperty(value))
                                {
                                    defaultMaterial.SetFloat(value, perMatConfig.Bind<float>("", value, defaultMaterial.GetFloat(value), new ConfigDescription("Sets the value for " + value)).Value);
                                }
                            }

                            foreach (string value in colorProperties)
                            {
                                if (defaultMaterial.HasProperty(value))
                                {
                                    defaultMaterial.SetColor(value, perMatConfig.Bind<Color>("", value, defaultMaterial.GetColor(value), new ConfigDescription("Sets the value for " + value)).Value);
                                }
                            }

                            foreach (string value in defaultMaterial.shaderKeywords)
                            {
                                bool isEnabled = perMatConfig.Bind<bool>("", value, defaultMaterial.IsKeywordEnabled(value), new ConfigDescription("Sets the value for " + value)).Value;

                                if (isEnabled)
                                {
                                    defaultMaterial.EnableKeyword(value);
                                }
                                else
                                {
                                    defaultMaterial.DisableKeyword(value);
                                }
                            }



                        }
                        else
                        {
                            base.Logger.LogError("Unsupported file found in material folder: " + FilePath);
                        }
                    }


                    array[index].defaultMaterial = defaultMaterial;

                }
            }

            skinDefInfo.RendererInfos = array;
            LoadoutAPI.AddSkinToCharacter(gameObject, skinDefInfo);

            SkinDef[] skins = gameObject.GetComponentInChildren<ModelLocator>().modelTransform.GetComponentInChildren<ModelSkinController>().skins;

            skins[skins.Length - 1].minionSkinReplacements = new SkinDef.MinionSkinReplacement[0];
            skins[skins.Length - 1].projectileGhostReplacements = new SkinDef.ProjectileGhostReplacement[0];


            if (isMinionSkin)
            {
                base.Logger.LogInfo("Is a minion skin");

                foreach (GameObject body in BodyCatalog.allBodyPrefabs)
                {
                    if (body.name == minionSkinForBody)
                    {
                        base.Logger.LogInfo("Applying minionskinreplacement");

                        SkinDef defForMaster = body.GetComponentInChildren<ModelSkinController>().skins.First(x => x.nameToken == minionSkinForSkin);

                        base.Logger.LogInfo("Size was " + defForMaster.minionSkinReplacements.Length);


                        Array.Resize(ref defForMaster.minionSkinReplacements, defForMaster.minionSkinReplacements.Length + 1);
                        defForMaster.minionSkinReplacements[defForMaster.minionSkinReplacements.Length - 1] = new SkinDef.MinionSkinReplacement() { minionBodyPrefab = gameObject, minionSkin = skins[skins.Length - 1] };

                        base.Logger.LogInfo("Size is now " + defForMaster.minionSkinReplacements.Length);

                    }
                }
            }

            SkinDef[][] newSkins = typeof(BodyCatalog).GetFieldValue<SkinDef[][]>("skins");
            newSkins[BodyCatalog.FindBodyIndex(gameObject)] = skins;
            typeof(BodyCatalog).SetFieldValue<SkinDef[][]>("skins", newSkins);


        }

        private Texture2D GetReadableTextureCopy(Texture2D baseTex)
        {
            if (baseTex)
            {
                RenderTexture active = RenderTexture.active;
                RenderTexture temporary = RenderTexture.GetTemporary(baseTex.width, baseTex.height, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
                Graphics.Blit(baseTex, temporary);
                RenderTexture.active = temporary;
                Texture2D texture2D = new Texture2D(baseTex.width, baseTex.height, TextureFormat.RGBAFloat, false);
                texture2D.ReadPixels(new Rect(0f, 0f, (float)temporary.width, (float)temporary.height), 0, 0);
                texture2D.Apply();
                RenderTexture.active = active;
                return texture2D;
            }

            return new Texture2D(1, 1);
        }

        public void DumpDefaultSkins()
        {
            foreach (SurvivorDef def in SurvivorCatalog.allSurvivorDefs)
            {
                if (SkinsAreApplicable(def.bodyPrefab))
                    DumpDefaultSkins(def.bodyPrefab);
            }
        }
        public void DumpAllSkins()
        {
            foreach (GameObject def in BodyCatalog.allBodyPrefabs)
            {
                DumpDefaultSkins(def);
            }
        }
        public void DumpDefaultSkins(GameObject gameObject)
        {
            if (gameObject)
            {
                base.Logger.LogMessage("Beginning dump on " + gameObject.name);

                if (gameObject.GetComponentInChildren<ModelLocator>())
                {
                    if (!gameObject.GetComponentInChildren<ModelLocator>().modelTransform.GetComponentInChildren<ModelSkinController>())
                    {
                        base.Logger.LogError("ERROR: " + gameObject.name + " Does not have a modelskincontroller");
                        return;
                    }
                    SkinDef[] skins = gameObject.GetComponentInChildren<ModelLocator>().modelTransform.GetComponentInChildren<ModelSkinController>().skins;

                    int skinIndex = 0;

                    base.Logger.LogMessage(gameObject.name + " Contains " + skins.Count() + " skins");

                    foreach (SkinDef currentSkin in skins)
                    {
                        if (!currentSkin.name.Contains("CUSTOM_"))
                        {
                            base.Logger.LogMessage("dumping skin: " + currentSkin.name);

                            string currentDirectory = BepInEx.Paths.PluginPath + @"\DefaultSkinModName\plugins\" + currentSkin.name;
                            if (!Directory.Exists(currentDirectory))
                            {
                                Directory.CreateDirectory(currentDirectory);
                            }

                            BepInEx.Configuration.ConfigFile perSkinConfig = new ConfigFile(currentDirectory + @"\Skin.cfg", true);
                            perSkinConfig.Bind<string>("", "CharacterBodyName", gameObject.name.Replace("(Clone)", ""), new ConfigDescription("Determines which characterbody this applies to"));
                            perSkinConfig.Bind<string>("", "MinionReplacementForBody", "NONE", new ConfigDescription("If this is a minion skin, determines the characterbody for it"));
                            perSkinConfig.Bind<string>("", "MinionReplacementForSkin", "NONE", new ConfigDescription("If this is a minion skin, determines the skin for it (Uses the name of the skin, like SkinName"));
                            perSkinConfig.Bind<int>("", "MeshReplacementIndex", -1, new ConfigDescription("[DEPRECATED] Which base skin to use the mesh for [use -1 for custom]"));
                            perSkinConfig.Bind<string>("", "SkinMeshToUse", currentSkin.nameToken, new ConfigDescription("Which base skin to use the mesh from"));
                            perSkinConfig.Bind<string>("", "SkinName", "DefaultName", new ConfigDescription("The name of this skin"));
                            perSkinConfig.Bind<string>("", "UnlockableName", "", new ConfigDescription("The unlockable this is tied to"));
                            perSkinConfig.Bind<Color>("", "IconColorTop", Color.magenta, new ConfigDescription("The icon colours"));
                            perSkinConfig.Bind<Color>("", "IconColorRight", Color.black, new ConfigDescription("The icon colours"));
                            perSkinConfig.Bind<Color>("", "IconColorBottom", Color.magenta, new ConfigDescription("The icon colours"));
                            perSkinConfig.Bind<Color>("", "IconColorLeft", Color.black, new ConfigDescription("The icon colours"));

                            base.Logger.LogMessage("config file created for skin: " + currentSkin.name);

                            foreach (CharacterModel.RendererInfo rendererInfo in currentSkin.rendererInfos)
                            {

                                string filePath = currentDirectory + @"\" + rendererInfo.renderer.name;
                                if (!Directory.Exists(filePath))
                                {
                                    Directory.CreateDirectory(filePath);
                                }

                                //Mesh mesh = null;

                                //if (rendererInfo.renderer.gameObject.GetComponentInChildren<MeshFilter>())
                                //{
                                //    mesh = rendererInfo.renderer.gameObject.GetComponentInChildren<MeshFilter>().mesh;
                                //}

                                //if ((rendererInfo.renderer as SkinnedMeshRenderer))
                                //{
                                //    (rendererInfo.renderer as SkinnedMeshRenderer).sharedMesh = mesh;
                                //}

                                //if (mesh)
                                //{
                                //    ObjExporter.MeshToFile(mesh, rendererInfo.renderer, filePath + @"\" + mesh.name + ".obj");
                                //}

                                //var filename = filePath + @"\" + rendererInfo.renderer.gameObject.name + ".fbx";
                                //using (StreamWriter sw = new StreamWriter(filename))
                                //{
                                //    sw.Write(FBXExporter.MeshToString(rendererInfo.renderer.gameObject, filePath));
                                //}

                                Material defaultMaterial = rendererInfo.defaultMaterial;
                                if (defaultMaterial)
                                {


                                    foreach (string textureProperty in defaultMaterial.GetTexturePropertyNames())
                                    {
                                        Texture2D defaultTexture = (Texture2D)defaultMaterial.GetTexture(textureProperty);
                                        Texture2D readableTexture = GetReadableTextureCopy(defaultTexture);

                                        System.IO.File.WriteAllBytes(filePath + @"\" + textureProperty + ".PNG", ImageConversion.EncodeToPNG(readableTexture));
                                    }

                                    BepInEx.Configuration.ConfigFile perMatConfig = new ConfigFile(currentDirectory + @"\" + rendererInfo.renderer.name + @"\Material.cfg", true);

                                    foreach (string value in floatProperties)
                                    {
                                        if (defaultMaterial.HasProperty(value))
                                        {
                                            perMatConfig.Bind<float>("", value, defaultMaterial.GetFloat(value), new ConfigDescription("Sets the value for " + value));

                                        }
                                    }

                                    foreach (string value in colorProperties)
                                    {
                                        if (defaultMaterial.HasProperty(value))
                                        {
                                            perMatConfig.Bind<Color>("", value, defaultMaterial.GetColor(value), new ConfigDescription("Sets the value for " + value));
                                        }
                                    }

                                    foreach (string value in defaultMaterial.shaderKeywords)
                                    {
                                        perMatConfig.Bind<bool>("", value, defaultMaterial.IsKeywordEnabled(value), new ConfigDescription("Sets the value for " + value));
                                    }

                                }

                            }

                            skinIndex++;
                        }

                    }
                }
                else
                {
                    base.Logger.LogError(gameObject.name + " had no modelLocator");
                }
            }
        }
    }
}