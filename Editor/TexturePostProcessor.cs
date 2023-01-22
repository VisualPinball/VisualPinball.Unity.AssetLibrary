// Visual Pinball Engine
// Copyright (C) 2023 freezy and VPE Team
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
					_maskMapPreset.ApplyTo(assetImporter);
				} else {
					Debug.LogWarning($"Unable to load mask map preset for {assetPath}.");
				}
			}

			if (Path.GetFileNameWithoutExtension(assetPath).EndsWith("Normal")) {
				if (_normalMapPreset == null) {
					_normalMapPreset = AssetDatabase.LoadAssetAtPath<Preset>($"{presetPath}/NormalMap.preset");
				}
				if (_normalMapPreset) {
					_normalMapPreset.ApplyTo(assetImporter);
				} else {
					Debug.LogWarning($"Unable to load normal map preset for {assetPath}.");
				}
			}
		}
	}
}
