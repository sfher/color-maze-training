# Third-Party Notices

Last updated: 2026-10-24

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
| freesound-drowning-02 | Cartoon Drowning 02.wav | LilMati | [Freesound 657930](https://freesound.org/s/657930/) | [CC0 1.0](https://creativecommons.org/publicdomain/zero/1.0/) | 2026-10-23 | Imported original WAV for drowning damage | Not required; credit appreciated |
| kenney-roguelike-rpg | Roguelike/RPG Pack (selected tiles) | Kenney (Kenney Vleugels) | [Roguelike/RPG Pack](https://kenney.nl/assets/roguelike-rpg-pack) | [CC0 1.0](https://creativecommons.org/publicdomain/zero/1.0/) | 2026-10-24 | Cropped chest/exit tiles from spritesheet; composited onto project-made floor background | Not required; credit appreciated |
| kenney-roguelike-caves | Roguelike Caves & Dungeons (selected tiles) | Kenney (Kenney Vleugels) | [Roguelike Caves & Dungeons](https://kenney.nl/assets/roguelike-caves-dungeons) | [CC0 1.0](https://creativecommons.org/publicdomain/zero/1.0/) | 2026-10-24 | Cropped water/material tiles from spritesheet; material composited onto project-made floor background | Not required; credit appreciated |

## Local Files

- Font and TMP asset: `Assets/ThirdParty/Fonts/FusionPixel/`
- Font license copy: `Assets/ThirdParty/Fonts/FusionPixel/LICENSE-OFL.txt`
- Icons: `Assets/ThirdParty/Icons/`
- Audio: `Assets/ThirdParty/Audio/Kenney/`
- Additional audio: `Assets/ThirdParty/Sounds/`
- Pixel tiles: `Assets/ThirdParty/Art/Kenney/RoguelikeCavesDungeons/`
  and `Assets/ThirdParty/Art/Kenney/RoguelikeRpg/`
- Original Kenney license copies:
  `License-InterfaceSounds.txt`, `License-RPGAudio.txt`,
  `License-DigitalAudio.txt`, and `License.txt` in each pixel tile folder
- Project-made art (not third-party): `Assets/Art/Tiles/` and
  `Assets/Art/Player/`

## Needs Review

No outstanding unverified files were found in
`Assets/ThirdParty/Sounds/` on 2026-10-23.

## License Texts

- Fusion Pixel Font: SIL Open Font License 1.1. The downloaded font asset
  folder retains `LICENSE-OFL.txt`. The upstream release also contains
  `OFL.txt` and `LICENSES/`; retain those files when redistributing the
  original pack.
- 1-bit Pixel Icons: CC0 1.0. No attribution is required, but the source
  page and author are recorded here for provenance.
- Kenney audio packs and pixel tile packs: CC0 1.0. Original license copies
  are kept in the imported folders.

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
| `657930__lilmati__cartoon-drowning-02.wav` | Freesound 657930 | `657930__lilmati__cartoon-drowning-02.wav` | Drowning health damage |

## Selected Art

Cell coordinates are (column,row) in the packed spritesheet; tiles are
16×16 with 1px spacing (stride 17), origin at top-left.

| Project file | Source pack | Sheet cell | Use |
|---|---|---|---|
| `tile_water.png` | Roguelike Caves & Dungeons | (1,15) | Water `B` |
| `tile_material.png` | Roguelike Caves & Dungeons | (15,13) | Material `M`, composited onto floor background |
| `tile_chest.png` | Roguelike/RPG Pack | (54,2) | Chest `C`, composited onto floor background |
| `tile_exit.png` | Roguelike/RPG Pack | (44,0) | Exit `E`, composited onto floor background |

## Original Art

- `Assets/Art/Tiles/tile_floor.png` — 2026-10-24, drawn with Aseprite batch
  scripting. Near-flat light tile designed to tile seamlessly; replaced the
  Kenney cave floor whose baked-in cracks pattern looked noisy when tiled.
- `Assets/Art/Tiles/tile_wall.png` — 2026-10-24, drawn with Aseprite batch
  scripting. Dark raised block with bevel edges; replaced the Kenney cave
  wall that was hard to distinguish from the floor.
- `Assets/Art/Tiles/tile_lava.png` — 2026-10-24, drawn with Aseprite batch
  scripting for this project. Neither Kenney pack ships a lava tile.
- `Assets/Art/Tiles/tile_portal.png` — 2026-10-24, drawn with Aseprite batch
  scripting. A white spiral on a dark pad: the whole sprite is tinted per
  portal id at runtime, and the dark pad keeps the spiral readable after
  tinting (a floor-colored pad would wash out the contrast).
- `Assets/Art/Tiles/tile_stone.png` — 2026-10-24, drawn with Aseprite batch
  scripting. Round boulder on a baked floor background. Neither Kenney pack
  ships a round boulder (candidates were columns, stalactites and frames).
- `Assets/Art/Player/player_*.png` — 2026-10-24, 8 hooded-adventurer skins
  (default/orange/yellow/green/teal/blue/purple/rose) drawn with Aseprite
  batch scripting from one template; intended as purchasable player skins.

## Suggested Credits

These credits are optional for CC0, but useful for transparency:

```text
1-bit Pixel Icons by Nikoichu (CC0 1.0)
Fusion Pixel Font by TakWolf (SIL OFL 1.1)
Kenney audio packs by Kenney (CC0 1.0)
Kenney Roguelike/RPG Pack and Roguelike Caves & Dungeons by Kenney (CC0 1.0)
LilMati on Freesound for Cartoon Drowning 02 (CC0 1.0)
```

## Asset Checklist

- [x] Icon source license verified as CC0 1.0
- [x] Font license file retained
- [x] Selected icon files are under `Assets/ThirdParty/Icons/`
- [x] Kenney audio licenses retained under `Assets/ThirdParty/Audio/Kenney/`
- [x] Kenney pixel tile pack licenses verified as CC0 1.0 and retained
- [x] Cropped tile provenance recorded in Selected Art
- [x] Modifications and source URLs recorded
- [ ] Confirm this file is included in the release build/notices

## Notes

- Do not add assets whose license cannot be identified.
- Keep source URLs and download dates; a link without a date becomes hard to
  audit later.
- If the font is updated, record the new release tag and regenerate the TMP
  Font Asset if the source font or atlas settings change.
