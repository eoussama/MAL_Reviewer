# Contributing to MAL Reviewer

Thanks for taking the time to help! Bug reports, ideas and code are all welcome.
Everyone taking part is expected to follow the [code of conduct](CODE_OF_CONDUCT.md).

## Reporting a bug or suggesting a feature
- Search the [existing issues](https://github.com/eoussama/MAL_Reviewer/issues) first; adding details to an open issue helps more than a duplicate.
- Open a new issue with the **Bug report** or **Feature request** form, which asks for what's needed to act on it.
- Security problems should not go in a public issue: see the [security policy](SECURITY.md).

## Before writing code
For anything bigger than a small fix, open an issue or comment on an existing one first, so we can agree on the approach before you spend time on it.

## Setting up
You need [Podman](https://podman.io/) and [Visual Studio Code](https://code.visualstudio.com/); .NET and Visual Studio don't have to be installed.
The [README](README.md#development) explains both ways of working: the Dev Container, or `scripts/dev.ps1` from a terminal.

The commands you'll use most:

```powershell
./scripts/dev.ps1 test                        # all the unit tests
./scripts/dev.ps1 coverage                    # tests with a coverage report in artifacts/coverage
./scripts/dev.ps1 build -Configuration Release
./scripts/dev.ps1 run                         # publishes and starts the app on Windows
```

## Making changes
- Branch from `master`, and keep each pull request to one fix or feature.
- Follow the style of the code around your change: `#region` blocks, XML documentation comments, and the existing naming.
- Write code, comments, interface text and file names in American English (color, center, behavior).
- Put logic in `MAL_Reviewer_Core` (or `MAL_Reviewer_API` for data sources) and cover it with unit tests.
  The Windows Forms project can't run in the Linux container, so code there is checked by hand.
- The forms are designed with the light colors. The theme swaps each designed color for the theme's color of the same role
  (see `MAL_Reviewer_UI/theming/Theme.cs`), so:
  - use the system colors already in use (`SystemColors.Control`, `ControlLight`, `ControlDarkDark`...) rather than new ones;
  - set colors that change at run time, (on hover for instance), with `SetThemedBackColor` and `SetThemedForeColor`;
  - check your change with the light, dark and custom themes.
- Keep `settings.json` readable by older and newer versions: new settings need defaults, and unknown or broken values must fall back
  to them rather than make the file unreadable.
- Network data comes from free services (Jikan, AniList and MyAnimeList's public pages); tests use recorded answers and never the network.

## Before opening a pull request
- `./scripts/dev.ps1 test` passes, and the Release build has no warnings.
- You tried the change in the app with `./scripts/dev.ps1 run`.
- The README is updated if the change affects how the app is used or developed.

Commit messages start with a short summary in the imperative mood (ex: "Add search history for users"), followed by the issue number
when there is one, (ex: "(#12)"), and a list of the main changes when it helps.

## License
By contributing, you agree that your contributions are licensed under the [MIT License](LICENSE).
