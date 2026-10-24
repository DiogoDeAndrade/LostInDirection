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
      particles, zombie follow)
- [x] Cutscenes: letterbox bars, controls locked, UI faded out, cutscene camera
- [x] Atmosphere: candles and fire baskets with flames and flickering lights
- [x] Inventory, with the shovel's icon
- [ ] Zombie scene: pick up the shovel, dig the grave, a zombie comes out and follows the actor (done), reach the
      crypt, "Cut!" and restart when touching a zombie
- [ ] Scene targets: where to stand, where to face, which interaction is right
- [ ] Director hot/cold meter: position and orientation cues
- [ ] Director life system: wrong interactions make him angrier
- [x] Director in the bottom-left corner (3D, in his own scene): body language for hot/cold, face for mood
- [ ] Director gibberish (speech balloons and voice)
- [ ] Scene complete and fired / game over
- [ ] Main menu/credits screen
- [ ] Sounds and music
- [ ] Itch.io page

## Art

- [Blocky Characters](https://kenney.nl/assets/blocky-characters) by [KenneyNL](https://kenney.nl/), [CC0] license.
- [Graveyard Kit](https://kenney.nl/assets/graveyard-kit) by [KenneyNL](https://kenney.nl/), [CC0] license.
- [Mini Characters](https://kenney.nl/assets/mini-characters) by [KenneyNL](https://kenney.nl/), [CC0] license.
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
[SIL Open Font License]:https://openfontlicense.org/
[NaughtyAttributes]:https://github.com/dbrizov/NaughtyAttributes
[Unity Common]:https://github.com/DiogoDeAndrade/UnityCommon
[Game Creator's Club]:https://game-creators-club.itch.io/
[MIT]:LICENSE
