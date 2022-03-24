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

        static void Prefix(IBeatmapLevel level)
        {
            Plugin.Log.Debug("SetContent");

            if (PluginConfig.Instance.enabled == false || BS_Utils.Plugin.LevelData.Mode == BS_Utils.Gameplay.Mode.Multiplayer || BS_Utils.Plugin.LevelData.Mode == BS_Utils.Gameplay.Mode.Mission)
            {
                // This prevents prevents new diffs from being generated in mp but
                // will not remove the ones generated recently while playing in solo then going into mp
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

            MirrorTransforms.rand = new System.Random(99);

            // Commission
            /*
            CustomDifficultyBeatmapSet h_beatmapset = new CustomDifficultyBeatmapSet(Create_BMCSO("Chirality.Icons.horizontal.png", "Horizontal", "Mirror Left-Right"));
            CustomDifficultyBeatmapSet v_beatmapset = new CustomDifficultyBeatmapSet(Create_BMCSO("Chirality.Icons.vertical.png", "Vertical", "Mirror Up-Down"));
            CustomDifficultyBeatmapSet i_beatmapset = new CustomDifficultyBeatmapSet(Create_BMCSO("Chirality.Icons.inverse.png", "Inverse", "Inverse"));

            CustomDifficultyBeatmap[] h_customDifficultyBeatmaps = level.beatmapLevelData.difficultyBeatmapSets[index].difficultyBeatmaps.Select(i => Create_Difficulty(i, h_beatmapset, 0)).ToArray();
            CustomDifficultyBeatmap[] v_customDifficultyBeatmaps = level.beatmapLevelData.difficultyBeatmapSets[index].difficultyBeatmaps.Select((i) => Create_Difficulty(i, v_beatmapset, 1)).ToArray();
            CustomDifficultyBeatmap[] i_customDifficultyBeatmaps = level.beatmapLevelData.difficultyBeatmapSets[index].difficultyBeatmaps.Select((i) => Create_Difficulty(i, i_beatmapset, 2)).ToArray();
            */

            // Community Release
            CustomDifficultyBeatmapSet h_beatmapset = new CustomDifficultyBeatmapSet(Create_BMCSO("Chirality.Icons.horizontal.png", "Horizontal", "Invert Left-Right"));
            CustomDifficultyBeatmapSet v_beatmapset = new CustomDifficultyBeatmapSet(Create_BMCSO("Chirality.Icons.vertical.png", "Vertical", "Invert Up-Down"));
            CustomDifficultyBeatmapSet i_beatmapset = new CustomDifficultyBeatmapSet(Create_BMCSO("Chirality.Icons.inverse.png", "Inverse", "Inverse"));

            CustomDifficultyBeatmap[] h_customDifficultyBeatmaps = level.beatmapLevelData.difficultyBeatmapSets[index].difficultyBeatmaps.Select(i => Create_DifficultyAsync(i, h_beatmapset, 3)).ToArray();
            CustomDifficultyBeatmap[] v_customDifficultyBeatmaps = level.beatmapLevelData.difficultyBeatmapSets[index].difficultyBeatmaps.Select((i) => Create_DifficultyAsync(i, v_beatmapset, 1)).ToArray();
            CustomDifficultyBeatmap[] i_customDifficultyBeatmaps = level.beatmapLevelData.difficultyBeatmapSets[index].difficultyBeatmaps.Select((i) => Create_DifficultyAsync(i, i_beatmapset, 4)).ToArray();

            h_beatmapset.SetCustomDifficultyBeatmaps(h_customDifficultyBeatmaps);
            v_beatmapset.SetCustomDifficultyBeatmaps(v_customDifficultyBeatmaps);
            i_beatmapset.SetCustomDifficultyBeatmaps(i_customDifficultyBeatmaps);

            //level.beatmapLevelData.difficultyBeatmapSets[0].difficultyBeatmaps[0].GetBeatmapDataBasicInfoAsync().Result.numberOfLines

            List<IDifficultyBeatmapSet> custom_difficultyBeatmapSets = new List<IDifficultyBeatmapSet>(level.beatmapLevelData.difficultyBeatmapSets)
            {
                h_beatmapset,
                v_beatmapset,
                i_beatmapset
            };

            if (level.beatmapLevelData is BeatmapLevelData beatmapLevelData)
            {
                Set_Field(beatmapLevelData, "_difficultyBeatmapSets", custom_difficultyBeatmapSets.ToArray());
            }
        }


        internal static async Task<CustomDifficultyBeatmap> Create_DifficultyAsync(IDifficultyBeatmap i, CustomDifficultyBeatmapSet beatmapset, int mode)
        {
            bool is_ME = false; // Chaos generator
            bool is_ME_or_NE = false; // Yeets walls

            if (i.level.levelID.StartsWith("custom_level"))
            {
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
            }

            IBeatmapDataBasicInfo beatmapDataBasicInfo = await i.GetBeatmapDataBasicInfoAsync();
            int numberOfLines = beatmapDataBasicInfo.numberOfLines;

            switch (mode)
            {
                // Commission
                case 0: return new CustomDifficultyBeatmap(i.level, beatmapset, i.difficulty, i.difficultyRank, i.noteJumpMovementSpeed, i.noteJumpStartBeatOffset, i.level.beatsPerMinute, MirrorTransforms.Mirror_Horizontal(((CustomDifficultyBeatmap)i).beatmapSaveData, numberOfLines, true, is_ME_or_NE, is_ME), beatmapDataBasicInfo);
                // Shared
                //case 1: return new CustomDifficultyBeatmap(i.level, beatmapset, i.difficulty, i.difficultyRank, i.noteJumpMovementSpeed, i.noteJumpStartBeatOffset, MirrorTransforms.Mirror_Vertical(i.beatmapData.GetCopy(), false, is_ME_or_NE, is_ME));
                // Commission
                //case 2: return new CustomDifficultyBeatmap(i.level, beatmapset, i.difficulty, i.difficultyRank, i.noteJumpMovementSpeed, i.noteJumpStartBeatOffset, MirrorTransforms.Mirror_Inverse(i.beatmapData.GetCopy(), true, false, is_ME_or_NE, is_ME));
                // Community release
                //case 3: return new CustomDifficultyBeatmap(i.level, beatmapset, i.difficulty, i.difficultyRank, i.noteJumpMovementSpeed, i.noteJumpStartBeatOffset, MirrorTransforms.Mirror_Horizontal(i.beatmapData.GetCopy(), false, is_ME_or_NE, is_ME));
                // Community release (sky)
                //case 4: return new CustomDifficultyBeatmap(i.level, beatmapset, i.difficulty, i.difficultyRank, i.noteJumpMovementSpeed, i.noteJumpStartBeatOffset, MirrorTransforms.Mirror_Inverse(i.beatmapData.GetCopy(), true, true, is_ME_or_NE, is_ME));
                // Maybe
                //case 5: return new CustomDifficultyBeatmap(i.level, beatmapset, i.difficulty, i.difficultyRank, i.noteJumpMovementSpeed, i.noteJumpStartBeatOffset, MirrorTransforms.Mirror_Inverse(i.beatmapData.GetCopy(), false, false, is_ME_or_NE, is_ME));

                default: return (CustomDifficultyBeatmap)i;
            }
        }


        internal static BeatmapCharacteristicSO Create_BMCSO(string filename, string name, string hint)
        {
            Sprite icon = SongCore.Utilities.Utils.LoadSpriteFromResources(filename);

            BeatmapCharacteristicSO beatmapCharacteristicSO = BeatmapCharacteristicSO.CreateInstance<BeatmapCharacteristicSO>();

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