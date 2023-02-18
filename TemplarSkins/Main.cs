using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using R2API;
using R2API.Utils;
using RoR2;
using UnityEngine;

namespace TemplarSkins
{
    [BepInDependency(R2API.R2API.PluginGUID)]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency("com.Tymmey.Templar")]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod)]
    public class Main : BaseUnityPlugin
    {
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "prodzpod";
        public const string PluginName = "TemplarSkins";
        public const string PluginVersion = "1.1.0";
        public static ManualLogSource Log;
        internal static PluginInfo pluginInfo;
        private static AssetBundle _assetBundle;
        public static ConfigFile Config;
        public static ConfigEntry<bool> EnableTemplarSkin;
        public static ConfigEntry<bool> EnableEngiVoidSkin;
        public static ConfigEntry<bool> EnableMalignantOrigins;
        public static AssetBundle AssetBundle
        {
            get
            {
                if (_assetBundle == null)
                    _assetBundle = AssetBundle.LoadFromFile(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(pluginInfo.Location), "templarskins"));
                return _assetBundle;
            }
        }
        public void Awake()
        {
            pluginInfo = Info;
            Log = Logger;
            Config = new ConfigFile(System.IO.Path.Combine(Paths.ConfigPath, PluginGUID + ".cfg"), true);
            EnableTemplarSkin = Config.Bind("General", "Add Templar Skins", true, "Set to false to disable it.");
            EnableEngiVoidSkin = Config.Bind("General", "Engi Void Skin", true, "Set to false to disable it.");
            EnableMalignantOrigins = Config.Bind("General", "Malignant Origins", true, "Set to false to disable it.");
            Skins.Patch();
            if (EnableMalignantOrigins.Value) Skills.Patch();
            // fix stuff
            LanguageAPI.AddOverlay("TEMPLAR_PRIMARY_MINIGUN_DESCRIPTION", "<style=cIsDamage>Rapidfire</style>. Rev up and fire your <style=cIsUtility>minigun</style>, dealing <style=cIsDamage>" + (Templar.Templar.minigunDamageCoefficient.Value * 100f).ToString() + "% damage</style> per bullet. <style=cIsUtility>Slow</style> your movement while shooting, but gain <style=cIsHealing>bonus armor</style>.");
            Templar.Templar.TemplarPrefab.GetComponent<DeathRewards>().logUnlockableDef = null;
            SkillLocator component = Templar.Templar.myCharacter.GetComponent<SkillLocator>();
            // goofy ahh character tbh
            ContentAddition.AddSkillFamily(component.primary.skillFamily);
            ContentAddition.AddSkillFamily(component.secondary.skillFamily);
            ContentAddition.AddSkillFamily(component.utility.skillFamily);
            ContentAddition.AddSkillFamily(component.special.skillFamily);
            CharacterBody body = Templar.Templar.myCharacter.GetComponent<CharacterBody>();
            body.portraitIcon = AssetBundle.LoadAsset<Sprite>("Assets/texSurvivorTemplar.png").texture;
            body.baseNameToken = "TEMPLAR_SURVIVOR_NAME";
            body.subtitleNameToken = "TEMPLAR_SURVIVOR_SUBTITLE";
        }
    }
}
