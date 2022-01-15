using System.Linq;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using Chirality.Configuration;
using UnityEngine;

namespace Chirality
{
    class ModUI : NotifiableSingleton<ModUI>
    {
        internal static StandardLevelDetailViewController standardLevelDetailViewController;

        public ModUI()
        {

        }

        [UIValue("mode")]
        private string Mode => ((PreferenceEnum)PluginConfig.Instance.mode).ToString();

        public enum PreferenceEnum
        {
            Standard = 0,
            NoArrows = 1,
            OneSaber = 2,
            Lawless = 3
        }


        /*[UIValue("increment_value")]
        private int Increment_Value
        {
            get => PluginConfig.Instance.mode;
            set
            {
                PluginConfig.Instance.mode = value;

                standardLevelDetailViewController = Resources.FindObjectsOfTypeAll<StandardLevelDetailViewController>().FirstOrDefault();
                standardLevelDetailViewController.ClearSelected();
            }
        }

        [UIAction("increment_formatter")]
        private string Increment_Formatter(int value) => ((PreferenceEnum)value).ToString();

        <increment-setting value='increment_value' apply-on-change='true' bind-value='true' text='Mode' integer-only='true' min='0' max='3' formatter='increment_formatter'/>
        */
    }
}
