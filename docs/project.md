# Project

**Lost In Translation**: Diogo de Andrade's entry for the October'2026 Game Creators Club
(https://game-creators-club.itch.io/) at Lusofona University. Theme: **"Lost In Translation"**.

## Game

Design decisions live here (mechanics, controls, scope, decisions and why); the player-facing summary is in the
root README's `## Game` section and the task list in its `## Todo` section.

### Concept (decided 2026-10-24)

Inspired by *Lost in Translation* (2003), the scene where Bill Murray's character shoots a whisky commercial: the
director gives long, passionate directions, and the character doesn't understand any of it and feels the
translation he gets is not accurate.

The player is an actor being directed on a set. The director speaks gibberish, and the player has to guess what
the scene needs them to do, reading the director's body language.

### Mechanics

- **Movement**: the player moves in 3D, with a fixed 45 degree camera, as in SkinFactory (`../SkinFactory`).
- **Interaction**: get close to an object and press Interact; a menu shows the interactions that object offers,
  and the player picks one.
- **Director**: a 2D image in the bottom-left corner. He works as a hot/cold meter: he gets more or less angry as
  the player does the right things, and the player reads his body cues to work out what to do.
  - **Position cue**: how close the player is to where they should be. Updates continuously as the player moves.
  - **Orientation cue**: once close to that position, whether the player is aiming in the right direction to
    interact with the right object.
  - **"Life system"**: interactions that are wrong for that moment make him angrier.

### Scenes

Each level is one scene of a film, in its own Unity scene. The first is a scene from a zombie movie, shot on a
graveyard set (`Assets/Scenes/ZombieMovie.unity`), because most of its assets (Kenney Graveyard Kit) are already in.

The set is a 13 x 8 graveyard (x -6.5..6.5, z -4..4, the camera looks towards +z) on a dark studio floor: iron
fence and gate at the back with pines behind, a stone wall on the left, a broken wooden fence on the right, a crypt
(back left), two rows of graves either side of a stone path, an open grave with a shovel and a coffin at the front,
lightposts, pumpkins, two zombie extras idling, and two film spotlights at the front corners. Invisible walls
(`Studio/Boundary`) keep the actor on the set. Props have box colliders fitted to their meshes; grave mounds, the
path and other flat pieces have none. The layout was generated once by a script and is edited by hand from now on.

### Characters and scale

- The actor is Kenney Mini Characters' `character-male-d` (dark suit and tie, close to Bill Murray's look in the
  film's commercial scene).
- Mini Characters and the Graveyard Kit share a scale: characters are about 0.67 units tall, fences 0.9, pines 2.3.
  The world is built at that scale (no rescaling). Blocky Characters are about 4x bigger and don't match it.
- All three packs' characters are Generic rigs with the same clip set inside each FBX (`idle`, `walk`, `sprint`,
  `sit`, `pick-up`, `interact-left/right`, `emote-yes/no`, `die`, `holding-*`, ...). `idle`, `walk` and `sprint` were
  set to loop in the import settings of every Mini Characters and Graveyard Kit character (Kenney ships them as
  non-looping).
- Animation uses `PlayableHandler` (see [conventions.md](conventions.md)); the FBXs have no `Animator`, so one is
  added to each animated model.

### Player and camera

- `Player` root: `Rigidbody` (rotation frozen), `CapsuleCollider`, `PlayerInput` (`InputSystem_Actions`, `Player`
  map), `HypertaggedObject` (`Assets/Data/MainPlayer`), `MovementDirectionalXZ` (camera-relative, speed 2, turns to
  face the input), `Player` (cross-fades `idle`/`walk` through the model's `PlayableHandler`). The model is the
  `Model` child.
- Main Camera: `OrbitCameraController` following `MainPlayer` at a 45 degree pitch, no yaw, distance 10, FOV 40
  (about half the set in view); tagged `Assets/Data/MainCamera`.

### Open questions

- Beats within a scene: one target, or a sequence (stand there, face that, do this)?
- Both cues and the life system show as anger on the same face. How does the player tell "cold" (far from the
  mark, recovers as they move) from lost patience (wrong interactions, cumulative)?
- What happens when patience runs out (fired, retake, game over), and what finishing a scene looks like.
- How the gibberish is presented: speech balloons with nonsense text, a babble voice, an interpreter whose
  "translation" is too short to be useful (as in the film).
- Film crew dressing around the set (camera, light stands, director's chair): none of the Kenney packs have it.

### Working method

The user wants the game built one step at a time, confirming each step before the next. The order follows the
README's `## Todo` list.

### Code to reuse

- SkinFactory (`../SkinFactory/Assets/Scripts`): `OrbitCameraController` and `MovementDirectionalXZ` are already
  copied here (the movement one without its Animator parameters). Still to take: `SpeechBalloon` /
  `SpeechBalloonManager` / `BossBabble` and `GameSounds.BabbleCR` (a talking boss with speech balloons and voice blips).
- UnityCommon `UC.Interaction` (`Systems/Actions`): `Interactable` with an `InteractionVerb`, conditions and
  `GameAction`s. Several on one object could make up its option menu.

## Current state (2026-10-24)

- Unity 6.6 "Universal 3D" (URP) template, plus UnityCommon, NaughtyAttributes and `com.unity.pipeline` (Unity
  CLI control of the Editor). Compiles cleanly.
- Done: the zombie movie set, and the player walking around it with idle/walk animations and the follow camera
  (tested in Play mode). Next on the Todo list: interaction.
- Only scene: `Assets/Scenes/ZombieMovie.unity` (the template's `SampleScene`, renamed; also the only build-list
  entry). It keeps the template's Global Volume.
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
| `Assets/Scripts` | Game code: `Camera/` (`OrbitCameraController`), `Player/` (`Player`, `MovementDirectionalXZ`) |
| `Assets/Data` | Hypertags (`MainPlayer`, `MainCamera`) |
| `Assets/Materials` | `SetGround`, `StudioFloor` (URP Lit, flat colours) |
| `Assets/Settings` | URP assets (`PC_RPAsset`, `Mobile_RPAsset`), renderers, volume profiles, `InputSystem_Actions.inputactions` (template Input System actions) |
| `docs/` | This context |
| `README.md`, `LICENSE` | Player-facing description and credits; MIT license |

New content follows the other GCC projects: code in `Assets/Scripts/`, and sibling folders `Prefabs/`,
`Materials/`, `Audio/`, `Data/`, `Fonts/`, ... as needed.
