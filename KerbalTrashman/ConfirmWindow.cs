using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KerbalTrashman
{
    internal class ConfirmWindow : MonoBehaviour
    {
        /// <summary>
        /// The title of the window.
        /// </summary>
        private const string WindowTitle = KerbalTrashmanController.PluginName;

        /// <summary>
        /// Position of the window.
        /// </summary>
        private static Rect windowPosition = new Rect(Screen.width / 4, Screen.height / 4, 1F, 1F);

        /// <summary>
        /// The unique ID of the window.
        /// </summary>
        private int id;

        /// <summary>
        /// Is the window open?
        /// </summary>
        private bool isWindowOpen = true;

        /// <summary>
        /// Called when the window is being loaded.
        /// </summary>
        public void Awake()
        {
            // Window setup.
            id = Guid.NewGuid().GetHashCode();

            // Add event handlers.
            GameEvents.onShowUI.Add(ShowUI);
            GameEvents.onHideUI.Add(HideUI);
            GameEvents.onGameSceneLoadRequested.Add(GameSceneLoadRequested);
        }

        /// <summary>
        /// Called when destroyed.
        /// </summary>
        public void OnDestroy()
        {
            // Remove event handlers.
            GameEvents.onShowUI.Remove(ShowUI);
            GameEvents.onHideUI.Remove(HideUI);
            GameEvents.onGameSceneLoadRequested.Remove(GameSceneLoadRequested);
        }

        /// <summary>
        /// Called for rendering and handling the GUI.
        /// </summary>
        public void OnGUI()
        {
            if (isWindowOpen)
            {
                windowPosition = GUILayout.Window(id, windowPosition, RenderWindow, WindowTitle, GUILayout.MinWidth(500));
            }
        }

        /// <summary>
        /// Shows the UI.
        /// </summary>
        private void ShowUI()
        {
            isWindowOpen = true;
        }

        /// <summary>
        /// Hides the UI.
        /// </summary>
        private void HideUI()
        {
            isWindowOpen = false;
        }

        /// <summary>
        /// Hides the UI during game scene change.
        /// </summary>
        /// <param name="newGameScene">the new game scene</param>
        private void GameSceneLoadRequested(GameScenes newGameScene)
        {
            KerbalTrashmanController.SetApplauncherButtonFalse();
        }

        /// <summary>
        /// The main method that renders the window.
        /// </summary>
        private void RenderWindow(int windowId)
        {
            List<Vessel> qualifyingVessels = QualifyingVessels();

            // Begin Window
            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));

            if (qualifyingVessels.Count == 0)
            {
                GUILayout.Label("There is no debris that can be removed.");
                if (GUILayout.Button("Ok", GUILayout.ExpandWidth(true)))
                {
                    KerbalTrashmanController.SetApplauncherButtonFalse();
                }
            }
            else
            {
                GUILayout.Label("Are you sure you wish to destroy the following?");

                int cnt = 1;
                foreach (Vessel vessel in qualifyingVessels)
                {
                    GUILayout.Label("  " + cnt + "." + vessel.vesselName);
                    cnt++;
                }

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Yes", GUILayout.ExpandWidth(true)))
                {
                    foreach (Vessel vessel in QualifyingVessels())
                    {
                        Debug.Log(vessel.name + " to be destroyed.");
                        SpaceTracking.DestroyObject(vessel);
                        SpaceTracking.StopTrackingObject(vessel);
                    }
                    
                    KerbalTrashmanController.SetApplauncherButtonFalse();
                }
                if (GUILayout.Button("No", GUILayout.ExpandWidth(true)))
                {
                    KerbalTrashmanController.SetApplauncherButtonFalse();
                }
                GUILayout.EndHorizontal();
            }
            

            GUILayout.EndVertical();

            GUI.DragWindow();
        }

        /// <summary>
        /// Finds the vessels that qualify for removal.
        /// </summary>
        /// <returns>The list of qualifying vessels.</returns>
        private static List<Vessel> QualifyingVessels()
        {
            return FlightGlobals.Vessels.Where(v => v.vesselType == VesselType.Debris)
                .Where(v => v.orbit.eccentricity < 1.0)
                .Where(v => v.mainBody.atmosphere)
                .Where(v => v.orbit.PeA < v.mainBody.atmosphereDepth)
                .ToList();
        }
    }
}
