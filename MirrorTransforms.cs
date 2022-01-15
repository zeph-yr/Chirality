using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chirality
{
    class MirrorTransforms
    {
        internal static Dictionary<NoteCutDirection, NoteCutDirection> vertical_cut_transform;

        internal static BeatmapData Mirror_Horizontal(BeatmapData beatmapData)
        {
			Plugin.Log.Debug("Mirror Horizontal");

			int numberOfLines = beatmapData.numberOfLines;
			BeatmapData h_beatmapData = new BeatmapData(numberOfLines);

			foreach (BeatmapObjectData beatmapObjectData in beatmapData.beatmapObjectsData)
			{
				BeatmapObjectData copy = beatmapObjectData.GetCopy();
				copy.Mirror(numberOfLines);
				h_beatmapData.AddBeatmapObjectData(copy);
			}

			return h_beatmapData;
		}


		internal static BeatmapData Mirror_Vertical(BeatmapData beatmapData)
        {
            Plugin.Log.Debug("Mirror Vertical");

            int numberOfLines = beatmapData.numberOfLines;
            BeatmapData v_beatmapData = new BeatmapData(numberOfLines);

            foreach (BeatmapObjectData beatmapObjectData in beatmapData.beatmapObjectsData)
            {
                NoteData noteData;
                if ((noteData = (beatmapObjectData as NoteData)) != null)
                {
                    v_beatmapData.AddBeatmapObjectData(Mirror_Vertical_Note(noteData));
                }

                ObstacleData obstacleData;
                if ((obstacleData = (beatmapObjectData as ObstacleData)) != null)
                {
                    v_beatmapData.AddBeatmapObjectData(Mirror_Vertical_Obstacle(obstacleData));
                }
            }

            return v_beatmapData;
        }


        internal static void Create_Vertical_Transforms()
        {
            Plugin.Log.Debug("Create Vertical Transforms");

            vertical_cut_transform = new Dictionary<NoteCutDirection, NoteCutDirection>();

            vertical_cut_transform.Add(NoteCutDirection.Up, NoteCutDirection.Down);
            vertical_cut_transform.Add(NoteCutDirection.Down, NoteCutDirection.Up);

            vertical_cut_transform.Add(NoteCutDirection.UpLeft, NoteCutDirection.DownLeft);
            vertical_cut_transform.Add(NoteCutDirection.DownLeft, NoteCutDirection.UpLeft);

            vertical_cut_transform.Add(NoteCutDirection.UpRight, NoteCutDirection.DownRight);
            vertical_cut_transform.Add(NoteCutDirection.DownRight, NoteCutDirection.UpRight);

            vertical_cut_transform.Add(NoteCutDirection.Left, NoteCutDirection.Left);
            vertical_cut_transform.Add(NoteCutDirection.Right, NoteCutDirection.Right);
            vertical_cut_transform.Add(NoteCutDirection.Any, NoteCutDirection.Any);
            vertical_cut_transform.Add(NoteCutDirection.None, NoteCutDirection.None);
        }


        private static NoteData Mirror_Vertical_Note(NoteData noteData)
        {
            NoteLineLayer v_noteLinelayer = (NoteLineLayer)(3 - 1 - (int)noteData.noteLineLayer);
            NoteCutDirection v_cutDirection = vertical_cut_transform[noteData.cutDirection];

            NoteData v_noteData = NoteData.CreateBasicNoteData(noteData.time, noteData.lineIndex, v_noteLinelayer, noteData.colorType, v_cutDirection);

            return v_noteData;
        }


        private static ObstacleData Mirror_Vertical_Obstacle(ObstacleData obstacleData)
        {
            if (obstacleData.obstacleType == ObstacleType.FullHeight)
            {
                return obstacleData;
            }

            else
            {
                obstacleData.MoveTime(-1); // To keep the number of walls the same

                return obstacleData;
            }
        }
	}
}
