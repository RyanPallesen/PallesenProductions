using R2API.Utils;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace VanillaTweaks
{
    static class MultiElites
    {

        public static bool Init()
        {
            //Allow any enemy to be a boss, allow any enemy to be elite.
            On.RoR2.CharacterSpawnCard.Awake += (orig, self) =>
            {
                orig(self);
                self.forbiddenAsBoss = false;
                self.noElites = false;
            };

            On.RoR2.CombatDirector.Awake += MultiEliteHook;


            return true;



        }



        private static void MultiEliteHook(On.RoR2.CombatDirector.orig_Awake orig, CombatDirector self)
        {
            orig(self);

            self.onSpawnedServer.AddListener((OG) => doMultiElite(OG, self));
        }

        private static void doMultiElite(GameObject gameObject, CombatDirector self)
        {
            DirectorCard card = self.lastAttemptedMonsterCard;

            float credit = self.monsterCredit;

            CharacterBody body = gameObject.GetComponentInChildren<CharacterMaster>().GetBody();
            CombatDirector.EliteTierDef[] eliteDefs = typeof(CombatDirector).GetFieldValue<CombatDirector.EliteTierDef[]>("eliteTiers");

            int currentEliteTypes = 1;

            foreach (BuffIndex buff in BuffCatalog.eliteBuffIndices)
            {
                BuffDef buffDef = BuffCatalog.GetBuffDef(buff);
                if (body.HasBuff(buff))
                {
                    currentEliteTypes += 1;
                }
            }
            if (currentEliteTypes > 1)
            {

                foreach (CombatDirector.EliteTierDef tierDef in eliteDefs)
                {
                    foreach (EliteIndex eliteIndex in tierDef.eliteTypes)
                    {

                        if (credit > card.cost * tierDef.costMultiplier * currentEliteTypes)
                        {
                            BuffDef buffDef = BuffCatalog.GetBuffDef(BuffIndex.None);

                            foreach (BuffIndex buff in BuffCatalog.eliteBuffIndices)
                            {
                                BuffDef tempDef = BuffCatalog.GetBuffDef(buff);

                                if (tempDef.eliteIndex == eliteIndex)
                                {
                                    buffDef = tempDef;
                                }
                            }

                            if (buffDef != null)
                            {
                                if (!(body.HasBuff(buffDef.buffIndex)))
                                {
                                    body.AddBuff(buffDef.buffIndex);
                                    self.monsterCredit -= card.cost * tierDef.costMultiplier * currentEliteTypes;
                                    credit -= card.cost * tierDef.costMultiplier * currentEliteTypes;
                                    currentEliteTypes++;

                                    float num3 = tierDef.healthBoostCoefficient;
                                    float damageBoostCoefficient = tierDef.damageBoostCoefficient;
                                    if (self.combatSquad)
                                    {
                                        int livingPlayerCount = Run.instance.livingPlayerCount;
                                        num3 *= Mathf.Pow((float)livingPlayerCount, 1f);
                                    }
                                    body.inventory.GiveItem(ItemIndex.BoostHp, Mathf.RoundToInt((num3 - 1f) * 10f));
                                    body.inventory.GiveItem(ItemIndex.BoostDamage, Mathf.RoundToInt((damageBoostCoefficient - 1f) * 10f));

                                    EliteDef eliteDef = EliteCatalog.GetEliteDef(eliteIndex);
                                    EquipmentIndex equipmentIndex = (eliteDef != null) ? eliteDef.eliteEquipmentIndex : EquipmentIndex.None;
                                    if (equipmentIndex != EquipmentIndex.None)
                                    {
                                        if (body.inventory.GetEquipmentIndex() == EquipmentIndex.None)
                                        {
                                            body.inventory.SetEquipmentIndex(equipmentIndex);
                                        }
                                    }
                                }
                            }

                        }
                    }
                }

            }
        }



    }
}
