# Setup and tooling

## Project facts

- Editor **6000.6.0f1** (Unity 6.6). Don't upgrade unless asked: UnityCommon is tracked on a 6.6-specific branch.
- **URP** 17.6.0, 3D renderer.
- **Input System only** (`activeInputHandler: 1`): the legacy `UnityEngine.Input` API is unavailable.
- Enter Play Mode Options on, **domain reload and scene reload disabled** (`m_EnterPlayModeOptions: 3`): statics
  survive between play sessions, so reset them explicitly
  (`[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]`).
- Git LFS for binaries (`.gitattributes` copied from the other GCC projects: images, audio, video, models, fonts,
  archives, DLLs, PDFs). `.gitignore` is the standard Unity one, same as the other GCC projects.

## Packages added on top of the template

| Package | Source | Why |
|---|---|---|
| `com.dbrizov.naughtyattributes` | `https://github.com/dbrizov/NaughtyAttributes.git#upm` | Inspector attributes; required by UnityCommon |
| `com.unity.pipeline` | Unity registry `0.9.1-exp.1` (experimental) | Lets the Unity CLI drive the open Editor (see below) |

`com.unity.ai.assistant` (Unity MCP server) was installed at first and removed on 2026-10-24 in favour of the CLI;
don't add it back unless asked.

## UnityCommon submodule

`Assets/Externals/UnityCommon` = https://github.com/DiogoDeAndrade/UnityCommon, namespace `UC`, tracking branch
**`unity6.6`** (`main` doesn't compile on 6.6; `.gitmodules` has `branch = unity6.6`). No asmdefs: it compiles into
`Assembly-CSharp` with the game code. Provides `Singleton<T>`, `SoundDef`/`SoundManager`, `Hypertag`,
`CooldownTimer`, UI helpers, a `.dialogue` system and more; check it before writing a generic utility.

- Changes to UnityCommon are commits in the submodule's own repo: commit (and push) there first, then commit the
  pointer bump here.
- Latest `unity6.6`: `git submodule update --remote Assets/Externals/UnityCommon`, then commit the pointer.

## New machine

1. `git lfs install`, then `git clone --recurse-submodules <url>` (or `git submodule update --init --recursive`
   on an existing clone). Check `git -C Assets/Externals/UnityCommon branch --show-current` is `unity6.6`.
2. Set the identity per repository, as in the other GCC repos:
   `git config user.name "Diogo de Andrade"` and `git config user.email "diogo.g.andrade@gmail.com"`.
3. Unity CLI (`unity --version`; install from https://docs.unity.com/en-us/unity-cli if missing), signed in
   (`unity auth status`) and licensed (`unity license status`). Install the editor with
   `unity install 6000.6.0f1 --yes --accept-eula` if `unity editors --installed` lacks it.
4. Open the project (`unity open <path>`) and wait for `unity status` to show it as `ready`; nothing else to configure.

## Driving the Editor (Unity CLI)

Act on the open Editor through the Unity CLI and the `com.unity.pipeline` package; this project does not use MCP.
Never hand-edit `.unity` / `.prefab` / `.asset` YAML while an Editor is open on the project: it won't see the
change, and GUIDs/fileIDs are easy to break.

- `unity status --format json`: the project's Editor should be listed with state `ready`.
- `unity command --format json`: the commands this Editor exposes (166 with package 0.9.1-exp.1), each with a
  `group` and a parameter schema; filter with `--query <term>` or `--tag <tag>`. Commands are invoked as
  `unity command <group> <name> ...` (the flat names in older docs, e.g. `get_scene_hierarchy`, no longer exist).
  Groups: `scene` (get_hierarchy, open, create, save, set_active, add_to_build), `gameobject` (create, create_many,
  find, set_transform, set_parent, rename, delete, set_active, set_layer, set_tag, selection), `component` (add,
  remove, get_properties, set_properties, set_field), `prefab`, `asset`, `material`, `script` (create, attach,
  recompile, eval, eval_file, run), `console` (get, clear), `capture` (screenshot, scene_view, game_view),
  `editor` / `editor.playmode` (start, stop, pause), `package`, `build`, `lighting`, `navmesh`, `test`,
  `animation.*`, `settings.*`, `runtime`, `job`.
- Run one with `unity command <group> <name> --project-path "C:/projects/GCC/LostInTranslation" --caller plugin --skill unity-cli ...`.
  Always pass `--project-path`, since other Editors may be open. Quick check that it's alive:
  `unity command eval 'return Application.unityVersion;' --project-path ...` returns `6000.6.0f1`.
- Can't connect? `unity pipeline list` shows whether the package is there, the server is reachable, or the
  Editor is in Safe Mode (compile errors stop the Pipeline from loading: fix the errors, then restart Unity).
- `unity pipeline upgrade --project-path <path>` moves to a newer package version when one is published.
- The package also ships a deeper `unity-pipeline` skill; `unity skill install claude-code --local` mirrors it
  into the project if it's wanted.

The user's global Unity MCP registration (`unity-mcp`, the AI Assistant relay) was removed on 2026-10-24 as well;
don't set up MCP for Unity unless asked.

## Compile checks without the Editor

- Build the generated project files (gitignored, regenerated by Unity; a new `.cs` file isn't in them until Unity
  regenerates them, so add a `<Compile Include=...>` line temporarily if needed):
  `& "C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe" Assembly-CSharp.csproj /t:Build /p:BuildProjectReferences=false /nologo /v:q /clp:ErrorsOnly`
  (the MSBuild path is machine-specific).
- Or, only when no Editor has the project open, a headless import:
  `unity run "<project>" --editor-version 6000.6.0f1 -- -logFile "<project>/Logs/import.log"`, then grep the log
  for `error CS`. The log path must be somewhere Unity can write (e.g. inside the project): with an unwritable path
  Unity exits with code 127 and writes nothing.
