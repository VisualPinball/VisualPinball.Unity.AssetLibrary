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
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEditor;
using UnityEngine;
using VisualPinball.Unity.Editor;

namespace VisualPinball.Unity.Library
{
	public class ThumbGenerator : MonoBehaviour
	{
		[SerializeReference]
		public Editor.AssetLibrary AssetLibrary;

		public bool IsProcessing { get; private set; }
		public int TotalProcessing { get; private set; }
		public int NumProcessed { get; private set; }

		private List<AssetMaterialCombination> _assets;
		private GameObject _currentGo;
		private ThumbGeneratorComponent _currentTbc;
		private Camera _camera;

		private PlayfieldComponent _pf;
		private Vector3 _tableCenter;

		public void StartProcessing(bool newOnly = false, bool selectedOnly = false)
		{
			_camera = Camera.main;

			_pf = GetComponentInChildren<PlayfieldComponent>();
			_tableCenter = new Vector3(_pf.Width / 2f, _pf.Height / 2f,0);

			var category = AssetLibrary.GetCategories().FirstOrDefault(c => c.Name.Contains("Flipper"));
			//var category = AssetLibrary.GetCategories().FirstOrDefault(c => c.Name.Contains("Flipper"));
			if (category != null) {
				Debug.Log($"Category: {category}");
				var query = new LibraryQuery {
					//Keywords = "post -hex - 1.2"
					//Categories = new List<AssetCategory> { category }
				};
				var assets = AssetLibrary.GetAssets(query).ToArray();

				if (assets.Length == 0) {
					Debug.LogWarning("No assets found.");
					return;
				}

				_assets = new List<AssetMaterialCombination>(assets
					.SelectMany(a => AssetMaterialCombination.GetCombinations(a.Asset))
				);

				if (newOnly) {
					_assets = _assets.Where(a => !a.HasThumbnail).ToList();
				}
				if (selectedOnly) {
					var selectedAssets = new HashSet<Asset>(EditorWindow.GetWindow<AssetBrowser>().SelectedAssets);
					_assets = _assets.Where(a => selectedAssets.Contains(a.Asset)).ToList();
				}

				NumProcessed = 0;
				TotalProcessing = _assets.Count;
				if (_assets.Count > 0) {
					IsProcessing = true;
					Process(NextAsset());
				} else {
					Debug.Log("No assets found to process.");
				}

			} else {
				Debug.Log($"No category found.");
			}
		}

		public void StopProcessing()
		{
			_assets.Clear();
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
				IsProcessing = false;
			}
		}

		private AssetMaterialCombination NextAsset()
		{
			if (_assets.Count == 0) {
				return null;
			}
			var next = _assets.First();
			_assets.RemoveAt(0);
			NumProcessed++;
			return next;
		}
	}
}

