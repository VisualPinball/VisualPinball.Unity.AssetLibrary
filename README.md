# VisualPinball AssetLibrary

This repository contains the asset library for the [Visual Pinball Engine](https://github.com/freezy/VisualPinball.Engine).

More to come.

## Setup

You'll need [git LFS](https://git-lfs.github.com/) in order to use this repo. [Install the extension](https://github.com/git-lfs/git-lfs/releases/download/v2.12.0/git-lfs-windows-v2.12.0.exe) if you don't already have. Then enable it before cloning the repo.

```sh
git lfs install
git clone https://github.com/freezy/VisualPinball.AssetLibrary.git
```

## Packages

*This section will move to somewhere more global.*

VPE comes in a bunch of Unity packages. There are two reasons there are so many of them:

1. HDRP and URP are incompatible.
2. We want to separate assets from the code in order to keep the code light.

We haven't got a player project yet. So there are even more packages to come! Anyway, because package names are long
and there are subtle differences, here's how will be referring to them:

|                         | Package Name                              | Repository                            |
|-------------------------|-------------------------------------------|---------------------------------------|
| vpe.unity (also "main") | org.visualpinball.engine.unity            | [VisualPinball.Engine](https://github.com/freezy/VisualPinball.Engine)                  |
| vpe.unity.hdrp          | org.visualpinball.engine.unity.hdrp       | [VisualPinball.Unity.Hdrp](https://github.com/freezy/VisualPinball.Unity.Hdrp)              |
| vpe.unity.assets        | org.visualpinball.unity.assetlibrary      | [VisualPinball.Unity.AssetLibrary](https://github.com/freezy/VisualPinball.AssetLibrary/tree/refactor/repos/VisualPinball.Unity.AssetLibrary)      |
| vpe.unity.assets.hdrp   | org.visualpinball.unity.assetlibrary.hdrp | [VisualPinball.Unity.AssetLibrary.Hdrp](https://github.com/freezy/VisualPinball.AssetLibrary/tree/refactor/repos/VisualPinball.Unity.AssetLibrary.Hdrp) |
| unity.hdrp              | com.unity.render-pipelines.high-definition | [Unity/Graphics](https://github.com/Unity-Technologies/Graphics/tree/master/com.unity.render-pipelines.high-definition) |


- **vpe.unity** is the main repo. It contains mainly the code, and some resources, which will 
  probably move into vpe.unity.assets at some point. It doesn't have any dependencies, neither
  to any of the other vpe packages nor to Unity's HDRP or URP. If only this repo is used in a 
  project, the standard renderpipeline is used.
- **vpe.unity.hdrp** contains HDRP-specific code.
- **vpe.unity.assets** contains common assets only, i.e. asses that are render pipeline-agnostic.
- **vpe.unity.assets.hdrp** contains HDRP-specific assets.

### Dependencies

We have the following dependencies:

- vpe.unity - *none*
- vpe.unity.hdrp -> unity.hdrp
- vpe.unity.hdrp -> vpe.unity
- vpe.unity.hdrp -> vpe.unity.assets.hdrp
- vpe.unity.assets.hdrp -> vpe.unity.assets
- vpe.unity.assets -> *none*

So we see that vpe.unity.hdrp pulls in three other dependencies. These have to be added manually at the moment. So when
working with HDRP, you would have the following packages under *Custom* in the package manager:

- Visual Pinball Engine
- Visual Pinball Engine (Common Assets)
- Visual Pinball Engine (HDRP Assets)
- Visual Pinball Engine (HDRP)


## License

[Creative Commons Attribution-ShareAlike 4.0](LICENSE.md)