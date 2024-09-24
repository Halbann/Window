using System.IO;

using UnityEngine;

namespace Window
{
    [KSPAddon(KSPAddon.Startup.FlightAndKSC, false)]
    public class Window : MonoBehaviour
    {
        private static string PluginData =>
            Path.Combine(KSPUtil.ApplicationRootPath, "GameData", "Window", "PluginData");

        private static string Config =>
            Path.Combine(PluginData, "settings.cfg");

        public static float fieldOfView = 60f;
        public static float fieldOfViewIVA = 60f;
        public static bool useWindow = false;
        public static float windowWidth = 60f;
        public static float windowDistance = 70f;

        public static float trueDefault = 60;

        public static bool runOnce = true;

        #region Core

        protected void Start()
        {
            if (runOnce)
            {
                Load();
                runOnce = false;
            }

            if (HighLogic.LoadedSceneIsFlight)
                Apply();
            else
                SetFOV(trueDefault);
        }

        public static void Apply()
        {
            if (useWindow)
            {
                float fov = CalculateFOV(windowDistance, windowWidth);
                SetFOV(fov, fov);
            }
            else
                SetFOV(fieldOfView, fieldOfViewIVA);
        }

        public static float CalculateFOV(float distance, float size)
        {
            return 2 * Mathf.Atan(size / (2 * distance)) * Mathf.Rad2Deg;
        }

        public static void SetFOV(float fov, float fovIVA = 60)
        {
            FlightCamera.fetch.SetFoV(fov);
            FlightCamera.fetch.fovDefault = fov;

            if (InternalCamera.Instance == null)
                return;

            InternalCamera.Instance.SetFOV(fovIVA);
            InternalCamera.Instance.GetType().GetField("currentFoV", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(InternalCamera.Instance, fovIVA);
        }

        #endregion

        #region Lazy Serialisation

        internal static void Save()
        {
            if (!Directory.Exists(PluginData))
                Directory.CreateDirectory(PluginData);

            ConfigNode settingsNode = new ConfigNode("WindowSettings");
            settingsNode.AddValue("version", 1);

            settingsNode.AddValue("fieldOfView", fieldOfView.ToString());
            settingsNode.AddValue("fieldOfViewIVA", fieldOfViewIVA.ToString());
            settingsNode.AddValue("useWindow", useWindow.ToString());
            settingsNode.AddValue("windowWidth", windowWidth.ToString());
            settingsNode.AddValue("windowDistance", windowDistance.ToString());

            ConfigNode file = new ConfigNode();
            file.AddNode(settingsNode);
            file.Save(Config);
        }

        internal void Load()
        {
            if (!File.Exists(Config))
                return;

            ConfigNode file = ConfigNode.Load(Config);
            ConfigNode settingsNode = file.GetNode("WindowSettings");

            settingsNode.TryGetValue("fieldOfView", ref fieldOfView);
            settingsNode.TryGetValue("fieldOfViewIVA", ref fieldOfViewIVA);
            settingsNode.TryGetValue("useWindow", ref useWindow);
            settingsNode.TryGetValue("windowWidth", ref windowWidth);
            settingsNode.TryGetValue("windowDistance", ref windowDistance);
        }

        #endregion
    }
}
