# Conventions

## Code

Same style as UnityCommon and the other GCC projects:

- Allman braces, 4-space indentation.
- Game scripts live in the global namespace and pull in UnityCommon with `using UC;`.
- Inspector fields are `[SerializeField] private`, camelCase, with types and names column-aligned in a block:

  ```csharp
  [SerializeField] private Player     playerPrefab;
  [SerializeField] private SoundDef   gameOverSound;
  [SerializeField] private float      respawnTime = 5.0f;

  CooldownTimer   playerRespawn = new();
  Vector3         respawnPosition;
  ```

- Private runtime fields have no access modifier and no prefix. An underscore prefix is only used for a serialized
  backing field exposed through a property (`[SerializeField] private bool _needLight;` / `public bool needLight => _needLight;`).
- Cache Animator parameter hashes in `static int ... = Animator.StringToHash(...)`.
- Use NaughtyAttributes for inspector polish (`[Button]`, `[ShowIf]`, `[Expandable]`, ...).
- Prefer an existing UnityCommon type (`Singleton<T>`, `SoundDef`, `Hypertag`, `CooldownTimer`, ...) over writing a new one.

## Root README

The root README follows the structure shared by the GCC projects; keep it:

1. `# TITLE` (uppercase), then `Developed for the <Month> [Game Creator's Club] at Lusofona University.` and
   `This month, the theme is "<theme>".`
2. `## Game` - what the game is and how it plays.
3. `## Todo` - checklist (`- [ ]` / `- [x]`).
4. `## Art`, `## Sound` - one bullet per third-party asset: `[Asset name](url) by [Author](author url), licensed through the [CC0] license.`
   (or `[CC-BY 4.0]`, etc.). Each list ends with `Everything else done by [Diogo de Andrade], licensed through the [CC0] license.`
5. `## Code` - UnityCommon, NaughtyAttributes and any other code dependency with its license, then
   `All remaining game source code by Diogo de Andrade is licensed under the [MIT] license.`
6. `## Metadata` - `- Autor: [Diogo de Andrade]`.
7. Reference-style link definitions at the bottom (license links, authors, `[MIT]:LICENSE`).

Add the credit line in the same commit that adds a third-party asset.

## Releases

When the first build ships, add `RELEASES.md` at the root, newest first:

```
# V1.0.0 (dd/mm/yyyy)

- Initial release, for the October Game Creators Club (theme "Lost In Translation")
```

## Commits

- One commit per logical change, message in plain English describing what changed, in the style of the other GCC
  repositories (a single descriptive line; several related changes joined with semicolons).
- No AI or tool attribution anywhere in the repository: no `Co-Authored-By` trailers, no "Generated with ..." footers,
  no assistant or tool names in commit messages, PR descriptions, code comments, docs or file names.
