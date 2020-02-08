using BepInEx;
using BepInEx.Configuration;
using R2API;
using R2API.Utils;
using RoR2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions.Must;

namespace PallesenProductions
{

    [R2APISubmoduleDependency("LoadoutAPI")]
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.PallesenProductions.RyanSkinAPI", "RyanSkinAPI", "2.0.0")]

    public class RyanSkinAPI : BaseUnityPlugin
    {

        public class Skin
        {

            public string bodyName;
            public int meshReplacement;
            public string skinName;

            public Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();
            public SkinDef.MeshReplacement[] meshReplacements;
        }

        public List<PendingSkin> pendingSkins = new List<PendingSkin>();

        private static BepInEx.Configuration.ConfigFile myConfig;

        private string directory;
        private List<string> floatProperties = new List<string>();
        private List<string> colorProperties = new List<string>();

        public struct PendingSkin
        {
            public string bodyName;

            public string SkinFolder;
            public ConfigFile config;
        }
        public void Awake()
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

            directory = Application.dataPath;
            directory = System.IO.Path.GetFullPath(System.IO.Path.Combine(directory, @"..\BepInEx\skins"));


            myConfig = new ConfigFile(@"BepInEx\skins\RyanSkinAPI.cfg", true);

            On.RoR2.SurvivorCatalog.Init += (orig) =>
            {
                orig();

                foreach (SurvivorDef survivorDef in SurvivorCatalog.allSurvivorDefs)
                {
                    //Skip 'finished'  prefabs. This is for custom survivors that do skins differently in an incompatible way to the RyanSkinAPI.
                    if (survivorDef.bodyPrefab && survivorDef.bodyPrefab.CompareTag("Finish"))
                    {
                        continue;
                    }

                    AddComponents(survivorDef);

                    if (myConfig.Bind<bool>(new ConfigDefinition("", "DumpBoilerplates"), false).Value)
                    {
                        base.Logger.LogInfo("Dumping " + survivorDef.name);

                        DumpDefaultSkins(survivorDef.bodyPrefab.gameObject);
                    }
                }

                foreach (string ModFolder in Directory.EnumerateDirectories(directory))
                {
                    foreach (string SkinFolder in Directory.EnumerateDirectories(ModFolder))
                    {

                        foreach (string SkinConfig in Directory.EnumerateFiles(SkinFolder))
                        {
                            if (SkinConfig.EndsWith("Skin.cfg"))
                            {
                                string[] newpath = SkinConfig.Split(new string[] { @"\" }, StringSplitOptions.RemoveEmptyEntries);

                                string RelativePath = @"BepInEx\skins\" + newpath[newpath.Length - 3] + @"\" + newpath[newpath.Length - 2] + @"\" + newpath[newpath.Length - 1];
                                ConfigFile config = new ConfigFile(RelativePath, false);
                                string body = config.Bind<string>("", "CharacterBodyName", "null").Value;

                                pendingSkins.Add(new PendingSkin() { config = config, bodyName = body, SkinFolder = SkinFolder });

                            }
                        }
                    }
                };

                foreach (SurvivorDef survivorDef in SurvivorCatalog.allSurvivorDefs)
                {
                    //Skip finished bodies.
                    if (survivorDef.bodyPrefab.CompareTag("Finish"))
                    {
                        continue;
                    }

                    foreach (PendingSkin pendingSkin in pendingSkins)
                    {
                        if (survivorDef.bodyPrefab.name == pendingSkin.bodyName)
                        {
                            AddSkin(survivorDef, pendingSkin.SkinFolder, pendingSkin.config);
                        }
                    }
                }
            };
        }


        #region addingSkins
        public void AddSkin(SurvivorDef def, string SkinFolder, ConfigFile config)
        {
            GameObject gameObject = def.bodyPrefab;

            LoadoutAPI.SkinDefInfo skinDefInfo = new LoadoutAPI.SkinDefInfo();
            skinDefInfo.BaseSkins = new SkinDef[0];
            skinDefInfo.Icon = LoadoutAPI.CreateSkinIcon(config.Bind<Color>("", "IconColorTop", Color.magenta).Value, config.Bind<Color>("", "IconColorRight", Color.black).Value, config.Bind<Color>("", "IconColorBottom", Color.magenta).Value, config.Bind<Color>("", "IconColorLeft", Color.black).Value);
            skinDefInfo.NameToken = config.Bind<string>("", "SkinName", "DefaultName").Value;
            skinDefInfo.UnlockableName = config.Bind<string>("", "UnlockableName", "").Value;
            skinDefInfo.RootObject = def.bodyPrefab.GetComponent<ModelLocator>().modelTransform.gameObject;
            skinDefInfo.RendererInfos = new CharacterModel.RendererInfo[0];
            skinDefInfo.MeshReplacements = new SkinDef.MeshReplacement[0];
            skinDefInfo.GameObjectActivations = new SkinDef.GameObjectActivation[0];
            skinDefInfo.Name = "SKIN_" + gameObject.name + "_DEFAULT";



            int MeshReplacementIndex = config.Bind<int>("", "MeshReplacementIndex", 0).Value;

            if (MeshReplacementIndex != -1)
            {
                skinDefInfo.MeshReplacements = gameObject.GetComponent<ModelLocator>().modelTransform.GetComponent<ModelSkinController>().skins[MeshReplacementIndex].meshReplacements;
                skinDefInfo.RendererInfos = gameObject.GetComponent<ModelLocator>().modelTransform.GetComponent<ModelSkinController>().skins[MeshReplacementIndex].rendererInfos;
            }

            CharacterModel.RendererInfo[] rendererInfos = skinDefInfo.RendererInfos;
            CharacterModel.RendererInfo[] array = new CharacterModel.RendererInfo[rendererInfos.Length];
            rendererInfos.CopyTo(array, 0);



            foreach (string MaterialFolder in Directory.EnumerateDirectories(SkinFolder))
            {

                string[] MaterialIndexVs = MaterialFolder.Split(new string[] { @"\" }, StringSplitOptions.None);
                string RendererName = MaterialIndexVs[MaterialIndexVs.Length - 1];

                int index = Array.IndexOf(array, array.FirstOrDefault(x => x.renderer.name == RendererName));

                Material defaultMaterial = array[index].defaultMaterial;
                if (defaultMaterial)
                {
                    defaultMaterial = UnityEngine.Object.Instantiate<Material>(defaultMaterial);

                    foreach (string FilePath in Directory.EnumerateFiles(MaterialFolder))
                    {
                        if (FilePath.EndsWith(".PNG") || FilePath.EndsWith(".png"))
                        {
                            string[] FilePathVs = FilePath.Split(new string[] { @"\" }, StringSplitOptions.None);
                            string FileName = FilePathVs[FilePathVs.Length - 1].Replace(".PNG", "");

                            Texture2D savedTex = new Texture2D(1, 1, TextureFormat.RGBAFloat, false, true);
                            ImageConversion.LoadImage(savedTex, System.IO.File.ReadAllBytes(FilePath));

                            savedTex.Apply();

                            if (defaultMaterial.HasProperty(FileName))
                            {
                                defaultMaterial.enableInstancing = true;
                                defaultMaterial.SetTexture(FileName, GetReadableTextureCopy(savedTex));
                            }

                        }
                        else if (FilePath.EndsWith(".cfg"))
                        {
                            string[] ConfigPathVs = FilePath.Split(new string[] { @"\" }, StringSplitOptions.None);
                            string ConfigName = @"BepInEx\skins\" + ConfigPathVs[ConfigPathVs.Length - 4] + @"\" + ConfigPathVs[ConfigPathVs.Length - 3] + @"\" + ConfigPathVs[ConfigPathVs.Length - 2] + @"\" + ConfigPathVs[ConfigPathVs.Length - 1];
                            BepInEx.Configuration.ConfigFile perMatConfig = new ConfigFile(ConfigName, true);

                            {
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

                        }
                        else if (FilePath.EndsWith(".obj"))
                        {

                            

                            Mesh holderMesh = new Mesh();
                            ObjImporter newMesh = new ObjImporter();
                            holderMesh = newMesh.ImportFile(FilePath);

                            if (array[index].renderer.gameObject.GetComponent<MeshFilter>())
                            {
                                Array.Resize(ref skinDefInfo.MeshReplacements, skinDefInfo.MeshReplacements.Length + 1);
                                skinDefInfo.MeshReplacements[skinDefInfo.MeshReplacements.Length - 1].mesh = holderMesh;
                                skinDefInfo.MeshReplacements[skinDefInfo.MeshReplacements.Length - 1].renderer = array[index].renderer;
                            }

                            if ((array[index].renderer as SkinnedMeshRenderer))
                            {
                                Array.Resize(ref skinDefInfo.MeshReplacements, skinDefInfo.MeshReplacements.Length + 1);
                                skinDefInfo.MeshReplacements[skinDefInfo.MeshReplacements.Length - 1].mesh = holderMesh;
                                skinDefInfo.MeshReplacements[skinDefInfo.MeshReplacements.Length - 1].renderer = array[index].renderer;

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

            base.Logger.LogInfo("Added new skin to " + def.bodyPrefab.name + " with name " + skinDefInfo.NameToken);

            skinDefInfo.RendererInfos = array;
            LoadoutAPI.AddSkinToCharacter(def.bodyPrefab, skinDefInfo);

            SkinDef[] skins = def.bodyPrefab.GetComponent<ModelLocator>().modelTransform.GetComponent<ModelSkinController>().skins;

            SkinDef[][] newSkins = typeof(BodyCatalog).GetFieldValue<SkinDef[][]>("skins");
            newSkins[SurvivorCatalog.GetBodyIndexFromSurvivorIndex(def.survivorIndex)] = skins;
            typeof(BodyCatalog).SetFieldValue<SkinDef[][]>("skins", newSkins);

        }
        #endregion addingSkins

        #region boilerplate
        public void DumpDefaultSkins(GameObject gameObject)
        {
            if (gameObject.GetComponent<ModelLocator>())
            {
                SkinDef[] skins = gameObject.GetComponent<ModelLocator>().modelTransform.GetComponent<ModelSkinController>().skins;

                int skinIndex = 0;

                foreach (SkinDef currentSkin in skins)
                {
                    string currentDirectory = directory + @"\Defaults\" + currentSkin.name;
                    if (!Directory.Exists(currentDirectory))
                    {
                        Directory.CreateDirectory(currentDirectory);
                    }

                    BepInEx.Configuration.ConfigFile perSkinConfig = new ConfigFile(@"BepInEx\skins\Defaults\" + currentSkin.name + @"\Skin.cfg", true);
                    perSkinConfig.Bind<string>("", "CharacterBodyName", gameObject.name, new ConfigDescription("Determines which characterbody this applies to"));
                    perSkinConfig.Bind<int>("", "MeshReplacementIndex", skinIndex, new ConfigDescription("Which base skin to use the mesh for [use -1 for custom]"));
                    perSkinConfig.Bind<string>("", "SkinName", "DefaultName", new ConfigDescription("The name of this skin"));
                    perSkinConfig.Bind<string>("", "UnlockableName", "", new ConfigDescription("The unlockable this is tied to"));
                    perSkinConfig.Bind<Color>("", "IconColorTop", Color.magenta, new ConfigDescription("The icon colours"));
                    perSkinConfig.Bind<Color>("", "IconColorRight", Color.black, new ConfigDescription("The icon colours"));
                    perSkinConfig.Bind<Color>("", "IconColorBottom", Color.magenta, new ConfigDescription("The icon colours"));
                    perSkinConfig.Bind<Color>("", "IconColorLeft", Color.black, new ConfigDescription("The icon colours"));


                    //UnityFBXExporter.FBXExporter.ExportGameObjToFBX(gameObject, currentDirectory + @"\MeshReplacement.fbx");


                    foreach (CharacterModel.RendererInfo rendererInfo in currentSkin.rendererInfos)
                    {
                        string filePath = currentDirectory + @"\" + rendererInfo.renderer.name;
                        if (!Directory.Exists(filePath))
                        {
                            Directory.CreateDirectory(filePath);
                        }

                        Mesh mesh = null;

                        if (rendererInfo.renderer.gameObject.GetComponent<MeshFilter>())
                        {
                            mesh = rendererInfo.renderer.gameObject.GetComponent<MeshFilter>().mesh;
                        }

                        if ((rendererInfo.renderer as SkinnedMeshRenderer))
                        {
                            mesh = (rendererInfo.renderer as SkinnedMeshRenderer).sharedMesh;
                        }

                        if (mesh)
                        {

                            string objFile = MeshToString(mesh);
                            FileStream stream = File.Create(filePath + @"\" + mesh.name + ".obj");
                            byte[] bytes = Encoding.ASCII.GetBytes(objFile);
                            stream.Write(bytes, 0, bytes.Length);
                            stream.Dispose();
                        }

                        Material defaultMaterial = rendererInfo.defaultMaterial;
                        if (defaultMaterial)
                        {


                            foreach (string textureProperty in defaultMaterial.GetTexturePropertyNames())
                            {
                                Texture2D defaultTexture = (Texture2D)defaultMaterial.GetTexture(textureProperty);
                                Texture2D readableTexture = GetReadableTextureCopy(defaultTexture);

                                System.IO.File.WriteAllBytes(filePath + @"\" + textureProperty + ".PNG", ImageConversion.EncodeToPNG(readableTexture));
                            }

                            BepInEx.Configuration.ConfigFile perMatConfig = new ConfigFile(@"BepInEx\skins\Defaults\" + currentSkin.name + @"\" + rendererInfo.renderer.name + @"\Material.cfg", true);


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
            else
            {
                base.Logger.LogError(gameObject.name + " had no modelLocator");
            }
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
        #endregion boilerplate


        #region Defaults
        public void AddComponents(SurvivorDef def)
        {
            base.Logger.LogInfo("Add Components called on " + def.name);

            ModelLocator modelLocator = def.bodyPrefab.GetComponent<ModelLocator>();
            if (modelLocator)
            {

                ModelSkinController component = modelLocator.modelTransform.GetComponent<ModelSkinController>();

                if (!component || component.skins.Length == 0)
                {
                    component = AddDefaultSkin(def);
                }
                else
                {
                }
            }

        }

        private ModelSkinController AddDefaultSkin(SurvivorDef def)
        {
            base.Logger.LogInfo("Adding default skin to " + def);

            ModelSkinController component = def.bodyPrefab.GetComponent<ModelLocator>().modelTransform.gameObject.AddComponent<ModelSkinController>();
            CharacterModel model = def.bodyPrefab.GetComponent<ModelLocator>().modelTransform.GetComponent<CharacterModel>();

            LoadoutAPI.SkinDefInfo skinDefInfo = default(LoadoutAPI.SkinDefInfo);
            skinDefInfo.BaseSkins = new SkinDef[0];
            skinDefInfo.Icon = LoadoutAPI.CreateSkinIcon(Color.black, Color.white, Color.black, Color.white);
            skinDefInfo.NameToken = "Default";
            skinDefInfo.UnlockableName = "";
            skinDefInfo.RootObject = def.bodyPrefab.GetComponent<ModelLocator>().modelTransform.gameObject;
            skinDefInfo.RendererInfos = model.baseRendererInfos;
            skinDefInfo.MeshReplacements = new SkinDef.MeshReplacement[0];
            skinDefInfo.GameObjectActivations = new SkinDef.GameObjectActivation[1] { new SkinDef.GameObjectActivation() { gameObject = def.bodyPrefab, shouldActivate = true } };
            skinDefInfo.Name = "SKIN_" + def.bodyPrefab.name + "_DEFAULT";


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

                SkinDef[] skins = component.skins;

                SkinDef[][] newSkins = typeof(BodyCatalog).GetFieldValue<SkinDef[][]>("skins");
                newSkins[SurvivorCatalog.GetBodyIndexFromSurvivorIndex(def.survivorIndex)] = skins;
                typeof(BodyCatalog).SetFieldValue<SkinDef[][]>("skins", newSkins);
            }
            else
            {
                base.Logger.LogError("Unable to create new skin for " + def);
            }

            return component;
        }
        #endregion Defaults

        public string MeshToString(Mesh mesh)
        {
            Mesh m = mesh;

            StringBuilder sb = new StringBuilder();

            sb.Append("g ").Append(mesh.name).Append("\n");
            foreach (Vector3 v in m.vertices)
            {
                sb.Append(string.Format("v {0} {1} {2}\n", v.x, v.y, v.z));
            }
            sb.Append("\n");
            foreach (Vector3 v in m.normals)
            {
                sb.Append(string.Format("vn {0} {1} {2}\n", v.x, v.y, v.z));
            }
            sb.Append("\n");
            foreach (Vector3 v in m.uv)
            {
                sb.Append(string.Format("vt {0} {1}\n", v.x, v.y));
            }
            for (int material = 0; material < m.subMeshCount; material++)
            {
                sb.Append("\n");

                int[] triangles = m.GetTriangles(material);
                for (int i = 0; i < triangles.Length; i += 3)
                {
                    sb.Append(string.Format("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\n",
                        triangles[i] + 1, triangles[i + 1] + 1, triangles[i + 2] + 1));
                }
            }
            return sb.ToString();
        }
    }
}


public class ObjImporter
{

    private struct meshStruct
    {
        public Vector3[] vertices;
        public Vector3[] normals;
        public Vector2[] uv;
        public Vector2[] uv1;
        public Vector2[] uv2;
        public int[] triangles;
        public int[] faceVerts;
        public int[] faceUVs;
        public Vector3[] faceData;
        public string name;
        public string fileName;
    }

    // Use this for initialization
    public Mesh ImportFile(string filePath)
    {
        meshStruct newMesh = createMeshStruct(filePath);
        populateMeshStruct(ref newMesh);

        Vector3[] newVerts = new Vector3[newMesh.faceData.Length];
        Vector2[] newUVs = new Vector2[newMesh.faceData.Length];
        Vector3[] newNormals = new Vector3[newMesh.faceData.Length];
        int i = 0;
        /* The following foreach loops through the facedata and assigns the appropriate vertex, uv, or normal
         * for the appropriate Unity mesh array.
         */
        foreach (Vector3 v in newMesh.faceData)
        {
            newVerts[i] = newMesh.vertices[(int)v.x - 1];
            if (v.y >= 1)
            {
                if (((int)v.y - 1) >= 0 && ((int)v.y - 1) < newMesh.uv.Length)
                {
                    newUVs[i] = newMesh.uv[(int)v.y - 1];
                }
            }

            if (v.z >= 1)
            {
                if (((int)v.z - 1) >= 0 && ((int)v.z - 1) < newMesh.normals.Length)
                {
                    newNormals[i] = newMesh.normals[(int)v.z - 1];
                }
            }

            i++;
        }

        Mesh mesh = new Mesh();

        mesh.vertices = newVerts;
        mesh.uv = newUVs;
        mesh.normals = newNormals;
        mesh.triangles = newMesh.triangles;

        mesh.RecalculateBounds();
        //mesh.Optimize();

        return mesh;
    }

    private static meshStruct createMeshStruct(string filename)
    {
        int triangles = 0;
        int vertices = 0;
        int vt = 0;
        int vn = 0;
        int face = 0;
        meshStruct mesh = new meshStruct();
        mesh.fileName = filename;
        StreamReader stream = File.OpenText(filename);
        string entireText = stream.ReadToEnd();
        stream.Close();
        using (StringReader reader = new StringReader(entireText))
        {
            string currentText = reader.ReadLine();
            char[] splitIdentifier = { ' ' };
            string[] brokenString;
            while (currentText != null)
            {
                if (!currentText.StartsWith("f ") && !currentText.StartsWith("v ") && !currentText.StartsWith("vt ")
                    && !currentText.StartsWith("vn "))
                {
                    currentText = reader.ReadLine();
                    if (currentText != null)
                    {
                        currentText = currentText.Replace("  ", " ");
                    }
                }
                else
                {
                    currentText = currentText.Trim();                           //Trim the current line
                    brokenString = currentText.Split(splitIdentifier, 50);      //Split the line into an array, separating the original line by blank spaces
                    switch (brokenString[0])
                    {
                        case "v":
                            vertices++;
                            break;
                        case "vt":
                            vt++;
                            break;
                        case "vn":
                            vn++;
                            break;
                        case "f":
                            face = face + brokenString.Length - 1;
                            triangles = triangles + 3 * (brokenString.Length - 2); /*brokenString.Length is 3 or greater since a face must have at least
                                                                                     3 vertices.  For each additional vertice, there is an additional
                                                                                     triangle in the mesh (hence this formula).*/
                            break;
                    }
                    currentText = reader.ReadLine();
                    if (currentText != null)
                    {
                        currentText = currentText.Replace("  ", " ");
                    }
                }
            }
        }
        mesh.triangles = new int[triangles];
        mesh.vertices = new Vector3[vertices];
        mesh.uv = new Vector2[vt];
        mesh.normals = new Vector3[vn];
        mesh.faceData = new Vector3[face];
        return mesh;
    }

    private static void populateMeshStruct(ref meshStruct mesh)
    {
        StreamReader stream = File.OpenText(mesh.fileName);
        string entireText = stream.ReadToEnd();
        stream.Close();
        using (StringReader reader = new StringReader(entireText))
        {
            string currentText = reader.ReadLine();

            char[] splitIdentifier = { ' ' };
            char[] splitIdentifier2 = { '/' };
            string[] brokenString;
            string[] brokenBrokenString;
            int f = 0;
            int f2 = 0;
            int v = 0;
            int vn = 0;
            int vt = 0;
            int vt1 = 0;
            int vt2 = 0;
            while (currentText != null)
            {
                if (!currentText.StartsWith("f ") && !currentText.StartsWith("v ") && !currentText.StartsWith("vt ") &&
                    !currentText.StartsWith("vn ") && !currentText.StartsWith("g ") && !currentText.StartsWith("usemtl ") &&
                    !currentText.StartsWith("mtllib ") && !currentText.StartsWith("vt1 ") && !currentText.StartsWith("vt2 ") &&
                    !currentText.StartsWith("vc ") && !currentText.StartsWith("usemap "))
                {
                    currentText = reader.ReadLine();
                    if (currentText != null)
                    {
                        currentText = currentText.Replace("  ", " ");
                    }
                }
                else
                {
                    currentText = currentText.Trim();
                    brokenString = currentText.Split(splitIdentifier, 50);
                    switch (brokenString[0])
                    {
                        case "g":
                            break;
                        case "usemtl":
                            break;
                        case "usemap":
                            break;
                        case "mtllib":
                            break;
                        case "v":
                            mesh.vertices[v] = new Vector3(System.Convert.ToSingle(brokenString[1]), System.Convert.ToSingle(brokenString[2]),
                                                     System.Convert.ToSingle(brokenString[3]));
                            v++;
                            break;
                        case "vt":
                            mesh.uv[vt] = new Vector2(System.Convert.ToSingle(brokenString[1]), System.Convert.ToSingle(brokenString[2]));
                            vt++;
                            break;
                        case "vt1":
                            mesh.uv[vt1] = new Vector2(System.Convert.ToSingle(brokenString[1]), System.Convert.ToSingle(brokenString[2]));
                            vt1++;
                            break;
                        case "vt2":
                            mesh.uv[vt2] = new Vector2(System.Convert.ToSingle(brokenString[1]), System.Convert.ToSingle(brokenString[2]));
                            vt2++;
                            break;
                        case "vn":
                            mesh.normals[vn] = new Vector3(System.Convert.ToSingle(brokenString[1]), System.Convert.ToSingle(brokenString[2]),
                                                    System.Convert.ToSingle(brokenString[3]));
                            vn++;
                            break;
                        case "vc":
                            break;
                        case "f":

                            int j = 1;
                            List<int> intArray = new List<int>();
                            while (j < brokenString.Length && ("" + brokenString[j]).Length > 0)
                            {
                                Vector3 temp = new Vector3();
                                brokenBrokenString = brokenString[j].Split(splitIdentifier2, 3);    //Separate the face into individual components (vert, uv, normal)
                                temp.x = System.Convert.ToInt32(brokenBrokenString[0]);
                                if (brokenBrokenString.Length > 1)                                  //Some .obj files skip UV and normal
                                {
                                    if (brokenBrokenString[1] != "")                                    //Some .obj files skip the uv and not the normal
                                    {
                                        temp.y = System.Convert.ToInt32(brokenBrokenString[1]);
                                    }
                                    temp.z = System.Convert.ToInt32(brokenBrokenString[2]);
                                }
                                j++;

                                mesh.faceData[f2] = temp;
                                intArray.Add(f2);
                                f2++;
                            }
                            j = 1;
                            while (j + 2 < brokenString.Length)     //Create triangles out of the face data.  There will generally be more than 1 triangle per face.
                            {
                                mesh.triangles[f] = intArray[0];
                                f++;
                                mesh.triangles[f] = intArray[j];
                                f++;
                                mesh.triangles[f] = intArray[j + 1];
                                f++;

                                j++;
                            }
                            break;
                    }
                    currentText = reader.ReadLine();
                    if (currentText != null)
                    {
                        currentText = currentText.Replace("  ", " ");       //Some .obj files insert double spaces, this removes them.
                    }
                }
            }
        }
    }
}