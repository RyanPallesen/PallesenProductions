using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace VanillaTweaks
{
    static class EliteEquipment
    {
        public static bool Init()
        {
            On.RoR2.CharacterMaster.GetDeployableSameSlotLimit += (orig, self, slot) =>
            {
                if (slot == (DeployableSlot)7)
                {
                    return self.inventory.GetItemCount(ItemIndex.EquipmentMagazine);
                }

                return orig(self, slot);
            };

            On.RoR2.EquipmentSlot.PerformEquipmentAction += (orig, self, equipmentIndex) =>
            {
                CharacterBody characterBody = self.characterBody;
                string characterMaster = "";

                bool temp = orig(self, equipmentIndex);

                if (self.characterBody.inputBank.activateEquipment.justPressed)
                {
                    switch (equipmentIndex)
                    {
                        case EquipmentIndex.AffixWhite:
                            {
                                characterMaster = "BisonMaster";

                                temp = true;
                            }

                            break;
                        case EquipmentIndex.AffixGold:
                            {
                                characterMaster = "TitanGoldMaster";

                                temp = true;
                            }
                            break;
                        case EquipmentIndex.AffixHaunted:
                            {
                                characterMaster = "BellMaster";

                                temp = true;
                            }
                            break;
                        case EquipmentIndex.AffixPoison:
                            {
                                characterMaster = "ImpMaster";

                                temp = true;
                            }
                            break;
                        case EquipmentIndex.AffixRed:
                            {
                                characterMaster = "LemurianMaster";

                            }
                            break;
                        case EquipmentIndex.AffixBlue:
                            {
                                characterMaster = "WispMaster";

                            }
                            break;
                        case EquipmentIndex.AffixYellow:
                            {
                                characterMaster = "ElectricWormMaster";
                                temp = true;
                            }
                            break;
                    }

                    if (characterMaster != "")
                    {
                        CharacterMaster newcharacterMaster = new MasterSummon
                        {
                            masterPrefab = MasterCatalog.FindMasterPrefab(characterMaster),
                            position = characterBody.footPosition + characterBody.transform.up,
                            rotation = characterBody.transform.rotation,
                            summonerBodyObject = null,
                            ignoreTeamMemberLimit = true,
                            teamIndexOverride = characterBody.teamComponent.teamIndex

                        }.Perform();

                        newcharacterMaster.inventory.CopyItemsFrom(characterBody.inventory);
                        newcharacterMaster.inventory.ResetItem(ItemIndex.AutoCastEquipment);
                        newcharacterMaster.inventory.CopyEquipmentFrom(characterBody.inventory);
                        newcharacterMaster.GetBody().modelLocator.modelBaseTransform.localScale *= 0.25f;

                        Deployable deployable = newcharacterMaster.gameObject.GetComponent<Deployable>();
                        if (!deployable)
                        {
                            deployable = newcharacterMaster.gameObject.AddComponent<Deployable>();
                        }


                        int deployableCount = characterBody.master.GetDeployableCount((DeployableSlot)7);

                        self.characterBody.master.AddDeployable(deployable, (DeployableSlot)7);
                        deployable.onUndeploy = new UnityEngine.Events.UnityEvent();
                        deployable.onUndeploy.AddListener(() => newcharacterMaster.TrueKill());


                    }
                }
                return temp;
            };

            return true;
        }
    }
}
