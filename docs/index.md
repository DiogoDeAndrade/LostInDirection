# Context for working on this repository

This folder is working context for an AI coding assistant, so that a fresh clone on any machine carries everything
needed to pick the project up. It is not player- or contributor-facing documentation; that is the root `README.md`.

Read all of it before starting:

- [project.md](project.md) - what the project is, current state, layout
- [setup.md](setup.md) - Unity version, packages, UnityCommon submodule, new-machine setup, driving the Editor (MCP / CLI), compile checks
- [conventions.md](conventions.md) - code style, root README and credits, releases, commit messages

## Working rules (from the user)

- **Commit messages never mention Claude or AI**: no `Co-Authored-By` trailer, no "Generated with ..." line, no
  assistant names. Crediting Claude Code in the root README's `## Code` section is fine and expected.
- The root `README.md` stays at the repository root and follows the structure in [conventions.md](conventions.md).
- Keep this folder current. When a fact recorded here changes (packages, scenes, systems, decisions, task list
  context), update the relevant file in the same commit, so the next session on another machine starts with the full picture.
- The user is Diogo de Andrade (GitHub `DiogoDeAndrade`). Other Game Creators Club projects live next to this one
  (`C:\projects\GCC\` on the main machine); `Shroomjumper` is the closest model for structure and README style.
