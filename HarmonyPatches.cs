using BeatmapSaveDataVersion3;
using Chirality.Configuration;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

namespace Chirality
{
    [HarmonyPatch (typeof(StandardLevelDetailView), "SetContent")]
    internal class StandardLevelDetailViewPatch
    {
        static async void Prefix(IBeatmapLevel level) // Turns out can have an async Patch!
        {
            Plugin.Log.Debug("SetContent");

            if (PluginConfig.Instance.enabled == false || BS_Utils.Plugin.LevelData.Mode == BS_Utils.Gameplay.Mode.Multiplayer || BS_Utils.Plugin.LevelData.Mode == BS_Utils.Gameplay.Mode.Mission)
            {
                // This prevents prevents new diffs from being generated in mp but will not remove the ones generated recently while playing in solo then going into mp
                return;
            }

            // BS 1.20.0 Not supporting OST / DLC anymore
            if (level.levelID.StartsWith("custom_level") == false)
            {
                return;
            }

            if (level.beatmapLevelData.difficultyBeatmapSets.FirstOrDefault() == null)
            {
                return;
            }

            if (level.beatmapLevelData.difficultyBeatmapSets.Any((i) => i.beatmapCharacteristic.serializedName.Contains("Horizontal")))
            {
                // This is needed to keep the diffs from multiplying like rabbits
                // however it also means modes cant be switched on the fly for maps with recently generated diffs (unless we remove the generated diffs)
                return;
            }

            int index = -1;

            for (int i = 0; i < level.beatmapLevelData.difficultyBeatmapSets.Count; i++)
            {
                if (level.beatmapLevelData.difficultyBeatmapSets[i].beatmapCharacteristic.serializedName == ((ModUI.PreferenceEnum)PluginConfig.Instance.mode).ToString())
                {
                    index = i;
                    break;
                }
            }

            if (index == -1)
            {
                return;
            }

            Plugin.Log.Debug("Index: " + index);

            MirrorTransforms.rand = new System.Random(99);


            /*
            // Commission
            CustomDifficultyBeatmapSet h_beatmapset = new CustomDifficultyBeatmapSet(Create_BMCSO("Chirality.Icons.horizontal.png", "Horizontal", "Mirror Left-Right"));
            CustomDifficultyBeatmapSet v_beatmapset = new CustomDifficultyBeatmapSet(Create_BMCSO("Chirality.Icons.vertical.png", "Vertical", "Mirror Up-Down"));
            CustomDifficultyBeatmapSet i_beatmapset = new CustomDifficultyBeatmapSet(Create_BMCSO("Chirality.Icons.inverse.png", "Inverse", "Inverse"));
            CustomDifficultyBeatmapSet it_beatmapset = new CustomDifficultyBeatmapSet(Create_BMCSO("Chirality.Icons.inverted.png", "Inverted", "Invert True")); // New for 1.20.0

            CustomDifficultyBeatmap[] h_customDifficultyBeatmaps = await Create_Difficulty_Array_Async(level.beatmapLevelData.difficultyBeatmapSets[index].difficultyBeatmaps, h_beatmapset, 0);
            CustomDifficultyBeatmap[] v_customDifficultyBeatmaps = await Create_Difficulty_Array_Async(level.beatmapLevelData.difficultyBeatmapSets[index].difficultyBeatmaps, v_beatmapset, 1);
            CustomDifficultyBeatmap[] i_customDifficultyBeatmaps = await Create_Difficulty_Array_Async(level.beatmapLevelData.difficultyBeatmapSets[index].difficultyBeatmaps, i_beatmapset, 2);
            CustomDifficultyBeatmap[] it_customDifficultyBeatmaps = await Create_Difficulty_Array_Async(level.beatmapLevelData.difficultyBeatmapSets[index].difficultyBeatmaps, it_beatmapset, 5);
            */


            // Community Release
            CustomDifficultyBeatmapSet h_beatmapset = new CustomDifficultyBeatmapSet(Create_BMCSO("Chirality.Icons.horizontal.png", "Horizontal", "Invert Left-Right"));
            CustomDifficultyBeatmapSet v_beatmapset = new CustomDifficultyBeatmapSet(Create_BMCSO("Chirality.Icons.vertical.png", "Vertical", "Invert Up-Down"));
            CustomDifficultyBeatmapSet i_beatmapset = new CustomDifficultyBeatmapSet(Create_BMCSO("Chirality.Icons.inverse.png", "Inverse", "Inverse"));
            CustomDifficultyBeatmapSet it_beatmapset = new CustomDifficultyBeatmapSet(Create_BMCSO("Chirality.Icons.inverted.png", "Inverted", "Invert True")); // New for 1.20.0

            // lol 4 layers of nested async stuff
            CustomDifficultyBeatmap[] h_customDifficultyBeatmaps = await Create_Difficulty_Array_Async(level.beatmapLevelData.difficultyBeatmapSets[index].difficultyBeatmaps, h_beatmapset, 3);
            CustomDifficultyBeatmap[] v_customDifficultyBeatmaps = await Create_Difficulty_Array_Async(level.beatmapLevelData.difficultyBeatmapSets[index].difficultyBeatmaps, v_beatmapset, 1);
            CustomDifficultyBeatmap[] i_customDifficultyBeatmaps = await Create_Difficulty_Array_Async(level.beatmapLevelData.difficultyBeatmapSets[index].difficultyBeatmaps, i_beatmapset, 4);
            CustomDifficultyBeatmap[] it_customDifficultyBeatmaps = await Create_Difficulty_Array_Async(level.beatmapLevelData.difficultyBeatmapSets[index].difficultyBeatmaps, it_beatmapset, 5);


            h_beatmapset.SetCustomDifficultyBeatmaps(h_customDifficultyBeatmaps);
            v_beatmapset.SetCustomDifficultyBeatmaps(v_customDifficultyBeatmaps);
            i_beatmapset.SetCustomDifficultyBeatmaps(i_customDifficultyBeatmaps);
            it_beatmapset.SetCustomDifficultyBeatmaps(it_customDifficultyBeatmaps);


            List<IDifficultyBeatmapSet> custom_difficultyBeatmapSets = new List<IDifficultyBeatmapSet>(level.beatmapLevelData.difficultyBeatmapSets)
            {
                h_beatmapset,
                v_beatmapset,
                it_beatmapset,
                i_beatmapset,
            };

            if (level.beatmapLevelData is BeatmapLevelData beatmapLevelData)
            {
                Set_Field(beatmapLevelData, "_difficultyBeatmapSets", custom_difficultyBeatmapSets.ToArray());
            }
        }


        internal static async Task<CustomDifficultyBeatmap[]> Create_Difficulty_Array_Async(IReadOnlyList<IDifficultyBeatmap> difficultyBeatmaps, CustomDifficultyBeatmapSet beatmapSet, int mode)
        {
            CustomDifficultyBeatmap[] customDifficultyBeatmaps = new CustomDifficultyBeatmap[difficultyBeatmaps.Count];
            for (int i = 0; i < difficultyBeatmaps.Count; i++)
            {
                customDifficultyBeatmaps[i] = await Create_Difficulty_Async(difficultyBeatmaps[i], beatmapSet, mode);
            }

            return customDifficultyBeatmaps;
        }


        internal static async Task<CustomDifficultyBeatmap> Create_Difficulty_Async(IDifficultyBeatmap i, CustomDifficultyBeatmapSet beatmapset, int mode)
        {
            bool is_ME = false; // Chaos generator
            bool is_ME_or_NE = false; // Yeets walls

            // Check for OST now done uptop. Removed OST support for 1.20.0
            if (SongCore.Collections.RetrieveDifficultyData(i).additionalDifficultyData._requirements.Contains("Mapping Extensions"))
            {
                is_ME = true;

                Plugin.Log.Debug("ME map: raising hell");
            }

            if (is_ME || SongCore.Collections.RetrieveDifficultyData(i).additionalDifficultyData._requirements.Contains("Noodle Extensions"))
            {
                is_ME_or_NE = true;

                Plugin.Log.Debug("ME-NE map: yeeting walls");
            }


            IBeatmapDataBasicInfo beatmapDataBasicInfo = await i.GetBeatmapDataBasicInfoAsync();
            int numberOfLines = beatmapDataBasicInfo.numberOfLines;

            BeatmapSaveData beatmapSaveData_transformed;
            switch (mode)
            {
                // Commission
                case 0: beatmapSaveData_transformed = MirrorTransforms.Mirror_Horizontal(((CustomDifficultyBeatmap)i).beatmapSaveData, numberOfLines, true, is_ME_or_NE, is_ME); break;
                // Shared
                case 1: beatmapSaveData_transformed = MirrorTransforms.Mirror_Vertical(((CustomDifficultyBeatmap)i).beatmapSaveData, false, is_ME_or_NE, is_ME); break;
                // Commission
                case 2: beatmapSaveData_transformed = MirrorTransforms.Mirror_Inverse(((CustomDifficultyBeatmap)i).beatmapSaveData, numberOfLines, true, false, is_ME_or_NE, is_ME); break;
                // Community release
                case 3: beatmapSaveData_transformed = MirrorTransforms.Mirror_Horizontal(((CustomDifficultyBeatmap)i).beatmapSaveData, numberOfLines, false, is_ME_or_NE, is_ME); break;
                // Community release (sky)
                case 4: beatmapSaveData_transformed = MirrorTransforms.Mirror_Inverse(((CustomDifficultyBeatmap)i).beatmapSaveData, numberOfLines, true, true, is_ME_or_NE, is_ME); break;
                // Community release 1.20.0 Invert True
                case 5: beatmapSaveData_transformed = MirrorTransforms.Mirror_Inverse(((CustomDifficultyBeatmap)i).beatmapSaveData, numberOfLines, false, false, is_ME_or_NE, is_ME); break;
                // Shared
                default: beatmapSaveData_transformed = ((CustomDifficultyBeatmap)i).beatmapSaveData; break;
            }

            return new CustomDifficultyBeatmap(i.level, beatmapset, i.difficulty, i.difficultyRank, i.noteJumpMovementSpeed, i.noteJumpStartBeatOffset, i.level.beatsPerMinute, beatmapSaveData_transformed, beatmapDataBasicInfo);
        }


        internal static BeatmapCharacteristicSO Create_BMCSO(string filename, string name, string hint)
        {
            BeatmapCharacteristicSO beatmapCharacteristicSO = BeatmapCharacteristicSO.CreateInstance<BeatmapCharacteristicSO>();

            Sprite icon = SongCore.Utilities.Utils.LoadSpriteFromResources(filename);
            string name_mode = name + ((ModUI.PreferenceEnum)PluginConfig.Instance.mode).ToString();
            string hint_mode = hint + " " + ((ModUI.PreferenceEnum)PluginConfig.Instance.mode).ToString();

            Set_Field(beatmapCharacteristicSO, "_icon", icon);
            Set_Field(beatmapCharacteristicSO, "_characteristicNameLocalizationKey", name_mode);
            Set_Field(beatmapCharacteristicSO, "_descriptionLocalizationKey", hint_mode);
            Set_Field(beatmapCharacteristicSO, "_serializedName", name_mode);
            Set_Field(beatmapCharacteristicSO, "_compoundIdPartName", name_mode);
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
}