using BepInEx;
using R2API;
using R2API.Utils;
using RoR2;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Assertions.Must;
using Random = UnityEngine.Random;

namespace PallesenProductions
{
    [R2APISubmoduleDependency(nameof(BuffAPI))]
    [R2APISubmoduleDependency(nameof(SurvivorAPI))]
    [R2APISubmoduleDependency(nameof(ItemAPI))]
    [R2APISubmoduleDependency(nameof(LanguageAPI))]
    [R2APISubmoduleDependency(nameof(R2API.CustomElite))]
    [R2APISubmoduleDependency(nameof(R2API.EliteAPI))]
    [R2APISubmoduleDependency("LoadoutAPI")]
    [R2APISubmoduleDependency("EntityAPI")]
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.PallesenProductions.Mithrix", "PlayableMithrix", "1.0.0")]
    public class PlayableMithrix : BaseUnityPlugin
    {
        private GameObject myCharacter;
        public void Awake()
        {
            myCharacter = Resources.Load<GameObject>("Prefabs/CharacterBodies/BrotherBody").InstantiateClone("BrotherPlayerBody");
            GameObject gameObject = myCharacter.GetComponent<ModelLocator>().modelBaseTransform.gameObject;
            gameObject.transform.localScale *= 0.45f;
            gameObject.transform.Translate(new Vector3(0, 3, 0));
            myCharacter.GetComponent<CharacterBody>().aimOriginTransform.Translate(new Vector3(0, -3, 0));

            BodyCatalog.getAdditionalEntries += delegate (List<GameObject> list)
            {
                list.Add(myCharacter);
            };

            gameObject.AddComponent<Animation>();

            CharacterBody component = myCharacter.GetComponent<CharacterBody>();
            component.baseJumpPower = Resources.Load<GameObject>("Prefabs/CharacterBodies/LoaderBody").GetComponent<CharacterBody>().baseJumpPower;
            component.baseMoveSpeed = Resources.Load<GameObject>("Prefabs/CharacterBodies/LoaderBody").GetComponent<CharacterBody>().baseMoveSpeed;
            component.levelMoveSpeed = Resources.Load<GameObject>("Prefabs/CharacterBodies/LoaderBody").GetComponent<CharacterBody>().levelMoveSpeed;
            component.baseDamage = 18f;
            component.levelDamage = 0.6f;
            component.baseCrit = 2f;
            component.levelCrit = 1f;
            component.baseMaxHealth = 300f;
            component.levelMaxHealth = 25f;
            component.baseArmor = 20f;
            component.baseRegen = 1f;
            component.levelRegen = 0.4f;
            component.baseMoveSpeed = 8f;
            //component.levelMoveSpeed = 0.25f;
            component.baseAttackSpeed = 5f;
            component.name = "PlayableMithrixBody";

            myCharacter.tag = "Player";

            myCharacter.AddComponent<ItemDisplay>();
            myCharacter.GetComponent<CharacterBody>().preferredPodPrefab = Resources.Load<GameObject>("Prefabs/CharacterBodies/toolbotbody").GetComponent<CharacterBody>().preferredPodPrefab;

            SurvivorDef survivorDef = new SurvivorDef
            {
                bodyPrefab = myCharacter,
                descriptionToken = "MyDescription",
                displayPrefab = gameObject,
                primaryColor = new Color(0.8039216f, 0.482352942f, 0.843137264f),
                name = "ScavPlayerBody",
                unlockableName = ""// "Logs.Stages.limbo"
            };
            SurvivorAPI.AddSurvivor(survivorDef);
        }
    }
}

