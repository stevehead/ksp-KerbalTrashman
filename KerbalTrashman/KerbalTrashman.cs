using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KerbalTrashman
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class KerbalTrashmanController : MonoBehaviour
    {
        /// <summary>
        /// The plugin's name.
        /// </summary>
        public const string PluginName = "Kerbal Trashman";

        /// <summary>
        /// The plugin's directory under GameData.
        /// </summary>
        public const string PluginDirectoryName = "KerbalTrashman";

        /// <summary>
        /// The stock toolbar icon.
        /// </summary>
        private const string PluginIconButtonStock = "icon_button_stock";

        /// <summary>
        /// The application launcher button that is created.
        /// </summary>
        private static ApplicationLauncherButton appLauncherButton = null;

        /// <summary>
        /// The scenes the button will be visible in.
        /// </summary>
        private static ApplicationLauncher.AppScenes ScenesApplicationVisibleIn
        {
            get { return ApplicationLauncher.AppScenes.TRACKSTATION; }
        }

        /// <summary>
        /// Called when the script is loaded.
        /// </summary>
        public void Awake()
        {
            GameEvents.onGUIApplicationLauncherReady.Add(AddAppLauncherButton);
            GameEvents.onGUIApplicationLauncherDestroyed.Add(RemoveAppLauncherButton);
        }

        /// <summary>
        /// Called to add the application to the stock toolbar.
        /// </summary>
        private void AddAppLauncherButton()
        {
            if (ApplicationLauncher.Ready && appLauncherButton == null)
            {
                string pluginIconButtonStockPath = String.Format("{0}/Textures/{1}", PluginDirectoryName, PluginIconButtonStock);
                appLauncherButton = ApplicationLauncher.Instance.AddModApplication(
                    OnAppLaunchClick, OnAppLaunchClick,
                    null, null,
                    null, null,
                    ScenesApplicationVisibleIn,
                    (Texture)GameDatabase.Instance.GetTexture(pluginIconButtonStockPath, false));
            }
        }

        /// <summary>
        /// Removes the application from the stock toolbar.
        /// </summary>
        private void RemoveAppLauncherButton()
        {
            ApplicationLauncher.Instance.RemoveApplication(appLauncherButton);
        }

        /// <summary>
        /// Action when the button is clicked.
        /// </summary>
        private void OnAppLaunchClick()
        {
            RemoveVessels();
        }

        /// <summary>
        /// Method to remove vessels
        /// </summary>
        private void RemoveVessels()
        {
            foreach (Vessel vessel in FlightGlobals.Vessels)
            {
                if (vessel.vesselType != VesselType.Debris)
                {
                    continue;
                }
                if (vessel.orbit.eccentricity >= 1.0)
                {
                    continue;
                }
                if (!vessel.mainBody.atmosphere)
                {
                    continue;
                }
                if (vessel.orbit.PeA < vessel.mainBody.atmosphereDepth)
                {
                    Debug.Log(vessel.name + " to be destroyed.");
                    Destroy(vessel);
                }
            }
        }
    }
}
