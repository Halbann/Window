using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Events;

using HarmonyLib;

namespace Window
{
    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    class Patcher : MonoBehaviour
    {
        public void Start()
        {
            var harmony = new Harmony("Halban.Window");
            harmony.PatchAll();
        }
    }

    [HarmonyPatch(typeof(VideoSettings))]
    [HarmonyPatch(nameof(VideoSettings.DrawMiniSettings))]
    class PatchDrawMiniSettings
    {
        private static DialogGUIHorizontalLayout AddSlider(List<DialogGUIBase> modifiedList, string name, Func<float> GetValue, UnityAction<float> SetValue)
        {
            // Generic Slider.

            DialogGUISlider slider = new DialogGUISlider(GetValue, 5, 120, true, 150f, -1f, SetValue);
            DialogGUILabel label = new DialogGUILabel(() => GetValue().ToString(), 80);

            DialogGUIHorizontalLayout entry = new DialogGUIHorizontalLayout(0f, 18f, 0f, new RectOffset(), TextAnchor.MiddleLeft,
                new DialogGUILabel(name + ":", 150f), slider, new DialogGUISpace(30f), label);

            modifiedList.Add(entry);
            
            return entry;
        }

        static void Postfix(ref DialogGUIBase[] __result)
        {
            // Modified from KSPCommunityFixes (MIT License)
            // https://github.com/KSPModdingLibs/KSPCommunityFixes/blob/441f2d7e0ab844cbc3bd86ef2c5410c245f08cb9/KSPCommunityFixes/Internal/PatchSettings.cs#L54

            List<DialogGUIBase> modifiedList = __result.ToList();

            // FOV sliders.

            modifiedList.Add(new DialogGUIBox("Field of View", -1f, 18f, null));

            var fovEntry = AddSlider(modifiedList, "Default Field of View", () => Window.fieldOfView, f => Window.fieldOfView = f);
            var ivaEntry = AddSlider(modifiedList, "Default Field of View (IVA)", () => Window.fieldOfViewIVA, f => Window.fieldOfViewIVA = f);
                                          
            // Use Window Toggle.

            var toggleLabel = new DialogGUILabel("Use Window:", 150f);
            var toggle = new DialogGUIToggle(() => Window.useWindow, "", b => Window.useWindow = b, 120f);
            modifiedList.Add(new DialogGUIHorizontalLayout(0f, 18f, 0f, new RectOffset(), TextAnchor.MiddleLeft, toggleLabel, toggle));

            // Window sliders.

            var widthEntry = AddSlider(modifiedList, "Window Width", () => Window.windowWidth, f => Window.windowWidth = f);
            var distanceEntry = AddSlider(modifiedList, "Window Distance", () => Window.windowDistance, f => Window.windowDistance = f);

            // Conditions.

            fovEntry.OptionInteractableCondition = () => !Window.useWindow;
            ivaEntry.OptionInteractableCondition = () => !Window.useWindow;

            widthEntry.OptionInteractableCondition = () => Window.useWindow;
            distanceEntry.OptionInteractableCondition = () => Window.useWindow;
            
            // Return.

            __result = modifiedList.ToArray();
        }
    }

    [HarmonyPatch(typeof(VideoSettings))]
    [HarmonyPatch(nameof(VideoSettings.ApplySettings))]
    class PatchApplySettings
    {
        static void Postfix()
        {
            // When the user clicks accept or apply on the mini-settings window.

            Window.Save();

            if (HighLogic.LoadedSceneIsFlight)
                Window.Apply();
        }
    }
}
