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
- **Director**: shown in the bottom-left corner. He works as a hot/cold meter: he gets more or less angry as
  the player does the right things, and the player reads his body cues to work out what to do.
  - **Position cue**: how close the player is to where they should be. Updates continuously as the player moves.
  - **Orientation cue**: once close to that position, whether the player is aiming in the right direction to
    interact with the right object.
  - **"Life system"**: interactions that are wrong for that moment make him angrier.
- **Director, decided 2026-10-24** (the user's call):
  - The scene's script is a sequence of beats, each with a current target (the shovel, the grave, the crypt).
  - **Body language = hot/cold**, more frustrated the colder it is. Far from the current target, it's the distance
    that counts; close to it, it's the orientation (which object the actor is aiming at).
  - **Face = mood**, which is the health: right actions improve it, wrong actions lower it, and staying far from
    the target too long, or aiming at the wrong object too long, makes it worse and worse over time.
  - "Cut!" and the restart (touching a zombie, running out of mood) come after the director.
  - Display: a 3D model with its own camera (the user's choice, 2026-10-24), in its own scene so every movie can
    reuse it. See Director below.

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
  `Zombie.all` lists the enabled ones (the DirectorDriver uses it). With no stopping distance it walks into the
  actor: touching, they're about 0.33 apart.
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
  straight on a `PlayableHandler`), `FX/Play Particle System` (every system under an `ObjectTarget`; waiting lasts
  until all their particles die) and `FX/Stop Particle System` (every system under the interactable by default,
  optionally clearing them), `Object/Set Light` (on, off or toggle every `Light` under the interactable by default),
  `UI/Show Text` (floating text over an object, see Red herrings), `Director/Change Mood` (-0.07 by default),
  `Zombie/Follow` (on/off, the zombie found through an `ObjectTarget`, by hypertag by default), and the conditions
  `Inventory/Has Item` and `Object/Light Is On` (any `Light` under the interactable enabled; negated, "is off").
  UnityCommon's own `RPG/Add Item` and `RPG/Destroy Entity` work on RPG entities, which this game doesn't use.
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
- Set up: the shovel (`shovel-dirt`) has "Pick up" (Add Item Shovel, Destroy the interactable) and "Lean on" (a
  red herring, see below); the grave ring has "Dig" on its `Mound` child (Has Item Shovel; the cutscene sequence in
  Scenes above), so after digging the grave has no verbs left.
- The project enters Play mode without reloading the domain or the scene (Enter Play Mode Options). Statics
  survive between plays, and a component added from an editor script keeps its in-memory field values: a
  `UC.Inventory` added that way had a null `initialItems` (UnityCommon doesn't null-check it) until the field was
  set through a `SerializedObject`.

### Red herrings

Props the script doesn't need offer verbs too, so the right one is harder to find (added 2026-10-24, the user's
list). Each ends with `Director/Change Mood` -0.07, half a face: the eight faces are 1/7 of mood apart, so two wrong
choices cost one face (measured in Play mode: 1, 0.93, 0.86, 0.79, 0.72, face 2).

- Candles (on `Assets/Prefabs/Candles.prefab`, so every candle has them; Interactable "Candles"): "Put out" while
  the light is on (Light Is On; the actor's `interact-left`, Set Light off, Stop Particle System) and "Light" while
  it's off (negated; `interact-left`, Set Light on, Play Particle System on the interactable). In Play mode the
  light and the three flames go out and come back, and the verbs swap.
- Gravestones (the 14 in `Set/Graves`, Interactable "Grave"): "Read" (an epitaph, a punny name, dates and a line,
  floating over the stone), "Dig" (needs the shovel; `interact-right`) and "Pray" (`emote-yes`). The user asked for
  something on "the dirt" too: each mound sits in front of its stone, and standing on an object counts as facing
  it, so a mound would always win targeting over its stone. The dirt's verbs (Dig, Pray) went on the gravestone
  instead, one target per grave. No gravestone is within 2 units of the real grave (`grave-border`, 3.2, -2.3).
- Pumpkins (the 3 in `Set/Props`, Interactable "Pumpkin"): "Kick" (`attack-kick-right`) and "Carve"
  (`interact-right`).
- The shovel's "Lean on" (`interact-left`).
- The coffin (`Set/Props/coffin`, Interactable "Coffin"): "Knock" (`interact-right`, Wait 0.6, then "You hear a
  faint knock back..." 0.8 above it).
- The right actions go the other way, a whole face each (+1/7, 0.143, just before the Set Player Target that moves
  the script on): the shovel's "Pick up" and the grave's "Dig". Dig's lands about 1 s after the cutscene ends (the
  HUD, with the portrait, is faded out during it). Measured: 0.5 to 0.643 to 0.786.
- Floating text: `HUD/CombatText` (the HUD's first child, so drawn under the prompt) holds UnityCommon's
  `CombatTextManager` with `Assets/Prefabs/UI_FloatingText.prefab` (TMP, Barlow Condensed 34, wrapped in 700x220,
  pivot at its bottom centre so it grows up from its anchor). A text stays, then fades out drifting up 12 px/s,
  keeping its place over the object as the camera moves. Epitaphs are anchored 1.15 above their stone, above the
  prompt, white for 4 s.
- A Screen Space - Camera canvas on the main camera was tried first, because `CombatTextManager` didn't work on an
  overlay canvas: post-processing greyed it, and it sat under the prompt. Fixed in UnityCommon on 2026-10-24:
  `WorldToLocal` gave its camera to `ScreenPointToLocalPointInRectangle`, which must get none for an overlay canvas
  (the camera still projects the world point).

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

- Lamps: `Assets/Prefabs/LightpostDouble.prefab` and `LightpostSingle.prefab` (the user's) carry their own point
  light, and use `Assets/Materials/lightpost-double Variant.mat`, whose emission map
  (`Assets/Materials/Textures/colormap_lightpost.png`, derived from Kenney's colormap) makes only the lamp
  glass glow. Two of each on the set; the old `Set/Lamps` lights are gone (the empty group remains).

- `UC.LightFlicker` (written here, moved to UnityCommon's `Animation/` on 2026-10-24) on a Light: its color follows a
  gradient that loops every `duration` seconds, the gradient's alpha scaling the light's own intensity, and its
  position wanders around its start with Perlin noise (`frequency`, per-axis `amplitude`). `randomStart` (on by
  default) starts each light at a random point of the gradient so they don't flicker in step; off, they all start
  at its beginning. The position noise always starts at a random point.
- `Assets/Prefabs/Candles.prefab` (the user's: Kenney `candle-multiple` and a point light): a small flame particle
  system on each of the three wicks (local space, about 12 particles each) and `LightFlicker` (warm, 0.8 s cycle).
  The wick tops in the mesh are at (-0.064, 0.120, 0.078), (-0.067, 0.170, -0.068), (0.049, 0.200, 0.019).
  It's also an Interactable ("Put out" / "Light", see Red herrings).
- `Assets/Prefabs/FireBasket.prefab` (the user's, with a purple point light): a purple fire emitted above the rim
  (world space, up to 60 particles, noise) and `LightFlicker` (purple, 0.6 s cycle, larger wander).
- Flames use `Assets/Materials/FlameParticles.mat`: URP's `ParticlesUnlit` copied and made additive, with an HDR
  tint so bloom picks it up.
- On the set: 8 candles (beside gravestones, by the coffin and the crypt, beside the path) and 5 fire baskets (left
  wall, both back corners, flanking the front of the path), under `Set/Props`. The NavMesh was rebaked after
  placing them.

### Director

- The model is the user's `Assets/Prefabs/Director.prefab`: a variant of Blocky Characters' `character-a` with
  `Assets/Materials/Director.mat` (`Assets/Materials/Textures/director_base.png`, beret, moustache, red scarf).
  Blocky characters are rigid parts under `root` (`leg-left`, `leg-right`, `torso` with `arm-left`, `arm-right`,
  `head`), all on the `Director` layer (6).
- `Assets/Scenes/Director.unity`, loaded additively by every movie (`SceneLoader` object with
  `AdditiveSceneLoader`, `Assets/Scripts/Scenes`; both scenes are in the build list). It holds the director at
  (0, -100, 0), far below any set, turned 165 degrees; `DirectorCamera` (FOV 30, Director layer only, solid dark
  background, no post-processing) renders him into `Assets/Materials/Textures/DirectorView.renderTexture`
  (512x512, MSAA 4); three short-range spot lights (key, fill, rim) on the Director layer. No directional light
  there, since it would compete with the movie's sun. Background by the user: a film set behind him
  (`Assets/Materials/MovieSet.mat`, `Textures/movie_set.png`) and zanerlak's Low-Poly Movie Camera
  (`Assets/Externals/Zanerlak/camera.fbx`, CC-BY 4.0, credited in the README).
- The movie's cameras leave out the Director layer. The portrait is `HUD/DirectorPortrait` (bottom-left, 300x300,
  a dark frame with a RawImage of the render texture), so it fades with the HUD in cutscenes. The inventory is in
  the bottom-right.
- `Director` (`Assets/Scripts/Director`, on the prefab root, `Director.instance`): `frustration` (0 pleased,
  1 losing it) and `mood` (1 delighted, 0 furious), both inspector sliders; what's shown eases towards them at
  1.5 per second. `DirectorDriver` sets them during play.
- `DirectorBody`: procedural body language on the rigid parts (no clips, no Animator), from the frustration, all
  blended continuously with a tempo that rises with it. Calm: breathing, slow nod, arms hanging. Impatient
  (around 0.5): leaning in, right arm gesturing forward, left arm held out, head shaking, right foot tapping.
  Furious (towards 1): arms waving over the head, hopping, stomping, head shaking fast and looking up.
- `DirectorFace`: the faces are `Assets/Materials/Textures/director_faces.png`, 8 sprites (`director_faces_0` to
  `_7`, 380x380, happiest to furious), each a whole head area as laid out in the top-left 380x380 of
  `director_base.png`. At runtime the director's material and texture are replaced by copies, and showing a face
  copies that sprite over the head area (CPU `GetPixels`/`SetPixels`, only when the face changes), so both
  textures have Read/Write enabled. Mood picks the face: 1 is face 0, 0 is face 7.

### Director driver (hot/cold and mood)

- The movie's script sets the actor's current target with the `Director/Set Player Target` action: an
  Interactable (go to it and aim at it) or a place (the first object with a hypertag); neither set clears it.
- `StartupActions` (`Assets/Scripts/Interaction`, an `ActionRunner` with the same action list and inspector as a
  Verb) runs actions when the scene starts, for the first beat. The actor is the context's trigger object.
- `DirectorDriver` (`Assets/Scripts/Director`, `DirectorDriver.instance`, one per movie, on `MovieScript` with the
  StartupActions) owns frustration and mood and writes them into `Director.instance` every frame (the Director scene
  may load a moment later). Frustration only moves at rates per second, so it never jumps (and the Director eases
  what he shows on top of that). All of it is in the inspector:
  - Far (straight-line distance to the target above `nearDistance`, 1.2): progress decides. The distance is
    measured every `measureInterval` (0.2 s) along the NavMesh (`usePathDistance`; straight line if the path
    fails), so going round obstacles doesn't count as moving away. Getting closer faster than `progressThreshold`
    (0.2 u/s): -`approachRate` (0.3/s); getting farther: +`retreatRate` (0.4/s); neither: +`idleRate` (0.08/s).
  - Near: aim decides. When the actor is aiming at an object (his current target, the one he'd interact with),
    that object decides: the target, -`aimRate` (0.4/s); any other, +`wrongObjectRate` (0.5/s). Only when he's
    aiming at nothing does the angle count: facing the target's closest point within `aimAngle` (45 degrees, the
    same as the actor's interaction angle): -`aimRate`; otherwise +`lookAwayRate` (0.25/s). (Fixed 2026-10-24: with
    the angle always deciding, at 30 degrees, an actor who had the grave selected 40 degrees off its closest point
    counted as looking away, and frustration climbed.)
  - Mood: frustration above `moodThreshold` (0.7) for longer than `moodGraceTime` (2 s) drains it at
    `moodDecayRate` (0.05/s). `ChangeMood(delta)` (the `Director/Change Mood` action) is for right and wrong
    actions (see Red herrings).
  - Zombies: a following zombie closer than `zombieRange` (0.8, on the ground plane) drains the mood at
    `zombieMoodRate` (0.03/s, a face in about 5 s), target or not; the inspector shows `zombieDistance` (-1 when
    none follows). Measured: the zombie caught the standing actor 1 s after the dig, then 0.786 to 0.575 in 7 s.
  - No target: -`relaxRate` (0.2/s). During cutscenes nothing changes. Starts at frustration 0.3, mood 1.
  - The inspector shows the live state, frustration, mood, distance and progress.
- Zombie scene targets: StartupActions targets the shovel; "Pick up" ends with the grave; "Dig" ends with
  `CryptEntrance` (hypertag `Assets/Data/CryptEntrance`, a child of the crypt in front of its door; the crypt is
  rotated 180 degrees, so that's its local +z).
- Measured in Play mode with the default values: standing far away 0.34 to 0.46 in 1.5 s; walking to the shovel
  0.68 to 0 in about 2.3 s; near and facing it stays at 0; turning away 0 to 0.37 in 1.5 s; walking away up to 1,
  then the mood drops 0.05/s from 2 s after passing 0.7.

### Director speech (briefing)

- `Director/Says` action: lines of what the director means, shown one balloon per line as gibberish (`translate`,
  on by default; `{braces}` keep their text, without the braces, e.g. `{Action!}`). Created with "wait" on, so the
  script goes on when he's done; it waits up to `maxWaitForDirector` (3 s) for the Director scene to load first.
- `Gibberish.Translate` (`Assets/Scripts/Director`): each word becomes 1 to 4 kana-like syllables picked from an
  FNV-1a hash of its letters, so a word always translates the same ("you" is "tepo", "the" is "gasu"); punctuation
  and capitals are kept.
- `DirectorSpeech` (`HUD/DirectorSpeech`, with `Balloon`: an off-white panel to the right of the portrait, growing
  up, with a tail pointing at him; 620 wide, text in Barlow Condensed, 30): types each line out at
  `charactersPerSecond` (30), with an optional `voice` SoundDef blip every `charactersPerBlip` (3) characters (none
  assigned yet), then holds it `holdTime` (1 s) + `holdPerCharacter` (0.03 s) per character. `skipInput` (the
  Player map's `Interact`, gamepad west / E, through the actor's `PlayerInput`) goes straight to the next line,
  typing or holding, and ends the speech on the last one. A press on the frame a line comes up doesn't count (the
  press that skipped the previous line, or an Interact that started the speech), and `Say` runs after the actor's
  Update, so the press that ends the speech was seen by the still-locked actor and interacts with nothing. Measured:
  the briefing skipped in 4 presses, about 2 s. While it runs,
  `DirectorSpeech.isTalking` is true, which holds the gameplay (below). The director's `talking` (set while a line
  types) adds nods, looking around and right-arm gestures to his body language.
- Zombie scene: `MovieScript`'s startup actions are the briefing (4 lines: come in through the gate as a
  gravedigger; pick up the shovel and dig the grave with passion; run to the crypt when the zombie comes out,
  don't let him touch you; "{Action!}"), the slate, then Set Player Target (the shovel). With the defaults the
  briefing lasts about 21 s and the slate about 2 s.

### Slate and gameplay hold

- The slate (clapperboard) is the user's, in the Director scene: `Slate`, an instance of
  `Assets/Externals/tech.service.allwe/Slate.fbx` (tech.service.allwe's "Black film slate or clapper", Sketchfab,
  CC-BY 4.0, credited in the README), with the user's
  `SlateCtrl.controller` (`Idle`, looping and default; `Clap`, the FBX's 2.04 s clip at speed 4, from the `Clap`
  trigger, no exit) on an Animator added at its root. It waits out of the director camera's view (left of him).
- `Slate` (`Assets/Scripts/Director`, on it, `Slate.instance`): `Play` slides it to `SlateShown` (a point in front of
  the director camera where it fills a bit under half the view, slightly left and down of centre; it keeps its
  rotation) in `moveTime` (0.4 s, easing out), waits `holdBeforeClap` (0.3 s), fires the trigger (and an optional
  `clapSound`), waits `clapTime` (0.55 s) + `holdAfterClap` (0.4 s), slides back (easing in) and resets to `Idle`.
- `Director/Slate` action (created with "wait" on): waits up to 3 s for the Director scene, then plays the slate.
- `Gameplay.isHeld` (`Assets/Scripts/Gameplay.cs`) is the one place that says gameplay is on hold: a cutscene, the
  director talking, or the slate playing. `Player` locks the controls and `DirectorDriver` pauses on it.
- Measured with the briefing sped up: the slate arrives 0.4 s after starting, claps 0.3 s later, is gone and reset
  about 1.1 s after that, and the actor can move straight away.
- The slate is lit only by spill from the director's lights, so it reads dark in the portrait.

### Outlines in the Director scene

- Black `UC.ScreenSpaceOutline` (4 px, the renderer feature's Max Width; visible only) on the Director prefab root,
  the `Slate` and the `CameraProp`. The feature runs for the director camera like for any other (tested in magenta).
- What to expect: the ring is drawn outside the silhouettes, over the background, so a black one only shows where
  the backdrop isn't black (the user's first try looked like it didn't work for that reason: black on near-black);
  and touching objects share one silhouette, so there are no lines between the director's parts.
- `SimpleOutline3d` (inverted hull, which does draw inner lines) was tried and dropped (the user wants the
  screen-space one). Found on the way: its shader (`FX/Outline/Simple/Resources/OutlineBackface.shader`) turned the
  extrude direction to world space with the inverse matrix instead of the inverse transpose (`mul(I_M, dir)`),
  which points the hull the wrong way on rotated objects. Fixed in UnityCommon on 2026-10-24 (the user asked): it
  now uses URP's `TransformObjectToWorldNormal`; tested on the rotated, non-uniformly scaled movie camera (its
  tripod went from scraps to a full outline). And in the component (`SimpleOutline3d.cs`): an outline material
  added past the last submesh only draws that submesh, so multi-material meshes were only partly outlined; fixed
  in UnityCommon on 2026-10-24 too (the user asked): `Update` draws the other submeshes of mesh renderers with
  `Graphics.RenderMesh` (outline material, the renderer's layer, no shadows) every frame. Skinned meshes with
  several submeshes still only get the last one. Tested in Play mode on the movie camera (two submeshes: body,
  reels and lens got their outline, not just the tripod).
- `DirectorFace` swaps only the material slots that hold the director's material (so an extra material on a part
  survives it).

### Font

UI font: Barlow Condensed Black (Google Fonts, SIL OFL; the same file SkinFactory uses), as a dynamic TMP font asset
in `Assets/Fonts`. Chequered Ink's "World" fonts were tried first and rejected for UI text: they swap letters on
purpose ("Not the far east" draws X like an H, "Typingrad" draws Y and B like Cyrillic letters), which breaks button
hints. "Not the far east" was then tried for the director's gibberish balloon; it looked right but was hard to read,
so the user switched the balloon to Barlow Condensed too (2026-10-24) and the font was removed. Barlow is the only
font. TextMesh Pro's essential resources are imported (`Assets/TextMesh Pro`).

### Open questions

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
| `Assets/Externals/Zanerlak` | zanerlak's Low-Poly Movie Camera (`camera.fbx`, CC-BY 4.0, Sketchfab; credited in the README) |
| `Assets/Externals/tech.service.allwe` | tech.service.allwe's slate model (`Slate.fbx`, its textures and materials; CC-BY 4.0, Sketchfab, credited in the README) and the user's `SlateCtrl.controller` |
| `Assets/Scenes` | `ZombieMovie` (the first movie; `ZombieMovie/` holds its baked NavMesh), `Director` (loaded additively by the movies) |
| `Assets/Scripts` | Game code: `Player/` (`Player`, `MovementDirectionalXZ`), `Characters/` (`CharacterAnimation`, `Zombie`), `Interaction/` (`Interactable`, `Verb`, `StartupActions`, `InteractionPrompt`, `Actions/`, `Conditions/`), `Cutscene/` (`Cutscene`), `Director/` (`Director`, `DirectorBody`, `DirectorFace`, `DirectorDriver`, `DirectorSpeech`, `Gibberish`, `Slate`), `Scenes/` (`AdditiveSceneLoader`), `Editor/` (`VerbEditor`, `StartupActionsEditor`); `Gameplay.cs` (gameplay hold) |
| `Assets/Prefabs` | `Zombie` (spawned from the grave), `Candles`, `FireBasket`, `LightpostDouble`, `LightpostSingle`, `UI_InventoryItem`, `UI_FloatingText` |
| `Assets/Sprites` | `Items/` (item icons: `Shovel`) |
| `Assets/Fonts` | `BarlowCondensed-Black` (TTF and its TMP SDF asset), for all the text |
| `Assets/TextMesh Pro` | TMP essential resources |
| `Assets/Data` | Hypertags (`MainPlayer`, `MainCamera`, `ZombieSpawn`, `Zombie`, `InventoryDisplay`, `CryptEntrance`); `Items/` (`Shovel`) |
| `Assets/Materials` | `SetGround`, `StudioFloor` (URP Lit, flat colours); `DirtParticles` (URP Particles Simple Lit); `FlameParticles` (additive URP Particles Unlit); `Director`; `NoFriction.physicMaterial` (player capsule); `Textures/` (`director_base`, `director_faces`, `DirectorView.renderTexture`, `colormap_lightpost`) |
| `Assets/Settings` | URP assets (`PC_RPAsset`, `Mobile_RPAsset`), renderers, volume profiles, `InputSystem_Actions.inputactions` (template Input System actions) |
| `docs/` | This context |
| `README.md`, `LICENSE` | Player-facing description and credits; MIT license |

New content follows the other GCC projects: code in `Assets/Scripts/`, and sibling folders `Prefabs/`,
`Materials/`, `Audio/`, `Data/`, `Fonts/`, ... as needed.
