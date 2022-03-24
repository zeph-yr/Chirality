using BeatmapSaveDataVersion3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chirality
{
    class MirrorTransforms
    {
        internal static System.Random rand;
        internal static List<NoteCutDirection> directions = new List<NoteCutDirection> {NoteCutDirection.Up, NoteCutDirection.Down, NoteCutDirection.Left , NoteCutDirection.Right,
                                                                                        NoteCutDirection.UpLeft, NoteCutDirection.UpRight, NoteCutDirection.DownLeft, NoteCutDirection.DownRight,
                                                                                        NoteCutDirection.Any, NoteCutDirection.None};
        internal static Dictionary<NoteCutDirection, NoteCutDirection> horizontal_cut_transform;
        internal static Dictionary<NoteCutDirection, NoteCutDirection> vertical_cut_transform;


        internal static BeatmapSaveData Mirror_Horizontal(BeatmapSaveData beatmapSaveData, int numberOfLines, bool flip_lines, bool remove_walls, bool is_ME)
        {
            // Bombs:
            List<BeatmapSaveData.BombNoteData> h_bombNotes = new List<BeatmapSaveData.BombNoteData>();
            foreach (BeatmapSaveData.BombNoteData bombNoteData in beatmapSaveData.bombNotes)
            {
                if (flip_lines == false)
                {
                    h_bombNotes.Add(new BeatmapSaveData.BombNoteData(bombNoteData.beat, bombNoteData.line, bombNoteData.layer));
                }
                else
                {
                    h_bombNotes.Add(new BeatmapSaveData.BombNoteData(bombNoteData.beat, numberOfLines - 1 - bombNoteData.line, bombNoteData.layer));
                }
            }

            // ColorNotes:
            List<BeatmapSaveData.ColorNoteData> h_colorNotes = new List<BeatmapSaveData.ColorNoteData>();
            foreach (BeatmapSaveData.ColorNoteData colorNote in beatmapSaveData.colorNotes)
            {
                h_colorNotes.Add(Mirror_Horizontal_Note(colorNote, numberOfLines, flip_lines, is_ME));
            }

            // Obstacles:
            List<BeatmapSaveData.ObstacleData> h_obstacleDatas = new List<BeatmapSaveData.ObstacleData>();
            if (remove_walls == false)
            {
                foreach (BeatmapSaveData.ObstacleData obstacleData in beatmapSaveData.obstacles)
                {
                    h_obstacleDatas.Add(Mirror_Horizontal_Obstacle(obstacleData, numberOfLines, flip_lines));
                }
            }

            return new BeatmapSaveData(beatmapSaveData.bpmEvents, beatmapSaveData.rotationEvents, h_colorNotes, h_bombNotes, h_obstacleDatas, beatmapSaveData.sliders, beatmapSaveData.burstSliders, beatmapSaveData.waypoints, beatmapSaveData.basicBeatmapEvents, beatmapSaveData.colorBoostBeatmapEvents, beatmapSaveData.lightColorEventBoxGroups, beatmapSaveData.lightRotationEventBoxGroups, beatmapSaveData.basicEventTypesWithKeywords, beatmapSaveData.useNormalEventsAsCompatibleEvents);
        }

        internal static BeatmapSaveData Mirror_Vertical(BeatmapSaveData beatmapSaveData, bool flip_rows, bool remove_walls, bool is_ME)
        {

            return beatmapSaveData;
        }

        /*internal static BeatmapSaveData Mirror_Inverse(BeatmapSaveData beatmapSaveData, bool flip_lines, bool flip_rows, bool remove_walls, bool is_ME)
        {
            //Plugin.Log.Debug("Mirror Inverse");
            return Mirror_Vertical(Mirror_Horizontal(beatmapSaveData, flip_lines, remove_walls, is_ME), flip_rows, remove_walls, is_ME);
        }*/



        internal static NoteCutDirection Get_Random_Direction()
        {
            int index = rand.Next(directions.Count);

            return directions[index];
        }


        #region "Horizontal Transform Functions"

        internal static void Create_Horizontal_Transforms()
        {
            Plugin.Log.Debug("Create Horizontal Transforms");

            horizontal_cut_transform = new Dictionary<NoteCutDirection, NoteCutDirection>();

            horizontal_cut_transform.Add(NoteCutDirection.Up, NoteCutDirection.Up);
            horizontal_cut_transform.Add(NoteCutDirection.Down, NoteCutDirection.Down);

            horizontal_cut_transform.Add(NoteCutDirection.UpLeft, NoteCutDirection.UpRight);
            horizontal_cut_transform.Add(NoteCutDirection.DownLeft, NoteCutDirection.DownRight);

            horizontal_cut_transform.Add(NoteCutDirection.UpRight, NoteCutDirection.UpLeft);
            horizontal_cut_transform.Add(NoteCutDirection.DownRight, NoteCutDirection.DownLeft);

            horizontal_cut_transform.Add(NoteCutDirection.Left, NoteCutDirection.Right);
            horizontal_cut_transform.Add(NoteCutDirection.Right, NoteCutDirection.Left);
            horizontal_cut_transform.Add(NoteCutDirection.Any, NoteCutDirection.Any);
            horizontal_cut_transform.Add(NoteCutDirection.None, NoteCutDirection.None);
        }


        private static BeatmapSaveData.ColorNoteData Mirror_Horizontal_Note(BeatmapSaveData.ColorNoteData colorNoteData, int numberOfLines, bool flip_lines, bool is_ME)
        {
            int h_line;

            BeatmapSaveData.NoteColorType color;
            if (colorNoteData.color == BeatmapSaveData.NoteColorType.ColorA)
            {
                color = BeatmapSaveData.NoteColorType.ColorB;
            }
            else
            {
                color = BeatmapSaveData.NoteColorType.ColorA;
            }
            //There was a bug where all the ME maps have the colors flipped oops check it again


            // Precision maps will not have indexes flipped (complicated math) but their colors will
            // Yes, it will be weird like streams will zigzag in the wrong direction...hence introducing chaos mode. Might as well make use of the weirdness!
            // Other option is to just not support ME and NE maps
            // Also Note: Not worth reusing check function because non-extended map block will become unnecessarily complicated

            if (colorNoteData.line >= 1000 || colorNoteData.line <= -1000)
            {
                h_line = colorNoteData.line / 1000 - 1; // Definition from ME
            }

            // Keep This Note: This isn't a robust way to check for extended maps
            /*if (noteData.lineIndex > 10 || noteData.lineIndex < 0) 
            {
                h_lineIndex = rand.Next(4); // ME chaos mode kekeke turns out this is too chaotic, not that fun
            }*/

            // Only non-precision-placement maps can have the option to be index flipped
            // Maps with extended non-precision-placement indexes are handled properly by numberOfLines
            else if (flip_lines)
            {
                h_line = numberOfLines - 1 - colorNoteData.line;
            }
            else
            {
                h_line = colorNoteData.line;
                color = colorNoteData.color;
            }

            NoteCutDirection h_cutDirection; // Yes, this is support for precision placement and ME LOL
            if (horizontal_cut_transform.TryGetValue(colorNoteData.cutDirection, out h_cutDirection) == false || is_ME)
            {
                h_cutDirection = Get_Random_Direction();
            }

            // Dunno what this is yet
            int h_angleOffset = colorNoteData.angleOffset;


            return new BeatmapSaveData.ColorNoteData(colorNoteData.beat, h_line, Check_Layer(colorNoteData.layer), color, h_cutDirection, h_angleOffset);
        }


        private static BeatmapSaveData.ObstacleData Mirror_Horizontal_Obstacle(BeatmapSaveData.ObstacleData obstacleData, int numberOfLines, bool flip_lines)
        {
            if (flip_lines)
            {
                return new BeatmapSaveData.ObstacleData(obstacleData.beat, numberOfLines - obstacleData.width - obstacleData.line, obstacleData.layer, obstacleData.duration, obstacleData.width, obstacleData.height);
            }

            return obstacleData;
        }
        #endregion


        #region "Vertical Transform Functions"

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


        private static NoteData Mirror_Vertical_Note(NoteData noteData, bool flip_rows, bool has_ME)
        {
            NoteLineLayer v_noteLineLayer;

            // All precision placements will not be layer-flipped (complicated math)
            // This could be weird, consider it part of chaos mode KEK
            if ((int)noteData.noteLineLayer >= 1000 || (int)noteData.noteLineLayer <= -1000)
            {
                v_noteLineLayer = (NoteLineLayer)((int)noteData.noteLineLayer / 1000) - 1; // Definition from ME
            }

            // Keep This Note: This is not a robust way to check for extended maps (see above)
            /*if ((int)noteData.noteLineLayer > 2)
            {
                v_noteLineLayer = (NoteLineLayer)rand.Next(3); // ME chaos mode
            }*/

            // Only non-precision-placement maps can have the option to be layer flipped
            // Maps with extended layers but non-precision-placement (eg: noteLineLayer is 5) may have odd results. Consider that part of chaos mode lol
            else if (flip_rows)
            {
                v_noteLineLayer = (NoteLineLayer)(3 - 1 - (int)noteData.noteLineLayer);
            }
            else
            {
                v_noteLineLayer = noteData.noteLineLayer;
            }

            NoteCutDirection v_cutDirection;
            if (vertical_cut_transform.TryGetValue(noteData.cutDirection, out v_cutDirection) == false || has_ME)
            {
                v_cutDirection = Get_Random_Direction();
            }

            NoteData v_noteData = NoteData.CreateBasicNoteData(noteData.time, Check_Index(noteData.lineIndex), v_noteLineLayer, noteData.colorType, v_cutDirection);

            return v_noteData;
        }


        /*private static ObstacleData Mirror_Vertical_Obstacle(ObstacleData obstacleData, bool flip_rows)
        {
            if (flip_rows && obstacleData.obstacleType == ObstacleType.Top)
            {
                obstacleData.MoveTime(-1); // To keep the number of walls the same
            }

            return obstacleData;
        }*/
        #endregion


        #region "Check Functions"
        internal static int Check_Index(int lineIndex)
        {
            if (lineIndex >= 500 || lineIndex <= -500)
            {
                return lineIndex / 1000;
                //return rand.Next(4); // ME chaos mode
            }

            return lineIndex;
        }

        internal static int Check_Layer(int lineLayer)
        {
            if (lineLayer >= 500 || lineLayer <= -500)
            {
                return lineLayer / 1000;
                //return (NoteLineLayer)rand.Next(3); // ME chaos mode
            }

            return lineLayer;
        }
        #endregion
    }
}