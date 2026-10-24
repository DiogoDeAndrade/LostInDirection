# Project

**Lost In Translation** is Diogo de Andrade's entry for the October'2026 [Game Creators Club](https://game-creators-club.itch.io/)
at Lusofona University. This month's theme is **"Lost In Translation"**.

## Game

Not designed yet. When the concept is decided, describe it here (and in the `## Game` section of the root README),
then keep the running task list in the README's `## Todo` section.

## Current state

- Fresh Unity 6.6 project from the "Universal 3D" (URP) template.
- Only scene: `Assets/Scenes/SampleScene.unity`, which is also the only entry in the build scene list.
- `Assets/TutorialInfo/` and `Assets/Readme.asset` are template leftovers and can be deleted once real content exists.
- `companyName` in Player Settings is still `DefaultCompany`.

## Repository layout

| Path | What it is |
|---|---|
| `Assets/Externals/UnityCommon` | Git submodule with the shared UnityCommon library (see [setup.md](setup.md)) |
| `Assets/Scenes` | Scenes |
| `Assets/Settings` | URP pipeline assets (`PC_RPAsset`, `Mobile_RPAsset`), renderers and volume profiles |
| `Assets/InputSystem_Actions.inputactions` | Default Input System actions from the template |
| `docs/` | These notes |
| `README.md`, `LICENSE` | Player-facing description and credits; MIT license |

Game code should go in `Assets/Scripts/`, with the usual sibling folders as content appears
(`Prefabs/`, `Materials/`, `Audio/`, `Data/`, `Fonts/`, ...), following the other GCC projects.
