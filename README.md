# LOST IN TRANSLATION

Developed for the October'2026 [Game Creator's Club] at Lusofona University.
This month, the theme is "Lost In Translation".

## Game

You are an actor on a film set, and the director is very passionate about what he wants from this scene.
The problem is that you don't understand a single word he says.

Walk around the set and interact with the props, and keep an eye on the director in the corner of the screen.
He calms down as you get closer to where you should be and face the right way, and gets angrier every time you
do something you are not supposed to. Work out what he wants before he loses his patience.

The idea came from the scene in [Lost in Translation](https://en.wikipedia.org/wiki/Lost_in_Translation_(film))
(2003) where Bill Murray's character is shooting a commercial: the director gives him long, passionate
directions, and he doesn't understand any of it and feels the translation is not accurate.

## Todo

- [x] Build the set (zombie movie scene)
- [x] Player movement with the 45 degree camera
- [x] Interaction: get close to an object and face it (right stick aims while standing still), cycle its options, perform one
- [x] Verbs with conditions and actions (add item, destroy object, spawn prefab, cutscenes, wait, animation,
      particles, lights, floating text, director's mood, zombie follow)
- [x] Cutscenes: letterbox bars, controls locked, UI faded out, cutscene camera
- [x] Atmosphere: candles and fire baskets with flames and flickering lights
- [x] Inventory, with the shovel's icon
- [x] Zombie scene: pick up the shovel, dig the grave, a zombie comes out and follows the actor, reach the crypt
- [x] Scene targets: where to stand, where to face, which interaction is right
- [x] Director hot/cold meter: position and orientation cues (current target set by the script)
- [x] Director life system: wrong interactions make him angrier (red herrings on the candles, graves, pumpkins,
      the coffin and the shovel)
- [x] Right interactions calm the director down (picking up the shovel, digging the grave)
- [x] The director gets angrier while a zombie is right next to the actor
- [x] Director in the bottom-left corner (3D, in his own scene): body language for hot/cold, face for mood
- [x] Director gibberish in speech balloons (the briefing); no control and no frustration change while he talks;
      Interact skips to the next balloon
- [x] Slate claps in front of the director's camera before the take starts
- [ ] Director voice (babble blips while he talks)
- [x] "Cut!" when the director's mood runs out: everything stops, fade out, the take starts again
- [x] Takes (lives): three for the whole movie, shown as slates in the top-right corner; losing the last one gets
      a "YOU'RE FIRED!"
- [x] Scene complete: "CUT! PRINT IT!" and on to the movie's next scene; "THAT'S A WRAP!" after the last one
- [ ] Main menu/credits screen (being fired or finishing the movie should go back to it; for now the movie starts
      again with all the takes)
- [ ] Sounds and music
- [ ] Itch.io page

## Art

- [Blocky Characters](https://kenney.nl/assets/blocky-characters) by [KenneyNL](https://kenney.nl/), [CC0] license.
- [Graveyard Kit](https://kenney.nl/assets/graveyard-kit) by [KenneyNL](https://kenney.nl/), [CC0] license.
- [Mini Characters](https://kenney.nl/assets/mini-characters) by [KenneyNL](https://kenney.nl/), [CC0] license.
- [Low-Poly Movie Camera](https://sketchfab.com/3d-models/low-poly-movie-camera-39240299af7344f687c25b2e76493ddd) by [zanerlak](https://sketchfab.com/zanerlak), licensed through the [CC-BY 4.0] license.
- [Black film slate or clapper](https://sketchfab.com/3d-models/black-film-slate-or-clapper-1638e9374913402daf115e62cfd080a4) by [tech.service.allwe](https://sketchfab.com/tech.service.allwe), licensed through the [CC-BY 4.0] license.
- Font [Barlow Condensed](https://fonts.google.com/specimen/Barlow+Condensed) by Jeremy Tribby, [SIL Open Font License].
- Everything else done by [Diogo de Andrade], licensed through the [CC0] license.

## Sound

- Everything else done by [Diogo de Andrade], licensed through the [CC0] license.

## Code

- Uses [Unity Common], [MIT] license.
- [NaughtyAttributes] by Denis Rizov available through the [MIT] license: https://github.com/dbrizov/NaughtyAttributes.git#upm
- AI assistance was used in this game (Claude Code).
- All remaining game source code by Diogo de Andrade is licensed under the [MIT] license.

## Metadata

- Autor: [Diogo de Andrade]

[Diogo de Andrade]:https://github.com/DiogoDeAndrade
[CC0]:https://creativecommons.org/publicdomain/zero/1.0/
[CC-BY 3.0]:https://creativecommons.org/licenses/by/3.0/
[CC-BY-NC 3.0]:https://creativecommons.org/licenses/by-nc/3.0/
[CC-BY-SA 4.0]:http://creativecommons.org/licenses/by-sa/4.0/
[CC-BY 4.0]:https://creativecommons.org/licenses/by/4.0/
[CC-BY-NC 4.0]:https://creativecommons.org/licenses/by-nc/4.0/
[SIL Open Font License]:https://openfontlicense.org/[NaughtyAttributes]:https://github.com/dbrizov/NaughtyAttributes
[Unity Common]:https://github.com/DiogoDeAndrade/UnityCommon
[Game Creator's Club]:https://game-creators-club.itch.io/
[MIT]:LICENSE
