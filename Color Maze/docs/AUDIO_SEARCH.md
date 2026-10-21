# Audio Search Brief

Updated: 2026-10-21

## Goal

Find and evaluate one short sound at a time for the current prototype. Do not
download a large unknown audio pack just to "have options".

## Starter Set (already acquired)

The first curated set is in `Assets/ThirdParty/Audio/Kenney/`. All three
source packs are CC0 1.0 and their original `License-*.txt` files are kept.

| Project file | Source pack | Original file | Duration |
|---|---|---|---:|
| `pickup_material.ogg` | RPG Audio | `handleCoins2.ogg` | 0.34s |
| `chest_open.ogg` | RPG Audio | `metalLatch.ogg` | 0.26s |
| `stone_break.ogg` | RPG Audio | `chop.ogg` | 0.24s |
| `portal_enter.ogg` | Digital Audio | `phaserUp5.ogg` | 0.31s |
| `ui_confirm.ogg` | Interface Sounds | `confirmation_001.ogg` | 0.29s |
| `level_complete.ogg` | Interface Sounds | `confirmation_003.ogg` | 0.32s |
| `ui_error.ogg` | Interface Sounds | `error_001.ogg` | 0.16s |
| `damage.ogg` | Interface Sounds | `error_003.ogg` | 0.53s |

Preview the files before wiring them. Filenames describe intended use, not a
final mix decision.

Two older files in `Assets/ThirdParty/Sounds/` (`Pick up Coin.wav` and
`Pickup.aiff`) still need source/license verification; see the
`THIRD_PARTY_NOTICES.md` "Needs Review" section before using them.

## Sound Brief

| Event | Duration | Feeling | Search terms |
|---|---:|---|---|
| Pick up material / coin | 0.15–0.35s | bright, rewarding, short | `8 bit coin pickup`, `retro item pickup`, `pixel collect` |
| Open chest | 0.20–0.50s | wooden click + small sparkle | `8 bit chest open`, `retro chest`, `treasure open` |
| Break stone | 0.20–0.50s | low crunch, short impact | `stone break`, `rock crack`, `8 bit impact` |
| Enter portal | 0.30–0.70s | whoosh or energy swell | `portal whoosh`, `teleport`, `sci fi teleport` |
| UI confirm / click | 0.05–0.15s | crisp, quiet, non-annoying | `ui click`, `menu confirm`, `8 bit select` |
| Failure / damage | 0.20–0.50s | obvious but not harsh | `8 bit damage`, `retro fail`, `game hurt` |

Prefer one-shots with clear attack, no long reverb, and no long silent tail.
Use WAV or OGG; keep short UI sounds mono.

## Where to Search

### Kenney (CC0, start here)

- [Interface Sounds](https://kenney.nl/assets/interface-sounds): UI clicks,
  confirms, pickups.
- [RPG Audio](https://kenney.nl/assets/rpg-audio): coins, doors, impacts,
  footsteps.
- [Digital Audio](https://kenney.nl/assets/digital-audio): portals, energy,
  sci-fi UI.

These packs are useful because they keep a consistent style and are usually
CC0. Check the license included with the specific pack before importing.

### Freesound (search, then filter)

Search in English, for example:

```text
8 bit coin pickup
retro item pickup
8 bit chest open
stone break game
ui confirm 8 bit
```

Then filter by:

- License: CC0 or the exact license you are prepared to attribute
- Duration: usually under 1 second
- Type: WAV/OGG
- Sample rate: 44.1 or 48 kHz
- Channels: mono for UI sounds

Listen to the **whole** file before downloading. Check for clipping, noise,
long silence, and an inconsistent room/reverb sound.

### OpenGameArt / itch.io

- [OpenGameArt](https://opengameart.org/)
- [itch.io Free Game Assets](https://itch.io/game-assets/free)

Both have mixed licenses. Open the asset page, read the license section, and
download the license file with the asset. A pack marked "free" is not
automatically CC0.

## Selection Checklist

- [ ] License is explicit and saved with the asset
- [ ] The sound matches one event in the brief
- [ ] Duration fits the event
- [ ] No clipping, harsh noise, or long silence
- [ ] Style matches the existing 8-bit/pixel direction
- [ ] Source URL, author, license, and modifications are recorded

## Import

Place sounds under:

```text
Assets/ThirdParty/Audio/<PackName>/
```

Keep the pack's `LICENSE`/`README`. In Unity, check the import settings for
load type and compression; short UI sounds usually use Decompress on Load and
low compression overhead. For the first prototype, prioritize clarity over
micro-optimizing compression.

## Notices Template

```markdown
| ID | Asset | Author | Source | License | Date | Modified | Attribution |
|---|---|---|---|---|---|---|---|
| sfx-pickup-01 | ... | ... | ... | CC0 1.0 | 2026-10-21 | Trimmed | Not required |
```

## Rule

If you cannot identify the license, do not import the sound. Choose another
source or make a temporary placeholder; a placeholder is safer than an
untraceable asset.
