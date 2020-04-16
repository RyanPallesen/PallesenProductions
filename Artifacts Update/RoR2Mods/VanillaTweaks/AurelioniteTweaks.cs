using EntityStates.Missions.Goldshores;
using R2API.Utils;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace VanillaTweaks
{
    static class AurelioniteTweaks
    {
        //Gets the current average number of halcyon seeds owned by each player.
        private static int GoldSpawns
        {
            get
            {
                int numItems = 0;
                System.Collections.ObjectModel.ReadOnlyCollection<TeamComponent> teamMembers = TeamComponent.GetTeamMembers(TeamIndex.Player);

                for (int i = 0; i < teamMembers.Count; i++)
                {
                    if (Util.LookUpBodyNetworkUser(teamMembers[i].gameObject))
                    {
                        CharacterBody component = teamMembers[i].GetComponent<CharacterBody>();
                        if (component && component.inventory)
                        {
                            numItems += component.inventory.GetItemCount(ItemIndex.TitanGoldDuringTP);
                        }
                    }
                }

                return Mathf.FloorToInt(numItems / Run.instance.participatingPlayerCount);
            }
        }

        public static bool Init()
        {
            On.RoR2.TeleporterInteraction.Awake += (orig, self) =>
            {
                orig(self);

                if (NetworkServer.active)
                {
                    if (Run.instance.stageRngGenerator.RangeFloat(0, 100) < 5)
                    {
                        self.shouldAttemptToSpawnGoldshoresPortal = true;
                    }
                }

            };

            //spawn extra beacons based on goldspawns
            On.RoR2.GoldshoresMissionController.SpawnBeacons += (orig, self) =>
            {
                self.beaconsToSpawnOnMap += GoldSpawns;
                self.beaconsRequiredToSpawnBoss = 0;
                orig(self);
            };

            //spawn the ally based on halcyon seeds
            On.RoR2.TeleporterInteraction.ChargingState.TrySpawnTitanGoldServer += (orig, self) =>
            {
                SpawnTitanByCompletion(TeamIndex.Player);
            };

            //logic for spawning the AI in goldshores
            On.EntityStates.Missions.Goldshores.GoldshoresBossfight.SpawnBoss += (orig, self) =>
            {

                ScriptedCombatEncounter scriptedCombatEncounter = self.GetFieldValue<ScriptedCombatEncounter>("scriptedCombatEncounter");

                if (!scriptedCombatEncounter)
                { 
                    scriptedCombatEncounter = UnityEngine.Object.Instantiate<GameObject>(GoldshoresBossfight.combatEncounterPrefab).GetComponent<ScriptedCombatEncounter>();
                    scriptedCombatEncounter.GetComponent<BossGroup>().dropPosition = self.GetFieldValue<GoldshoresMissionController>("missionController").bossSpawnPosition;
                    NetworkServer.Spawn(scriptedCombatEncounter.gameObject);
                }

                CharacterBody tempBody = SpawnTitanByCompletion(TeamIndex.Monster);

                if(tempBody != null)
                {
                    scriptedCombatEncounter.combatSquad.AddMember(tempBody.master);
                }

                self.SetFieldValue<ScriptedCombatEncounter>("scriptedCombatEncounter", scriptedCombatEncounter);

                orig(self);

            };

            //handle super elite equipments
            On.RoR2.CharacterBody.OnEquipmentGained += (orig, self, def) =>
            {

                if (def.equipmentIndex == EquipmentIndex.AffixGold)
                {
                    self.AddBuff(BuffIndex.BugWings);
                    self.AddBuff(BuffIndex.AffixBlue);
                    self.AddBuff(BuffIndex.AffixWhite);
                    self.AddBuff(BuffIndex.AffixRed);
                }
                if (def.equipmentIndex == EquipmentIndex.AffixYellow)
                {
                    self.AddBuff(BuffIndex.AffixHaunted);
                    self.AddBuff(BuffIndex.AffixHauntedRecipient);
                    self.AddBuff(BuffIndex.AffixPoison);
                    self.AddBuff(BuffIndex.AffixBlue);
                    self.AddBuff(BuffIndex.AffixWhite);
                    self.AddBuff(BuffIndex.AffixRed);
                }

                orig(self, def);

            };

            //handle super elite equipments
            On.RoR2.CharacterBody.OnEquipmentLost += (orig, self, def) =>
            {
                if (def.equipmentIndex == EquipmentIndex.AffixGold)
                {
                    self.RemoveBuff(BuffIndex.BugWings);
                    self.RemoveBuff(BuffIndex.AffixBlue);
                    self.RemoveBuff(BuffIndex.AffixWhite);
                    self.RemoveBuff(BuffIndex.AffixRed);
                }
                if (def.equipmentIndex == EquipmentIndex.AffixYellow)
                {
                    self.RemoveBuff(BuffIndex.AffixHaunted);
                    self.RemoveBuff(BuffIndex.AffixHauntedRecipient);
                    self.RemoveBuff(BuffIndex.AffixPoison);
                    self.RemoveBuff(BuffIndex.AffixBlue);
                    self.RemoveBuff(BuffIndex.AffixWhite);
                    self.RemoveBuff(BuffIndex.AffixRed);
                }


                orig(self, def);
            };




            return true;
        }

        private static CharacterBody SpawnTitanByCompletion(TeamIndex index = TeamIndex.Monster)
        {
            int cacheGoldSpawns = GoldSpawns;
            int tempGoldSpawns = cacheGoldSpawns;


            CharacterBody tempBody = null;

            while (cacheGoldSpawns > 0)
            {
                if (cacheGoldSpawns > 8)
                {
                    tempGoldSpawns = 8;
                }
                else
                {
                    tempGoldSpawns = cacheGoldSpawns;
                }

                if (tempGoldSpawns == 1)
                {
                    tempBody = SpawnTitan(EquipmentIndex.None, index);

                }
                if (tempGoldSpawns == 2)
                {
                    tempBody = SpawnTitan(EquipmentIndex.AffixWhite, index);

                }
                if (tempGoldSpawns == 3)
                {
                    tempBody = SpawnTitan(EquipmentIndex.AffixBlue, index);

                }
                if (tempGoldSpawns == 4)
                {
                    tempBody = SpawnTitan(EquipmentIndex.AffixRed, index);

                }
                if (tempGoldSpawns == 5)
                {
                    tempBody = SpawnTitan(EquipmentIndex.AffixPoison, index);

                }
                if (tempGoldSpawns == 6)
                {
                    tempBody = SpawnTitan(EquipmentIndex.AffixGold, index);

                }
                if (tempGoldSpawns == 7)
                {
                    tempBody = SpawnTitan(EquipmentIndex.AffixHaunted, index);

                }
                if (tempGoldSpawns == 8)
                {
                    tempBody = SpawnTitan(EquipmentIndex.AffixYellow, index);

                }
                cacheGoldSpawns -= tempGoldSpawns;
            }

            return tempBody;
        }

        private static CharacterBody SpawnTitan(EquipmentIndex affix, TeamIndex index = TeamIndex.Monster)
        {
            int numGoldItems = 0;

            System.Collections.ObjectModel.ReadOnlyCollection<TeamComponent> teamMembers = TeamComponent.GetTeamMembers(TeamIndex.Player);
            for (int i = 0; i < teamMembers.Count; i++)
            {
                if (Util.LookUpBodyNetworkUser(teamMembers[i].gameObject))
                {
                    CharacterBody component = teamMembers[i].GetComponent<CharacterBody>();
                    if (component && component.inventory)
                    {
                        numGoldItems += component.inventory.GetItemCount(ItemIndex.TitanGoldDuringTP);
                    }
                }
            }

            if (index == TeamIndex.Monster)
            {
                numGoldItems += Run.instance.stageClearCount;
                numGoldItems *= Run.instance.livingPlayerCount;
            }

            Vector3 position = Vector3.zero;
            if (index == TeamIndex.Player)
            {
                position = TeleporterInteraction.instance.transform.position;
            }

            DirectorPlacementRule placementRule = new DirectorPlacementRule
            {
                placementMode = DirectorPlacementRule.PlacementMode.NearestNode,
                minDistance = 20f,
                maxDistance = 130f,
                position = position
            };
            if (index == TeamIndex.Monster)
            {
                placementRule.placementMode = DirectorPlacementRule.PlacementMode.Random;
            }

            DirectorSpawnRequest directorSpawnRequest = new DirectorSpawnRequest(Resources.Load<SpawnCard>("SpawnCards/CharacterSpawnCards/cscTitanGold"), placementRule, Run.instance.spawnRng);
            directorSpawnRequest.ignoreTeamMemberLimit = true;
            directorSpawnRequest.teamIndexOverride = new TeamIndex?(index);
            GameObject gameObject = DirectorCore.instance.TrySpawnObject(directorSpawnRequest);
            if (gameObject)
            {
                float num2 = 1f;
                float num3 = 1f;
                num3 += Run.instance.difficultyCoefficient / 8f;
                num2 += Run.instance.difficultyCoefficient / 2f;
                CharacterMaster component2 = gameObject.GetComponent<CharacterMaster>();
                if (index == TeamIndex.Monster)
                {
                    component2.isBoss = true;

                }
                CharacterBody body = component2.GetBody();

                if (index == TeamIndex.Monster)
                {
                    body.isChampion = true;
                }

                int livingPlayerCount = Run.instance.livingPlayerCount;
                num2 *= Mathf.Pow((float)numGoldItems, 1f);
                num3 *= Mathf.Pow((float)numGoldItems, 0.5f);
                component2.inventory.GiveItem(ItemIndex.BoostHp, Mathf.RoundToInt((num2 - 1f) * 10f));
                component2.inventory.GiveItem(ItemIndex.BoostDamage, Mathf.RoundToInt((num3 - 1f) * 10f));
                component2.inventory.SetEquipmentIndex(affix);

                return body;
            }

            return null;
        }
    }
}
