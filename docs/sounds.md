# Sound effects

The sounds the game needs (2026-10-25), set by set, starting with the main menu. Music is done (see
[project.md](project.md), Music and options); this is everything else.

- **Type**: one-shot or loop; 2D (flat, UI and the director) or 3D (positional, from the object).
- **Group**: the `MainMixer` group it should play through: `FX` (gameplay), `FX2` (UI), `Ambient` (beds and loops),
  `Voice` (the director, the actor, the girl, the orcs).
- **Vars**: how many variants, so repeats don't sound the same (UnityCommon's `SoundDef` picks one, with pitch and
  volume ranges).
- **Need**: **must** for the game to feel finished, *polish* if there's time. Done ones are marked ✓, with their
  SoundDef (in `Assets/Audio/SFX`) and clip.

## Everywhere

### Menus and UI (UnityCommon's `Globals` already has the three slots)

| Sound | When | Type | Group | Vars | Need |
|---|---|---|---|---|---|
| `ui_move` | Moving between options (`uiMoveSnd`) | one-shot 2D | FX2 | 1 | **must** ✓ `UI_Move` (Interface Sounds `select_001`) |
| `ui_select` | Confirming an option (`uiSelectSnd`) | one-shot 2D | FX2 | 1 | **must** ✓ `UI_Select` (`confirmation_001`) |
| `ui_change_value` | Music volume, brightness left/right (`uiChangeValueSnd`) | one-shot 2D | FX2 | 1 | **must** ✓ `UI_ChangeValue` (`tick_002`) |
| `ui_back` | Back out of the options or the credits (`MainMenu.backSound`) | one-shot 2D | FX2 | 1 | **must** ✓ `UI_Back` (`back_002`) |

### The director (his portrait, in every movie scene)

| Sound | When | Type | Group | Vars | Need |
|---|---|---|---|---|---|
| `director_babble` | Each blip of his gibberish while a balloon types out (`DirectorSpeech.voice`, every 3 characters; the README's "director voice") | one-shot 2D | Voice | 4-6 syllables | **must** |
| `director_pleased` | The mood goes up a face (right actions, kills, the key) | one-shot 2D | Voice | 2-3 | **must** |
| `director_annoyed` | The mood goes down (wrong verbs, hits, the locked cage) | one-shot 2D | Voice | 2-3 | **must** |
| `director_impatient` | Frustration stays high (a sigh, a tut, fingers tapping), now and then | one-shot 2D | Voice | 2-3 | *polish* |
| `balloon_pop` | A speech balloon appears | one-shot 2D | FX2 | 1 | *polish* |

### Takes, the slate and the cut

| Sound | When | Type | Group | Vars | Need |
|---|---|---|---|---|---|
| `slate_clap` | The clapperboard snaps shut, "action!" (`Slate.clapSound`; also on the menu's Play) | one-shot 2D | FX | 1 | **must** ✓ `SlateClap` (Freesound "film_slate" by themfish) |
| `cut_sting` | "CUT!": the mood ran out (a record scratch, or the crew groaning) | one-shot 2D | FX | 1 | **must** |
| `take_lost` | A slate icon breaks off the takes display | one-shot 2D | FX2 | 1 | *polish* |
| `fired_sting` | "YOU'RE FIRED!" (a sad trombone) | one-shot 2D | FX | 1 | **must** |
| `print_it_sting` | "CUT! PRINT IT!": a scene done (the crew's applause) | one-shot 2D | FX | 1 | **must** |
| `wrap_sting` | "THAT'S A WRAP!": the movie done (bigger applause, cheers) | one-shot 2D | FX | 1 | **must** |
| `cutscene_bars` | The cutscene bars slide in and out | one-shot 2D | FX2 | 1 | *polish* |

### The actor

| Sound | When | Type | Group | Vars | Need |
|---|---|---|---|---|---|
| `step_dirt` | Footsteps on the graveyard ground | one-shot 3D | FX | 4-6 | **must** |
| `step_stone` | Footsteps in the labyrinth and the temple | one-shot 3D | FX | 4-6 | **must** |
| `verb_cycle` | Cycling the options on an object (NextOption) | one-shot 2D | FX2 | 1 | **must** |
| `pickup` | Anything picked up going into the inventory (with the item's own sound) | one-shot 2D | FX | 1 | **must** |
| `actor_hurt` | Hurt by a spike trap or an orc (with the red flash and "Ouch!"): an impact and his "oof" | one-shot 3D | Voice | 3 | **must** |

## Main menu

| Sound | When | Type | Group | Vars | Need |
|---|---|---|---|---|---|
| `menu_room_tone` | Behind the menu: an empty film studio (a soft hum, a crew murmuring far off) | loop 2D | Ambient | 1 | *polish* |
| (UI, slate) | Navigating, the options, Play clapping the slate: the shared sounds above | | | | |
| `menu_whoosh` | Play: the menu fading out before the slate | one-shot 2D | FX2 | 1 | *polish* |

## Zombie movie (the graveyard)

| Sound | When | Type | Group | Vars | Need |
|---|---|---|---|---|---|
| `graveyard_ambience` | The set at night: wind, crickets, an owl or a crow now and then | loop 2D | Ambient | 1 | **must** |
| `fire_crackle` | The fire baskets (also in the temple) | loop 3D | Ambient | 1 | **must** |
| `candle_out` | Candles: "Put out" (a blow, the flame going) | one-shot 3D | FX | 1 | **must** |
| `candle_light` | Candles: "Light" (a match struck, the flame catching) | one-shot 3D | FX | 1 | **must** |
| `shovel_pickup` | The shovel: "Pick up" (a metal clank) | one-shot 3D | FX | 1 | **must** |
| `shovel_lean` | The shovel: "Lean on" (a creak, a sigh) | one-shot 3D | FX | 1 | *polish* |
| `gravestone_read` | Gravestones: "Read" (a brush over stone, a thoughtful "hm") | one-shot 3D | FX | 2 | *polish* |
| `dig` | Digging, the right grave and the wrong ones (a shovel in dirt) | one-shot 3D | FX | 3 | **must** |
| `pray` | Gravestones: "Pray" (a soft hum, a small chime) | one-shot 3D | FX | 1 | *polish* |
| `pumpkin_kick` | Pumpkins: "Kick" (a hollow thud) | one-shot 3D | FX | 2 | **must** |
| `pumpkin_carve` | Pumpkins: "Carve" (a knife through a squash) | one-shot 3D | FX | 1 | *polish* |
| `coffin_knock` | The coffin: "Knock" (three knocks on wood) | one-shot 3D | FX | 1 | **must** |
| `coffin_knock_back` | The faint knock back from inside, a moment later | one-shot 3D | FX | 1 | **must** |
| `zombie_emerge` | The dig cutscene: the ground bursts and the zombie climbs out (rumble, dirt) | one-shot 3D | FX | 1 | **must** |
| `zombie_groan` | The zombie, now and then while it follows (and as it comes out) | one-shot 3D | Voice | 4 | **must** |
| `zombie_shuffle` | The zombie's dragging footsteps | one-shot 3D | FX | 3 | *polish* |
| `zombie_close` | The zombie right next to the actor (a growl, with the mood draining) | one-shot 3D | Voice | 2 | *polish* |
| `crypt_door` | Reaching the crypt (a heavy door creaking) | one-shot 3D | FX | 1 | *polish* |

## Labyrinth

| Sound | When | Type | Group | Vars | Need |
|---|---|---|---|---|---|
| `dungeon_ambience` | The maze: low wind through corridors, distant drips, far rumbles | loop 2D | Ambient | 1 | **must** |
| `torch_crackle` | The actor's torch, close and quiet | loop 3D | Ambient | 1 | *polish* |
| `coin` | Each coin (pitch going up a little for coins picked up in quick succession) | one-shot 2D | FX | 1 | **must** |
| `key_pickup` | The key (a jingle; also the temple's) | one-shot 3D | FX | 1 | **must** |
| `barrel_inspect` | Barrels: "Inspect" (a knock on wood) | one-shot 3D | FX | 3 | **must** |
| `barrel_hollow` | The barrel that's "Hollow. The sound echoes for a while." (a knock with a long echo) | one-shot 3D | FX | 1 | *polish* |
| `locked` | The exit door without the key (a rattle; also the temple's cage) | one-shot 3D | FX | 1 | **must** |
| `unlock` | The key turning in the exit door | one-shot 3D | FX | 1 | **must** |
| `gate_open` | The exit gate opening (iron and stone grinding) | one-shot 3D | FX | 1 | **must** |
| `spikes_up` | A spike trap's spikes shooting up (a metal shing) | one-shot 3D | FX | 2 | **must** |
| `spikes_down` | Spikes sliding back down | one-shot 3D | FX | 1 | *polish* |

## Temple

| Sound | When | Type | Group | Vars | Need |
|---|---|---|---|---|---|
| `temple_ambience` | The hall: a big stone room's air, a low drone, distant wind | loop 2D | Ambient | 1 | **must** |
| (fire, candles) | The fire baskets and candles: `fire_crackle` above | | | | |
| `bow_pickup` | The bow: "Pick up" (wood and a string) | one-shot 3D | FX | 1 | **must** |
| `bow_draw` | Starting to aim (the right stick): the string drawn back | one-shot 3D | FX | 2 | *polish* |
| `bow_shoot` | Each shot (a twang and the arrow's whoosh) | one-shot 3D | FX | 3 | **must** |
| `arrow_wall` | An arrow sticking in a wall | one-shot 3D | FX | 3 | **must** |
| `arrow_hit` | An arrow hitting an orc (with the blood: a wet impact) | one-shot 3D | FX | 3 | **must** |
| `orc_spawn` | An orc coming out of its alcove (a snarl) | one-shot 3D | Voice | 3 | **must** |
| `orc_step` | Orc footsteps, heavy | one-shot 3D | FX | 4 | *polish* |
| `orc_attack` | An orc's swing (a whoosh and a grunt; `actor_hurt` if it lands) | one-shot 3D | Voice | 3 | **must** |
| `orc_death` | An orc dying | one-shot 3D | Voice | 4 | **must** |
| `blood_splat` | Blood hitting the floor | one-shot 3D | FX | 3 | *polish* |
| `wave_horn` | A wave starting (a war horn, far off) | one-shot 2D | FX | 1 | **must** |
| `wave_cleared` | The last orc of a wave down (a short drum hit) | one-shot 2D | FX | 1 | *polish* |
| (the key) | Found in its alcove: `key_pickup` above | | | | |
| (locked cage) | The cage without the key: `locked` above, and `director_annoyed` for the two faces | | | | |
| `cage_open` | Free her: the spears pulled out of the cage (wood clattering) | one-shot 3D | FX | 1 | **must** |
| `girl_step` | The girl's footsteps, light | one-shot 3D | FX | 3 | *polish* |
| `kiss` | Her "Mwah!" | one-shot 3D | Voice | 1 | **must** |
| `spell_start` | She starts casting (a rising shimmer) | one-shot 3D | FX | 1 | **must** |
| `spell_loop` | While she casts (a magic hum, sparkles) | loop 3D | FX | 1 | **must** |
| `forcefield_hum` | The force field round the artifact, breathing with its light | loop 3D | Ambient | 1 | **must** |
| `forcefield_dissolve` | The field going down (a glassy shatter, then sparkles falling) | one-shot 3D | FX | 1 | **must** |
| `artifact_hum` | The golden shield, spinning (a soft shimmer) | loop 3D | Ambient | 1 | *polish* |
| `artifact_take` | "Take": the artifact (a bright magical chime) | one-shot 3D | FX | 1 | **must** |

## Totals and sources

74 sounds: 52 **must**, 22 *polish*. Several are shared across sets (fire, the key, locked, the actor's
hurt and footsteps), so the zombie scene, the labyrinth and the temple each need only their own column on top of
the shared ones.

To hook them up, the game needs a `Sound/Play` action (a `SoundDef`, from the actor, the interactable or an object)
for the verbs and cutscenes, and the props, characters and scenes need their own fields for the rest (loops,
footsteps on the walk animation, the trap, the arrow, the orcs).

Possible free sources (CC0): Kenney's audio packs (Interface Sounds, Impact Sounds, RPG Audio, Casino Audio, Music
Jingles for the stings), like the art; whatever comes from elsewhere goes in the README's Sound credits.
