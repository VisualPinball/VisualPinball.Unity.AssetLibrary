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

namespace VisualPinball.Unity.AssetLibrary
{
	/// <summary>
	/// Communal storage for render pipeline-agnostic asset paths
	/// </summary>
	///
	/// <remarks>
	/// All path names don't have trailing slashes.
	/// </remarks>
	public class AssetPath
	{
		/// <summary>
		/// Root package folder for the shared asset library.
		/// </summary>
		private const string RootPath = "Packages/org.visualpinball.unity.assetlibrary";

		/// <summary>
		/// Root folder for the Assets.
		/// </summary>
		private const string AssetRootPath = RootPath + "/Assets";

		/// <summary>
		/// HDRI Environments folder path.
		/// </summary>
		public const string HdriEnvPath = AssetRootPath + "/Art/Textures/Environment/HDR";

		/// <summary>
		/// EditorGUI icons folder path.
		/// </summary>
		public const string IconPath = AssetRootPath + "EditorResources/Icons/EditorGUI";
	}
}