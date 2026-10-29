# Changelog

## 0.2.0

### Platform
- Moved from .NET Framework 4.6.1 to .NET 11, with SDK-style projects, central package management and an `.slnx` solution.
- Replaced Newtonsoft.Json and Microsoft.AspNet.WebApi.Client with the built-in System.Text.Json.
- Added a Podman development container and `scripts/dev.ps1`: building, testing and publishing no longer need .NET or Visual Studio on the machine.
- The app is published as a single self-contained Windows executable.

### Data
- Replaced the discontinued Jikan v3 API with Jikan v4, falling back to AniList for searches and details when Jikan is unavailable.
- User lists are read from MyAnimeList's public list pages, since Jikan no longer provides them.
- Rate limited requests are retried, searches are URL-encoded, and network failures are reported separately from "not found".
- When Jikan fails, searches and details go straight to AniList for two minutes instead of waiting for Jikan to time out every time.

### Settings
- Settings are saved as JSON (`settings.json`); saves are atomic and an unreadable file no longer stops the app from starting.
- Settings from earlier versions (`settings.dat`) are migrated automatically without BinaryFormatter.
- Settings are saved after every change instead of only when the app closes.

### Fixes
- The project failed to build outside of the author's machine because of an absolute project path.
- Loading a user could leave the dashboard loading forever, crash on a missing avatar or statistics, or show favorites in reverse order.
- "Unload this MAL account" opened the loader instead of unloading the account.
- Switching between Anime and Manga could keep the other kind's results, and previewing them showed the wrong entry.
- Failed searches could not be retried with the same text, and could leave the search box disabled.
- Clicking search results quickly could display a different entry than the selected one.
- Every visit to the template settings made Enter add an aspect once more.
- Updating a template reset its creation date and lost its aspect reviews; saving a template without a name crashed the app.
- Deleting a template reported success even when it failed; the list jumped back to the first template after every change.
- Up to 11 templates could be created instead of 10, and new template names could repeat after deletions.
- Two-character MAL usernames were rejected, and the default user could not be removed.
- Links, folders and the contact email failed to open on .NET.
- Lists showed "5/ 12" and hid real zero values; entries without a link crashed the list.
- Saving shorter settings could corrupt the settings file.
- Fixed leaks of images, fonts, forms and controls, and image downloads that froze the interface.
- Unexpected errors now show a message instead of closing the app, and settings are saved first.

### Tests
- Added unit tests for the API and Core projects (205 tests, 99% line coverage).
