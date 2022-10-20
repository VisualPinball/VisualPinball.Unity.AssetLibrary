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
using System.IO;
using NetVips;
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

		public string ThumbnailGuid;

		private const int NumPreFrames = 8;
		private const int NumPostFrames = 4;
		private int _frame;

		public event EventHandler OnScreenshot;

		private void Start()
		{
			_frame = 0;
			TriggerRender();
		}

		private void OnRenderObject()
		{
			if (_frame == NumPreFrames) {
				Screenshot();
			}
			if (_frame == NumPreFrames + NumPostFrames - 1) {
				Resize(ThumbnailGuid);
			}
			if (_frame++ < NumPreFrames + NumPostFrames) {
				TriggerRender();
				return;
			}
			OnScreenshot?.Invoke(this, EventArgs.Empty);
		}

		private static void TriggerRender()
		{
			InternalEditorUtility.RepaintAllViews();
		}

		private void Screenshot()
		{
			if (!string.IsNullOrEmpty(ThumbnailGuid)) {
				Screenshot(ThumbnailGuid);

			} else if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(Prefab, out var guid, out long _)) {
				Screenshot(guid);

			} else {
				Debug.LogWarning($"Cannot find GUID for {Prefab.name}.");
			}
		}

		private void Screenshot(string guid)
		{
			try {
				var path = @$"{AssetBrowser.ThumbPath}/{guid}_large.png";
				ScreenCapture.CaptureScreenshot(path, 2);
				Debug.Log($"Screenshot for \"{Prefab.name}\" saved at {path}");

			} catch (Exception e) {
				Debug.LogError(e);
			}
		}

		private static void Resize(string guid)
		{
			const int r = 72;
			const int s = 1024;
			var src = Path.GetFullPath(@$"{AssetBrowser.ThumbPath}/{guid}_large.png");
			var dest = Path.GetFullPath(@$"{AssetBrowser.ThumbPath}/{guid}.png");
			var mask = Image.NewFromBuffer($"<svg viewBox=\"0 0 {s} {s}\"><rect x=\"0\" y=\"0\" width=\"{s}\" height=\"{s}\" rx=\"{r}\" ry=\"{r}\" fill=\"#fff\"/></svg>");
			using var large = Image.NewFromFile(src);
			using var rounded = large.Bandjoin(mask);
			using var resized = rounded.Resize(0.25);
			resized.WriteToFile(dest);
			File.Delete(src);
		}
	}
}
