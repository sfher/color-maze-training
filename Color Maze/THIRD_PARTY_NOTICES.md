# Third-Party Notices

Last updated: 2026-10-21

This file records the provenance and license of external assets used in the
Color Maze Unity project. It is an inventory, not a replacement for the
original license texts.

## Asset Inventory

| ID | Asset | Author | Source | License | Date | Modified | Attribution |
|---|---|---|---|---|---|---|---|
| font-fusion-pixel | Fusion Pixel Font, 12px proportional, zh-Hans | TakWolf | [GitHub release 2026.10.16](https://github.com/TakWolf/fusion-pixel-font/releases/tag/2026.10.16) | [SIL OFL 1.1](https://openfontlicense.org/) for font; MIT for repository code | 2026-10-21 | Imported TTF; generated dynamic TMP Font Asset; enabled Multi Atlas; added HUD glyphs | Preserve license text; credit appreciated |
| icons-1bit | 1-bit Pixel Icons: Bomb, Coin, Health, Material, Oxygen | Nikoichu | [1-bit Pixel Icons on itch.io](https://nikoichu.itch.io/pixel-icons) | [CC0 1.0](https://creativecommons.org/publicdomain/zero/1.0/) | 2026-10-21 | Selected 5 icons; renamed for project use; imported for HUD use | Not required; credit appreciated |
| kenney-interface-sounds | Interface Sounds (1.0) | Kenney (Kenney Vleugels) | [Interface Sounds](https://kenney.nl/assets/interface-sounds) | [CC0 1.0](https://creativecommons.org/publicdomain/zero/1.0/) | 2026-10-21 | Selected UI confirm, complete, error, damage sounds; renamed | Not required; credit appreciated |
| kenney-rpg-audio | RPG Audio | Kenney (Kenney Vleugels) | [RPG Audio](https://kenney.nl/assets/rpg-audio) | [CC0 1.0](https://creativecommons.org/publicdomain/zero/1.0/) | 2026-10-21 | Selected pickup, chest latch, stone impact; renamed | Not required; credit appreciated |
| kenney-digital-audio | Digital Audio | Kenney (Kenney Vleugels) | [Digital Audio](https://kenney.nl/assets/digital-audio) | [CC0 1.0](https://creativecommons.org/publicdomain/zero/1.0/) | 2026-10-21 | Selected portal/phaser sound; renamed | Not required; credit appreciated |

## Local Files

- Font and TMP asset: `Assets/ThirdParty/Fonts/FusionPixel/`
- Font license copy: `Assets/ThirdParty/Fonts/FusionPixel/LICENSE-OFL.txt`
- Icons: `Assets/ThirdParty/Icons/`
- Audio: `Assets/ThirdParty/Audio/Kenney/`
- Original Kenney license copies:
  `License-InterfaceSounds.txt`, `License-RPGAudio.txt`,
  `License-DigitalAudio.txt`

## Needs Review

The following files existed before the curated Kenney audio set was added.
They are not cleared for release until their author, source, and license are
recorded:

- `Assets/ThirdParty/Sounds/Pick up Coin.wav`
- `Assets/ThirdParty/Sounds/Pickup.aiff`

Do not ship these two files until the missing license record is filled in or
they are replaced by the Kenney CC0 set.

## License Texts

- Fusion Pixel Font: SIL Open Font License 1.1. The downloaded font asset
  folder retains `LICENSE-OFL.txt`. The upstream release also contains
  `OFL.txt` and `LICENSES/`; retain those files when redistributing the
  original pack.
- 1-bit Pixel Icons: CC0 1.0. No attribution is required, but the source
  page and author are recorded here for provenance.
- Kenney audio packs: CC0 1.0. Original license copies are kept in the
  imported audio folder.

## Selected Audio

| Project file | Source pack | Original file | Use |
|---|---|---|---|
| `pickup_material.ogg` | RPG Audio | `handleCoins2.ogg` | Material/coin pickup |
| `chest_open.ogg` | RPG Audio | `metalLatch.ogg` | Chest open |
| `stone_break.ogg` | RPG Audio | `chop.ogg` | Stone impact/break |
| `portal_enter.ogg` | Digital Audio | `phaserUp5.ogg` | Portal entry |
| `ui_confirm.ogg` | Interface Sounds | `confirmation_001.ogg` | UI confirm |
| `level_complete.ogg` | Interface Sounds | `confirmation_003.ogg` | Level complete |
| `ui_error.ogg` | Interface Sounds | `error_001.ogg` | Invalid action |
| `damage.ogg` | Interface Sounds | `error_003.ogg` | Damage/failure feedback |

## Suggested Credits

These credits are optional for CC0, but useful for transparency:

```text
1-bit Pixel Icons by Nikoichu (CC0 1.0)
Fusion Pixel Font by TakWolf (SIL OFL 1.1)
Kenney audio packs by Kenney (CC0 1.0)
```

## Asset Checklist

- [x] Icon source license verified as CC0 1.0
- [x] Font license file retained
- [x] Selected icon files are under `Assets/ThirdParty/Icons/`
- [x] Kenney audio licenses retained under `Assets/ThirdParty/Audio/Kenney/`
- [x] Modifications and source URLs recorded
- [ ] Confirm this file is included in the release build/notices

## Notes

- Do not add assets whose license cannot be identified.
- Keep source URLs and download dates; a link without a date becomes hard to
  audit later.
- If the font is updated, record the new release tag and regenerate the TMP
  Font Asset if the source font or atlas settings change.
