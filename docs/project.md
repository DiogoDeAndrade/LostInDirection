# Project

**Lost in Direction** (the game's name, from the user, 2026-10-25; the repository and Unity project are still
called LostInTranslation): Diogo de Andrade's entry for the October'2026 Game Creators Club
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
  - "Cut!" and the restart come when the mood runs out (decided 2026-10-25; a zombie next to the actor drains the
    mood rather than cutting on touch). The actor has a few takes (3) for the whole movie; losing the last gets him
    fired. See Takes below.
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

Built so far: steps 2 to 4 (reaching the crypt completes the scene, see Takes below) and step 6. Step 5 became
"Cut!" when the mood runs out, a zombie next to the actor draining it. The grave is the `grave-border` ring next to the shovel,
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
  non-looping); the Mini Dungeon (`character-orc`, `character-human`) and Mini Forest (`character-archer`) ones too
  (2026-10-25: the orcs slid along frozen until then).
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
  `Inventory/Has Item`, `Object/Light Is On` (any `Light` under the interactable enabled; negated, "is off") and
  `Director/Target Is`; `Animation/Play Clip` (samples a clip over its length on an object with no Animator: a prop's
  own clips, like the gate opening), `Object/Set Active`, `Object/Move To` (eases an object to another's position and
  rotation), `Character/Walk To` (a NavMeshAgent character to an object, then facing it; created waiting),
  `Character/Face`, `Player/Bow Mode`, `Orcs/Start Spawning`, `FX/Lower Force Field`, `Director/Stay Near`,
  `Sound/Play Music`, `Camera/Follow` and `Character/Watch`. The movie's own actions (Director/..., Movie/Scene Complete) are described below.
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
- `ActionTrigger` (`Assets/Scripts/Interaction`, with a trigger collider; inspector `ActionTriggerEditor` like the
  Verb's): runs its actions, on the actor, once the actor is in the zone and its conditions hold. Checked in
  `OnTriggerStay`, so conditions can come true while he's inside, and not while gameplay is held. `once` (on by
  default); off, it can fire again after he leaves. `Director/Target Is` (an Interactable, or a place by hypertag)
  is the condition for "the movie is at this beat".
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
  - Stay near (`SetStayNear(place, radius)`, the `Director/Stay Near` action; instead of a target, which clears it,
    as Set Player Target clears it): within the radius on the ground plane -`stayInsideRate` (0.3/s), beyond it
    +`stayOutsideRate` (0.35/s), whichever way he faces; the mood rules as above. Measured: 0.29 to 0.99 in 2 s
    at 9.6 from the place.
  - Zombies: a following zombie closer than `zombieRange` (0.8, on the ground plane) drains the mood at
    `zombieMoodRate` (0.03/s, a face in about 5 s), target or not; the inspector shows `zombieDistance` (-1 when
    none follows). Measured: the zombie caught the standing actor 1 s after the dig, then 0.786 to 0.575 in 7 s.
  - No target: -`relaxRate` (0.2/s). During cutscenes nothing changes. Starts at frustration 0.3, mood 1.
  - The inspector shows the live state, frustration, mood, distance and progress.
- Zombie scene targets: StartupActions targets the shovel; "Pick up" ends with the grave; "Dig" ends with
  `CryptEntrance` (hypertag `Assets/Data/CryptEntrance`, a child of the crypt in front of its door; the crypt is
  rotated 180 degrees, so that's its local +z). `CryptEntrance` is also the finish: a trigger box (0.9 x 1 x 0.6,
  centred 0.5 up; the crypt's collider front is 0.2 behind it) with an `ActionTrigger`: Target Is CryptEntrance,
  then Scene Complete and the actor's `emote-yes`. Walking in before the dig does nothing.
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
  assigned yet), then holds it `holdTime` (1 s) + `holdPerCharacter` (0.03 s) per character (the user set 60 characters a second and
  0.5 s in the scene). `skipInput` (the
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
  director talking, the slate playing, or the take ending (`GameManager.isTakeOver`: cut or complete). `Player` locks the controls,
  `DirectorDriver` pauses, and following zombies stop where they are (`NavMeshAgent.isStopped`, velocity zeroed).
- Measured with the briefing sped up: the slate arrives 0.4 s after starting, claps 0.3 s later, is gone and reset
  about 1.1 s after that, and the actor can move straight away.
- The slate is lit only by spill from the director's lights, so it reads dark in the portrait.

### Takes (lives), the cut and scene complete

Added 2026-10-25, the user's design.

- `GameManager` (`Assets/Scripts/GameManager.cs`, `Assets/Prefabs/GameManager.prefab`, at the root of every movie
  scene): the first one keeps itself (`DontDestroyOnLoad`) and the copies that come with later scenes destroy
  themselves (the pattern of the user's other games, e.g. Golf). It holds `maxTakes` (3) and `takesLeft`, which
  survives scene loads, and `movieScenes`, the movie's scenes in order (NaughtyAttributes `[Scene]`; the prefab
  has `ZombieMovie`). The first GameManager's settings are the ones used, so they belong on the prefab.
- Both endings set `isTakeOver` (holding the gameplay until the next scene is loaded) and the director's
  frustration to 0, fade the music out (`musicFadeTime`, 0.6 s), and end with `FadeAndLoad`.
- `DirectorDriver`: when the mood is 0 and gameplay isn't held (a cutscene a wrong action started finishes first),
  it calls `GameManager.Cut()` (a warning, once, if there's no GameManager).
- The cut (a coroutine on the GameManager, so it outlives the scene): the actor and zombies stop; frustration 0,
  so the body calms down and the furious face is what shows; the director says `cutLine` (`{CUT!}`, braces so it
  isn't translated); a take is lost; `pauseAfterLine` (1 s). With no takes left he says `firedLine`
  (`{YOU'RE FIRED!}`) and waits again. Then `FullscreenFader.FadeOut` (0.75 s, black) and the active scene loads
  again (the Director scene with it, through its `AdditiveSceneLoader`); being fired goes to the main menu.
- Scene complete (the `Movie/Scene Complete` action; the crypt's trigger): mood 1 (delighted) and frustration 0,
  `printLine` (`{CUT! PRINT IT!}`), the pause, then the next of `movieScenes`, keeping the takes. After the last
  (or a scene that isn't in the list) he also says `wrapLine` (`{THAT'S A WRAP!}`), and it's the main menu.
- The main menu is `mainMenuScene` (`MainMenu`): going there resets the takes. With it empty, the movie's first
  scene loads instead (or the current one with no list). `isTakeOver` clears once the new scene is loaded. The skip
  button works on all these lines.
- Fader: UnityCommon's `QuickPrefabs/FaderCanvas` prefab at the scene root as `Fader` (sorting order 100, over the
  cutscene bars at 10): it starts black and fades in over 0.5 s on every load.
- `TakesDisplay` (`Assets/Scripts/UI`, on `HUD/Takes`, top right, a right-aligned `HorizontalLayoutGroup`): one
  `Assets/Prefabs/UI_TakeIcon.prefab` (120x120) per take left; a lost take's icon, the leftmost, so the others stay
  put, swells to 1.5 and fades out over 0.6 s. The icon `Assets/Sprites/UI/Slate.png` (256x256) is the slate model
  rendered from the front with an 8 px white outline baked in (it's black, the set is dark).
- The script runs again on every take: the briefing (skippable), the slate, the shovel. Retakes keep the briefing
  (the user's call, 2026-10-25).
- Measured in Play mode: a cut with the zombie chasing, the stick held: neither the zombie nor the actor moved in
  the next second; frustration 0 at once (the body eases to it in 0.5 s). Takes 3, 2, 1, then fired: "YOU'RE
  FIRED!", and back to 3 takes. The new scene's fader is black when it loads and down to 0.38 a third of a second later.
  Scene complete, with 2 takes left: the actor ran from the grave to the crypt along the NavMesh in 3.5 s (the
  zombie behind); the zone fired with the actor's capsule at its corner; "CUT! PRINT IT!" with the delighted face,
  "THAT'S A WRAP!", the restart with 3 takes, 5 s from the zone to the new scene.

### Labyrinth

Added 2026-10-25, the user's brief: a maze the actor has to get through with the director's help (he gives
directions at the start), barrels to inspect, traps that make him stop now and then, a more top-down camera so it's
harder to get one's bearings. Then (same day): a bigger maze, a key that opens the locked door at the end, coins
everywhere to reward exploring, stairs down from the shut entrance (as if he came from above), a red flash and two
faces per trap hit, and all of it darker, lit by the actor's light with its shadows. Then open rooms among the
corridors, so it's less monotonous. Built with Kenney's Mini
Dungeon (added by the user; 1-unit blocks: `wall` 1 x 1.1 x 1, `floor`, `stairs` (high on its -z side), `trap` with a
`spikes` part and show / hide clips, `gate` with a `door` part and open / close clips, `key`, `coin`; no bow in it).

- `Assets/Scenes/Labyrinth.unity`, after ZombieMovie in `movieScenes` and in the build list. It started as a copy of
  ZombieMovie, so it has the same player, HUD, director, takes, fader and GameManager. Its `Set` is generated by an
  editor script that isn't kept in the project (it rebuilds the open scene in place), so changes are made in the
  scene now.
- The maze: 11 x 11 cells, 23 x 23 blocks centred on the origin, a recursive backtracker plus 2 walls knocked out for
  loops, then five open rooms: every wall inside a rectangle of cells comes down (3 x 3, 3 x 2, 2 x 3, 2 x 2, 2 x 2
  cells, a cell apart, not over the stairs' cell), which adds loops of their own. The first cell is an alcove open
  only to the north, holding `EntranceStairs` down from the shut `EntranceGate` (a `wall-opening` in the outer wall);
  the actor starts at their foot, facing north. Of 300 seeds the builder kept 2169: at least 8 dead ends, the key in
  the dead end farthest from the start (at least 50 blocks from both the start and the exit), and the way through
  (start, key, exit) closest to 160 blocks (104 + 58; 88 + 48 units along the NavMesh). The ways it plans (the
  briefing's turns, where traps go) are the shortest with the fewest turns, so they don't zigzag across the rooms.
  Walls have 1 x 1.1 x 1 box colliders; the floor is tiles (15% `floor-detail`) over one box.
- The rooms (`Set/Rooms`): `column` pillars where the walls between their cells met (box colliders; one carries a
  candle), two `banner`s on the walls around, facing in, and a set piece in a corner cell no corridor opens onto:
  a `chest` ("Open": `interact-right`, Play Clip its `open`, "Empty. The props department strikes again.", -0.07
  mood), a `table` with two `chair`s, `rocks`, or two barrels. The 3 x 3 hall has a FireBasket in the middle (range
  4.5, intensity 1.5): a purple landmark. No traps in the rooms, and more coins.
- The key (`Set/Props/Key`): the Mini Dungeon key in `Assets/Materials/GoldGlow.mat` (the kit's colormap, emitting
  its own colours) with a small warm light; a trigger capsule on Ignore Raycast. "Pick up": `pick-up`, Add Item
  `Assets/Data/Items/Key` (icon `Assets/Sprites/Items/Key.png`, rendered from the model), +1 face (a right action),
  destroy it, Set Player Target the door. The icon is fitted to the key's own outline (its vertices as the icon
  camera sees them), not its bounding box, so it fills the inventory slot like the shovel.
- The exit door: `ExitGate` (the gate in the top right `wall-opening`), with `Blocker` (a solid box) and `Lock` (the
  Interactable "Door", a trigger), both on Ignore Raycast. "Open" with the key: `interact-right`, Play Clip (the
  gate's `open`), Set Active off on `Blocker` and `Lock`, +1 face, Set Player Target `LabyrinthExit`. "Open" without
  it: "Locked. The key must be somewhere in here..." (no mood cost). Beyond: one corridor block, `ExitStairs` up, and
  `Set/Exit` (hypertag `Assets/Data/LabyrinthExit`, a trigger on Ignore Raycast; `ActionTrigger`: Target Is
  LabyrinthExit, then Scene Complete and `emote-yes`).
- The director's targets follow the beats: the key (StartupActions, after the briefing and slate), the door (picking
  up the key), the exit (opening the door). His hot/cold measures along the NavMesh: the `Set`'s `NavMeshSurface`
  (physics colliders, its layer mask without Ignore Raycast, so neither triggers nor the door's blocker are in it and
  the way through the door counts) baked to `Assets/Scenes/Labyrinth/NavMesh-Set.asset`.
- Briefing: the intro, "the door at the far end is locked, and the key is somewhere in here", the way to the key as
  turns from the start, eight to a balloon, at most three ("..., and so on, and there it is!"), "Then the door.
  Mind the spikes, and forget the barrels!", "{Action!}". The gibberish is consistent (right is always "bibune", left
  "muwa"), so the directions can in principle be decoded.
- Coins (`Set/Coins`, 100): the Mini Dungeon coin at 0.8 scale in GoldGlow (no shadows), on 60% of the room blocks,
  55% of the other blocks off the way through and 20% on it. `Coin` (`Assets/Scripts/Props`) spins and bobs, and pops (swells, shrinks away) when the
  actor comes within `pickupRadius` (0.35) on the ground plane; no collider, no count, no mood: a reward for exploring
  (which the director's hot/cold doesn't like).
- Barrels in 12 of the other dead ends: Interactable "Barrel", "Inspect" (`interact-right`, one of 12 lines about the
  barrel, -0.07 mood).
- `SpikeTrap` (`Assets/Scripts/Props`) on 13 traps: 9 on straight stretches spread along the way through, 4 off it.
  It samples the model's show / hide clips; down `downTime` (1.6 s), up `upTime` (1.0 s), from a random `phase`.
  Standing on it (within `halfSize` 0.39 of its centre) while the spikes are up hurts once per cycle:
  `hitMoodCost` (2/7, two faces), `ScreenFlash.Flash()` (the HUD's `HitFlash`: red, up to 40% in 0.05 s and gone in
  0.6 s), "Ouch!" in red, and the actor is put `pushDistance` (0.65) off its centre along the corridor, on the side
  he's on. Not while gameplay is held. Rushing through costs the take: a bot that never waits lost it at the fourth
  hit, before the key.
- Dark: the directional light and the film lights are off, the ambient is near black (0.008, 0.008, 0.03), and the
  34 wall-top candles and the pillar ones (the Candles prefab, at least 3.6 apart; they can be put out) reach only 3
  units. The scene is
  carried by `Player/Torch`: a warm point light 2.4 above the actor (range 6, intensity 2.6, soft shadows at full
  strength, high resolution), so the walls throw shadows and the corridors beyond stay dark. At 1.6 up it blew out
  his head.
- Camera: `OrbitCameraController` angleX 72 (ZombieMovie 45), distance 5; `CameraLimits` 19 x 10 x 21 around
  (0, 5, -1).
- Measured in Play mode, a bot following the NavMesh with the trap damage switched off: the key at 43 s (the target
  becomes the door), the door at 68 s (the blocker and lock go off, the target becomes the exit), the take over at
  69 s; 21 of the 100 coins on the way; 8 trap hits (red flashes). A bot that steers straight at the next NavMesh
  corner can wedge itself on a wall corner after a trap pushes it (it stood there until the director cut the take);
  it needs to sidestep when it stops moving, as a player would.

### Temple

Added 2026-10-25, the user's brief: grab a bow, which brings a new option, shooting arrows (an aiming state switched
on and off by an action: the right stick aims, the left trigger shoots); orcs come in from time to time and have to
be shot; with them dealt with, free a girl locked in a cage; she comes out, kisses the hero (there's no kiss clip: a
reach and a "Mwah!"), walks to the middle of the room and takes down the force field around an artifact; taking it
ends the scene. Built with Kenney's Mini Dungeon (walls, columns, gates, the orc), Mini Forest (added by the user:
`weapon-bow`, `weapon-arrow`, `building-structure` for the cage frame, `platform`, `target`, plants, rocks) and Mini
Characters (`character-female-c`, the girl).

- `Assets/Scenes/Temple.unity`, after Labyrinth in `movieScenes` and in the build list (the last scene, so finishing
  it is "That's a wrap!" and the menu). A copy of ZombieMovie (player, HUD, director, takes, fader, GameManager) with
  a hall instead of the graveyard, built by an editor script that isn't kept in the project.
- The hall: 13 x 13 inside Mini Dungeon walls at +-7 (blocks of 1, centred on the origin); the shut `EntranceGate` at
  the south with `EntranceStairs` down from it, the actor at their foot; four doors (`wall-opening`s with gates
  sampled open) at the west and east sides and two at the north, each with a vestibule behind it, where the orcs
  appear; six columns down both sides (two with candles), banners, plants, rocks, a fire basket in each corner.
- The bow (`Set/Props/Bow`, a trigger box on Ignore Raycast round the `weapon-bow` lying on a `table` by the stairs,
  next to an archery `target`): "Pick up": `pick-up`, Add Item `Assets/Data/Items/Bow` (icon
  `Assets/Sprites/Items/Bow.png`), +1 face, destroy it, Player/Bow Mode on, Set Player Target the cage as a place
  (hypertag `Assets/Data/TempleCage`, on `Set/Props/Cage`: a place, not its Interactable, so "Free her" doesn't open
  up early), the director's "Good! Now, to the girl's cage!".
- The fight starts at the cage (the user's call): `Cage/GuardZone`, a 6 x 6 trigger on Ignore Raycast with an
  `ActionTrigger` (Target Is the cage): Director/Stay Near the cage (radius 3), "Here they come! Stay by the girl,
  and shoot them all!", Orcs/Start Spawning. While the orcs are on, the director wants him by the cage: inside 3
  units his frustration falls, outside it climbs, whichever way he faces (and with it, held high, the mood), which
  also puts him near the two north doors.
- `Archer` (`Assets/Scripts/Player`, on the actor): armed, `MovementDirectionalXZ.aimWhileMoving` is on (the right
  stick turns him while he walks, twin-stick), `HeldBow` (the bow, 0.8 scale) is held out in front of him
  (`holdPosition` (0, 0.16, 0.24) and `holdRotation` from his root; it was in his left hand at first) while he aims
  (`MovementDirectionalXZ.isAiming`, the right stick past its dead zone) and for `showAfterShot` (0.5 s) after a
  shot, and hidden otherwise (and in cutscenes), and `Shoot` (the Player map's new action: either trigger, Space, left mouse button) looses an
  `Assets/Prefabs/Arrow.prefab` along his facing from 0.35 up and ahead, at most every `cooldown` (0.45 s), with
  `holding-both-shoot`. `Arrow` (`Assets/Scripts/Props`) flies at 14 u/s for at most 14, sweeping a 0.08 sphere
  (triggers and the shooter ignored): the first live orc dies; anything else stops it, stuck for a second. Its first
  sweep starts inside the shooter, level with it, and an orc the sweep starts inside still counts: an orc in melee
  range has its collider round the arrow's spawn point (0.35 ahead), and casts skip what they start in, so
  point-blank shots used to go through (found once there were more orcs, 2026-10-25). It
  leaves a short trail (a `TrailRenderer` on the prefab: 0.25 s, 0.05 wide narrowing to nothing, warm white fading
  out, `FlameParticles`, no shadows).
- `Orc` (`Assets/Scripts/Characters`, `Assets/Prefabs/Orc.prefab`: built like the zombie, a kinematic body,
  NavMeshAgent at 1.5 (the actor's 2), stopping 0.4, CharacterAnimation; the Mini Dungeon orc as its model, with an empty Animator
  for its PlayableHandler: without one the handler "plays" (its debug shows the clip running) and nothing moves,
  which is how the orc and the girl first came out): goes after
  `MainPlayer`; within `attackRange` (0.55) it swings (`attack-melee-right`) and, if he's still in reach when it
  lands (0.3 s), costs a face (`hitMoodCost`), flashes the screen red and floats "Ouch!"; then `attackCooldown`
  (1.3 s). One arrow kills it (`Hit`, with the arrow's direction): it turns to face the shot, flashes `hitColor`
  (red, fading over `flashTime` 0.4 s, through a MaterialPropertyBlock), is knocked back `knockback` (0.35) along the
  arrow's line in 0.2 s, falls with `die` straight on its PlayableHandler (its CharacterAnimation off, so it stays
  down), lies there `corpseTime` (1.5 s) and sinks away; `killMoodReward` (+0.07). It stops while gameplay is held. `Orc.all` lists them.
- Blood (the user asked for it, 2026-10-25): on the hit the orc spawns `Assets/Prefabs/BloodSplash.prefab` at
  `bloodHeight` (0.45), facing along the arrow: 52 drops in two bursts out of a 32 degree cone (1.2 to 3.6 u/s,
  heavy gravity, stretched by their speed, gone where they land: world collisions) and a puff of mist, in
  `Assets/Materials/BloodParticles.mat` (URP Particles Unlit, plain alpha, a round drop, `Textures/BloodDrop.png`);
  it destroys itself. Then `bloodSplats` (2 to 4) floor splats of `splatSize` (0.3 to 0.6) 0.6 to 1.6 past the hit
  along the arrow's line, the further ones a little later (past where the body falls: nearer, it covered them),
  and, once it's down, a pool of `poolSize` (1) spreading from under its chest while it lies there (it shows once
  the body sinks).
- `BloodDecal` (`Assets/Scripts/Props`, `Assets/Prefabs/BloodDecal.prefab`): a flat quad with one of the four
  splats of `Assets/Materials/Textures/BloodSplat.png` (a 2x2 atlas, generated; the tile through `_BaseMap_ST` in a
  property block), turned at random, laid on the floor under the point (a raycast that skips orcs and the actor, a
  few millimetres up, each a little higher than the last so they don't flicker), growing to its size (splats at
  once, pools over 2.5 s), staying `lifetime` (40 s), fading over 2 s; past `maxDecals` (80) the oldest fade early.
  `Assets/Materials/BloodDecal.mat` is URP Lit, transparent with plain alpha (a copy of the force field's, which is
  premultiplied and would keep highlights where the splat is clear), dark red, wet (smoothness 0.75), with a faint
  red emission: lit only by the temple's blue light it read as black. URP's own decals aren't used (no decal
  renderer feature; the temple's floor is flat).
- `OrcSpawner` (`Set/Orcs`, an ActionRunner; inspector `OrcSpawnerEditor`, with both action lists): once begun,
  its `waves` in turn, each with its own `count`, `maxAlive` (at once), `interval` (between orcs) and `speed` (times
  the prefab's 1.5); the first orc after `firstDelay` (1 s), each at one of the four vestibule spawn points (not the
  one nearest the actor). In the temple: 10 orcs, 7 at once, every 1.5 s, at 1.1x speed; then 12, 8 at once,
  every 1.2 s, at 1.2x; then 15, 10 at once, every second, at 1.25x (1.875, the actor's 2). When a wave is all
  dead it runs `onWaveCleared` (the director's "More of them! Keep shooting!") and the next comes `wavePause` (3 s)
  later; after the last it runs `onCleared`: "Bravo! Now find the key to her cage!" and Set Player Target the key.
  It was one stream of 12 at first (8, 3 and 3.5 s before that); the user asked for one more wave, 5 per wave,
  then, still too easy, for more orcs (8 a wave, 6 at once, every 1.8 s), then for a third wave harder than the
  second (8, 8, then 12, 8 at once, every 1.2 s, 1.2x), which is why waves have their own numbers, then for each
  wave to be about the next one's (all 2026-10-25).
- The key (the user's, 2026-10-25: the cage needs one, hidden in one of the orc alcoves): `Set/Props/Key`, the
  labyrinth's key copied over (the Mini Dungeon `key` in GoldGlow, its glow light, outline, trigger capsule;
  `Assets/Data/Items/Key`). `RandomSpot` (`Assets/Scripts/Props`) puts it at one of `Set/Props/KeySpots` when the
  scene starts, a different alcove every take: each 0.25 behind its spawn point, against the back wall. The
  alcoves are shallow (the back wall 0.45 behind the spawn point, the walkable floor half a unit across); spots
  beside the doorways were tried first, and were outside the walls. "Pick up" (Target Is the key): `pick-up`, Add
  Item, +1 face, destroy it, "The key! Now go and free her!", Set Player Target the cage; before the director wants
  it, "Not now. The director wants something else.".
- The cage (`Set/Props/Cage`): the `building-structure` frame with 12 `weapon-spear`s round it (1.4 scale; the three
  in front are `FrontSpears`), a solid `Blocker` and the `Lock` (the Interactable "Cage", a trigger), both on Ignore
  Raycast, so the NavMesh runs under the cage and the girl can walk out once it's open. The girl (`Set/Girl`, built
  like the orc, the female model, agent at 1.4) stands inside.
- The cage's verbs, all "Free her": while the director wants the key, `interact-right` and "Locked. The key must be
  somewhere in here..."; before (the fight, the bow), "The orcs first!"; both cost two faces (the user's call,
  2026-10-25: trying the cage without the key is a wrong move). With the cage his target and the key in the
  inventory (Inventory/Has Item), the cutscene.
- "Free her" (Target Is the cage, has the key): a cutscene through `CutsceneCamera`, which starts
  close on the cage front: the actor's `interact-right`; Set Active off on `FrontSpears`, `Blocker` and `Lock`; +1
  face; Character/Walk To (the girl to the actor, 0.45 short); Character/Face (him to her); her `interact-left`
  with a pink "Mwah!"; his `emote-yes`; Character/Watch (he keeps facing her to the end of the cutscene, 240
  degrees a second: the user asked for him to look at her, 2026-10-25); Camera/Follow (the cutscene camera follows her, from (-1.8, 1.8, -3) off her,
  south-west and a little above, so she walks round the field towards it, smoothing 0.4 s: more and she gains on it,
  close and cropped); Walk To (her to `AltarFront`); Face (her to the altar); Camera/Follow again, the casting shot
  (from (-2.7, 1.25, 1.9) off her, looking 0.9 ahead of her: a 3/4 view from the west, her face, the spell and most of
  the shield, smoothing 0.7 s) and a 0.4 s wait; the spell: FX/Play Particle System and Object/Set Light on
  `Set/Girl/Spell`, her `interact-right`, `interact-left` (both waited) and `interact-right` again as FX/Lower Force
  Field runs; then Stop Particle System and the light off, her `emote-yes`, and a 2.5 s hold on the shot while the
  sparks fall; End Cutscene; Set Player Target the artifact. The camera used to ease back to `OverviewShot` (high over
  the south end, like the gameplay view; still in the scene, unused) while she walked; the user asked for it to keep
  following her, and for more time after the shield drops (it was 0.3 s), 2026-10-25.
- `ShotFollow` (`Assets/Scripts/Cutscene`, on `CutsceneCamera`): while it has a subject, the camera smooth-damps to
  the subject plus an offset (world space) and looks at a smooth-damped point (the subject plus a look offset), so
  the first shot starts from what the camera was looking at and each new one is a smooth move. Switching the camera
  off stops it. `Camera/Follow` (`GameAction_CameraFollow`, no wait) sets it (adding one if missing) or stops it.
- `Watcher` (`Assets/Scripts/Characters`): keeps a character turned towards a target, at a set speed, through its
  Rigidbody when it has one (the actor's movement only turns him with input or velocity, so it doesn't fight it);
  started in a cutscene, it ends with it. `Character/Watch` (`GameAction_Watch`, no wait) sets it (adding one if
  missing) or stops it.
- The girl's spell (the user asked for "a magic spell", 2026-10-25): `Set/Girl/Spell`, in front of her hands (0.38
  up, 0.2 ahead), looping and off until cast, all in `FlameParticles` (additive) in world space: the stream (a narrow
  cone ahead of her, 120 sparks/s at 1.8 to 2.6 u/s, pink to cyan, with noise, living just long enough to reach
  the field from `AltarFront`, 0.8 m), `Glow` (sparks round her hands) and `Aura` (a ring at her feet, 0.32 across,
  sparks rising), and `Light` (pink point light, range 2.4, no shadows, off).
- The altar (`Set/Props/Altar`, a Mini Forest `platform`, the Interactable "Artifact"): the Mini Dungeon
  `shield-round` in GoldGlow, floating and turning (`Spinner`), with a warm light; round it `ForceField`
  (`Assets/Scripts/Props`): a sphere of radius 1 in `Assets/Materials/ForceField.mat` (URP Lit, transparent, cyan,
  emissive) with its collider (in the NavMesh, so the orcs go round) and a cyan light; it breathes, and Lower() fades
  it out as it swells, then switches it off. As it starts going down it plays `dissolve` (the user asked for sparks
  off its surface that drop down, an after effect, 2026-10-25): `Set/Props/Altar/FieldDissolve` (not under the field,
  which is switched off), at the field's centre: a burst of 450 and 300/s for 1.2 s off the upper half of the field's
  surface (a hemisphere shell, radius 1.1, the floor hides the rest), cyan to pale blue, 1.6 to 2.6 s each, falling
  with a little gravity held back by a speed limit, so they drift down like ash, a little noise, settling on the
  floor (world collisions). "Take" (Target Is the artifact): `pick-up`, the shield and its light
  off, +1 face, Scene Complete.
- Floating text has its own overlay canvas in this scene (`FloatingText`, sorting 5, over the HUD and under the
  cutscene bars): in the HUD it faded out with it in cutscenes, so the "Mwah!" didn't show. UnityCommon's
  `CombatTextManager` now places texts through whichever camera draws the screen when its own is switched off (the
  cutscene camera), in UnityCommon since 2026-10-25.
- Light: the directional light as moonlight (0.45, bluish, from above), ambient (0.11, 0.1, 0.14), two warm fills from
  above (range 14, intensity 2.2, soft shadows), the corner fire baskets (range 7, intensity 2.5), candles. The first
  build (ambient 0.05, no moon, fills at 0.7) was too dark to fight in.
- Camera: `OrbitCameraController` angleX 55, distance 7; `CameraLimits` 8 x 12 x 9 around (0, 5.5, -3).
- Measured in Play mode with a bot that aims perfectly, before the guard zone and the bigger wave: the bow at 1 s, the
  8 orcs dead 21 s after (none reached it), the cage, the cutscene (8.5 s: the kiss with the "Mwah!", the pull-back, the field down), the shield, "THAT'S
  A WRAP!", the menu.
- Measured again with the waves, the spell and the music (2026-10-25): no music during the briefing, The Call to
  Adventure from the slate on; at the cage 15.6 s in, the first wave of 5, "More of them!" 8 s later, the second
  wave dead 16 s after (no hits taken); the cutscene 8.3 s, the spell on with its light, about 800 dissolve sparks
  as the field went (350 still falling a second later); "THAT'S A WRAP!" and the menu. The first sizes (sparks 0.03
  to 0.07) read as faint glitter from the overview camera, so they went up.
- With the follow camera and more orcs (2026-10-25): the waves of 8 dead 25.7 s after reaching the cage; the cutscene
  9.7 s, ending 3 s after the field went. One run, before the point-blank fix, the orcs got to the bot (it stands
  still by the cage) and the take was cut; afterwards orcs placed 0.6, 0.45 and 0.33 in front of the actor died on
  the first shot. A bot that never misses and never moves isn't a player, so the difficulty wants a hands-on try.
- With the third wave and the blood (2026-10-25): 8, 8 and 12 orcs, 28 in 42 s from the first to the last dead (the
  third wave 17.5 s), the director's line before each new wave, 64 blood decals on the floor at the end (older ones
  fading); a single kill up close: the spray, the splats past the body, the pool once it sank.
- With the key and the steeper waves (2026-10-25): 10, 12 and 15 orcs, 37 dead 44.5 s after reaching the cage;
  "Free her" without the key said "Locked..." and started nothing; the key (in the north-east alcove that take)
  2.8 s away, the cage 2 s after, the cutscene 10.1 s, the actor's facing at most 0.3 degrees off the girl while she
  walked and cast; "THAT'S A WRAP!". Each of the four spots tested on its own: the actor walking in from the hall
  has the key as his target within a second.

### Main menu

Added 2026-10-25: the user asked for a gamepad menu like their other games' (UnityCommon's `MenuGamepad Variant`
and `CreditsGamepad Variant` quick prefabs, as in Deathfloor and WrongWay) over something like the Director scene.

- `Assets/Scenes/MainMenu.unity`, first in the build list, with its own copy of the director's set under `Set`
  (the user's call, 2026-10-25: the menu gets its own angle; loading the Director scene additively was tried first).
  The Director scene's objects (the Director prefab instance, `CameraProp`, the three spot lights, the backdrop
  `Quad`, the `Slate` and `SlateShown`; not its `DirectorCamera`) were moved over with their prefab links and
  references, still on the Director layer at y -100, without the outlines (the user doesn't want them here).
  Changes to the Director scene don't reach the menu.
- `Main Camera` (Director layer only, fov 30, at (1.42, -97.75, -7.46) looking down +z; its own `Global Volume`
  with the movies' `SampleSceneProfile`): the director left of centre, cropped above the feet (there's no real
  floor, only the painted one, so his feet would float), the painted set filling the screen (the backdrop quad is
  20 x 11.25, 16:9), the slate out of view to the left. `SlateShown` is at (0.73, -98.6, -0.91), so the clap covers
  his face.
- The movie camera prop stands at x 2.43, between the director and the menu. Having it aim at the selected option
  was tried and dropped (the user's call, 2026-10-25): the model is one mesh, so the whole tripod turned and tilted;
  it would have to be split into the camera and its stand first.
- `MenuInput`: a `PlayerInput` on `InputSystem_Actions` (Player map). UnityCommon's menus read
  `MoveHorizontal` / `MoveVertical` (1D axes: d-pad, left stick, WASD, arrows), confirm with `UIConfirm` (gamepad
  A or X, Enter, Space, E) and go back with `UIBack` (gamepad B, Escape, Backspace). All four were added to the Player
  map for this, like Deathfloor's.
- `MenuCanvas` (overlay, 1920x1080): `Menu`, the `MenuGamepad Variant` instance: title "LOST IN DIRECTION" (Barlow
  150) and the tagline "One actor. One director. Zero understanding." (the user's) at the top; `MenuItself` (its `UIGroup`)
  at the bottom right with Play, Options, Credits, Quit. Buttons are white, right-aligned, yellow over a dark bar
  when selected, and wrap around. `Options` (the prefab's panel, its `UIGroup` on `MenuInput`, `UIConfirm`, off
  until chosen) takes the buttons' place: `MusicVolume` (labelled "Music", a `UIFloatSelector` setting the Music
  volume, 0 to 1 in 0.1s, saved by the SoundManager), `GammaControl` ("Brightness", the `Gamma` preference, -0.7 to
  0.7 in 0.025s) and `Back`. The prefab had the labels at the fill bar's X scale (0.54), so they were squashed. Then `CreditsDim` and
  `Credits`, the `CreditsGamepad Variant` instance: its `BigTextScroll` reads `Assets/Data/credits.txt` (the README's
  credits in the Deathfloor format, `<size>` / `<color>` headings), one `Assets/Prefabs/UI_CreditsLine.prefab` per
  line, 70 px/s from the bottom of the screen; UIBack (or reaching the end) returns to the menu.
- Text over the set uses `Assets/Fonts/BarlowCondensed-Black SDF Outline.mat` (black outline and a soft shadow).
- `MainMenu` (`Assets/Scripts/UI`): Play fades the menu out, plays the slate's clap (`Slate.instance.Play()`) and
  fades out to `firstScene` (`ZombieMovie`). It hides the title and dims the set (`CreditsDim`, 0.6) while the
  credits roll (the menu's group is off without Play pressed), and makes the director gesture now and then
  (`talking` for 1 to 2.5 s every 4 to 8 s). Options, Credits and Quit are UIButton events (Switch Panel, Show
  Credits, Quit Application). The credits count as on while their own group (`creditsGroup`) shows, so the options
  don't dim the set. Back (`backControl`, UIBack) presses the options' `Back` while they're up (UnityCommon's
  `UIGroup` has no back of its own). It plays `music` (Waltz of the Café) from the start.
- `Assets/Resources/Globals.asset` (`Globals`, `Assets/Scripts`, a `UC.GlobalsBase`): UnityCommon's menus look it up
  for their UI sounds (none set yet) and log an error every move without one.
- `PlayerSettings.productName` is "Lost in Direction".
- Measured in Play mode, started from the menu: navigation wraps; Credits dims the set and hides the title, Back
  restores them; Play fades the menu, the slate claps in front of the director, and ZombieMovie is up 1.6 s later
  with 3 takes; fired, the game comes back to the menu with the takes reset. Options (2026-10-25): the panel comes
  up with Music selected and the buttons off; left and right change the music volume (and its saved preference)
  and the brightness; its Back or the gamepad's B returns to the menu.

### Music and options

Added 2026-10-25: the user's four songs in `Assets/Audio/Music` (MP3, Decompress On Load), one per scene, each
movie's starting only once "action" is called.

- The user's `Assets/Prefabs/SoundManager.prefab` (UnityCommon's `SoundManager`, `dontDestroyOnLoad`, routed through
  their `Assets/Settings/MainMixer.mixer`: Master, FX, FX2, Ambient, Voice, Music, Movies, with exposed volumes) is
  at the root of every scene; the first one keeps itself. Volumes are saved as `{SoundType}Volume` preferences.
- `Sound/Play Music` (`GameAction_PlayMusic`): plays a clip as the music, cross-fading from the current one
  (`crossfadeTime`, 1 s); no wait. In each movie's `MovieScript` it comes right after Director/Slate, which waits
  for the clap: ZombieMovie The Creeping Motif, Labyrinth Searching the Lost Maze, Temple The Call to Adventure.
  The menu (`MainMenu.music`) plays Waltz of the Café.
- The cut and scene complete fade the music out (0.6 s). That has to finish before the next scene's music starts:
  UnityCommon's fade-out (`PlayMusic` with no clip) clears its music source when it ends, so music started during
  it would be lost; the screen fade (0.75 s) is longer.
- Gamma: a `LiftGammaGainCtrl` (UnityCommon) on every scene's `Global Volume` reads the `Gamma` preference into the
  profile's Lift Gamma Gain every frame (`PlayerPrefRead`, as Deathfloor's), so the menu's Brightness shows at once
  and carries into the movie. The preference is a string ("1.0;1.0;1.0;w") written and parsed in the system's
  culture (UnityCommon's `UINumberSelector` and `PlayerPrefsHelpers`): fine with a dot for decimals, broken where
  the decimal separator is a comma.

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

## Current state (2026-10-25)

- Unity 6.6 "Universal 3D" (URP) template, plus UnityCommon, NaughtyAttributes and `com.unity.pipeline` (Unity
  CLI control of the Editor). Compiles cleanly.
- Done: the zombie movie set; the player walking around it with idle/walk animations, the limited follow camera
  and right-stick aiming; interaction (targeting, outline, prompt, verb cycling); verbs as components with
  conditions and actions, the shovel pickup; the dig cutscene (bars, cutscene camera, animation, dirt, the zombie
  coming out) and the zombie chasing the actor on the NavMesh; the director (portrait, hot/cold and mood, briefing,
  slate); red herrings; takes, the cut and scene complete, so the zombie scene can be lost and won; the main menu
  and credits; the labyrinth and the temple; music and the options (music volume, brightness). All tested in Play
  mode. Next (README Todo): the director's voice and sound effects, the itch.io page.
- Scenes: `MainMenu` (first in the build list, with its own copy of the director's set), the movie's `ZombieMovie`
  (the template's `SampleScene`, renamed; it keeps the template's Global Volume), `Labyrinth` and `Temple`, and
  `Director` (loaded additively by the movies).
- Art so far: five Kenney 3D packs (see Layout), added by the user; third-party assets go under
  `Assets/Externals/<Source>/<Pack>/`, each with a README credit line.
- Template tutorial content (`Assets/TutorialInfo/`, `Assets/Readme.asset`) removed by the user.
- Player Settings `productName` is "Lost in Direction"; `companyName` is still `DefaultCompany`.

## Layout

| Path | What it is |
|---|---|
| `Assets/Externals/UnityCommon` | Git submodule, shared library (see [setup.md](setup.md)) |
| `Assets/Externals/Kenney.NL` | Kenney CC0 packs, FBX format: `BlockyCharacters`, `GraveyardKit`, `MiniCharacters`, `MiniDungeon`, `MiniForest` (credited in the README) |
| `Assets/Externals/Zanerlak` | zanerlak's Low-Poly Movie Camera (`camera.fbx`, CC-BY 4.0, Sketchfab; credited in the README) |
| `Assets/Externals/tech.service.allwe` | tech.service.allwe's slate model (`Slate.fbx`, its textures and materials; CC-BY 4.0, Sketchfab, credited in the README) and the user's `SlateCtrl.controller` |
| `Assets/Scenes` | `MainMenu`, `ZombieMovie` (the first movie; `ZombieMovie/` holds its baked NavMesh), `Labyrinth` (the second; `Labyrinth/` its NavMesh), `Temple` (the third; `Temple/` its NavMesh), `Director` (loaded additively by the movies) |
| `Assets/Scripts` | Game code: `Player/` (`Player`, `MovementDirectionalXZ`, `Archer`), `Characters/` (`CharacterAnimation`, `Zombie`, `Orc`, `OrcSpawner`, `Watcher`), `Interaction/` (`Interactable`, `Verb`, `StartupActions`, `ActionTrigger`, `InteractionPrompt`, `Actions/`, `Conditions/`), `Cutscene/` (`Cutscene`, `ShotFollow`), `Director/` (`Director`, `DirectorBody`, `DirectorFace`, `DirectorDriver`, `DirectorSpeech`, `Gibberish`, `Slate`), `Scenes/` (`AdditiveSceneLoader`), `Props/` (`SpikeTrap`, `Coin`, `Arrow`, `ForceField`, `Spinner`, `BloodDecal`, `RandomSpot`), `UI/` (`TakesDisplay`, `MainMenu`, `ScreenFlash`), `Editor/` (`VerbEditor`, `StartupActionsEditor`, `ActionTriggerEditor`, `OrcSpawnerEditor`); `Gameplay.cs` (gameplay hold), `GameManager.cs` (takes, the cut, scene complete), `Globals.cs` (UnityCommon globals) |
| `Assets/Prefabs` | `GameManager`, `SoundManager` (the user's), `Zombie` (spawned from the grave), `Candles`, `FireBasket`, `LightpostDouble`, `LightpostSingle`, `UI_InventoryItem`, `UI_FloatingText`, `UI_TakeIcon`, `UI_CreditsLine`, `Arrow`, `Orc`, `BloodSplash`, `BloodDecal` |
| `Assets/Audio` | `Music/` (the user's four songs: Searching the Lost Maze, The Call to Adventure, The Creeping Motif, Waltz of the Café) |
| `Assets/Resources` | `Globals` (UnityCommon's globals asset) |
| `Assets/Sprites` | `Items/` (item icons: `Shovel`, `Key`, `Bow`), `UI/` (`Slate`, the takes icon) |
| `Assets/Fonts` | `BarlowCondensed-Black` (TTF and its TMP SDF asset), for all the text; `BarlowCondensed-Black SDF Outline` (material preset: outline and shadow, for text over the set) |
| `Assets/TextMesh Pro` | TMP essential resources |
| `Assets/Data` | Hypertags (`MainPlayer`, `MainCamera`, `ZombieSpawn`, `Zombie`, `InventoryDisplay`, `CryptEntrance`, `LabyrinthExit`, `TempleCage`); `Items/` (`Shovel`, `Key`, `Bow`); `credits.txt` (the menu's credits) |
| `Assets/Materials` | `SetGround`, `StudioFloor` (URP Lit, flat colours); `DirtParticles` (URP Particles Simple Lit); `FlameParticles` (additive URP Particles Unlit); `Director`; `GoldGlow` (the Mini Dungeon colormap, emissive: coins, the key, the artifact); `ForceField` (transparent, cyan, emissive); `BloodDecal` (URP Lit, transparent, the splats atlas), `BloodParticles` (URP Particles Unlit, alpha); `NoFriction.physicMaterial` (player capsule); `Textures/` (`director_base`, `director_faces`, `DirectorView.renderTexture`, `colormap_lightpost`, `BloodSplat` (four splats, 2x2), `BloodDrop`) |
| `Assets/Settings` | URP assets (`PC_RPAsset`, `Mobile_RPAsset`), renderers, volume profiles, `MainMixer.mixer` (the user's audio mixer), `InputSystem_Actions.inputactions` (the template's Input System actions, plus the game's: `Interact`, `NextOption`, `Aim`, the menus' `MoveHorizontal`, `MoveVertical`, `UIConfirm`, `UIBack`, and `Shoot` for the bow) |
| `docs/` | This context |
| `README.md`, `LICENSE` | Player-facing description and credits; MIT license |

New content follows the other GCC projects: code in `Assets/Scripts/`, and sibling folders `Prefabs/`,
`Materials/`, `Audio/`, `Data/`, `Fonts/`, ... as needed.
