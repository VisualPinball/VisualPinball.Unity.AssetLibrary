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

		private List<AssetResult> _assets;
		private GameObject _currentGo;
		private ThumbGeneratorComponent _currentTbc;

		public void StartProcessing()
		{
			var category = AssetLibrary.GetCategories().FirstOrDefault(c => c.Name.Contains("Posts"));
			if (category != null) {
				Debug.Log($"Category: {category}");
				var query = new LibraryQuery {
					Categories = new List<LibraryCategory> { category }
				};
				var assets = AssetLibrary.GetAssets(query).ToArray();

				if (assets.Length == 0) {
					Debug.LogWarning("No assets found.");
				}

				_assets = new List<AssetResult>(assets);
				Process(NextResult().Asset);

			} else {
				Debug.Log($"No category found.");
			}
		}

		private void Process(LibraryAsset asset)
		{
			var pf = GetComponentInChildren<PlayfieldComponent>();
			var parent = pf.gameObject;

			_currentGo = PrefabUtility.InstantiatePrefab(asset.Object, parent.transform) as GameObject;
			_currentTbc = _currentGo!.AddComponent<ThumbGeneratorComponent>();
			_currentGo!.transform.localPosition = new Vector3(pf.Width / 2f, pf.Height / 2f,0);
			_currentTbc!.Prefab = asset.Object;
			_currentTbc!.OnScreenshot += DoneProcessing;
		}

		private void DoneProcessing(object sender, EventArgs e)
		{
			_currentTbc!.OnScreenshot -= DoneProcessing;
			DestroyImmediate(_currentGo);

			var next = NextResult();
			if (next != null) {
				Process(next.Asset);
			} else {
				Debug.Log("All done!");
			}
		}

		private AssetResult NextResult()
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

