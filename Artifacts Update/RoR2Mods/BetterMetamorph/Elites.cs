using BepInEx;
using EliteSpawningOverhaul;
using EntityStates.Destructible;
using HG;
using PallesenProductions;
using R2API;
using R2API.Utils;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace BetterMetamorph
{
    public class Elites
    {
        public static bool Init()
        {
            List<Func<bool>> eliteInits = new List<Func<bool>>();

            eliteInits.Add(BlightborneElite.Init);
            eliteInits.Add(PoisonousElite.Init);
            eliteInits.Add(TarborneElite.Init);

            foreach (Func<bool> func in eliteInits)
            {
                func.Invoke();
            }


            On.RoR2.CharacterModel.UpdateMaterials += CharacterModelOnUpdateMaterials;

            return true;
        }

        private static void CharacterModelOnUpdateMaterials(On.RoR2.CharacterModel.orig_UpdateMaterials orig, CharacterModel self)
        {
            orig(self);

            //Vanilla elites aren't adjusted
            var eliteIndex = self.GetFieldValue<EliteIndex>("myEliteIndex");
            if (eliteIndex < EliteIndex.Count)
                return;

            var eliteDef = EliteCatalog.GetEliteDef(eliteIndex);
            var rendererInfos = self.baseRendererInfos;
            var propertyStorage = self.GetFieldValue<MaterialPropertyBlock>("propertyStorage");
            for (int i = rendererInfos.Length - 1; i >= 0; --i)
            {
                var baseRendererInfo = rendererInfos[i];
                Renderer renderer = baseRendererInfo.renderer;
                renderer.GetPropertyBlock(propertyStorage);
                propertyStorage.SetInt((int)CommonShaderProperties._EliteIndex, 0);
                propertyStorage.SetColor("_Color", eliteDef.color);
                renderer.SetPropertyBlock(propertyStorage);
            }
        }
    }
    public class BlightborneElite
    {
        public static string EliteName = "Blightborne";
        public static string BuffName = "Affix_Blightborne";
        public static string EquipName = "Blight_Equip";

        private static EliteIndex _eliteIndex;
        private static BuffIndex _buffIndex;
        private static EquipmentIndex _equipIndex;

        public static EliteAffixCard Card { get; set; }

        private static void GlobalEventManager_ServerDamageDealt(On.RoR2.GlobalEventManager.orig_ServerDamageDealt orig, DamageReport damageReport)
        {
            orig(damageReport);

            if ((damageReport.attackerBody) && damageReport.dotType == DotController.DotIndex.None && damageReport.attackerBody.HasBuff(_buffIndex))
            {
                damageReport.victimBody.AddTimedBuff(BuffIndex.Blight, 2f);
                DotController.InflictDot(damageReport.victimBody.gameObject, damageReport.attacker, DotController.DotIndex.Blight, 2f, 1f);

            }
        }

        private static void OnSpawned(CharacterMaster master)
        {
            var bodyObj = master.GetBodyObject();
        }

        public static bool Init()
        {
            On.RoR2.GlobalEventManager.ServerDamageDealt += GlobalEventManager_ServerDamageDealt;

            var equipDef = new EquipmentDef
            {
                name = EquipName,
                cooldown = 10f,
                pickupModelPath = "",
                pickupIconPath = "",
                nameToken = EquipName,
                pickupToken = "Pickup_Blightborne",
                descriptionToken = "Description_Blightborne",
                canDrop = false,
                enigmaCompatible = false
            };

            var equip = new CustomEquipment(equipDef, new ItemDisplayRule[0]);

            var buffDef = new BuffDef
            {
                name = BuffName,
                buffColor = new Color32(141, 51, 19, 255),
                iconPath = "",
                canStack = false
            };
            var buff = new CustomBuff(buffDef);

            var eliteDef = new EliteDef
            {
                name = EliteName,
                modifierToken = "ELITE_MODIFIER_BLIGHTBORNE",
                color = buffDef.buffColor,
                eliteEquipmentIndex = _equipIndex
            };

            var elite = new CustomElite(eliteDef, 1);

            _eliteIndex = EliteAPI.Add(elite);
            _buffIndex = BuffAPI.Add(buff);
            _equipIndex = ItemAPI.Add(equip);
            eliteDef.eliteEquipmentIndex = _equipIndex;
            equipDef.passiveBuff = _buffIndex;
            buffDef.eliteIndex = _eliteIndex;

            On.RoR2.CharacterBody.OnEquipmentLost += CharacterBody_OnEquipmentLost;
            On.RoR2.CharacterBody.OnEquipmentGained += CharacterBody_OnEquipmentGained;
            var card = new EliteAffixCard
            {
                spawnWeight = 0.5f,
                costMultiplier = 16f,
                damageBoostCoeff = 2.0f,
                healthBoostCoeff = 8.0f,
                eliteOnlyScaling = 1f,
                eliteType = _eliteIndex,
                onSpawned = OnSpawned,
                isAvailable = new Func<bool>(() => Run.instance.stageClearCount > 3 && Run.instance.selectedDifficulty != DifficultyIndex.Easy)

            };

            //Register the card for spawning if ESO is enabled
            EsoLib.Cards.Add(card);
            Card = card;

            //Description of elite in UI when boss
            LanguageAPI.Add(eliteDef.modifierToken, "Blightborne {0}");
            LanguageAPI.Add(equipDef.pickupToken, "Blight's Embrace");
            LanguageAPI.Add(equipDef.descriptionToken, "Become an aspect of Blight");

            return true;
        }
        private static void CharacterBody_OnEquipmentGained(On.RoR2.CharacterBody.orig_OnEquipmentGained orig, CharacterBody self, EquipmentDef equipmentDef)
        {
            orig(self, equipmentDef);

            if (equipmentDef.equipmentIndex == _equipIndex)
            {
                self.AddBuff(_buffIndex);
            }
        }
        private static void CharacterBody_OnEquipmentLost(On.RoR2.CharacterBody.orig_OnEquipmentLost orig, CharacterBody self, EquipmentDef equipmentDef)
        {
            orig(self, equipmentDef);

            if (equipmentDef.equipmentIndex == _equipIndex)
            {
                self.RemoveBuff(_buffIndex);
            }
        }


    }
    public class PoisonousElite
    {
        public static string EliteName = "Poisonous";
        public static string BuffName = "Affix_Poisonous";
        public static string EquipName = "Poisonous_Equip";

        private static EliteIndex _eliteIndex;
        private static BuffIndex _buffIndex;
        private static EquipmentIndex _equipIndex;

        public static EliteAffixCard Card { get; set; }

        private static void GlobalEventManager_ServerDamageDealt(On.RoR2.GlobalEventManager.orig_ServerDamageDealt orig, DamageReport damageReport)
        {
            orig(damageReport);

            if ((damageReport.attackerBody) && damageReport.dotType == DotController.DotIndex.None && damageReport.attackerBody.HasBuff(_buffIndex))
            {
                damageReport.victimBody.AddTimedBuff(BuffIndex.Poisoned, 2f);
                DotController.InflictDot(damageReport.victimBody.gameObject, damageReport.attacker, DotController.DotIndex.Poison, 2f, 1f);

            }
        }

        private static void OnSpawned(CharacterMaster master)
        {
            var bodyObj = master.GetBodyObject();
        }

        public static bool Init()
        {
            On.RoR2.GlobalEventManager.ServerDamageDealt += GlobalEventManager_ServerDamageDealt;

            var equipDef = new EquipmentDef
            {
                name = EquipName,
                cooldown = 10f,
                pickupModelPath = "",
                pickupIconPath = "",
                nameToken = EquipName,
                pickupToken = "Pickup_Poisonous",
                descriptionToken = "Description_Poisonous",
                canDrop = false,
                enigmaCompatible = false
            };

            On.RoR2.CharacterBody.OnEquipmentLost += CharacterBody_OnEquipmentLost;
            On.RoR2.CharacterBody.OnEquipmentGained += CharacterBody_OnEquipmentGained;
            var equip = new CustomEquipment(equipDef, new ItemDisplayRule[0]);

            var buffDef = new BuffDef
            {
                name = BuffName,
                buffColor = new Color32(21, 109, 51, 255),
                iconPath = "",
                canStack = false
            };
            var buff = new CustomBuff(buffDef);

            var eliteDef = new EliteDef
            {
                name = EliteName,
                modifierToken = "ELITE_MODIFIER_POISONOUS",
                color = buffDef.buffColor,
                eliteEquipmentIndex = _equipIndex
            };

            var elite = new CustomElite(eliteDef, 1);

            _eliteIndex = EliteAPI.Add(elite);
            _buffIndex = BuffAPI.Add(buff);
            _equipIndex = ItemAPI.Add(equip);
            eliteDef.eliteEquipmentIndex = _equipIndex;
            equipDef.passiveBuff = _buffIndex;
            buffDef.eliteIndex = _eliteIndex;


            var card = new EliteAffixCard
            {
                spawnWeight = 0.5f,
                costMultiplier = 12f,
                damageBoostCoeff = 0.8f,
                healthBoostCoeff = 5.0f,
                eliteOnlyScaling = 1f,
                eliteType = _eliteIndex,
                onSpawned = OnSpawned,
                isAvailable = new Func<bool>(() => Run.instance.stageClearCount > 3 && Run.instance.selectedDifficulty != DifficultyIndex.Easy)

            };

            //Register the card for spawning if ESO is enabled
            EsoLib.Cards.Add(card);
            Card = card;

            //Description of elite in UI when boss
            LanguageAPI.Add(eliteDef.modifierToken, "Poisonous {0}");
            LanguageAPI.Add(equipDef.pickupToken, "Acrid's Brood");
            LanguageAPI.Add(equipDef.descriptionToken, "The poison corrupts....");

            return true;
        }

        private static void CharacterBody_OnEquipmentGained(On.RoR2.CharacterBody.orig_OnEquipmentGained orig, CharacterBody self, EquipmentDef equipmentDef)
        {
            orig(self, equipmentDef);

            if (equipmentDef.equipmentIndex == _equipIndex)
            {
                self.AddBuff(_buffIndex);
            }
        }
        private static void CharacterBody_OnEquipmentLost(On.RoR2.CharacterBody.orig_OnEquipmentLost orig, CharacterBody self, EquipmentDef equipmentDef)
        {
            orig(self, equipmentDef);

            if (equipmentDef.equipmentIndex == _equipIndex)
            {
                self.RemoveBuff(_buffIndex);
            }
        }

    }

    public class TarborneElite
    {
        public static string EliteName = "Tarborne";
        public static string BuffName = "Affix_Tarborne";
        public static string EquipName = "Tarborne_Equip";

        private static EliteIndex _eliteIndex;
        private static BuffIndex _buffIndex;
        private static EquipmentIndex _equipIndex;
        private static GameObject _tetherPrefab;

        public static EliteAffixCard Card { get; set; }


        private static void OnSpawned(CharacterMaster master)
        {
            var bodyObj = master.GetBodyObject();

            BuildTetherMaster(bodyObj);
        }

        public static bool Init()
        {

            On.RoR2.GlobalEventManager.OnCharacterDeath += GlobalEventManager_OnCharacterDeath;

            var equipDef = new EquipmentDef
            {
                name = EquipName,
                cooldown = 10f,
                pickupModelPath = "",
                pickupIconPath = "",
                nameToken = EquipName,
                pickupToken = "Pickup_Tarborne",
                descriptionToken = "Description_Tarborne",
                canDrop = false,
                enigmaCompatible = false
            };

            On.RoR2.CharacterBody.OnEquipmentLost += CharacterBody_OnEquipmentLost;
            On.RoR2.CharacterBody.OnEquipmentGained += CharacterBody_OnEquipmentGained;
            var equip = new CustomEquipment(equipDef, new ItemDisplayRule[0]);

            var buffDef = new BuffDef
            {
                name = BuffName,
                buffColor = new Color32(41, 41, 41, 255),
                iconPath = "",
                canStack = false
            };
            var buff = new CustomBuff(buffDef);

            var eliteDef = new EliteDef
            {
                name = EliteName,
                modifierToken = "ELITE_MODIFIER_TARBORNE",
                color = buffDef.buffColor,
                eliteEquipmentIndex = _equipIndex
            };

            var elite = new CustomElite(eliteDef, 1);

            _eliteIndex = EliteAPI.Add(elite);
            _buffIndex = BuffAPI.Add(buff);
            _equipIndex = ItemAPI.Add(equip);
            eliteDef.eliteEquipmentIndex = _equipIndex;
            equipDef.passiveBuff = _buffIndex;
            buffDef.eliteIndex = _eliteIndex;


            var card = new EliteAffixCard
            {
                spawnWeight = 1f,
                costMultiplier = 6f,
                damageBoostCoeff = 2.0f,
                healthBoostCoeff = 4.0f,
                eliteOnlyScaling = 1f,
                eliteType = _eliteIndex,
                onSpawned = OnSpawned,
                isAvailable = new Func<bool>(() => true)

            };

            //Register the card for spawning if ESO is enabled
            EsoLib.Cards.Add(card);
            Card = card;

            //Description of elite in UI when boss
            LanguageAPI.Add(eliteDef.modifierToken, "Tarborne {0}");
            LanguageAPI.Add(equipDef.pickupToken, "Dunestrider's Dominion");
            LanguageAPI.Add(equipDef.descriptionToken, "Become an aspect of Tar");

            return true;
        }

        private static TarBorneTetherMaster BuildTetherMaster(GameObject gameObj)
        {
            var tetherMaster = gameObj.AddComponent<TarBorneTetherMaster>();
            tetherMaster.TetherPrefab = _tetherPrefab;
            tetherMaster.CanTether = TetherMasterCanTether;
            tetherMaster.Radius = 20f;
            tetherMaster.enabled = true;

            return tetherMaster;
        }

        private static void DestroyTetherMaster(GameObject gameObj)
        {
            var tetherMaster = gameObj.AddComponent<TarBorneTetherMaster>();
            GameObject.Destroy(tetherMaster);


        }

        private static bool TetherMasterCanTether(GameObject gameObj)
        {
            var body = gameObj.GetComponent<CharacterBody>();
            if (body == null)
                return false;

            return !body.HasBuff(_buffIndex);
        }

        private static void CharacterBody_OnEquipmentGained(On.RoR2.CharacterBody.orig_OnEquipmentGained orig, CharacterBody self, EquipmentDef equipmentDef)
        {
            orig(self, equipmentDef);

            if (equipmentDef.equipmentIndex == _equipIndex)
            {
                self.AddBuff(_buffIndex);
                BuildTetherMaster(self.gameObject);

            }
        }
        private static void CharacterBody_OnEquipmentLost(On.RoR2.CharacterBody.orig_OnEquipmentLost orig, CharacterBody self, EquipmentDef equipmentDef)
        {
            orig(self, equipmentDef);

            if (equipmentDef.equipmentIndex == _equipIndex)
            {
                self.RemoveBuff(_buffIndex);
                DestroyTetherMaster(self.gameObject);

            }
        }

        private static void GlobalEventManager_OnCharacterDeath(On.RoR2.GlobalEventManager.orig_OnCharacterDeath orig, GlobalEventManager self, DamageReport damageReport)
        {
            orig(self, damageReport);

            if (damageReport.victimBody.HasBuff(_buffIndex))
            {
                if (ExplosivePotDeath.chargePrefab)
                {
                    UnityEngine.Object.Instantiate<GameObject>(ExplosivePotDeath.chargePrefab, damageReport.victimBody.transform);

                    if (ExplosivePotDeath.explosionEffectPrefab)
                    {
                        EffectManager.SpawnEffect(ExplosivePotDeath.explosionEffectPrefab, new EffectData
                        {
                            origin = damageReport.victimBody.transform.position,
                            scale = ExplosivePotDeath.explosionRadius,
                            rotation = Quaternion.identity,

                        }, true);
                    }
                    new BlastAttack
                    {
                        attacker = damageReport.victimBody.gameObject,
                        damageColorIndex = DamageColorIndex.Item,
                        baseDamage = damageReport.victimBody.damage * 0.1f,
                        radius = ExplosivePotDeath.explosionRadius,
                        falloffModel = BlastAttack.FalloffModel.None,
                        procCoefficient = ExplosivePotDeath.explosionProcCoefficient,
                        teamIndex = TeamIndex.None,
                        damageType = DamageType.ClayGoo,
                        position = damageReport.victimBody.transform.position,
                        baseForce = ExplosivePotDeath.explosionForce * 4f,
                        attackerFiltering = AttackerFiltering.NeverHit

                    }.Fire();
                }
            }
        }
    }
}

public sealed class TarBorneTetherMaster : MonoBehaviour
{
    private List<GameObject> _newTethered = new List<GameObject>();
    private List<GameObject> _tetheredObjects = new List<GameObject>();
    private TeamComponent _teamComponent;
    private Collider[] _colliders;

    public GameObject TetherPrefab;

    public float Radius = 5f;

    public Func<GameObject, bool> CanTether = x => x.GetComponent<CharacterBody>() != null;

    public IReadOnlyList<GameObject> GetTetheredObjects() => _tetheredObjects;

    private void Awake()
    {
        _teamComponent = GetComponent<TeamComponent>();
        selfBody = GetComponent<CharacterBody>();
        _colliders = new Collider[20];
    }

    private void AddToList(GameObject affectedObject)
    {
        if (TetherPrefab != null)
        {
            var tetherObj = Instantiate(TetherPrefab, affectedObject.transform);
            tetherObj.SetActive(true);

            var component = tetherObj.GetComponent<TetherVfx>();
            component.tetherEndTransform = transform;
            //component.tetherMaxDistance = Radius + 1.5f;
        }

        _tetheredObjects.Add(affectedObject);
    }

    public float timer = 0f;
    public float cooldown = 4f;

    private float TimeToSuckFor = 4f;
    private float TimeSpentSucking = 0f;

    private bool shouldSuck;

    private CharacterBody selfBody;
    private void FixedUpdate()
    {
        int colliderCount = Physics.OverlapSphereNonAlloc(transform.position, Radius, _colliders, LayerIndex.defaultLayer.mask);
        _newTethered.Clear();
        for (var i = 0; i < colliderCount; i++)
        {
            var collider = _colliders[i];
            var colliderObj = collider.gameObject;
            bool canTether = colliderObj != gameObject && CanTether(colliderObj);
            if (canTether && _teamComponent)
            {
                var teamComponent = collider.GetComponent<TeamComponent>();
                canTether = teamComponent && teamComponent.teamIndex == _teamComponent.teamIndex;
            }

            if (canTether)
            {
                _newTethered.Add(colliderObj);
            }
        }

        //Any new objects should have a tether constructed
        foreach (var tethered in _newTethered)
        {
            if (!_tetheredObjects.Contains(tethered))
                AddToList(tethered);
        }

        //Replace the list so it only shows what's currently in-range and thus tethered
        _tetheredObjects.Clear();
        _tetheredObjects.AddRange(_newTethered);

        timer += Time.deltaTime;

        if (timer > cooldown && shouldSuck == false)
        {
            TimeSpentSucking = 0;

            if (selfBody.healthComponent.combinedHealth < selfBody.healthComponent.fullCombinedHealth / 3f)
            {
                timer = 0;
                shouldSuck = true;
            }
        }

        if (TimeSpentSucking > TimeToSuckFor)
        {
            TimeSpentSucking = 0;
            timer = -cooldown;
            shouldSuck = false;
        }

        if (shouldSuck && NetworkServer.active) UpdateSuck();
    }

    private int timesSucked = 0;
    public void UpdateSuck()
    {
        TimeSpentSucking += Time.deltaTime;

        foreach (var tethered in _tetheredObjects)
        {
            if (selfBody.healthComponent.health <= selfBody.healthComponent.fullHealth * (1 - timesSucked))
            {
                CharacterBody cb = tethered.GetComponent<CharacterBody>();
                DamageInfo damageInfo = new DamageInfo();
                damageInfo.attacker = selfBody.gameObject;
                damageInfo.inflictor = base.gameObject;
                damageInfo.position = transform.position;
                damageInfo.crit = selfBody.RollCrit();
                damageInfo.damage = selfBody.damage / 16f;
                damageInfo.damageColorIndex = DamageColorIndex.Poison;
                damageInfo.force = Vector3.zero;
                damageInfo.procCoefficient = 10f;
                damageInfo.damageType = DamageType.ClayGoo;
                damageInfo.procChainMask = default(ProcChainMask);
                cb.healthComponent.TakeDamage(damageInfo);
                selfBody.healthComponent.Heal(selfBody.damage / 16f, default(ProcChainMask));
            }
        }
    }
}


//heal on server only
//befuddle on server only
///panic butto not working on clients\
//beffudle reset on run start