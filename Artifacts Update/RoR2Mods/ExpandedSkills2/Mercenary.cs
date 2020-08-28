
using EntityStates;
using PallesenProductions;
using R2API;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ExpandedSkills2
{
    public static class Mercenary
    {
        public static void Setup()
        {


        }
    }
}


namespace VTStates.States.Mercenary
{
    public class SummonClones : EntityStates.Commando.CommandoWeapon.CastSmokescreen
    {
        public override void OnEnter()
        {
            base.OnEnter();
            base.OnEnter();

            //for (int i = 0; i < 4; i++)
            {
                //ExpandedSkills.SendNetworkMessage(base.characterBody.networkIdentity.netId, 3);
            }

            base.OnExit();
        }


    }

    public class SummonManyClones : BaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();

            for (int i = 0; i < 4; i++)
            {
                //ExpandedSkills.SendNetworkMessage(base.characterBody.networkIdentity.netId, 2);
            }

            base.OnExit();
        }

    }
}