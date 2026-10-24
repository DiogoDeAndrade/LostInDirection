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

The zombie scene's script ("the script of the movie", from the user, 2026-10-24):

1. The actor comes in at the top entrance (the gate).
2. Goes to the shovel and picks it up.
3. With the shovel, "Dig" becomes available on the grave next to the shovel. Digging opens the grave and a
   zombie comes out.
4. The zombie follows the actor, who has to run and get to the crypt.
5. Touching any zombie gets a "Cut!" from the director, and the take restarts.
6. Doing anything other than this makes the director angry.

Built so far: steps 2 and 3, and the zombie following. The grave is the `grave-border` ring next to the shovel,
with a `Mound` child (the Kenney `grave` model) that is removed by digging, leaving the open ring, a `DigDirt`
particle burst, and a `ZombieSpawn` child (hypertag `Assets/Data/ZombieSpawn`, placed by the user) where
`Assets/Prefabs/Zombie.prefab` appears, with the spawn point's rotation.

Digging is a cutscene (the user's sequence), all actions on the Mound's "Dig" verb:
Start Cutscene (camera `CutsceneCamera`) - Wait 0.5 - Play Animation (actor, `interact-right`) - Play Particle
System (`DigDirt`) - Wait 1 - Destroy (Mound) - Wait 0.5 - Instantiate at Hypertag (Zombie at ZombieSpawn) -
Wait 1 - End Cutscene (waited) - Wait 1 - Zombie Follow (hypertag `Zombie`). Measured in Play mode: cutscene on at
once, the mound gone at 1.5 s, the zombie out at 2 s, controls back at about 3.5 s, the zombie chasing from 4.5 s.
`CutsceneCamera` (disabled, a copy of the main camera's settings with post-processing) looks at the grave from the
left side, so the actor's face, the grave and the spawn point are in frame.

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
- `CharacterAnimation` (`Assets/Scripts/Characters`) on the actor and the zombie prefab: idle/walk from the
  character's ground speed (a NavMeshAgent's velocity, else a non-kinematic Rigidbody's, else how far it moved),
  cross-faded; `PlayOneShot(clip)` (the Play Animation action) takes over until the clip's completion event, then
  idle/walk resumes.
- `Zombie` (prefab root, with a kinematic Rigidbody, a `NavMeshAgent` (radius 0.18, height 0.8, speed 1.4 against
  the actor's 2, no auto braking) and the `Zombie` hypertag): stands until follow mode is switched on, then
  re-paths to `MainPlayer` every 0.25 s. The two zombie extras on the set are plain models, not `Zombie`s.
- NavMesh: `NavMeshSurface` on `Set` (children, physics colliders), baked to `Assets/Scenes/ZombieMovie/NavMesh-Set.asset`.
  The default agent type (Humanoid, the only one) was resized for the Kenney scale: radius 0.18, height 0.8,
  climb 0.2, slope 45. The shovel has a `NavMeshModifier` ignoring it, since it's picked up. Rebake after moving
  props (NavMeshSurface's Bake button).

### Player and camera

- `Player` root: `Rigidbody` (rotation frozen), `CapsuleCollider`, `PlayerInput` (`InputSystem_Actions`, `Player`
  map), `HypertaggedObject` (`Assets/Data/MainPlayer`), `MovementDirectionalXZ` (camera-relative, speed 2, turns to
  face the input), `Player` (interaction, control lock), `CharacterAnimation`. The model is the `Model` child.
- `Player.LockControls` (driven by `Cutscene.isActive` every frame) switches off movement, aiming and interaction,
  and drops the current target.
- The capsule uses `Assets/Materials/NoFriction.physicMaterial` (zero friction, combine Minimum). Movement sets the
  velocity directly, so with friction the actor stuck to whatever it pushed against; without it, it slides along
  walls at the tangential part of its speed (moving at 45 degrees into the back fence slides at speed x cos 45).
- Main Camera: `UC.OrbitCameraController` (moved to UnityCommon on 2026-10-24) following `MainPlayer` at a
  45 degree pitch, no yaw, distance 5 (the user's value), FOV 40; tagged `Assets/Data/MainCamera`. Limit mode
  `Position`: the camera position is clamped to the disabled `BoxCollider` on `Studio/CameraLimits`
  (x -4.07..3.74, z -6.2..-0.8, tuned by the user), which keeps the actor in frame in every corner of the set.
  Changing the camera distance or angle means resizing that box.
- Aiming: while there's no move input, `MovementDirectionalXZ.aimInput` (the `Aim` action, gamepad right stick)
  turns the actor on the spot, camera-relative, at the normal turn speed. Keyboard players turn by moving.

### Interaction

- `Interactable` (`Assets/Scripts/Interaction`) on any object the actor can interact with: a display name (the
  object's name if empty) and a prompt anchor offset. Its verbs are the `Verb`s on the object and its children,
  found on Start (no explicit references, the user's call), in hierarchy order, which is the order the prompt
  cycles through them. It requires a
  `UC.ScreenSpaceOutline` (warm yellow, 3 px), kept disabled except while it's the target. `Interact` logs
  `"<actor>: <verb> <name>"` and performs the verb.
- `Verb` (a component; an object can have several): the display name, `[SerializeReference]` lists of UnityCommon
  `UC.Interaction.Condition`s and `GameAction`s. Offered only while all its conditions hold; an interactable with no
  available verb isn't a target at all. Its inspector (`Assets/Scripts/Editor/VerbEditor.cs`) uses UnityCommon's
  `ManagedReferenceListHelper`, so both lists have an "Add" menu of every condition/action type.
- Actions run through `GameAction.RunActions` on the actor: `Player` derives from `ActionRunner`, so an action that
  destroys the verb's own object doesn't stop the rest. The context has the actor as `triggerGameObject` and the
  interactable as `targetGameObject`.
- Game actions and conditions (`Assets/Scripts/Interaction/Actions`, `.../Conditions`; generic enough for UnityCommon
  if the user wants them there): `Inventory/Add Item` (a `UC.Item` into the actor's `UC.Inventory`),
  `Object/Destroy` (an `ObjectTarget`: the interactable, the actor, a specific object or the first with a hypertag),
  `Object/Instantiate at Hypertag` (a prefab at the first object with a hypertag, optionally with its rotation),
  `Cutscene/Start` (optional camera) and `Cutscene/End`, `Flow/Wait` (created with "wait" on, otherwise it does
  nothing), `Animation/Play` (an `ObjectTarget`, the actor by default; a one-shot through `CharacterAnimation`, or
  straight on a `PlayableHandler`), `FX/Play Particle System` (the first one under an `ObjectTarget`; waiting lasts
  until its particles die), `Zombie/Follow` (on/off, the zombie found through an `ObjectTarget`, by hypertag by
  default), and the condition `Inventory/Has Item`. UnityCommon's own `RPG/Add Item` and `RPG/Destroy Entity` work
  on RPG entities, which this game doesn't use.
- "Wait" on an action (UnityCommon's `GameAction.wait`, the checkbox on the right of each action) makes the list
  wait for it to finish; without it the action runs alongside the next ones.
- Items live in `Assets/Data/Items` (`Shovel`). The player has a `UC.Inventory` (UI: see Inventory below).
- Targeting is in `Player`: among enabled interactables, those whose collider's closest point is within
  `interactRange` (0.6) on the ground plane and within `interactAngle` (45 degrees) of the actor's facing, and that
  have an available verb; the one closest to straight ahead wins. The available verbs are rebuilt every frame
  (conditions change while targeting); the selection follows the selected verb, or goes back to the first one.
- Inputs (Player map of `InputSystem_Actions`): `Interact` performs the selected verb (gamepad west / E; the
  template's Hold interaction removed, it used to be on north), `NextOption` cycles the verbs (gamepad north / Q),
  `Aim` turns the actor (right stick). The template's `Look` isn't used for aiming because it's also bound to the
  mouse delta.
- Prompt: the `HUD` canvas (screen space overlay, scaled from 1920x1080) holds `InteractionPrompt`, a dark panel
  placed to the right of the target's anchor on screen and kept inside the canvas. It shows the name, the verbs
  (the selected one in yellow, prefixed with `»`) and a hint for the active control scheme (`(Y) next (X) do it` on
  gamepad, `[Q] next [E] do it` otherwise).
- Outlines need `UC.ScreenSpaceOutlineFeature` on the URP renderer: both `PC_Renderer` and `Mobile_Renderer` have it.
- Set up: the shovel (`shovel-dirt`) has "Pick up" (Add Item Shovel, Destroy the interactable) and "Lean on" (no
  actions, a wrong choice for the director); the grave ring has "Dig" on its `Mound` child (Has Item Shovel; the
  cutscene sequence in Scenes above), so after digging the grave has no verbs left.
- The project enters Play mode without reloading the domain or the scene (Enter Play Mode Options). Statics
  survive between plays, and a component added from an editor script keeps its in-memory field values: a
  `UC.Inventory` added that way had a null `initialItems` (UnityCommon doesn't null-check it) until the field was
  set through a `SerializedObject`.

### Inventory

The user built the inventory UI: `HUD/Inventory` has a `UC.InventoryDisplay` (slots from
`Assets/Prefabs/UI_InventoryItem.prefab`) and the `Assets/Data/InventoryDisplay` hypertag; `Player.Start` finds it
through `Player.uiInventoryDisplay` (that hypertag) and gives it the actor's `UC.Inventory`. Item icons are the
items' `displaySprite`: `Assets/Sprites/Items/Shovel.png` (256x256, transparent) was rendered from Kenney's `shovel`
model with an orthographic camera (key and fill lights on a layer of its own, no post-processing).

### Cutscene

`Cutscene` (`Assets/Scripts/Cutscene`, on the root `Cutscene` canvas, sorting order 10 above the `HUD`):
`Begin(camera)` fades out the CanvasGroups in `fadeDuringCutscene` (the `HUD`'s, which holds the prompt and the
inventory; UnityCommon's `CanvasGroup.FadeOut`), enables the given camera and disables the main one, and slides
black bars in at the top and bottom (12% of the screen height each, 0.5 s, smoothstep). `End` switches the camera
back, fades the UI in and slides the bars out; `isActive` stays true until then, so the actor gets control back
when the bars are gone. Disabling the main camera's Camera component (not its GameObject) keeps its AudioListener
alive. The bars are `Cutscene/TopBar` and `Cutscene/BottomBar` (black Images), collapsed to zero height at the
screen edges outside cutscenes; the component's "Show bars (edit)" / "Hide bars (edit)" buttons open and close
them in the editor.

### Atmosphere

The user darkened the lighting (flat ambient around 0.03, directional light 0.05), so the set is lit by candles,
fire baskets and the lamps.

- `UC.LightFlicker` (written here, moved to UnityCommon's `Animation/` on 2026-10-24) on a Light: its color follows a
  gradient that loops every `duration` seconds, the gradient's alpha scaling the light's own intensity, and its
  position wanders around its start with Perlin noise (`frequency`, per-axis `amplitude`). Random start per light,
  so they don't flicker in step.
- `Assets/Prefabs/Candles.prefab` (the user's: Kenney `candle-multiple` and a point light): a small flame particle
  system on each of the three wicks (local space, about 12 particles each) and `LightFlicker` (warm, 0.8 s cycle).
  The wick tops in the mesh are at (-0.064, 0.120, 0.078), (-0.067, 0.170, -0.068), (0.049, 0.200, 0.019).
- `Assets/Prefabs/FireBasket.prefab` (the user's, with a purple point light): a purple fire emitted above the rim
  (world space, up to 60 particles, noise) and `LightFlicker` (purple, 0.6 s cycle, larger wander).
- Flames use `Assets/Materials/FlameParticles.mat`: URP's `ParticlesUnlit` copied and made additive, with an HDR
  tint so bloom picks it up.
- On the set: 8 candles (beside gravestones, by the coffin and the crypt, beside the path) and 5 fire baskets (left
  wall, both back corners, flanking the front of the path), under `Set/Props`. The NavMesh was rebaked after
  placing them.

### Font

UI font: Barlow Condensed Black (Google Fonts, SIL OFL; the same file SkinFactory uses), as a dynamic TMP font asset
in `Assets/Fonts`. Chequered Ink's "World" fonts were tried first and rejected for UI text: they swap letters on
purpose ("Not the far east" draws X like an H, "Typingrad" draws Y and B like Cyrillic letters), which breaks button
hints. "Not the far east" would suit the director's gibberish, though. TextMesh Pro's essential resources are
imported (`Assets/TextMesh Pro`).

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

- SkinFactory (`../SkinFactory/Assets/Scripts`): `MovementDirectionalXZ` is already copied here (without its
  Animator parameters), and `OrbitCameraController` went from there into UnityCommon. Still to take: `SpeechBalloon` /
  `SpeechBalloonManager` / `BossBabble` and `GameSounds.BabbleCR` (a talking boss with speech balloons and voice blips).
- UnityCommon `UC.Interaction` (`Systems/Actions`): `Interactable` with an `InteractionVerb`, conditions and
  `GameAction`s. Not used: the user wanted verbs as plain strings on one component, so the game has its own
  `Interactable` (no namespace); `UC.ScreenSpaceOutline` is used for the highlight.

## Current state (2026-10-24)

- Unity 6.6 "Universal 3D" (URP) template, plus UnityCommon, NaughtyAttributes and `com.unity.pipeline` (Unity
  CLI control of the Editor). Compiles cleanly.
- Done: the zombie movie set; the player walking around it with idle/walk animations, the limited follow camera
  and right-stick aiming; interaction (targeting, outline, prompt, verb cycling); verbs as components with
  conditions and actions, the shovel pickup; the dig cutscene (bars, cutscene camera, animation, dirt, the zombie
  coming out) and the zombie chasing the actor on the NavMesh. All tested in Play mode. Next, from the scene's
  script: reaching the crypt, "Cut!" on touching a zombie.
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
| `Assets/Scenes` | Scenes; `ZombieMovie/` holds that scene's baked NavMesh |
| `Assets/Scripts` | Game code: `Player/` (`Player`, `MovementDirectionalXZ`), `Characters/` (`CharacterAnimation`, `Zombie`), `Interaction/` (`Interactable`, `Verb`, `InteractionPrompt`, `Actions/`, `Conditions/`), `Cutscene/` (`Cutscene`), `Editor/` (`VerbEditor`) |
| `Assets/Prefabs` | `Zombie` (spawned from the grave), `Candles`, `FireBasket`, `UI_InventoryItem` |
| `Assets/Sprites` | `Items/` (item icons: `Shovel`) |
| `Assets/Fonts` | `BarlowCondensed-Black` (TTF and its TMP SDF asset) |
| `Assets/TextMesh Pro` | TMP essential resources |
| `Assets/Data` | Hypertags (`MainPlayer`, `MainCamera`, `ZombieSpawn`, `Zombie`, `InventoryDisplay`); `Items/` (`Shovel`) |
| `Assets/Materials` | `SetGround`, `StudioFloor` (URP Lit, flat colours); `DirtParticles` (URP Particles Simple Lit); `FlameParticles` (additive URP Particles Unlit); `NoFriction.physicMaterial` (player capsule) |
| `Assets/Settings` | URP assets (`PC_RPAsset`, `Mobile_RPAsset`), renderers, volume profiles, `InputSystem_Actions.inputactions` (template Input System actions) |
| `docs/` | This context |
| `README.md`, `LICENSE` | Player-facing description and credits; MIT license |

New content follows the other GCC projects: code in `Assets/Scripts/`, and sibling folders `Prefabs/`,
`Materials/`, `Audio/`, `Data/`, `Fonts/`, ... as needed.
