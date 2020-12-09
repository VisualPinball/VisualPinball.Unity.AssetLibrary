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

namespace VisualPinball.Unity.AssetLibrary.Hdrp
{
	/// <summary>
	/// Communal storage for render HDRP-specific asset paths
	/// </summary>
	///
	/// <remarks>
	/// All paths contain the entire path, so they don't need to be concatenated.
	/// </remarks>
	public class HdrpAssetPath : AssetPath
	{
		/// <summary>
		/// Root package folder for the shared asset library.
		/// </summary>
		private const string RootPath = "Packages/org.visualpinball.unity.assetlibrary.hdrp";

		/// <summary>
		/// HDRP Prefab path.
		/// </summary>
		private const string PrefabPath = RootPath + "/EditorResources/Prefabs";

		/// <summary>
		/// Light environment editor prefab.
		/// </summary>
		public const string LightingPrefab = PrefabPath + "/Lighting/EditorLighting.prefab";

		/// <summary>
		/// Camera prefab.
		/// </summary>
		public const string CameraPrefab = PrefabPath + "/Camera/EditorCamera.prefab";

		/// <summary>
		/// Post process prefab.
		/// </summary>
		public const string PostProcessPrefab = PrefabPath + "/PostProcess/EditorPostProcess.prefab";

		/// <summary>
		/// Blueprint projector prefab.
		/// </summary>
		public const string BlueprintPrefab = PrefabPath + "/Tools/BlueprintProjector.prefab";
	}
}