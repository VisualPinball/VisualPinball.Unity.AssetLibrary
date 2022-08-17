# Assets for Visual Pinball Engine

*Common assets for all render pipelines.*

## Setup

You'll need [git LFS](https://git-lfs.github.com/) in order to use this repo. [Install the extension](https://github.com/git-lfs/git-lfs/releases/download/v2.12.0/git-lfs-windows-v2.12.0.exe) if you don't already have. Then enable it before cloning the repo.

```sh
git lfs install
git clone https://github.com/VisualPinball/VisualPinball.Unity.AssetLibrary.git
```

## Overview

We're trying to be in sync with vbousquet's [pinball-parts](https://github.com/vbousquet/pinball-parts) repo. To facilitate this, we're splitting the library items the same way, so you'll find the same Blender files in this repo. However, since we also bake down details into normals, we don't use pinball-parts' original files, but our own. 

These Blender files are our "source of truth", and are located in the `src~` folder in each directory of the assets. They are, however, not what ends up being imported into Unity. For Unity, we're exporting the Blender files to FBX files, typically into even smaller chunks, to avoid having too much overhead.

### Baking

Most of the Blender files have low-poly geometry which is then enhanced with modifiers, typically bevels. In Unity, we use the low-poly geometry. In order to export to FBX, we export without modifiers, another time with modifiers applied to a hi-poly FBX, and bake the hi-poly geometry into the normal map. However, sometimes, there are more complicated differences like screw threads, in which case we keep two versions (low-poly and hi-poly) in the Blender file, and toggle each version when exporting.

### Materials

Most of the materials have been created in Substance Painter, which we also use for baking the normal map as well as a thickness map where needed. The resulting textures are kept in the same folder as the meshes, along with the Unity material and prefab.

## Workflow

How to add or update items can vary depending on the asset, and will be specified in the next section. The principles are always the same:

- One Blender file may result in multiple exports. For example, `Plastic Posts.blend` exports to *Plastic Posts* and *Plastic Spacers*. The different exports are grouped by categories in the Blender file.
- Items of an FBX file all share the same material. There may be several materials because there are meshes with multiple material slots, but one material type is applied to all the items in the FBX with that material type.
- If a Blender file has complex geometry that is baked down that cannot be implemented purely with modifiers, there will be additional categories with low-poly and hi-poly meshes. These need to be toggled when exporting.

With that out of the way, here is a typical workflow for adding or updating items.

1. Update or add the Blender file with the asset. If it's an asset that doesn't exist in `pinball-parts` yet, contact vbousquet or create a pull request in [his repo](https://github.com/vbousquet/pinball-parts) to add it there as well.
2. Select the low-poly items of the category you're going to export. Since they all use the same material, UV-unwrap them all together.
3. If the file has separate categories for low-poly and high-poly, hide the hi-poly category. With the items selected, export to FBX, and be sure to check *Limit to: Selected Objects* and uncheck *Apply Modifers* under *Geometry*. The exported file should be in the asset folder, not in `src~`.
4. Export the selected items as FBX again, this time with *Apply Modifiers* enabled, if available with the low-poly category hidden and the hi-poly category visible. The exported file should be in the `src~` folder, suffixed with ` - Hipoly`.
5. Open Substance Painter, import the low-poly FBX (or open your existing SSP file and update the project configuration). Bake the mesh maps, using the hi-poly FBX under *high definition meshes*. Do the texturing.
6. Export the textures into the asset folder. Sometimes, you'll additionally need to export the mesh maps, too.
7. If you've just updated an item, you're done. If you have added an item, open Unity and drag the low-poly FBX into the scene.
8. Unpack the prefab in the scene and assign the material to the new item. If you've created a new FBX and there is no material yet, create a new one named after your FBX and assign it the exported texture maps.
9. Drag the new item from the scene into your asset folder to create a new prefab.
10. Set the position of the prefab to (0,0,0) and the X-rotation to -90°.
11. Delete the game object you've added from the scene.
12. Add the prefab to the asset library and add the meta data (we will document how to apply meta data soon).
13. Commit and push the new or updated files in git.

## Library

The library assets can be found at [Assets/Library](Assets/Library). We'll go through each of the curated assets with a short description about how to deal with them.

### Posts

- `Plastic Posts.blend` contains both *Plastic Posts* and *Plastic Spacers*.
- `Metal Posts.blend` contains *Metal Posts*, *Metal Hex Spacers* and *Metal Round Spacers*.

#### Plastic Posts

- **Source**: [`Posts/src~/Plastic Posts.blend`](Assets/Library/Posts/src%7E/Plastic%20Posts.blend)
- **Reference**: [`/Posts/Plastic Posts.blend`](https://github.com/vbousquet/pinball-parts/blob/main/Posts/Plastic%20Posts.blend)
- **Asset Folder**: [`Posts/Plastic Posts`](Assets/Library/Posts/Plastic%20Posts)
- **Export**: Select items in the *Plastic Posts* category. The only difference between hi-poly and low-poly are the modifiers.
- **Material**: We don't use an albedo texture for the plastic, because it should be easily customizable. The other maps aren't very important, but *edgeglowplastic* seems to work well.
- **Notes**: There are two materials, *Plastic - Peg Red* and *Metal - Peg*, which need to be renamed to *Plastic* and *Metal* respectively. Only the plastic needs any exported textures, the metal is generic. The plastic however needs a **thickness map** as well.

#### Plastic Spacers

- **Source**: [`Posts/src~/Plastic Posts.blend`](Assets/Library/Posts/src%7E/Plastic%20Posts.blend)
- **Reference**: [`/Posts/Plastic Posts.blend`](https://github.com/vbousquet/pinball-parts/blob/main/Posts/Plastic%20Posts.blend)
- **Asset Folder**: [`Posts/Plastic Spacers`](Assets/Library/Posts/Plastic%20Spacers)
- **Export**: Select items in the *Plastic Spacers* category. The only difference between hi-poly and low-poly are the modifiers.
- **Material**: We don't use an albedo texture for the plastic, because it should be easily customizable. The other maps aren't very important, but *edgeglowplastic* seems to work well.
- **Notes**: There are two materials, *Plastic - Peg Red* and *Metal - Peg*, which need to be renamed to *Plastic* and *Metal* respectively. Only the plastic needs any exported textures, the metal is generic. The plastic however needs a **thickness map** as well.


#### Metal Posts

- **Source**: [`Posts/src~/Metal Posts.blend`](Assets/Library/Posts/src%7E/Metal%20Posts.blend)
- **Reference**: [`/Posts/Metal Posts.blend`](https://github.com/vbousquet/pinball-parts/blob/main/Posts/Metal%20Posts.blend)
- **Asset Folder**: [`Posts/Metal Posts`](Assets/Library/Posts/Metal%20Posts)
- **Export**: Select items in the *Metal Posts* category. Apart from the modifiers, we have a low-poly and hi-poly category that need to be toggled when exporting.
- **Material**: We currently use *Steel Stained* with *Luminosity* of *Steel Base* set to `0.55`.
- **Notes**: There are two materials, *Plastic - Sleeve Yellow* and *Metal - Peg*, which need to be renamed to *Plastic* and *Metal* respectively. Both materials are exported to textures.


#### Metal Hex Spacers

- **Source**: [`Posts/src~/Metal Posts.blend`](Assets/Library/Posts/src%7E/Metal%20Posts.blend)
- **Reference**: [`/Posts/Metal Posts.blend`](https://github.com/vbousquet/pinball-parts/blob/main/Posts/Metal%20Posts.blend)
- **Asset Folder**: [`Posts/Metal Hex Spacers`](Assets/Library/Posts/Metal%20Hex%20Spacers)
- **Export**: Select items in the *Hex Spacers* category. Toggling modifiers is enough.
- **Material**: We currently use *Steel Stained* with *Luminosity* of *Steel Base* set to `0.55`.
- **Notes**: Only one material - straight forward.


#### Metal Round Spacers

- **Source**: [`Posts/src~/Metal Posts.blend`](Assets/Library/Posts/src%7E/Metal%20Posts.blend)
- **Reference**: [`/Posts/Metal Posts.blend`](https://github.com/vbousquet/pinball-parts/blob/main/Posts/Metal%20Posts.blend)
- **Asset Folder**: [`Posts/Metal Round Spacers`](Assets/Library/Posts/Metal%20Hex%20Spacers)
- **Export**: Select items in the *Round Spacers* category. Toggling modifiers is enough.
- **Material**: We currently use *Steel Stained* with *Luminosity* of *Steel Base* set to `0.55`.
- **Notes**: Only one material - straight forward.

## Dependency Graph

Both the HDRP and URP packages have a dependency to this, however the main project maintains its own assets for now.

![image](https://user-images.githubusercontent.com/70426/103706934-feefd880-4fad-11eb-95c3-820ec6738076.png)


## Credits

Most of the assets in this library are derivations of [pinball-parts](https://github.com/vbousquet/pinball-parts). We use the low-poly geometry of these assets, and bake them into normal maps.

## License

[Creative Commons Attribution-ShareAlike 4.0](LICENSE.md)
