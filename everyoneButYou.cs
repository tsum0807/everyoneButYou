using RoR2;
using BepInEx;


namespace beneb
{
    [BepInDependency("com.bepis.r2api")]
    //Change these
    [BepInPlugin("everyoneButYou", "everyoneButYou", "1.0.0")]
    public class everyoneButYou : BaseUnityPlugin
    {
        public void Awake()
        {
            Chat.AddMessage("Loaded everyoneButYou");
        }
    }
}