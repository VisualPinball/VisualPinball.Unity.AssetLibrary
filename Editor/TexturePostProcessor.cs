// Visual Pinball Engine
// Copyright (C) 2022 freezy and VPE Team
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
using System.IO;
using UnityEditor;
using UnityEditor.Presets;
using UnityEngine;

namespace VisualPinball.Unity.AssetLibrary.Editor
{
	public class TexturePostProcessor : AssetPostprocessor
	{
		private static Preset _maskMapPreset;
		private static Preset _normalMapPreset;

		private static readonly string[] ExcludedProps = { "m_MaxTextureSize", "m_PlatformSettings" };

		private void OnPreprocessAsset()
		{
			if (string.IsNullOrEmpty(assetPath)) {
				return;
			}

			if (Path.GetExtension(assetPath).ToLowerInvariant() != ".png" && Path.GetExtension(assetPath).ToLowerInvariant() != ".jpg") {
				return;
			}
			const string presetPath = "Packages/org.visualpinball.unity.assetlibrary/Editor/Presets";

			if (Path.GetFileNameWithoutExtension(assetPath).EndsWith("MaskMap")) {
				if (_maskMapPreset == null) {
					_maskMapPreset = AssetDatabase.LoadAssetAtPath<Preset>($"{presetPath}/MaskMap.preset");
				}
				if (_maskMapPreset) {
					ApplyPresetExcludingProperties(_maskMapPreset, assetImporter, ExcludedProps);
				} else {
					Debug.LogWarning($"Unable to load mask map preset for {assetPath}.");
				}
			}

			if (Path.GetFileNameWithoutExtension(assetPath).EndsWith("Normal")) {
				if (_normalMapPreset == null) {
					_normalMapPreset = AssetDatabase.LoadAssetAtPath<Preset>($"{presetPath}/NormalMap.preset");
				}
				if (_normalMapPreset) {
					ApplyPresetExcludingProperties(_normalMapPreset, assetImporter, ExcludedProps);
				} else {
					Debug.LogWarning($"Unable to load normal map preset for {assetPath}.");
				}
			}
		}

		private static void ApplyPresetExcludingProperties(Preset preset, Object target, params string[] excludedPropertyPaths)
		{
			var appliedPropertyPaths = GetAllPropertyPaths(target);
			foreach(var excludedPropertyPath in excludedPropertyPaths) {
				appliedPropertyPaths.Remove(excludedPropertyPath);
			}
			preset.ApplyTo(target, appliedPropertyPaths.ToArray());
		}

		private static List<string> GetAllPropertyPaths(Object target)
		{
			var serializedObject = new SerializedObject(target);
			var propertyPaths = new List<string>(10);
			var serializedProperty = serializedObject.GetIterator();
			if (!serializedProperty.NextVisible(true)) {
				return propertyPaths;
			}

			while(serializedProperty.NextVisible(false)) {
				//Debug.Log($"Property: {serializedProperty.propertyPath}");
				propertyPaths.Add(serializedProperty.propertyPath);
			}

			return propertyPaths;
		}
	}
}