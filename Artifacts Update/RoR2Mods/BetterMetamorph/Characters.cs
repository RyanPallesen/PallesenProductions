using BepInEx;
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BetterMetamorph
{
    public class PlayableScavenger : BaseUnityPlugin
    {
        private static GameObject myCharacter;

        public static bool Init()
        {
            #region characters
            {
                myCharacter = Resources.Load<GameObject>("Prefabs/CharacterBodies/ScavBody").InstantiateClone("ScavPlayerBody");
                GameObject gameObject = myCharacter.GetComponent<ModelLocator>().modelBaseTransform.gameObject;
                gameObject.transform.localScale *= 0.25f;
                gameObject.transform.Translate(new Vector3(0, 3, 0));
                myCharacter.GetComponent<CharacterBody>().aimOriginTransform.Translate(new Vector3(0, -3, 0));

                BodyCatalog.getAdditionalEntries += delegate (List<GameObject> list)
                {
                    list.Add(myCharacter);
                };

                gameObject.AddComponent<Animation>();

                CharacterBody component = myCharacter.GetComponent<CharacterBody>();
                component.baseDamage = 12f;
                component.levelDamage = 0.4f;
                component.baseCrit = 2f;
                component.levelCrit = 1f;
                component.baseMaxHealth = 300f;
                component.levelMaxHealth = 25f;
                component.baseArmor = 20f;
                component.baseRegen = 1f;
                component.levelRegen = 0.4f;
                component.baseMoveSpeed = 8f;
                //component.levelMoveSpeed = 0.25f;
                component.baseAttackSpeed = 1f;
                component.name = "ScavPlayerBody";

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
            #endregion characters
            return true;
        }
    }
}
