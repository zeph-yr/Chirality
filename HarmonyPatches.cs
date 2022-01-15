using Chirality.Configuration;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using static PlayerSaveData;

/// <summary>
/// See https://github.com/pardeike/Harmony/wiki for a full reference on Harmony.
/// </summary>
namespace Chirality
{
    [HarmonyPatch (typeof(StandardLevelDetailView), "SetContent")]
    internal class StandardLevelDetailViewPatch
    {
        static void Prefix(ref IBeatmapLevel level, BeatmapDifficulty defaultDifficulty, BeatmapCharacteristicSO defaultBeatmapCharacteristic, PlayerData playerData)
        {
            Plugin.Log.Debug("SetContent");

            List<IDifficultyBeatmapSet> difficultyBeatmapSets_2 = new List<IDifficultyBeatmapSet>(level.beatmapLevelData.difficultyBeatmapSets);



            //if (level.beatmapLevelData.difficultyBeatmapSets.Any(/*(a) => a.beatmapCharacteristic.serializedName == "Standard")*/) == true)
            //{
            //    Plugin.Log.Debug("Any difficultyBeatmapSets: " + level.beatmapLevelData.difficultyBeatmapSets[0].beatmapCharacteristic);
            //}


            // Later add other modes
            if (level.beatmapLevelData.difficultyBeatmapSets.FirstOrDefault() != null && level.beatmapLevelData.difficultyBeatmapSets[0].beatmapCharacteristic.serializedName == "Standard")
            {
                Plugin.Log.Debug("Any difficultyBeatmapSets: " + level.beatmapLevelData.difficultyBeatmapSets[0].beatmapCharacteristic.serializedName);
            }
        


            CustomDifficultyBeatmapSet h_beatmapset = new CustomDifficultyBeatmapSet(Create_BMCSO("Horizontal", "Mirror Left-Right"));
            CustomDifficultyBeatmapSet v_beatmapset = new CustomDifficultyBeatmapSet(Create_BMCSO("Vertical", "Mirror Up-Down"));
            CustomDifficultyBeatmapSet i_beatmapset = new CustomDifficultyBeatmapSet(Create_BMCSO("Invert", "Invert"));

            //Plugin.Log.Debug("SetContent 1");


            CustomDifficultyBeatmap[] customDifficultyBeatmaps = level.beatmapLevelData.difficultyBeatmapSets[0].difficultyBeatmaps.Select((i) => new CustomDifficultyBeatmap(i.level, h_beatmapset, i.difficulty, i.difficultyRank, i.noteJumpMovementSpeed, i.noteJumpStartBeatOffset, MirrorTransforms.Mirror_Horizontal(i.beatmapData.GetCopy()))).ToArray();
            //Plugin.Log.Debug("SetContent 2");

            h_beatmapset.SetCustomDifficultyBeatmaps(customDifficultyBeatmaps);
            //Plugin.Log.Debug("SetContent 3");

            difficultyBeatmapSets_2.Add(h_beatmapset);
            //Plugin.Log.Debug("SetContent 4");

            if (level.beatmapLevelData is BeatmapLevelData data)
            {
                Plugin.Log.Debug("SetContent 5");

                Set_Field(data, "_difficultyBeatmapSets", difficultyBeatmapSets_2.ToArray());

                /*if (!Set(data, "_difficultyBeatmapSets", difficultyBeatmapSets_2.ToArray()))
                {
                    Plugin.Log.Warn("Could not set new difficulty sets");
                    return;
                }*/
            }
            else
            {
                Plugin.Log.Info("Unsupported beatmapLevelData: " + (level.beatmapLevelData?.GetType().FullName ?? "null"));
            }

            Plugin.Log.Debug("SetContent 6");

            //Plugin.Log.Debug("njs" + level.beatmapLevelData.difficultyBeatmapSets[0].difficultyBeatmaps[0].noteJumpMovementSpeed);
            //Plugin.Log.Debug(level.beatmapLevelData.difficultyBeatmapSets[1].ToString());
            //Plugin.Log.Debug(level.beatmapLevelData.difficultyBeatmapSets[2].ToString());
            //Plugin.Log.Debug(level.beatmapLevelData.difficultyBeatmapSets.Length.ToString());

        }


        internal static BeatmapCharacteristicSO Create_BMCSO(string name, string hint)
        {
            Texture2D texture = new Texture2D(50, 50);
            Sprite icon = Sprite.Create(texture, new Rect(0, 0, 50, 50), new Vector2(0.5f, 0.5f));

            BeatmapCharacteristicSO beatmapCharacteristicSO = BeatmapCharacteristicSO.CreateInstance<BeatmapCharacteristicSO>();

            Set_Field(beatmapCharacteristicSO, "_icon", icon);
            Set_Field(beatmapCharacteristicSO, "_characteristicNameLocalizationKey", name);
            Set_Field(beatmapCharacteristicSO, "_descriptionLocalizationKey", hint);
            Set_Field(beatmapCharacteristicSO, "_serializedName", name);
            Set_Field(beatmapCharacteristicSO, "_compoundIdPartName", name);
            Set_Field(beatmapCharacteristicSO, "_sortingOrder", 100);
            Set_Field(beatmapCharacteristicSO, "_containsRotationEvents", false);
            Set_Field(beatmapCharacteristicSO, "_requires360Movement", false);
            Set_Field(beatmapCharacteristicSO, "_numberOfColors", 2);

            return beatmapCharacteristicSO;
        }


        internal static void Set_Field(object obj, string fieldName, object value)
        {
            FieldInfo fieldInfo = obj.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            if (fieldInfo != null)
            {
                fieldInfo.SetValue(obj, value);
            }
        }
    }











    /*[HarmonyPatch(typeof(StandardLevelDetailViewController), MethodType.Constructor)]
    internal class StandardLevelDetailViewControllerPatch
    {
        static void Postfix(ref StandardLevelDetailViewController __instance)
        {
            Plugin.leveldetail = __instance;
        }
    }*/


    /*[HarmonyPatch(typeof(BeatmapDataTransformHelper), "CreateTransformedBeatmapData")]
    internal class BeatmapDataTransformPatch
    {
        static IReadonlyBeatmapData Postfix(IReadonlyBeatmapData __result)
        {
            if (!PluginConfig.Instance.enabled)
            {
                return __result;
            }

            Plugin.Log.Debug("Mirror Horizontal");

            __result = MirrorTransforms.Mirror_Horizontal(__result);
            __result = MirrorTransforms.Mirror_Vertical(__result);

            return __result;
        }
    }*/
}