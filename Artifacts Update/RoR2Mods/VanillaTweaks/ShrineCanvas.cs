using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

namespace VanillaTweaks
{
    internal static class ShrineCanvasTweak
    {
        public static bool Init()
        {
            On.RoR2.ShrineChanceBehavior.Awake += (orig, self) =>
            {
                addCanvasToShrine(self);
                orig(self);
            };

            On.RoR2.ShrineChanceBehavior.AddShrineStack += (orig, self, activator) =>
            {
                orig(self, activator);
                self.GetComponent<ShrineCanvas>().UpdateTextMeshPros();

            };




            return true;
        }

        public class ShrineCanvas : MonoBehaviour
        {
            public ShrineChanceBehavior self;

            public TextMeshPro[] textMeshPros = new TextMeshPro[5];

            public void Init()
            {


                for (int i = 0; i < 5; i++)
                {
                    GameObject canvas = Instantiate(Resources.Load<GameObject>("Prefabs/CostHologramContent"), self.transform);

                    CostHologramContent content = canvas.GetComponentInChildren<CostHologramContent>();

                    content.costType = CostTypeIndex.PercentHealth;
                    content.displayValue = 69;

                    TextMeshPro targetTextMesh = content.targetTextMesh;
                    canvas.transform.position = self.symbolTransform.position;
                    canvas.transform.Translate(new Vector3(0, -2 - i, 0) + transform.forward);
                    canvas.transform.rotation = self.transform.rotation;
                    canvas.transform.Rotate(new Vector3(0, 180, 0));

                    canvas.SetActive(true);
                    textMeshPros[i] = targetTextMesh;
                    MonoBehaviour.Destroy(content);


                }

                UpdateTextMeshPros();
            }

            public void UpdateTextMeshPros()
            {
                Vector4 percents = Vector4.zero;

                float max = (self.equipmentWeight + self.failureWeight + self.tier1Weight + self.tier2Weight + self.tier3Weight) / 100;

                for (int i = 0; i < 5; i++)
                {
                    string outputText;
                    Color outputColor = Color.black;

                    float tierWeight = 0f;

                    switch (i)
                    {
                        case 0:
                            outputColor = ColorCatalog.GetColor(ColorCatalog.ColorIndex.Tier3Item);
                            tierWeight = self.tier3Weight;
                            break;

                        case 1:
                            outputColor = ColorCatalog.GetColor(ColorCatalog.ColorIndex.Tier2Item);
                            tierWeight = self.tier2Weight;
                            break;

                        case 2:
                            outputColor = ColorCatalog.GetColor(ColorCatalog.ColorIndex.Tier1Item);
                            tierWeight = self.tier1Weight;
                            break;

                        case 3:
                            outputColor = ColorCatalog.GetColor(ColorCatalog.ColorIndex.Equipment);
                            tierWeight = self.equipmentWeight;
                            break;

                        case 4:
                            outputColor = ColorCatalog.GetColor(ColorCatalog.ColorIndex.LunarItemDark);
                            tierWeight = self.failureWeight;
                            break;
                    }
                    GameObject canvas = textMeshPros[i].gameObject;


                    outputText = (int)(tierWeight / max) + "%";

                    textMeshPros[i].color = outputColor;
                    textMeshPros[i].text = outputText;
                    textMeshPros[i].overflowMode = TextOverflowModes.Overflow;

                    if (i != 4)
                    {
                        percents[i] = (tierWeight / max);
                    }
                    else
                    {

                        SendUpdateShrineCanvas(self.netId, percents);

                    }
                }
            }
        }


        public static ShrineCanvas addCanvasToShrine(ShrineChanceBehavior shrine)
        {
            ShrineCanvas shrineCanvas = shrine.gameObject.AddComponent<ShrineCanvas>();
            shrineCanvas.self = shrine;
            shrineCanvas.Init();
            return shrineCanvas;
        }


        public const Int16 HandleId = 88;

        private class UpdateShrineCanvas : MessageBase
        {
            public NetworkInstanceId objectID;
            public Vector4 percents;

            public override void Serialize(NetworkWriter writer)
            {
                writer.Write(objectID);
                writer.Write(percents);
            }

            public override void Deserialize(NetworkReader reader)
            {
                objectID = reader.ReadNetworkId();
                percents = reader.ReadVector4();
            }
        }

        private static void SendUpdateShrineCanvas(NetworkInstanceId shrineID, Vector4 readPercents)
        {
            NetworkServer.SendToAll(HandleId, new UpdateShrineCanvas
            {

                objectID = shrineID,
                percents = readPercents
            });
        }

        [RoR2.Networking.NetworkMessageHandler(msgType = HandleId, client = true)]
        private static void HandleDropItem(NetworkMessage netMsg)
        {
            var dropItem = netMsg.ReadMessage<UpdateShrineCanvas>();
            var percents = dropItem.percents;

            if (!dropItem.objectID.IsEmpty())
            {
                GameObject obj = ClientScene.FindLocalObject(dropItem.objectID);
                {
                    if (obj.GetComponent<ShrineChanceBehavior>())
                    {
                        if (!obj.GetComponent<ShrineCanvas>())
                        {
                            addCanvasToShrine(obj.GetComponent<ShrineChanceBehavior>());
                        }
                        else
                        {
                            ShrineCanvas canvas = obj.GetComponent<ShrineCanvas>();
                            canvas.self.tier3Weight = percents.x;
                            canvas.self.tier2Weight = percents.y;
                            canvas.self.tier1Weight = percents.z;
                            canvas.self.equipmentWeight = percents.w;
                            canvas.self.failureWeight = 100 - (percents.x + percents.y + percents.z + percents.w);
                            if (!NetworkServer.active)
                            {
                                canvas.UpdateTextMeshPros();
                            }
                        }
                    }

                }
            }
        }

    }
}
