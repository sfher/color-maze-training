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

## Local Files

- Font and TMP asset: `Assets/ThirdParty/Fonts/FusionPixel/`
- Font license copy: `Assets/ThirdParty/Fonts/FusionPixel/LICENSE-OFL.txt`
- Icons: `Assets/ThirdParty/Icons/`

## License Texts

- Fusion Pixel Font: SIL Open Font License 1.1. The downloaded font asset
  folder retains `LICENSE-OFL.txt`. The upstream release also contains
  `OFL.txt` and `LICENSES/`; retain those files when redistributing the
  original pack.
- 1-bit Pixel Icons: CC0 1.0. No attribution is required, but the source
  page and author are recorded here for provenance.

## Suggested Credits

These credits are optional for CC0, but useful for transparency:

```text
1-bit Pixel Icons by Nikoichu (CC0 1.0)
Fusion Pixel Font by TakWolf (SIL OFL 1.1)
```

## Asset Checklist

- [x] Icon source license verified as CC0 1.0
- [x] Font license file retained
- [x] Selected icon files are under `Assets/ThirdParty/Icons/`
- [x] Modifications and source URLs recorded
- [ ] Confirm this file is included in the release build/notices

## Notes

- Do not add assets whose license cannot be identified.
- Keep source URLs and download dates; a link without a date becomes hard to
  audit later.
- If the font is updated, record the new release tag and regenerate the TMP
  Font Asset if the source font or atlas settings change.
