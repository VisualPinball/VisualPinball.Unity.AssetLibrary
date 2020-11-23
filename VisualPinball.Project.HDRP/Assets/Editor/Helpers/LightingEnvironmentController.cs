// Visual Pinball Engine
// Copyright (C) 2020 freezy and VPE Team
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <https://www.gnu.org/licenses/>.

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Reflection.Emit;
using UnityEditor.Rendering;
using Unity.Entities;
using UnityEditor.SceneManagement;
using VisualPinball.Unity.Patcher.Matcher;


//TODO: Split drawing from logic. -Pandeli


namespace VisualPinball.Unity.Editor
{

    [EditorTool("Light Environment Controller")]
    public class LightingEnvironmentController : EditorWindow
    {
        #region Constants

        //Path to folder containing HDRI environment images. 
        const string pathToHDRIEnvs = AssetPaths.HDRIEnvs; 
        const string iconPath = AssetPaths.iconPath; 
        
        #endregion

        #region Variables

        //Whether to enable HDRI viewing.  
        [SerializeField]
        bool enableHDRI;

        [SerializeField]
        static HDRISky envComponent;
        //Selection of environment
        [SerializeField]
        int environmentSelection = 0;

        //A collection of the available environment maps. 
        [SerializeField]
        private static List<Cubemap> environs;

        [SerializeField]
        static int environmentCount = -1;

        [SerializeField]
        private float environExp = 10f;

        [SerializeField]
        private float environRot = 0;

        private static bool initialized = false;
        private static Texture revertButton;
        private static GameObject lightEnvironment; 


        public string editorObjectName = "EditorScene";


        #endregion

        #region Builtin Methods
        public static void LaunchEnvWindow()
        {
            var envWindow = GetWindow<LightingEnvironmentController>("Light Environment");

            Init();

            envWindow.Show();

        }

        /// <summary>
        /// Finds the active HDRI Volume in the scene. 
        /// </summary>
        private static void InitializeHDRIVolume()
        {
            Volume[] volumes;
            GameObject[] rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
            envComponent = null;

            foreach(var rootGameObject in rootGameObjects)
            {

                volumes = rootGameObject.GetComponentsInChildren<Volume>();
                foreach(Volume volume in volumes)
                {
                    VolumeProfile vp = volume.profile;
                    if(vp)
                    {
                        vp.TryGet<HDRISky>(out envComponent);
                    }
                    if(envComponent) break;

                }

            }

            if(!envComponent)
            {
                //TODO: Pop dialog to request permission to add lighting environment.  
            }


        }
        #endregion

        private static void Init()
        {
            //TODO: Fix this. 
            revertButton = AssetDatabase.LoadAssetAtPath<Texture>(iconPath + "/revert.png");

            environs = new List<Cubemap>();
            InitializeHDRIVolume();
            CollectAssets();

            initialized = true;
        }

        private void OnGUI()
        {
            if(!initialized) Init();

            DrawLightEnvironmentWindow();

            Repaint();
        }


        private void DrawLightEnvironmentWindow()
        {
            EditorGUILayout.LabelField("Lighting Environment", EditorStyles.boldLabel);
            EditorGUILayout.Separator();

            Rect position = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);

            InitializeHDRIVolume();

            if(!envComponent)
            {
                EditorGUILayout.LabelField("No Light Environment Found", EditorStyles.boldLabel);
                if(GUILayout.Button("Add Light Environment")) 
                {
                    AddLightEnvironment();
                }
            }
            else
            {
                position = EditorGUI.PrefixLabel(position, new GUIContent("Environment Selection"));
                environmentSelection = EditorGUI.IntSlider(position, environmentSelection, 0, environs.Count - 1);
                position = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);

                position = EditorGUI.PrefixLabel(position, new GUIContent("Exposure"));
                position.width -= EditorGUIUtility.singleLineHeight;
                environExp = EditorGUI.Slider(position, environExp, 5f, 15f);
                position.x += position.width;
                if(GUI.Button(new Rect(position.x, position.y, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight), "<"))
                {
                    environExp = 10;
                }
                position.y += EditorGUIUtility.singleLineHeight;
                position = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);

                position = EditorGUI.PrefixLabel(position, new GUIContent("Rotation"));
                position.width -= EditorGUIUtility.singleLineHeight;
                environRot = EditorGUI.Slider(position, environRot, 0f, 360f);
                position.x += position.width;
                if(GUI.Button(new Rect(position.x, position.y, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight), "<"))
                {
                    environRot = 0f;
                }

                envComponent.hdriSky.Override(environs[environmentSelection]);
                envComponent.exposure.Override(environExp);
                envComponent.rotation.Override(environRot);

            }

            if(envComponent)
            {
                EditorUtility.SetDirty(envComponent);
            }
        }

        /// <summary>
        /// Creates the necessary hierarchy elements if not present and sets up a light environment. 
        /// </summary>
        private void AddLightEnvironment()
        {
            SetupEditorHierarchy.CreateLightEnvironment(); 
        }

		/// <summary>
		/// Collects the available environment assets.  
		/// </summary>
		public static bool CollectAssets()
        {
            bool returnValue = false;

            var assets = AssetDatabase.FindAssets("t:Cubemap", new[] { pathToHDRIEnvs });
            if(assets.Length > 0)
            {
                foreach(var guid in assets)
                {
                    var env = AssetDatabase.LoadAssetAtPath<Cubemap>(AssetDatabase.GUIDToAssetPath(guid));
                    environs.Add(env);

                }
                if(environs.Count > 0) returnValue = true;

            }

            environmentCount = environs.Count;

            return returnValue;
        }

    }

}