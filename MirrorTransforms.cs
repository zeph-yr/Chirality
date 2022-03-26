using BeatmapSaveDataVersion3;
using System.Collections.Generic;

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


        #region "Main Transform Functions"
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

            // Sliders:
            List<BeatmapSaveData.SliderData> h_sliderDatas = new List<BeatmapSaveData.SliderData>();
            foreach (BeatmapSaveData.SliderData sliderData in beatmapSaveData.sliders)
            {
                h_sliderDatas.Add(Mirror_Horizontal_Slider(sliderData, numberOfLines, flip_lines, is_ME));
            }

            // BurstSliders:
            List<BeatmapSaveData.BurstSliderData> h_burstSliderDatas = new List<BeatmapSaveData.BurstSliderData>();
            foreach (BeatmapSaveData.BurstSliderData burstSliderData in beatmapSaveData.burstSliders)
            {
                h_burstSliderDatas.Add(Mirror_Horizontal_BurstSlider(burstSliderData, numberOfLines, flip_lines, is_ME));
            }


            return new BeatmapSaveData(beatmapSaveData.bpmEvents, beatmapSaveData.rotationEvents, h_colorNotes, h_bombNotes, h_obstacleDatas, h_sliderDatas, 
                                       h_burstSliderDatas, beatmapSaveData.waypoints, beatmapSaveData.basicBeatmapEvents, beatmapSaveData.colorBoostBeatmapEvents, 
                                       beatmapSaveData.lightColorEventBoxGroups, beatmapSaveData.lightRotationEventBoxGroups, beatmapSaveData.basicEventTypesWithKeywords, 
                                       beatmapSaveData.useNormalEventsAsCompatibleEvents);
        }


        internal static BeatmapSaveData Mirror_Vertical(BeatmapSaveData beatmapSaveData, bool flip_rows, bool remove_walls, bool is_ME)
        {
            // Bombs:
            List<BeatmapSaveData.BombNoteData> v_bombNotes = new List<BeatmapSaveData.BombNoteData>();
            foreach (BeatmapSaveData.BombNoteData bombNoteData in beatmapSaveData.bombNotes)
            {
                if (flip_rows)
                {
                    v_bombNotes.Add(new BeatmapSaveData.BombNoteData(bombNoteData.beat, bombNoteData.line, 3 - 1 - bombNoteData.layer));
                }
                else
                {
                    v_bombNotes.Add(bombNoteData);
                }
            }

            // ColorNotes:
            List<BeatmapSaveData.ColorNoteData> v_colorNotes = new List<BeatmapSaveData.ColorNoteData>();
            foreach (BeatmapSaveData.ColorNoteData colorNote in beatmapSaveData.colorNotes)
            {
                v_colorNotes.Add(Mirror_Vertical_Note(colorNote, flip_rows, is_ME));
            }

            // Obstacles:
            List<BeatmapSaveData.ObstacleData> v_obstacleDatas = new List<BeatmapSaveData.ObstacleData>();
            if (remove_walls == false)
            {
                foreach (BeatmapSaveData.ObstacleData obstacleData in beatmapSaveData.obstacles)
                {
                    v_obstacleDatas.Add(Mirror_Vertical_Obstacle(obstacleData, flip_rows));
                }
            }

            // Sliders:
            List<BeatmapSaveData.SliderData> v_sliderDatas = new List<BeatmapSaveData.SliderData>();
            foreach (BeatmapSaveData.SliderData sliderData in beatmapSaveData.sliders)
            {
                v_sliderDatas.Add(Mirror_Vertical_Slider(sliderData, flip_rows, is_ME));
                //v_sliderDatas.Add((BeatmapSaveData.SliderData)Mirror_Vertical_Slider_Generic(sliderData, flip_rows, is_ME));
            }

            // BurstSliders:
            List<BeatmapSaveData.BurstSliderData> v_burstSliderDatas = new List<BeatmapSaveData.BurstSliderData>();
            foreach (BeatmapSaveData.BurstSliderData burstSliderData in beatmapSaveData.burstSliders)
            {
                v_burstSliderDatas.Add(Mirror_Vertical_BurstSlider(burstSliderData, flip_rows, is_ME));
                //v_burstSliderDatas.Add((BeatmapSaveData.BurstSliderData)Mirror_Vertical_Slider_Generic(burstSliderData, flip_rows, is_ME));
            }


            return new BeatmapSaveData(beatmapSaveData.bpmEvents, beatmapSaveData.rotationEvents, v_colorNotes, v_bombNotes, v_obstacleDatas, v_sliderDatas,
                                       v_burstSliderDatas, beatmapSaveData.waypoints, beatmapSaveData.basicBeatmapEvents, beatmapSaveData.colorBoostBeatmapEvents,
                                       beatmapSaveData.lightColorEventBoxGroups, beatmapSaveData.lightRotationEventBoxGroups, beatmapSaveData.basicEventTypesWithKeywords,
                                       beatmapSaveData.useNormalEventsAsCompatibleEvents);
        }


        internal static BeatmapSaveData Mirror_Inverse(BeatmapSaveData beatmapSaveData, int numberOfLines, bool flip_lines, bool flip_rows, bool remove_walls, bool is_ME)
        {
            //Plugin.Log.Debug("Mirror Inverse");
            
            return Mirror_Vertical(Mirror_Horizontal(beatmapSaveData, numberOfLines, flip_lines, remove_walls, is_ME), flip_rows, remove_walls, is_ME);
        }
        #endregion


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
                color = colorNoteData.color; // Actually fixed the color swap here for BS 1.20.0
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

        private static BeatmapSaveData.SliderData Mirror_Horizontal_Slider(BeatmapSaveData.SliderData sliderData, int numberOfLines, bool flip_lines, bool is_ME)
        {
            int h_headline;
            int h_tailline;

            BeatmapSaveData.NoteColorType color;
            if (sliderData.colorType == BeatmapSaveData.NoteColorType.ColorA)
            {
                color = BeatmapSaveData.NoteColorType.ColorB;
            }
            else
            {
                color = BeatmapSaveData.NoteColorType.ColorA;
            }


            // Precision maps will not have indexes flipped (complicated math) but their colors will
            // Yes, it will be weird like streams will zigzag in the wrong direction...hence introducing chaos mode. Might as well make use of the weirdness!
            // Other option is to just not support ME and NE maps
            // Also Note: Not worth reusing check function because non-extended map block will become unnecessarily complicated

            if (sliderData.headLine >= 1000 || sliderData.headLine <= -1000)
            {
                h_headline = sliderData.headLine / 1000 - 1; // Definition from ME
                color = sliderData.colorType; // Actually fixed the color swap here for BS 1.20.0
            }
            else if (flip_lines)
            {
                h_headline = numberOfLines - 1 - sliderData.headLine;
            }
            else
            {
                h_headline = sliderData.headLine;
                color = sliderData.colorType;
            }

            if (sliderData.tailLine >= 1000 || sliderData.tailLine <= -1000)
            {
                h_tailline = sliderData.tailLine / 1000 - 1; // Definition from ME
                color = sliderData.colorType; // Actually fixed the color swap here for BS 1.20.0
            }

            // Only non-precision-placement maps can have the option to be index flipped
            // Maps with extended non-precision-placement indexes are handled properly by numberOfLines
            else if (flip_lines)
            {
                h_tailline = numberOfLines - 1 - sliderData.tailLine;
            }
            else
            {
                h_tailline = sliderData.tailLine;
                color = sliderData.colorType;
            }

            NoteCutDirection h_headcutDirection; // Yes, this is support for precision placement and ME LOL
            if (horizontal_cut_transform.TryGetValue(sliderData.headCutDirection, out h_headcutDirection) == false || is_ME)
            {
                h_headcutDirection = Get_Random_Direction();
            }

            NoteCutDirection h_tailcutDirection; // Yes, this is support for precision placement and ME LOL
            if (horizontal_cut_transform.TryGetValue(sliderData.tailCutDirection, out h_tailcutDirection) == false || is_ME)
            {
                h_headcutDirection = Get_Random_Direction();
            }

            return new BeatmapSaveData.SliderData(color, sliderData.beat, h_headline, Check_Layer(sliderData.headLayer), sliderData.headControlPointLengthMultiplier, h_headcutDirection,
                                                  sliderData.tailBeat, h_tailline, Check_Layer(sliderData.tailLayer), sliderData.tailControlPointLengthMultiplier, h_tailcutDirection, sliderData.sliderMidAnchorMode);
        }


        private static BeatmapSaveData.BurstSliderData Mirror_Horizontal_BurstSlider(BeatmapSaveData.BurstSliderData burstSliderData, int numberOfLines, bool flip_lines, bool is_ME)
        {
            int h_headline;
            int h_tailline;

            BeatmapSaveData.NoteColorType color;
            if (burstSliderData.colorType == BeatmapSaveData.NoteColorType.ColorA)
            {
                color = BeatmapSaveData.NoteColorType.ColorB;
            }
            else
            {
                color = BeatmapSaveData.NoteColorType.ColorA;
            }


            // Precision maps will not have indexes flipped (complicated math) but their colors will
            // Yes, it will be weird like streams will zigzag in the wrong direction...hence introducing chaos mode. Might as well make use of the weirdness!
            // Other option is to just not support ME and NE maps
            // Also Note: Not worth reusing check function because non-extended map block will become unnecessarily complicated

            if (burstSliderData.headLine >= 1000 || burstSliderData.headLine <= -1000)
            {
                h_headline = burstSliderData.headLine / 1000 - 1; // Definition from ME
                color = burstSliderData.colorType; // Actually fixed the color swap here for BS 1.20.0
            }
            else if (flip_lines)
            {
                h_headline = numberOfLines - 1 - burstSliderData.headLine;
            }
            else
            {
                h_headline = burstSliderData.headLine;
                color = burstSliderData.colorType;
            }

            if (burstSliderData.tailLine >= 1000 || burstSliderData.tailLine <= -1000)
            {
                h_tailline = burstSliderData.tailLine / 1000 - 1; // Definition from ME
                color = burstSliderData.colorType; // Actually fixed the color swap here for BS 1.20.0
            }

            // Only non-precision-placement maps can have the option to be index flipped
            // Maps with extended non-precision-placement indexes are handled properly by numberOfLines
            else if (flip_lines)
            {
                h_tailline = numberOfLines - 1 - burstSliderData.tailLine;
            }
            else
            {
                h_tailline = burstSliderData.tailLine;
                color = burstSliderData.colorType;
            }

            NoteCutDirection h_headcutDirection; // Yes, this is support for precision placement and ME LOL
            if (horizontal_cut_transform.TryGetValue(burstSliderData.headCutDirection, out h_headcutDirection) == false || is_ME)
            {
                h_headcutDirection = Get_Random_Direction();
            }

            return new BeatmapSaveData.BurstSliderData(color, burstSliderData.beat, h_headline, Check_Layer(burstSliderData.headLayer), h_headcutDirection,
                                                  burstSliderData.tailBeat, h_tailline, Check_Layer(burstSliderData.tailLayer), burstSliderData.sliceCount, burstSliderData.squishAmount);
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


        private static BeatmapSaveData.ColorNoteData Mirror_Vertical_Note(BeatmapSaveData.ColorNoteData colorNoteData, bool flip_rows, bool has_ME)
        {
            int v_noteLineLayer;

            // All precision placements will not be layer-flipped (complicated math)
            // This could be weird, consider it part of chaos mode KEK
            if (colorNoteData.layer >= 1000 || colorNoteData.layer <= -1000)
            {
                v_noteLineLayer = (colorNoteData.layer / 1000) - 1; // Definition from ME
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
                v_noteLineLayer = 3 - 1 - colorNoteData.layer;
            }
            else
            {
                v_noteLineLayer = colorNoteData.layer;
            }

            NoteCutDirection v_cutDirection;
            if (vertical_cut_transform.TryGetValue(colorNoteData.cutDirection, out v_cutDirection) == false || has_ME)
            {
                v_cutDirection = Get_Random_Direction();
            }

            return new BeatmapSaveData.ColorNoteData(colorNoteData.beat, Check_Index(colorNoteData.line), v_noteLineLayer, colorNoteData.color, v_cutDirection, colorNoteData.angleOffset);
        }


        private static BeatmapSaveData.ObstacleData Mirror_Vertical_Obstacle(BeatmapSaveData.ObstacleData obstacleData, bool flip_rows)
        {
            if (flip_rows)
            {
                return new BeatmapSaveData.ObstacleData(obstacleData.beat, 0, 0, 0, 0, 0);
            }

            return obstacleData;
        }

        private static BeatmapSaveData.SliderData Mirror_Vertical_Slider(BeatmapSaveData.SliderData sliderData, bool flip_rows, bool has_ME)
        {
            int v_head_noteLineLayer;
            int v_tail_noteLineLayer;

            // All precision placements will not be layer-flipped (complicated math)
            // This could be weird, consider it part of chaos mode KEK
            if (sliderData.headLayer >= 1000 || sliderData.headLayer<= -1000)
            {
                v_head_noteLineLayer = (sliderData.headLayer / 1000) - 1; // Definition from ME
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
                v_head_noteLineLayer = 3 - 1 - sliderData.headLayer;
            }
            else
            {
                v_head_noteLineLayer = sliderData.headLayer;
            }


            if (sliderData.tailLayer >= 1000 || sliderData.tailLayer <= -1000)
            {
                v_tail_noteLineLayer = (sliderData.tailLayer / 1000) - 1; // Definition from ME
            }
            else if (flip_rows)
            {
                v_tail_noteLineLayer = 3 - 1 - sliderData.tailLayer;
            }
            else
            {
                v_tail_noteLineLayer = sliderData.tailLayer;
            }


            NoteCutDirection v_headcutDirection;
            if (vertical_cut_transform.TryGetValue(sliderData.headCutDirection, out v_headcutDirection) == false || has_ME)
            {
                v_headcutDirection = Get_Random_Direction();
            }

            NoteCutDirection v_tailcutDirection;
            if (vertical_cut_transform.TryGetValue(sliderData.tailCutDirection, out v_tailcutDirection) == false || has_ME)
            {
                v_tailcutDirection = Get_Random_Direction();
            }


            return new BeatmapSaveData.SliderData(sliderData.colorType, sliderData.beat, Check_Index(sliderData.headLine), v_head_noteLineLayer, sliderData.headControlPointLengthMultiplier, v_headcutDirection,
                                                              sliderData.tailBeat, Check_Index(sliderData.tailLine), v_tail_noteLineLayer, sliderData.tailControlPointLengthMultiplier, v_tailcutDirection, sliderData.sliderMidAnchorMode);
        }


        private static BeatmapSaveData.BurstSliderData Mirror_Vertical_BurstSlider(BeatmapSaveData.BurstSliderData burstSliderData, bool flip_rows, bool has_ME)
        {
            int v_head_noteLineLayer;
            int v_tail_noteLineLayer;

            // All precision placements will not be layer-flipped (complicated math)
            // This could be weird, consider it part of chaos mode KEK
            if (burstSliderData.headLayer >= 1000 || burstSliderData.headLayer <= -1000)
            {
                v_head_noteLineLayer = (burstSliderData.headLayer / 1000) - 1; // Definition from ME
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
                v_head_noteLineLayer = 3 - 1 - burstSliderData.headLayer;
            }
            else
            {
                v_head_noteLineLayer = burstSliderData.headLayer;
            }


            if (burstSliderData.tailLayer >= 1000 || burstSliderData.tailLayer <= -1000)
            {
                v_tail_noteLineLayer = (burstSliderData.tailLayer / 1000) - 1; // Definition from ME
            }
            else if (flip_rows)
            {
                v_tail_noteLineLayer = 3 - 1 - burstSliderData.tailLayer;
            }
            else
            {
                v_tail_noteLineLayer = burstSliderData.tailLayer;
            }


            NoteCutDirection v_headcutDirection;
            if (vertical_cut_transform.TryGetValue(burstSliderData.headCutDirection, out v_headcutDirection) == false || has_ME)
            {
                v_headcutDirection = Get_Random_Direction();
            }

            return new BeatmapSaveData.BurstSliderData(burstSliderData.colorType, burstSliderData.beat, Check_Index(burstSliderData.headLine), v_head_noteLineLayer, v_headcutDirection,
                                                              burstSliderData.tailBeat, Check_Index(burstSliderData.tailLine), v_tail_noteLineLayer, burstSliderData.sliceCount, burstSliderData.squishAmount);
        }


        // Experiment with reusing this function. Not sure its actually better with casting in the main function
        /*private static BeatmapSaveData.BaseSliderData Mirror_Vertical_Slider_Generic(BeatmapSaveData.BaseSliderData baseSliderData, bool flip_rows, bool has_ME)
        {
            int v_head_noteLineLayer;
            int v_tail_noteLineLayer;

            // All precision placements will not be layer-flipped (complicated math)
            // This could be weird, consider it part of chaos mode KEK
            if (baseSliderData.headLayer >= 1000 || baseSliderData.headLayer <= -1000)
            {
                v_head_noteLineLayer = (baseSliderData.headLayer / 1000) - 1; // Definition from ME
            }

            // Only non-precision-placement maps can have the option to be layer flipped
            // Maps with extended layers but non-precision-placement (eg: noteLineLayer is 5) may have odd results. Consider that part of chaos mode lol
            else if (flip_rows)
            {
                v_head_noteLineLayer = 3 - 1 - baseSliderData.headLayer;
            }
            else
            {
                v_head_noteLineLayer = baseSliderData.headLayer;
            }


            if (baseSliderData.tailLayer >= 1000 || baseSliderData.tailLayer <= -1000)
            {
                v_tail_noteLineLayer = (baseSliderData.tailLayer / 1000) - 1; // Definition from ME
            }
            else if (flip_rows)
            {
                v_tail_noteLineLayer = 3 - 1 - baseSliderData.tailLayer;
            }
            else
            {
                v_tail_noteLineLayer = baseSliderData.tailLayer;
            }


            NoteCutDirection v_headcutDirection;
            if (vertical_cut_transform.TryGetValue(baseSliderData.headCutDirection, out v_headcutDirection) == false || has_ME)
            {
                v_headcutDirection = Get_Random_Direction();
            }

            BeatmapSaveData.SliderData sliderData;
            NoteCutDirection v_tailcutDirection;
            if ((sliderData = baseSliderData as BeatmapSaveData.SliderData) != null)
            {
                if (vertical_cut_transform.TryGetValue(sliderData.tailCutDirection, out v_tailcutDirection) == false || has_ME)
                {
                    v_tailcutDirection = Get_Random_Direction();
                }

                return new BeatmapSaveData.SliderData(baseSliderData.colorType, baseSliderData.beat, Check_Index(baseSliderData.headLine), v_head_noteLineLayer, sliderData.headControlPointLengthMultiplier, v_headcutDirection,
                                                      baseSliderData.tailBeat, Check_Index(baseSliderData.tailLine), v_tail_noteLineLayer, sliderData.tailControlPointLengthMultiplier, v_tailcutDirection, sliderData.sliderMidAnchorMode);
            }

            else
            {
                BeatmapSaveData.BurstSliderData burstSliderData = (BeatmapSaveData.BurstSliderData)baseSliderData;

                return new BeatmapSaveData.BurstSliderData(baseSliderData.colorType, baseSliderData.beat, Check_Index(baseSliderData.headLine), v_head_noteLineLayer, v_headcutDirection,
                                                           baseSliderData.tailBeat, Check_Index(baseSliderData.tailLine), v_tail_noteLineLayer, burstSliderData.sliceCount, burstSliderData.squishAmount);
            }
        }*/

        #endregion


        #region "Check Functions"
        internal static NoteCutDirection Get_Random_Direction()
        {
            int index = rand.Next(directions.Count);

            return directions[index];
        }

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