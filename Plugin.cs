using Chirality.Configuration;
using IPA;
using IPA.Config;
using IPA.Config.Stores;
using System;
using System.Reflection;
using UnityEngine;
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

        //public BeatmapCharacteristicSO h_beatmapCharacteristicSO;
        //public BeatmapCharacteristicSO v_beatmapCharacteristicSO;
        //public BeatmapCharacteristicSO i_beatmapCharacteristicSO;


        [Init]
        public Plugin(IPALogger logger, Config conf)
        {
            Instance = this;
            Plugin.Log = logger;
            Plugin.Log?.Debug("Logger initialized.");

            Configuration.PluginConfig.Instance = conf.Generated<Configuration.PluginConfig>();
            Plugin.Log?.Debug("Config loaded");


            /*Texture2D texture = new Texture2D(50, 50);
            Sprite icon = Sprite.Create(texture, new Rect(0f, 0f, 50, 50), new Vector2(0.5f, 0.5f));

            h_beatmapCharacteristicSO = SongCore.Collections.RegisterCustomCharacteristic(icon, "Horizontal", "Mirror Horizontally", "Horizontal", "Horizontal");
            v_beatmapCharacteristicSO = SongCore.Collections.RegisterCustomCharacteristic(icon, "Vertical", "Mirror Vertically", "Vertical", "Vertical");
            i_beatmapCharacteristicSO = SongCore.Collections.RegisterCustomCharacteristic(icon, "Invert", "Invert", "Invert", "Invert");
            */
        }

        
        [OnEnable]
        public void OnEnable()
        {
            MirrorTransforms.Create_Vertical_Transforms();

            ApplyHarmonyPatches();

            //if (PluginConfig.Instance.enabled)
            //{
                BeatSaberMarkupLanguage.GameplaySetup.GameplaySetup.instance.AddTab("Chirality", "Chirality.ModUI.bsml", ModUI.instance);
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