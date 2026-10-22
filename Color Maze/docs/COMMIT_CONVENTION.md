# Commit Convention

This project uses a lightweight form of
[Conventional Commits](https://www.conventionalcommits.org/en/v1.0.0/).
It is a team convention, not a Unity or Git requirement.

## Format

```text
type(scope): summary

Optional body: why the change was made, constraints, or notable tradeoffs.

Refs: #12
```

The scope is optional. Use a short English summary or a consistent Chinese
summary; do not mix styles randomly within one project.

## Types

| Type | Meaning | Example |
|---|---|---|
| `feat` | New user-visible capability | `feat(audio): 接入事件驱动音效` |
| `fix` | Bug fix | `fix(ui): 绑定结算文本材质` |
| `docs` | Documentation only | `docs(assets): 记录 CC0 资源来源` |
| `refactor` | Internal change, same behavior | `refactor(level): 拆出 LevelDefinition` |
| `test` | Tests only | `test(rules): 覆盖氧气耗尽` |
| `style` | Formatting/whitespace only | `style: format C# scripts` |
| `perf` | Performance improvement | `perf(render): 减少每帧分配` |
| `build` | Build/dependency changes | `build: configure Windows player` |
| `ci` | CI pipeline changes | `ci: add build workflow` |
| `chore` | Maintenance/tooling | `chore: remove temporary editor script` |
| `revert` | Revert an earlier commit | `revert: revert portal teleport change` |

## Scopes Used Here

`player`, `level`, `ui`, `audio`, `assets`, `input`, `save`, `build`, `docs`.

Scopes are labels, not permissions. Add a new one only when it helps find
related changes.

## What "issue" Means

An issue is a tracked task, bug, or feature request in a tool such as GitHub
Issues, GitLab Issues, or Jira. It is not a commit type.

Put the issue number in the commit body/footer:

```text
fix(audio): 绑定过关音效

The Level Complete clip was not assigned in the AudioManager component.

Fixes #12
```

Common links:

- `Fixes #12` / `Closes #12` / `Resolves #12`: may auto-close the issue when
  the commit/PR reaches the default branch.
- `Refs #12` / `Part of #12`: related, but do not auto-close.
- Jira-style keys may appear as a scope or footer: `feat(AUDIO-12): ...`.

Plain local Git does not manage issues; the linking behavior comes from the
hosting platform (GitHub/GitLab/etc.).

## Rules

- One logical change per commit. Do not mix gameplay, documentation, and
  unrelated assets in a giant commit.
- Use the imperative mood: “add”, “fix”, “remove”, not “added” or “adds”.
- Keep the summary short (roughly 50–72 characters).
- No period at the end of the summary.
- The body explains **why**, not a line-by-line restatement of the diff.
- Commit only after the project can run, unless the commit is explicitly a
  checkpoint/WIP.
- Include `.meta` files and license records with imported Unity assets.

## Breaking Changes

Use `!` after the type/scope or a `BREAKING CHANGE:` footer:

```text
feat(level)!: replace the level format

BREAKING CHANGE: old level assets must be migrated to LevelDefinition.
```

## Choosing the Right Type

Ask:

1. Does the user see a new capability? → `feat`
2. Does it repair incorrect behavior? → `fix`
3. Is it only internal structure with unchanged behavior? → `refactor`
4. Is it only documentation? → `docs`
5. Is it only tests? → `test`
6. Is it tooling/dependencies/maintenance? → `chore` or `build`

## References

- [Conventional Commits 1.0.0](https://www.conventionalcommits.org/en/v1.0.0/)
- [Chris Beams: How to Write a Git Commit Message](https://cbea.ms/git-commit/)
- [GitHub: Linking a pull request to an issue](https://docs.github.com/en/issues/tracking-your-work-with-issues/using-issues/linking-a-pull-request-to-an-issue)
- [Git commit documentation](https://git-scm.com/docs/git-commit)
- [Keep a Changelog](https://keepachangelog.com/en/1.1.0/) for release notes,
  not for individual commit messages

## Project Note

Earlier commits in this repository do not all follow one convention. Do not
rewrite published history just to make it look uniform; start using this
convention from the next meaningful commit.
