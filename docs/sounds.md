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

Closed (2026-10-26): every sound is done, apart from four the user decided weren't needed.

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
| `director_babble` | Each blip of his gibberish while a balloon types out (`DirectorSpeech.voice`, every 3 characters; the README's "director voice") | one-shot 2D | Voice | 4-6 syllables | **must** ✓ `DirectorVoice` (the user's pick: Timbre's "the muppet's aren't in" remix, Freesound, CC-BY-NC 4.0; SkinFactory's `boss_blips` before) |
| `director_pleased` | The mood goes up a face (right actions, kills, the key) | one-shot 2D | Voice | 2-3 | **must** ✓ `DirectorPleased` (Freesound Mmm or Mm - Female Voice Various), `DirectorSpeech.pleasedSound` |
| `director_annoyed` | The mood goes down (wrong verbs, hits, the locked cage) | one-shot 2D | Voice | 2-3 | **must** ✓ `DirectorAnnoyed` (Freesound Angry frustrated and disappointed), `DirectorSpeech.annoyedSound` |
| `director_impatient` | Frustration stays high (a sigh, a tut, fingers tapping), now and then | one-shot 2D | Voice | 2-3 | not needed (the user's call, 2026-10-26) |
| `balloon_pop` | A speech balloon appears | one-shot 2D | FX2 | 1 | *polish* ✓ `BalloonPop` (Freesound Bubble_5, a bubble pop; Interface `open_001` first) |

### Takes, the slate and the cut

| Sound | When | Type | Group | Vars | Need |
|---|---|---|---|---|---|
| `slate_clap` | The clapperboard snaps shut, "action!" (`Slate.clapSound`; also on the menu's Play) | one-shot 2D | FX | 1 | **must** ✓ `SlateClap` (Freesound "film_slate" by themfish); also with "CUT!" and "CUT! PRINT IT!" (`GameManager.slateSound`) |
| `cut_sting` | "CUT!": the mood ran out (a record scratch, or the crew groaning) | one-shot 2D | FX | 1 | **must** ✓ `CutSting` (Interface `scratch_004`) |
| `take_lost` | A slate icon breaks off the takes display | one-shot 2D | FX2 | 1 | *polish* ✓ `TakeLost` (Impact `impactPlank_medium`, 5) |
| `fired_sting` | "YOU'RE FIRED!" (a sad trombone) | one-shot 2D | FX | 1 | **must** ✓ `FiredSting` (Music Jingles `jingles_SAX07`) |
| `print_it_sting` | "CUT! PRINT IT!": a scene done (the crew's applause) | one-shot 2D | FX | 1 | **must** ✓ `PrintSting` (`jingles_SAX02`) |
| `wrap_sting` | "THAT'S A WRAP!": the movie done (bigger applause, cheers) | one-shot 2D | FX | 1 | **must** ✓ `WrapSting` (`jingles_HIT15`) |
| `cutscene_bars` | The cutscene bars slide in and out | one-shot 2D | FX2 | 1 | *polish* ✓ `BarsIn` (RPG Audio `cloth1`) and `BarsOut` (the same, backwards) |

### The actor

| Sound | When | Type | Group | Vars | Need |
|---|---|---|---|---|---|
| `step_dirt` | Footsteps on the graveyard ground | one-shot 3D | FX | 4-6 | **must** ✓ `FootstepGrass` (Impact `footstep_grass`, 5) |
| `step_stone` | Footsteps in the labyrinth and the temple | one-shot 3D | FX | 4-6 | **must** ✓ `FootstepStone` (Impact `footstep_concrete`, 5; the labyrinth and the temple) |
| `interact` | The actor performing a verb, before the verb's own sound (`Globals.interactSnd`) | one-shot 2D | FX | 5 | **must** ✓ `Interact` (Impact `impactWood_light`, soft: 0.3) |
| `verb_cycle` | Cycling the options on an object (NextOption) | one-shot 2D | FX2 | 1 | **must** ✓ `VerbCycle` (Interface `tick_004`) |
| `pickup` | Anything picked up going into the inventory (with the item's own sound) | one-shot 2D | FX | 1 | **must** ✓ `Pickup` (Interface `pluck_001`) |
| `actor_hurt` | Hurt by a spike trap or an orc (with the red flash and "Ouch!"): an impact and his "oof" | one-shot 3D | Voice | 3 | **must** ✓ `Hurt` (Freesound Me Hurt Sounds, a grunt, pitch 0.9 to 1.1; Globals, from the traps and the orcs) |

## Main menu

| Sound | When | Type | Group | Vars | Need |
|---|---|---|---|---|---|
| `menu_room_tone` | Behind the menu: an empty film studio (a soft hum, a crew murmuring far off) | loop 2D | Ambient | 1 | not needed (the user's call, 2026-10-26) |
| (UI, slate) | Navigating, the options, Play clapping the slate: the shared sounds above | | | | |
| `menu_whoosh` | Play: the menu fading out before the slate | one-shot 2D | FX2 | 1 | *polish* ✓ `MenuWhoosh` (RPG Audio `cloth4`, low) |

## Zombie movie (the graveyard)

| Sound | When | Type | Group | Vars | Need |
|---|---|---|---|---|---|
| `graveyard_ambience` | The set at night: wind, crickets, an owl or a crow now and then | loop 2D | Ambient | 1 | **must** ✓ `GraveyardAmbience` (Freesound Night_crickets_Loopable, 0.25, `AmbientLoop`) |
| `fire_crackle` | The fire baskets (also in the temple) | loop 3D | Ambient | 1 | **must** ✓ an AudioSource on the `FireBasket` prefab (Freesound Fire Burning) |
| `candle_out` | Candles: "Put out" (a blow, the flame going) | one-shot 3D | FX | 1 | **must** ✓ `CandleOut` (Freesound 17-puff) |
| `candle_light` | Candles: "Light" (a match struck, the flame catching) | one-shot 3D | FX | 1 | **must** ✓ `CandleLight` (RPG Audio `metalClick`, a lighter; a placeholder) |
| `shovel_pickup` | The shovel: "Pick up" (a metal clank) | one-shot 3D | FX | 1 | **must** ✓ `ShovelPickup` (Impact `impactMetal_light`, 5) |
| `shovel_lean` | The shovel: "Lean on" (a creak, a sigh) | one-shot 3D | FX | 1 | *polish* ✓ `ShovelLean` (RPG Audio `creak`, 3) |
| `gravestone_read` | Gravestones: "Read" (a brush over stone, a thoughtful "hm") | one-shot 3D | FX | 2 | *polish* ✓ `GravestoneRead` (RPG Audio `bookFlip`, 3) |
| `dig` | Digging, the right grave and the wrong ones (a shovel in dirt) | one-shot 3D | FX | 3 | **must** ✓ `Dig` (Freesound New Recording dig, cut when the digging's done) |
| `pray` | Gravestones: "Pray" (a soft hum, a small chime) | one-shot 3D | FX | 1 | *polish* ✓ `Pray` (Interface `glass_004`, low and quiet) |
| `pumpkin_kick` | Pumpkins: "Kick" (a hollow thud) | one-shot 3D | FX | 2 | **must** ✓ `PumpkinKick` (Impact `impactSoft_medium`, 5) |
| `pumpkin_carve` | Pumpkins: "Carve" (a knife through a squash) | one-shot 3D | FX | 1 | *polish* ✓ `PumpkinCarve` (RPG Audio `knifeSlice`, 2) |
| `coffin_knock` | The coffin: "Knock" (three knocks on wood) | one-shot 3D | FX | 1 | **must** ✓ `CoffinKnock` (Impact `impactWood_medium`, 5; three knocks) |
| `coffin_knock_back` | The faint knock back from inside, a moment later | one-shot 3D | FX | 1 | **must** ✓ `CoffinKnockBack` (Impact `impactWood_light`, quiet and low; two) |
| `zombie_emerge` | The dig cutscene: the ground bursts and the zombie climbs out (rumble, dirt) | one-shot 3D | FX | 1 | not needed (the user's call): its grunt as it appears |
| `zombie_groan` | The zombie, now and then while it follows (and as it comes out) | one-shot 3D | Voice | 4 | **must** ✓ `ZombieGroan` (Freesound Zombie_Thrown), 3D |
| `zombie_shuffle` | The zombie's dragging footsteps | one-shot 3D | FX | 3 | *polish* ✓ `ZombieStep` (Impact `footstep_grass`, low), 3D, a `Footsteps` on the zombie |
| `zombie_close` | The zombie right next to the actor (a growl, with the mood draining) | one-shot 3D | Voice | 2 | not needed (the user's call, 2026-10-26) |
| `crypt_door` | Reaching the crypt (a heavy door creaking) | one-shot 3D | FX | 1 | *polish* ✓ `CryptDoor` (Freesound Hidden Wall Opening), with a hold before the cut |

## Labyrinth

| Sound | When | Type | Group | Vars | Need |
|---|---|---|---|---|---|
| `dungeon_ambience` | The maze: low wind through corridors, distant drips, far rumbles | loop 2D | Ambient | 1 | **must** ✓ `LabyrinthAmbience` (Freesound Water drops in crypt - Arles, streamed, 0.6) |
| `torch_crackle` | The actor's torch, close and quiet | loop 3D | Ambient | 1 | *polish* ✓ an AudioSource on `Player/Torch` (the fire clip, 0.15) |
| `coin` | Each coin (pitch going up a little for coins picked up in quick succession) | one-shot 2D | FX | 1 | **must** ✓ `Coin` (Interface `glass`, 4), the streak on `Coin` |
| `key_pickup` | The key (a jingle; also the temple's) | one-shot 3D | FX | 1 | **must** ✓ `KeyPickup` (RPG Audio `handleCoins2`) |
| `barrel_inspect` | Barrels: "Inspect" (a knock on wood) | one-shot 3D | FX | 3 | **must** ✓ `BarrelKnock` (Impact `impactPlank_medium`, 5) |
| `barrel_hollow` | The barrel that's "Hollow. The sound echoes for a while." (a knock with a long echo) | one-shot 3D | FX | 1 | *polish* ✓ `BarrelHollow` (the same, pitched to 0.65; no echo) |
| `locked` | The exit door without the key (a rattle; also the temple's cage) | one-shot 3D | FX | 1 | **must** ✓ `Locked` (RPG Audio `metalLatch`) |
| `unlock` | The key turning in the exit door | one-shot 3D | FX | 1 | **must** ✓ `Unlock` (RPG Audio `metalClick`, pitch 0.8) |
| `gate_open` | The exit gate opening (iron and stone grinding) | one-shot 3D | FX | 1 | **must** ✓ `GateOpen` (RPG Audio `doorOpen`, 2, pitch 0.8), 0.3 s after the click |
| `spikes_up` | A spike trap's spikes shooting up (a metal shing) | one-shot 3D | FX | 2 | **must** ✓ `SpikesUp` (RPG Audio `drawKnife`, 3), 3D, only within 6 of the listener |
| `spikes_down` | Spikes sliding back down | one-shot 3D | FX | 1 | *polish* ✓ `SpikesDown` (RPG Audio `drawKnife`, 3, played backwards: the SoundDef's `reverse`), 3D, only within 6 |

## Temple

| Sound | When | Type | Group | Vars | Need |
|---|---|---|---|---|---|
| `temple_ambience` | The hall: a big stone room's air, a low drone, distant wind | loop 2D | Ambient | 1 | **must** ✓ `TempleAmbience` (Freesound ambience 4, streamed, 0.4) |
| (fire, candles) | The fire baskets and candles: `fire_crackle` above | | | | |
| `bow_pickup` | The bow: "Pick up" (wood and a string) | one-shot 3D | FX | 1 | **must** ✓ `BowPickup` (RPG Audio `handleSmallLeather`, 2) |
| `bow_draw` | Starting to aim (the right stick): the string drawn back | one-shot 3D | FX | 2 | *polish* ✓ `BowDraw` (RPG Audio `creak`, 3, pitched up), `Archer.drawSound` |
| `bow_shoot` | Each shot (a twang and the arrow's whoosh) | one-shot 3D | FX | 3 | **must** ✓ `BowShoot` (Freesound Bow_release), `Archer.shootSound` |
| `arrow_wall` | An arrow sticking in a wall | one-shot 3D | FX | 3 | **must** ✓ `ArrowWall` (Impact `impactMining`, 5), 3D, `Arrow.wallSound` |
| `arrow_hit` | An arrow hitting an orc (with the blood: a wet impact) | one-shot 3D | FX | 3 | **must** ✓ `ArrowHit` (Impact `impactPunch_medium`, 5), 3D, `Orc.hitSound` |
| `orc_spawn` | An orc coming out of its alcove (a snarl) | one-shot 3D | Voice | 3 | **must** ✓ `OrcSpawn` (the zombie's Zombie_Thrown, pitch 0.7 to 0.85: its own SoundDef, to replace apart), 3D, `Orc.spawnSound` |
| `orc_step` | Orc footsteps, heavy | one-shot 3D | FX | 4 | *polish* ✓ `OrcStep` (Impact `footstep_concrete`, low; 4 at most at once), 3D, a `Footsteps` on the orc |
| `orc_attack` | An orc's swing (a whoosh and a grunt; `actor_hurt` if it lands) | one-shot 3D | Voice | 3 | **must** ✓ `OrcAttack` (RPG Audio `cloth`, 4, low: a swish), 3D, `Orc.attackSound`; the grunt is still the user's |
| `orc_death` | An orc dying | one-shot 3D | Voice | 4 | **must** ✓ `OrcDeath` (Freesound Orc Grunt 3), 3D, `Orc.deathSound` |
| `blood_splat` | Blood hitting the floor | one-shot 3D | FX | 3 | *polish* ✓ `BloodSplat` (Impact `impactSoft_medium`, high and quiet), 3D, `Orc.splatSound` |
| `wave_horn` | A wave starting (a war horn, far off) | one-shot 2D | FX | 1 | **must** ✓ `WaveStart` (Freesound Dramatic9), `OrcSpawner.waveStartSound` |
| `wave_cleared` | The last orc of a wave down (a short drum hit) | one-shot 2D | FX | 1 | *polish* ✓ `WaveCleared` (`jingles_HIT13`), `OrcSpawner.waveClearedSound` |
| (the key) | Found in its alcove: `key_pickup` above | | | | |
| (locked cage) | The cage without the key: `locked` above, and `director_annoyed` for the two faces | | | | |
| `cage_open` | Free her: the spears pulled out of the cage (wood clattering) | one-shot 3D | FX | 1 | **must** ✓ `CageOpen` (Impact `impactPlank_medium`, three 0.12 s apart) |
| `girl_step` | The girl's footsteps, light | one-shot 3D | FX | 3 | *polish* ✓ `GirlStep` (Impact `footstep_concrete`, high and quiet), a `Footsteps` on her |
| `kiss` | Her "Mwah!" | one-shot 3D | Voice | 1 | **must** ✓ `Kiss` (Freesound Finger Kiss), with the "Mwah!" |
| `spell_start` | She starts casting (a rising shimmer) | one-shot 3D | FX | 1 | **must** ✓ `SpellStart` (Interface `maximize_005`) |
| `spell_loop` | While she casts (a magic hum, sparkles) | loop 3D | FX | 1 | **must** ✓ an AudioSource on `Girl/Spell` (Freesound Magic Powerup Loop, its trailing silence trimmed), fading in and out with the particles (`ParticleLoopSound`) |
| `forcefield_hum` | The force field round the artifact, breathing with its light | loop 3D | Ambient | 1 | **must** ✓ an AudioSource on the field (Freesound Magic - Magnetic Force Field), fading out with it |
| `forcefield_dissolve` | The field going down (a glassy shatter, then sparkles falling) | one-shot 3D | FX | 1 | **must** ✓ `FieldShatter` (Impact `impactGlass_heavy`, 5), 3D, `ForceField.lowerSound` |
| `artifact_hum` | The golden shield, spinning (a soft shimmer) | loop 3D | Ambient | 1 | *polish* ✓ the same hum on the shield, pitch 1.5, 0.2 |
| `artifact_take` | "Take": the artifact (a bright magical chime) | one-shot 3D | FX | 1 | **must** ✓ `ArtifactTake` (Interface `confirmation_004`) |

## Totals and sources

74 sounds: 52 **must**, 22 *polish*. Several are shared across sets (fire, the key, locked, the actor's
hurt and footsteps), so the zombie scene, the labyrinth and the temple each need only their own column on top of
the shared ones.

To hook them up, the game needs a `Sound/Play` action (a `SoundDef`, from the actor, the interactable or an object)
for the verbs and cutscenes, and the props, characters and scenes need their own fields for the rest (loops,
footsteps on the walk animation, the trap, the arrow, the orcs).

Possible free sources (CC0): Kenney's audio packs (Interface Sounds, Impact Sounds, RPG Audio, Casino Audio, Music
Jingles for the stings), like the art; whatever comes from elsewhere goes in the README's Sound credits.
