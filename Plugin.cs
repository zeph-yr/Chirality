using IPA;
using IPA.Config;
using IPA.Config.Stores;
using System;
using System.Reflection;
using IPALogger = IPA.Logging.Logger;

namespace Chirality
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        public const string HarmonyId = "com.zephyr.BeatSaber.Chirality";
        internal static readonly HarmonyLib.Harmony harmony = new HarmonyLib.Harmony(HarmonyId);

        internal static Plugin Instance { get; private set; }
        internal static IPALogger Log { get; private set; }


        [Init]
        public Plugin(IPALogger logger, Config conf)
        {
            Instance = this;
            Plugin.Log = logger;
            Plugin.Log?.Debug("Logger initialized.");

            Configuration.PluginConfig.Instance = conf.Generated<Configuration.PluginConfig>();
            Plugin.Log?.Debug("Config loaded");
        }

        
        [OnEnable]
        public void OnEnable()
        {
            MirrorTransforms.Create_Horizontal_Transforms();
            MirrorTransforms.Create_Vertical_Transforms();

            ApplyHarmonyPatches();

            BeatSaberMarkupLanguage.Settings.BSMLSettings.instance.AddSettingsMenu("Chirality", "Chirality.ModUI.bsml", ModUI.instance);
            //if (PluginConfig.Instance.enabled)
            //{
                //BeatSaberMarkupLanguage.GameplaySetup.GameplaySetup.instance.AddTab("Chirality", "Chirality.ModUI.bsml", ModUI.instance, BeatSaberMarkupLanguage.GameplaySetup.MenuType.Solo);
            //}
        }


        [OnDisable]
        public void OnDisable()
        {
            RemoveHarmonyPatches();
        }

        internal static void ApplyHarmonyPatches()
        {
            try
            {
                Plugin.Log?.Debug("Applying Harmony patches.");
                harmony.PatchAll(Assembly.GetExecutingAssembly());
            }
            catch (Exception ex)
            {
                Plugin.Log?.Error("Error applying Harmony patches: " + ex.Message);
                Plugin.Log?.Debug(ex);
            }
        }


        internal static void RemoveHarmonyPatches()
        {
            try
            {
                harmony.UnpatchSelf();
            }
            catch (Exception ex)
            {
                Plugin.Log?.Error("Error removing Harmony patches: " + ex.Message);
                Plugin.Log?.Debug(ex);
            }
        }
    }
}