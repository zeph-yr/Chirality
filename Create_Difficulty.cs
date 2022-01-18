using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chirality
{
    class Map_Maker
    {
        internal static CustomDifficultyBeatmap Create_Difficulty(IDifficultyBeatmap i, CustomDifficultyBeatmapSet beatmapset, int mode)
        {
            bool skip_walls = false;
            List<string> requirements = new List<string>();

            if (i.level.levelID.StartsWith("custom_level"))
            {
                if (SongCore.Collections.RetrieveDifficultyData(i).additionalDifficultyData._requirements.Contains("Mapping Extensions") || SongCore.Collections.RetrieveDifficultyData(i).additionalDifficultyData._requirements.Contains("Noodle Extensions"))
                {
                    skip_walls = true;
                    Plugin.Log.Debug("has ME or NE" + skip_walls);

                    requirements = SongCore.Collections.RetrieveDifficultyData(i).additionalDifficultyData._requirements.ToList<string>();

                    Plugin.Log.Debug("req:" + requirements.ToString());
                }
            }

            


            if (mode == 0)
            {
                return new CustomDifficultyBeatmap(i.level, beatmapset, i.difficulty, i.difficultyRank, i.noteJumpMovementSpeed, i.noteJumpStartBeatOffset, MirrorTransforms.Mirror_Horizontal(i.beatmapData.GetCopy()));                
            }

            else if (mode == 1)
            {
                return new CustomDifficultyBeatmap(i.level, beatmapset, i.difficulty, i.difficultyRank, i.noteJumpMovementSpeed, i.noteJumpStartBeatOffset, MirrorTransforms.Mirror_Vertical(i.beatmapData.GetCopy(), false, skip_walls));
            }

            else
                return new CustomDifficultyBeatmap(i.level, beatmapset, i.difficulty, i.difficultyRank, i.noteJumpMovementSpeed, i.noteJumpStartBeatOffset, MirrorTransforms.Mirror_Inverse(i.beatmapData.GetCopy(), true, skip_walls));

        }
    }
}
