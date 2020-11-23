using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace VisualPinball.Unity.Editor
{
    public class LightingEnvironmentControllerMenu : MonoBehaviour
    {
        [MenuItem("Visual Pinball/Editor Lighting Environment")]
        public static void StartLightEnvEditor()
        {
            LightingEnvironmentController.LaunchEnvWindow();
        }

    }
}