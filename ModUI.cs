using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using Chirality.Configuration;
using UnityEngine;

namespace Chirality
{
    class ModUI : NotifiableSingleton<ModUI>
    {
        internal static StandardLevelDetailView standardLevelDetailView;

        internal static StandardLevelDetailViewController standardLevelDetailViewController;

        public ModUI()
        {

        }

        [UIValue("increment_value")]
        private int Increment_Value
        {
            get => PluginConfig.Instance.mode;
            set
            {
                standardLevelDetailView = Resources.FindObjectsOfTypeAll<StandardLevelDetailView>().FirstOrDefault();
                standardLevelDetailViewController = Resources.FindObjectsOfTypeAll<StandardLevelDetailViewController>().FirstOrDefault();

                PluginConfig.Instance.mode = value;

                standardLevelDetailViewController.ClearSelected();
                //standardLevelDetailView.SetContent(standardLevelDetailViewController.selectedDifficultyBeatmap.level, );
            }
        }

        [UIAction("increment_formatter")]
        private string Increment_Formatter(int value) => ((PreferenceEnum)value).ToString();

        public enum PreferenceEnum
        {
            Standard = 0,
            NoArrows = 1,
            OneSaber = 2,
            Lawless = 3
        }
    }
}
