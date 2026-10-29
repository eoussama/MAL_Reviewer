<p align="center">
    <img src="MAL_Reviewer/MAL_Reviewer_UI/images/logo/logo.svg" width="200">
</p>
<h1 align="center">MAL Reviewer</h1>

## Description
MAL Reviewer is an unofficial productivity booster application that organizes and simplifies writing Anime and Manga reviews for [MyAnimeList](https://myanimelist.net/). For you out there who like to keep records of their reviews on a local drive, you can finally ditch the lengthy manual process of managing your reviews in separate text files and take advantage of MAL reviewer and its wide range of tools.

## Requirements
MAL Reviewer is a Windows desktop application (.NET 11, Windows Forms).

To work on it you only need:

- [Podman](https://podman.io/) (with a running Podman machine)
- [Visual Studio Code](https://code.visualstudio.com/)

The .NET SDK lives in a container, and the app is published self-contained, so neither .NET nor Visual Studio has to be installed.

## Development

### Option 1: Dev Container (recommended)
1. Install the [Dev Containers](https://marketplace.visualstudio.com/items?itemName=ms-vscode-remote.remote-containers) extension.
2. Point it at Podman in your VS Code **user** settings:
   ```json
   "dev.containers.dockerPath": "podman"
   ```
3. Open the repository and run **Dev Containers: Reopen in Container**.

VS Code then runs inside a .NET 11 container with C# Dev Kit, so IntelliSense, building and the Test Explorer work as usual.
The Windows app can be built in the container, but it has to run on Windows: use `scripts/dev.ps1 run` from a terminal on the host.

### Option 2: Script on the host
`scripts/dev.ps1` runs every .NET command through Podman. The same commands are available as VS Code tasks (**Terminal → Run Task**).

| Command | What it does |
| --- | --- |
| `./scripts/dev.ps1 image` | Builds the `mal-reviewer-dev` image (done automatically when missing). |
| `./scripts/dev.ps1 build` | Builds the solution. Add `-Configuration Release` for a release build. |
| `./scripts/dev.ps1 test` | Runs all the unit tests. |
| `./scripts/dev.ps1 coverage` | Runs the tests with code coverage; the report is written to `artifacts/coverage/index.html`. |
| `./scripts/dev.ps1 publish` | Publishes `artifacts/publish/MAL_Reviewer_UI.exe`, a single self-contained Windows executable. |
| `./scripts/dev.ps1 run` | Publishes, then starts the app on Windows. |
| `./scripts/dev.ps1 shell` | Opens a shell in the container. |
| `./scripts/dev.ps1 clean` | Deletes the build outputs and artifacts. |

If PowerShell refuses to run the script, start it with `powershell -ExecutionPolicy Bypass -File ./scripts/dev.ps1 <command>`.

### Project layout
| Project | Purpose |
| --- | --- |
| `MAL_Reviewer/MAL_Reviewer_API` | Fetches MyAnimeList data from free services (`net11.0`). |
| `MAL_Reviewer/MAL_Reviewer_Core` | Settings, review templates and the logic behind the interface (`net11.0`). |
| `MAL_Reviewer/MAL_Reviewer_UI` | The Windows Forms application (`net11.0-windows`). |
| `MAL_Reviewer/MAL_Reviewer_Sandbox` | A console app for trying things out. |
| `MAL_Reviewer/tests` | xUnit tests of the API and Core projects. |

Package versions are managed centrally in `MAL_Reviewer/Directory.Packages.props`, and the app version in `MAL_Reviewer/Directory.Build.props`.

## Data sources
All the data comes from free services that need no account or API key:

- [AniList](https://anilist.co/) for searches, since it answers several times faster, with Jikan as the fallback.
- [Jikan v4](https://jikan.moe/) for Anime and Manga details, user profiles, statistics and favorites, with AniList as the fallback for details.

Searches and details are kept in memory for 10 minutes. When a service fails, the other one is used for the next 2 minutes.
- MyAnimeList's public list pages for users' Anime and Manga lists (Jikan stopped serving them in 2022). Private lists show as private.

User profiles are only available through Jikan, so loading a user needs Jikan to be up.

To try the fallback, point Jikan to an address that refuses connections before starting the app:
```powershell
$env:MAL_REVIEWER_JIKAN_URL = "http://127.0.0.1:9/"; ./scripts/dev.ps1 run
```
`MAL_REVIEWER_ANILIST_URL` overrides the AniList address the same way.

## Settings
Settings are stored in `%APPDATA%\MAL_Reviewer\settings.json`.
Settings saved by versions before 0.2.0 (`settings.dat`) are converted on the first start, and the old file is kept as `settings.dat.migrated`.
An unreadable settings file is set aside as `settings.corrupt-<date>.json` and the defaults are used instead.

The default MAL user is loaded when the app starts, which can be turned off in **Settings → User**.
Their data is kept in `user.json` next to the settings, and shown when MAL can't be reached at start.
The last 10 loaded users are listed under the username box of the loading window, to load them again with a click.

**Settings → General** exports the settings to a file and imports them back, and exports the loaded user's Anime and Manga lists,
either as MAL XML (the format of MyAnimeList's own export, which MyAnimeList and AniList can import) or as CSV for spreadsheets.

The theme is picked in **Settings → Theme** and applies right away: the same as Windows (the default, following the light or dark mode of apps in the Windows settings), light, dark or custom colors.
The forms are designed with the light colors; `MAL_Reviewer_UI/theming/Theme.cs` swaps each of them for the theme's color of the same role (see `MAL_Reviewer_Core/helpers/ThemeMap.cs`).

## Credits
Aside from myself, this project wouldn't have been possible if not for the effort of the following people.

- [Irfan](https://github.com/irfan-dahir)
    - Big thanks to him and all of the contributors that made the [Jikan API](https://github.com/jikan-me) the reliable tool it is. Jikan is an unofficial API for MAL that handles various data fetching operations.

- [AniList](https://anilist.co/)
    - For its free GraphQL API, used when Jikan can't be reached.

- [Freepik](https://www.freepik.com/)
    - The logo of this application was created and is licensed by Freepik, the leading search engine of free vector designs.

- [Dave Gandy](https://twitter.com/davegandy?lang=en)
    - Every other icon used in this application is part of the huge [FontAwesome pack](https://www.flaticon.com/packs/font-awesome).
