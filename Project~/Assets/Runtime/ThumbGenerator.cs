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

// ReSharper disable InconsistentNaming

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using VisualPinball.Unity.Editor;

namespace VisualPinball.Unity.Library
{
	public class ThumbGenerator : MonoBehaviour
	{
		[SerializeReference]
		public Editor.AssetLibrary AssetLibrary;

		private List<AssetMaterialCombination> _assets;
		private GameObject _currentGo;
		private ThumbGeneratorComponent _currentTbc;
		private Camera _camera;

		private PlayfieldComponent _pf;
		private Vector3 _tableCenter;

		public void StartProcessing()
		{
			_camera = Camera.main;

			_pf = GetComponentInChildren<PlayfieldComponent>();
			_tableCenter = new Vector3(_pf.Width / 2f, _pf.Height / 2f,0);

			var category = AssetLibrary.GetCategories().FirstOrDefault(c => c.Name.Contains("Flipper"));
			//var category = AssetLibrary.GetCategories().FirstOrDefault(c => c.Name.Contains("Flipper"));
			if (category != null) {
				Debug.Log($"Category: {category}");
				var query = new LibraryQuery {
					Categories = new List<AssetCategory> { category }
				};
				var assets = AssetLibrary.GetAssets(query).ToArray();

				if (assets.Length == 0) {
					Debug.LogWarning("No assets found.");
					return;
				}

				_assets = new List<AssetMaterialCombination>(assets
					.SelectMany(a => AssetMaterialCombination.GetCombinations(a.Asset))
				);
				Process(NextAsset());

			} else {
				Debug.Log($"No category found.");
			}
		}

		private void Process(AssetMaterialCombination a)
		{

			if (a.Asset.ThumbCameraPreset != null) {
				a.Asset.ThumbCameraPreset.ApplyTo(_camera.transform);
			} else {
				AssetLibrary.DefaultThumbCameraPreset.ApplyTo(_camera.transform);
			}

			if (a.Asset.Scale == AssetScale.Table) {
				var parent = _pf.gameObject;
				_currentGo = PrefabUtility.InstantiatePrefab(a.Asset.Object, parent.transform) as GameObject;
				_currentGo!.transform.localPosition = _tableCenter;

			} else {
				_currentGo = PrefabUtility.InstantiatePrefab(a.Asset.Object) as GameObject;
			}

			a.Apply(_currentGo);

			Debug.Log($"Processing {_currentGo!.name}");
			_currentTbc = _currentGo!.AddComponent<ThumbGeneratorComponent>();
			_currentTbc!.ThumbnailGuid = a.ThumbId;
			_currentTbc!.Prefab = a.Asset.Object;
			_currentTbc!.OnScreenshot += DoneProcessing;
		}

		private void DoneProcessing(object sender, EventArgs e)
		{
			_currentTbc!.OnScreenshot -= DoneProcessing;
			DestroyImmediate(_currentGo);

			var next = NextAsset();
			if (next != null) {
				Process(next);
			} else {
				AssetLibrary.DefaultThumbCameraPreset.ApplyTo(_camera.transform);
				Debug.Log("All done!");
			}
		}

		private AssetMaterialCombination NextAsset()
		{
			if (_assets.Count == 0) {
				return null;
			}
			var next = _assets.First();
			_assets.RemoveAt(0);
			return next;
		}
	}
}

