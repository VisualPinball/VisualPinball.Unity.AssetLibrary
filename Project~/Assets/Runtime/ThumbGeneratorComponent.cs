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

using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using VisualPinball.Unity.Editor;
using Object = UnityEngine.Object;

namespace VisualPinball.Unity.Library
{
	[ExecuteInEditMode]
	public class ThumbGeneratorComponent : MonoBehaviour
	{
		[NonSerialized]
		public Object Prefab;

		private const int NumPreFrames = 32;
		private const int NumPostFrames = 2;
		private int _frame;

		public event EventHandler OnScreenshot;

		private void Start()
		{
			_frame = NumPreFrames + NumPostFrames;
			InternalEditorUtility.RepaintAllViews();
		}

		private void OnRenderObject()
		{
			if (_frame == NumPreFrames) {
				Screenshot();
			}
			if (_frame-- > 0) {
				InternalEditorUtility.RepaintAllViews();
				return;
			}
			OnScreenshot?.Invoke(this, EventArgs.Empty);
		}

		private void Screenshot()
		{
			if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(Prefab, out var guid, out long _)) {

				var path = @$"{AssetBrowser.ThumbPath}/{guid}.png";
				ScreenCapture.CaptureScreenshot(path);
				Debug.Log($"Screenshot for \"{Prefab.name}\" saved at {path}");

			} else {
				Debug.LogWarning($"Cannot find GUID for {Prefab.name}.");
			}
		}
	}
}
