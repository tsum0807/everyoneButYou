
using BepInEx;
//using On.RoR2;
using RoR2;
using BepInEx.Configuration;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace EveryoneButYou
{
    [BepInDependency("com.bepis.r2api")]
    //Change these
    [BepInPlugin("com.beneb.EveryoneButYou", "EveryoneButYou", "1.0.0")]
    public class EveryoneButYou : BaseUnityPlugin
    {
        private List<string> BannedCharacters;
        public void Awake()
        {
            this.BannedCharacters = new List<string>();
            this.BannedCharacters.Add("Commando");
            this.BannedCharacters.Add("Engi");
            this.BannedCharacters.Add("Huntress");
            this.BannedCharacters.Add("Loader");
            this.BannedCharacters.Add("Mage");
            this.BannedCharacters.Add("Mercenary");
            this.BannedCharacters.Add("Toolbot");
            RoR2.Chat.AddMessage("Loaded EveryoneButYou");
            /*On.RoR2.CharacterMaster.PickRandomSurvivorBodyPrefab += (GameObject orig, Xoroshiro128Plus rng, List<UnlockableDef> availableUnlockableDefs) =>
            {
                Chat.AddMessage("Randomising survivor");
                CharacterMaster_PickRandomSurvivorBodyPrefab(orig, rng, availableUnlockableDefs);
            };*/
            GameObject test;
            On.RoR2.CharacterMaster.PickRandomSurvivorBodyPrefab += delegate(On.RoR2.CharacterMaster.orig_PickRandomSurvivorBodyPrefab orig, 
                global::Xoroshiro128Plus rng, List<RoR2.UnlockableDef> availableUnlockableDefs)
            {
                //test = orig.Invoke(rng, availableUnlockableDefs);
                Chat.AddMessage("hey at least i got to here");
                return this.CharacterMaster_PickRandomSurvivorBodyPrefab(orig, rng, availableUnlockableDefs);

            };
            /*GlobalEventManager.OnCharacterDeath += delegate (GlobalEventManager.orig_OnCharacterDeath orig, GlobalEventManager self, DamageReport damageReport)
            {
                orig.Invoke(self, damageReport);
                this.HookOnBodyDeath(damageReport);
            };*/

        }

        private GameObject CharacterMaster_PickRandomSurvivorBodyPrefab(On.RoR2.CharacterMaster.orig_PickRandomSurvivorBodyPrefab orig,
            Xoroshiro128Plus rng, List<UnlockableDef> availableUnlockableDefs)
        {
            foreach(UnlockableDef def in availableUnlockableDefs.ToList())
            {
                if (def.name.Contains("Characters."))
                {
                    foreach(string character in BannedCharacters)
                    {
                        if (def.name.Contains(character))
                        {
                            availableUnlockableDefs.Remove(def);
                            break;
                        }

                    }
                }
            }
            Chat.AddMessage("------------------");
            foreach (UnlockableDef def in availableUnlockableDefs)
            {
                Chat.AddMessage(def.name);
            }
            Chat.AddMessage("------------------");

            return orig(rng, availableUnlockableDefs);
        }
        /*private static void PickupDropletController_CreatePickupDroplet(On.RoR2.PickupDropletController.orig_CreatePickupDroplet orig, 
            PickupIndex pickupIndex, UnityEngine.Vector3 position, UnityEngine.Vector3 velocity)
        {
            if (pickupIndex == PickupCatalog.FindPickupIndex("LunarCoin.Coin0"))
            {
                pickupIndex = PickupCatalog.FindPickupIndex(ItemIndex.Hoof);
            }
            orig(pickupIndex, position, velocity);
        }*/
    }
}