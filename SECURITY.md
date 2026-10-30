# Security policy

## Supported versions
Security fixes are made for the latest release only.

| Version | Supported |
| --- | --- |
| 0.2.x | Yes |
| Older than 0.2.0 | No |

## Reporting a vulnerability
Please don't report security problems in public issues.

Report them privately through [the private reporting form of this repository](https://github.com/eoussama/MAL_Reviewer/security/advisories/new),
which only the maintainers can read. Include what you found, the steps to reproduce it, and the version of the app.

You can expect a first answer within a week. Once the problem is confirmed, it is fixed in a new release,
and you are credited in the advisory unless you'd rather not be.

## Scope
MAL Reviewer runs on your computer. It reads public data from Jikan, AniList and MyAnimeList, needs no account or password,
and stores its settings and the default user's data in `%APPDATA%\MAL_Reviewer`.
Problems in those services themselves should be reported to them.
