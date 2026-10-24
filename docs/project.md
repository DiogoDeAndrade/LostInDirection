# Project

**Lost In Translation**: Diogo de Andrade's entry for the October'2026 Game Creators Club
(https://game-creators-club.itch.io/) at Lusofona University. Theme: **"Lost In Translation"**.

## Game

No concept decided yet. Once it is, record the design here (mechanics, controls, scope, decisions and why), and
keep the player-facing summary in the root README's `## Game` section and the task list in its `## Todo` section.

## Current state (2026-10-24)

- Fresh project from the Unity 6.6 "Universal 3D" (URP) template, plus UnityCommon, NaughtyAttributes and
  `com.unity.pipeline` (Unity CLI control of the Editor). Compiles cleanly; no game code yet.
- Only scene: `Assets/Scenes/SampleScene.unity` (also the only build-list entry).
- Art so far: three Kenney 3D packs (see Layout), added by the user; third-party assets go under
  `Assets/Externals/<Source>/<Pack>/`, each with a README credit line.
- Template tutorial content (`Assets/TutorialInfo/`, `Assets/Readme.asset`) removed by the user.
- Player Settings `companyName` is still `DefaultCompany`.

## Layout

| Path | What it is |
|---|---|
| `Assets/Externals/UnityCommon` | Git submodule, shared library (see [setup.md](setup.md)) |
| `Assets/Externals/Kenney.NL` | Kenney CC0 packs, FBX format: `BlockyCharacters`, `GraveyardKit`, `MiniCharacters` (credited in the README) |
| `Assets/Scenes` | Scenes |
| `Assets/Settings` | URP assets (`PC_RPAsset`, `Mobile_RPAsset`), renderers, volume profiles, `InputSystem_Actions.inputactions` (template Input System actions) |
| `docs/` | This context |
| `README.md`, `LICENSE` | Player-facing description and credits; MIT license |

New content follows the other GCC projects: code in `Assets/Scripts/`, and sibling folders `Prefabs/`,
`Materials/`, `Audio/`, `Data/`, `Fonts/`, ... as needed.
