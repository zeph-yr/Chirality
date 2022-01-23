using System.Linq;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using Chirality.Configuration;
using UnityEngine;

namespace Chirality
{
    class ModUI : NotifiableSingleton<ModUI>
    {
        //internal static StandardLevelDetailViewController standardLevelDetailViewController;
        //internal static StandardLevelDetailView standardLevelDetailView;

        public ModUI()
        {

        }

        [UIValue("mode")]
        private string Mode => ((PreferenceEnum)PluginConfig.Instance.mode).ToString();

        [UIValue("enabled")]
        private string Enabled => Get_Enabled();

        internal string Get_Enabled()
        {
            if (PluginConfig.Instance.enabled)
            {
                return "To disable mod, set enabled in Chirality.json to false and restart game.";
            }
            else
                return "To enable mod, set enabled in Chirality.json to true and restart game.";
        }

        public enum PreferenceEnum
        {
            Standard = 0,
            NoArrows = 1,
            OneSaber = 2,
            Lawless = 3
        }


        // Was for in-game mode switching, but the anti-rabbit-multiplier is needed, which stops this from working immediately on maps that already have recently generated diffs
        // 13 unique map selections are needed to refresh the buffer of loaded songs before the new mode can be applied to a map with previously generated diffs, so...
        // May be too confusing for player. Restarting game to switch modes may be more easily understood
        /*
        [UIValue("increment_value")]
        private int Increment_Value
        {
            get => PluginConfig.Instance.mode;
            set
            {
                PluginConfig.Instance.mode = value;

                // This approach doesn'ts work:
                //standardLevelDetailViewController = Resources.FindObjectsOfTypeAll<StandardLevelDetailViewController>().FirstOrDefault();
                //standardLevelDetailView = Resources.FindObjectsOfTypeAll<StandardLevelDetailView>().FirstOrDefault();

                //standardLevelDetailViewController.ClearSelected();
                //standardLevelDetailViewController.RefreshContentLevelDetailView();

                //standardLevelDetailView.ClearContent();
                //standardLevelDetailView.RefreshContent();
    
                //SongCore.Loader loader = Resources.FindObjectsOfTypeAll<SongCore.Loader>().FirstOrDefault();
                //loader.RefreshSongs();
            }
        }

        [UIAction("increment_formatter")]
        private string Increment_Formatter(int value) => ((PreferenceEnum)value).ToString();

        //	<increment-setting value='increment_value' apply-on-change='true' bind-value='true' text='Mode' integer-only='true' min='0' max='3' formatter='increment_formatter'/>
        */
    }
}
