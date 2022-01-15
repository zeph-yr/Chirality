using IPA;
using IPA.Config;
using IPA.Config.Stores;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
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
        //internal static ChiralityController PluginController { get { return ChiralityController.Instance; } }

        public static StandardLevelDetailViewController leveldetail;

        public BeatmapCharacteristicSO h_beatmapCharacteristicSO;
        public BeatmapCharacteristicSO v_beatmapCharacteristicSO;
        public BeatmapCharacteristicSO i_beatmapCharacteristicSO;


        [Init]
        /// <summary>
        /// Called when the plugin is first loaded by IPA (either when the game starts or when the plugin is enabled if it starts disabled).
        /// [Init] methods that use a Constructor or called before regular methods like InitWithConfig.
        /// Only use [Init] with one Constructor.
        /// </summary>
        public Plugin(IPALogger logger, Config conf)
        {
            Instance = this;
            Plugin.Log = logger;
            Plugin.Log?.Debug("Logger initialized.");

            Configuration.PluginConfig.Instance = conf.Generated<Configuration.PluginConfig>();
            Plugin.Log?.Debug("Config loaded");


            Texture2D texture = new Texture2D(50, 50);
            Sprite icon = Sprite.Create(texture, new Rect(0f, 0f, 50, 50), new Vector2(0.5f, 0.5f));

            h_beatmapCharacteristicSO = SongCore.Collections.RegisterCustomCharacteristic(icon, "Horizontal", "Mirror Horizontally", "Horizontal", "Horizontal");
            v_beatmapCharacteristicSO = SongCore.Collections.RegisterCustomCharacteristic(icon, "Vertical", "Mirror Vertically", "Vertical", "Vertical");
            i_beatmapCharacteristicSO = SongCore.Collections.RegisterCustomCharacteristic(icon, "Invert", "Invert", "Invert", "Invert");
        }

        


        #region Disableable

        /// <summary>
        /// Called when the plugin is enabled (including when the game starts if the plugin is enabled).
        /// </summary>
        [OnEnable]
        public void OnEnable()
        {
            //new GameObject("ChiralityController").AddComponent<ChiralityController>();

            MirrorTransforms.Create_Vertical_Transforms();

            ApplyHarmonyPatches();

            //BS_Utils.Utilities.BSEvents.lateMenuSceneLoadedFresh += BSEvents_lateMenuSceneLoadedFresh;
        }

        /// <summary>
        /// Called when the plugin is disabled and on Beat Saber quit. It is important to clean up any Harmony patches, GameObjects, and Monobehaviours here.
        /// The game should be left in a state as if the plugin was never started.
        /// Methods marked [OnDisable] must return void or Task.
        /// </summary>
        [OnDisable]
        public void OnDisable()
        {
            //if (PluginController != null)
            //    GameObject.Destroy(PluginController);
            RemoveHarmonyPatches();
        }

        /*
        /// <summary>
        /// Called when the plugin is disabled and on Beat Saber quit.
        /// Return Task for when the plugin needs to do some long-running, asynchronous work to disable.
        /// [OnDisable] methods that return Task are called after all [OnDisable] methods that return void.
        /// </summary>
        [OnDisable]
        public async Task OnDisableAsync()
        {
            await LongRunningUnloadTask().ConfigureAwait(false);
        }
        */
        #endregion

        // Uncomment the methods in this section if using Harmony
        #region Harmony
        /// <summary>
        /// Attempts to apply all the Harmony patches in this assembly.
        /// </summary>
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

        /// <summary>
        /// Attempts to remove all the Harmony patches that used our HarmonyId.
        /// </summary>
        internal static void RemoveHarmonyPatches()
        {
            try
            {
                // Removes all patches with this HarmonyId
                harmony.UnpatchSelf();
            }
            catch (Exception ex)
            {
                Plugin.Log?.Error("Error removing Harmony patches: " + ex.Message);
                Plugin.Log?.Debug(ex);
            }
        }
        #endregion


        /*private void BSEvents_lateMenuSceneLoadedFresh(ScenesTransitionSetupDataSO obj)
        {
            //var leveldetail = Resources.FindObjectsOfTypeAll<StandardLevelDetailViewController>().FirstOrDefault();
            Plugin.Log.Debug("#### 0");

            if (leveldetail != null)
            {
                Plugin.Log.Debug("#### 1");
                leveldetail.didChangeContentEvent += Leveldetail_didChangeContentEvent;
            }
        }



        private void Leveldetail_didChangeContentEvent(StandardLevelDetailViewController arg1, StandardLevelDetailViewController.ContentType arg2)
        {
            Plugin.Log.Debug("#### 2");

            if (arg1.beatmapLevel != null && arg1.selectedDifficultyBeatmap != null)
            {
                Plugin.Log.Debug("#### 3");

                //Plugin.Log.Debug("characteristic name:" + arg1.beatmapLevel.previewDifficultyBeatmapSets[0].beatmapCharacteristic.name);
                //Plugin.Log.Debug("characteristic name:" + arg1.selectedDifficultyBeatmap.level.beatmapLevelData.difficultyBeatmapSets[0].difficultyBeatmaps[0].noteJumpMovementSpeed);
                //Plugin.Log.Debug("characteristic name:" + arg1.beatmapLevel.previewDifficultyBeatmapSets[0].beatmapDifficulties[0].NoteJumpMovementSpeed());

                

                foreach (PreviewDifficultyBeatmapSet diffset in arg1.beatmapLevel.previewDifficultyBeatmapSets)
                {
                    if (diffset != null)
                    {
                        Plugin.Log.Debug("BMSO: " + diffset.beatmapCharacteristic.serializedName);

                       

                        foreach (BeatmapDifficulty diff in diffset.beatmapDifficulties)
                        {
                            if (diff != null)
                            {
                                Plugin.Log.Debug("BeatmapDifficulty: " + diff.Name() + " " + diff.NoteJumpMovementSpeed() + " " + diff.ToString());

                            }
                        }
                    }
                }
            }
        }*/



    }
}

/*
[DEBUG @ 15:33:40 | Chirality] #### 2
[DEBUG @ 15:33:40 | Chirality] #### 3
[DEBUG @ 15:33:40 | Chirality] BMSO: StandardBeatmapCharacteristic (BeatmapCharacteristicSO)
[DEBUG @ 15:33:40 | Chirality] BeatmapDifficulty: Easy 10 Easy
[DEBUG @ 15:33:40 | Chirality] BeatmapDifficulty: Normal 10 Normal
[DEBUG @ 15:33:40 | Chirality] BeatmapDifficulty: Hard 10 Hard
[DEBUG @ 15:33:40 | Chirality] BeatmapDifficulty: Expert 12 Expert
[DEBUG @ 15:33:40 | Chirality] BeatmapDifficulty: Expert+ 16 ExpertPlus
*/