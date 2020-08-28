using RoR2;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace VanillaTweaks
{
    public static class TrueSuicideVoidSector
    {
        public static bool Init()
        {
            On.RoR2.ArenaMissionController.EndRound += (orig, self) =>
            {
                if (self.currentRound >= self.totalRoundsMax)
                {
                    ReadOnlyCollection<TeamComponent> teamMembers = TeamComponent.GetTeamMembers(TeamIndex.Monster);
                    for (int j = teamMembers.Count - 1; j >= 0; j--)
                    {
                        teamMembers[j].body.inventory.ResetItem(ItemIndex.ExtraLife);
                    }
                }

                orig(self);
            };

            return true;
        }
    }
}
