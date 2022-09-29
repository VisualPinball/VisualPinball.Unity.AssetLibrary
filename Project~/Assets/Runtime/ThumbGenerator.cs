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

		private List<AssetWithVariation> _assets;
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


				_assets = new List<AssetWithVariation>(assets
					.SelectMany(a => {
						var variations = new List<AssetWithVariation>(new[] { new AssetWithVariation(a.Asset) });
						if (a.Asset.MaterialVariations == null || a.Asset.MaterialVariations.Count == 0) {
							return variations;
						}

						foreach (var variation in a.Asset.MaterialVariations) {
							foreach (var variationOverride in variation.Overrides) {
								variations.Add(new AssetWithVariation(a.Asset, variation, variationOverride));
							}
						}
						return variations;
					})
				);
				Process(NextAsset());

			} else {
				Debug.Log($"No category found.");
			}
		}

		private void Process(AssetWithVariation a)
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

			if (a.HasVariation) {
				var obj = _currentGo!.transform.Find(a.MaterialVariation.Object.name);
				var materials = obj.gameObject.GetComponent<MeshRenderer>().sharedMaterials;
				materials[a.MaterialVariation.Slot] = a.MaterialOverride.Material;
				obj.gameObject.GetComponent<MeshRenderer>().sharedMaterials = materials;
			}

			Debug.Log($"Processing {_currentGo!.name}");
			_currentTbc = _currentGo!.AddComponent<ThumbGeneratorComponent>();
			if (a.HasVariation) {
				_currentTbc!.ThumbnailGuid = a.MaterialOverride.Id;
			}
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

		private AssetWithVariation NextAsset()
		{
			if (_assets.Count == 0) {
				return null;
			}
			var next = _assets.First();
			_assets.RemoveAt(0);
			return next;
		}
	}

	internal class AssetWithVariation
	{
		public readonly Asset Asset;
		public readonly AssetMaterialVariation MaterialVariation;
		public readonly AssetMaterialOverride MaterialOverride;

		public bool HasVariation => MaterialVariation != null && MaterialOverride != null;

		public AssetWithVariation(Asset asset)
		{
			Asset = asset;
		}

		public AssetWithVariation(Asset asset, AssetMaterialVariation materialVariation, AssetMaterialOverride materialOverride)
		{
			Asset = asset;
			MaterialVariation = materialVariation;
			MaterialOverride = materialOverride;
		}
	}
}

