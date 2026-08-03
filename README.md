# TABG Dedicated Server

[![WindowsGSH](.github/assets/windowsgsh-badge.svg)](https://windowsgsh.com)
[![Status](https://img.shields.io/badge/status-needs_live_test-f59e0b)](#status)
[![Module version](https://img.shields.io/badge/dynamic/json?url=https%3A%2F%2Fraw.githubusercontent.com%2FWindowsGSH%2FWindowsGSH.TABG%2Fmain%2FTABG.mod%2Fmodule.json&query=%24.version&prefix=v&label=module&color=1E8449)](TABG.mod/module.json)
[![Requires WindowsGSH](https://img.shields.io/badge/dynamic/json?url=https%3A%2F%2Fraw.githubusercontent.com%2FWindowsGSH%2FWindowsGSH.TABG%2Fmain%2FTABG.mod%2Fmodule.json%3Fbadge%3Dminimum&query=%24.minimumWindowsGshVersion&prefix=v&label=requires%20WindowsGSH&color=2563EB)](TABG.mod/module.json)
[![Licence](https://img.shields.io/badge/licence-MIT-64748B)](LICENSE.md)

This WindowsGSH module installs, configures, starts, stops, monitors, and backs up a Totally Accurate Battlegrounds community server.

## Status

**NEEDS LIVE TEST.** The native module targets the current documented executable and vendor configuration format and passes focused host tests. A fresh Steam installation and relay join test are still required.

## Installation

The module installs Steam app `2311970` anonymously and launches `TABG.exe`.

1. Import the `TABG.mod` folder or its repository root into WindowsGSH.
2. Add a TABG server and run Install.
3. Confirm the installation contains `TABG.exe`, `game_settings.txt`, and `VerifyAllowedWords.exe`.
4. Validate the chosen name and description with `VerifyAllowedWords.exe` before starting.

If `game_settings.txt` is missing, run Verify Files. The module does not invent the vendor file because a generated replacement could omit required settings from a newer build.

### Import an existing server

WindowsGSH can import either a normal server installation folder or a WindowsGSM server folder containing `serverfiles`. The preview verifies the server executable, reads supported settings when present, and lets you copy the installation into WindowsGSH or adopt it in place. Review every previewed/defaulted value before completing the import; the source installation is not modified during preview.

## Configuration

| WindowsGSH setting | TABG setting | Notes |
| --- | --- | --- |
| Server Name | `ServerName` | Must use words accepted by `VerifyAllowedWords.exe`. |
| Description | `Description` | Landfall's allowed-word restrictions also apply. |
| Password | `Password` | Leave empty for no password. |
| Max Players | `MaxPlayers` | Configurable from 1 to 100; practical limits need live validation. |
| Use Relay | `Relay` | Enabled by default and recommended by Landfall. |
| Direct Game Port | `Port` | Shown only when relay mode is disabled. |
| Additional Arguments | command line | Appended without modifying `game_settings.txt`. |

WindowsGSH updates known keys in place and preserves unknown settings, blank lines, and comments. Writes use a temporary file followed by replacement.

## Networking

Relay mode is the safe default. It produces a join code and normally avoids direct router forwarding. The module deliberately declares no automatic UPnP mappings.

Relay-disabled direct mode is not advertised as supported until a current server is socket-tested. Its port and protocol must be proven before adding any public port declaration or UPnP eligibility.

## Query, console, and administration

- Status is process-based.
- The module does not claim A2S queries or player counts.
- It does not claim RCON, an administration API, or an embedded interactive console.
- Stop requests ask the server window to close before WindowsGSH's bounded fallback handling.

## Files and backups

| Purpose | Path |
| --- | --- |
| Executable | `TABG.exe` |
| Managed configuration | `game_settings.txt` |
| Allowed-word verifier | `VerifyAllowedWords.exe` |
| Backup target | `game_settings.txt` |

World/save backup behavior is not claimed because its current storage layout has not been verified.

## Known limitations

- Availability and practical usability of the current dedicated server need a fresh-install test.
- Relay join-code behavior is not yet live-verified.
- Direct-mode ports, protocols, and forwarding requirements are deliberately undeclared.
- The module backs up configuration only until the current save layout is proven.
- The vendor requires passwords in `game_settings.txt`; protect the server directory and redact support material.

## Beta verification checklist

- [ ] Fresh-install Steam app `2311970` and confirm all three expected files.
- [ ] Validate the server name and description with `VerifyAllowedWords.exe`.
- [ ] Save configuration and confirm unrelated settings and comments survive.
- [ ] Start in relay mode, confirm the card/PID, obtain a join code, and join remotely.
- [ ] Restart WindowsGSH and confirm process reattachment; then test clean Stop and crash detection.
- [ ] Test direct mode separately and record actual listening sockets before declaring ports or UPnP support.
- [ ] Test update, Verify Files, configuration backup, and restore; identify the current save layout.

## Support

Report module issues at <https://github.com/WindowsGSH/WindowsGSH.TABG>. Include the WindowsGSH/module versions, a redacted support bundle, and relevant server output. Never post passwords or unredacted configuration.

## Support development

If you like the work I do and would like to support continued WindowsGSH and module development, you can contribute here:

- [Ko-fi](https://ko-fi.com/shenniko)
- [PayPal](https://paypal.me/shenniko)

## Trust and source

Modules execute with the same Windows permissions as WindowsGSH. Review `TABG.mod/module.json`, `TABG.mod/TabgModule.cs`, and [SECURITY.md](SECURITY.md) before installing a build from an unfamiliar source.
