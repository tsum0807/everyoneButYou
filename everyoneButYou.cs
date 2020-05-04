
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
using System.IO;

namespace EveryoneButYou
{
    [BepInDependency("com.bepis.r2api")]
    //Change these
    [BepInPlugin("com.beneb.EveryoneButYou", "EveryoneButYou", "1.0.0")]
    public class EveryoneButYou : BaseUnityPlugin
    {
        public void Awake()
        {
            this.BannedCharacters = new List<string>();
            /*this.BannedCharacters.Add("Commando");
            this.BannedCharacters.Add("Engi");
            this.BannedCharacters.Add("Huntress");
            this.BannedCharacters.Add("Loader");
            this.BannedCharacters.Add("Mage");
            this.BannedCharacters.Add("Mercenary");
            this.BannedCharacters.Add("Toolbot");*/

            this.getConfig();

            Chat.AddMessage("Loaded EveryoneButYou");
            Chat.AddMessage("---------------------");
            Chat.AddMessage("These characters will not be selected in Artifact of Metamorphosis");
            foreach (string c in this.BannedCharacters)
            {
                Chat.AddMessage(c);
            }
            Chat.AddMessage("---------------------");

            On.RoR2.CharacterMaster.PickRandomSurvivorBodyPrefab += delegate(On.RoR2.CharacterMaster.orig_PickRandomSurvivorBodyPrefab orig, 
                global::Xoroshiro128Plus rng, List<UnlockableDef> availableUnlockableDefs)
            {
                return this.CharacterMaster_PickRandomSurvivorBodyPrefab(orig, rng, availableUnlockableDefs);
            };

        }

        private GameObject CharacterMaster_PickRandomSurvivorBodyPrefab(On.RoR2.CharacterMaster.orig_PickRandomSurvivorBodyPrefab orig,
            Xoroshiro128Plus rng, List<UnlockableDef> availableUnlockableDefs)
        {
            // Find and remove characters from copied unlocked list
            foreach(UnlockableDef def in availableUnlockableDefs.ToList())
            {
                if (def.name.Contains("Characters."))
                {
                    //Chat.AddMessage(def.name);
                    foreach(string character in this.BannedCharacters)
                    {
                        if (def.name.Contains(character))
                        {
                            //Chat.AddMessage("banned " + def.name);
                            availableUnlockableDefs.Remove(def);
                            break;
                        }

                    }
                }
            }
            // Run original rng with modified unlocked list
            return orig(rng, availableUnlockableDefs);
        }

        private void getConfig()
        {
            string configPath = Paths.BepInExRootPath + "/config/" + this.configFile;
            if (!File.Exists(configPath))
            {
                // Create config file if doesn't exist
                StreamWriter streamWriter = new StreamWriter(configPath, false);

                string configText = "[General]" +
                    "\n\n## Enable EveryoneButYou Mod\n# Setting type: Boolean\n# Default value: true\nenabled = " + this.modEnabled.ToString() + 
                    "\n\n## Characters to disable when playing with Artifact of Metamorphosis\n##{Engineer, Huntress, Loader, Mage, Mercenary, Toolbot, Treebot, Croco}" +
                    "\n# Setting type: string\n# Default value: Engineer, Toolbot\ncharactersToBan = " + this.charactersToBan;

                streamWriter.Write(configText);
                streamWriter.Close();
            }
            if(File.Exists(configPath))
            {
                // Read config
                List<string> data = new List<string>();
				StreamReader streamReader = new StreamReader(configPath);
				while (streamReader.Peek() >= 0)
				{
					data.Add(streamReader.ReadLine());
				}
                streamReader.Close();
                foreach(string line in data)
                {
                    if (!line.StartsWith("#"))
                    {
                        string[] components = line.Split(new char[]{'='});
                        // Strip whitespace
                        for(int i = 0; i < components.Length; i++)
                        {
                            components[i] = components[i].Trim();
                        }
                        if(components[0].StartsWith("enabled"))
                        {
                            this.modEnabled = bool.Parse(components[1]);
                        }
                        if(components[0].StartsWith("charactersToBan"))
                        {
                            this.BannedCharacters = this.parseCharacters(components[1]);
                        }

                    }
                }
            }
        }

        private List<string> parseCharacters(string characters)
        {
            List<string> cList = new List<string>();
            string[] c = characters.Split(new char[] { ',' });
            for (int i = 0; i < c.Length; i++)
            {
                // Strip whitespace
                c[i] = c[i].Trim();
                // Add to list
                cList.Add(char.ToUpper(c[i][0])+c[i].Substring(1));
            }
            return cList;
        }

        private List<string> BannedCharacters;

        private string configFile = "EveryoneButYouConfig.cfg";
        private string charactersToBan = "Engineer, Toolbot";
        private bool modEnabled = true;

    }
}