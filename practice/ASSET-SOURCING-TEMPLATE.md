# Asset Sourcing Template

## 1. Asset Spec

| Field | Value |
|---|---|
| Asset type |  |
| Purpose |  |
| Style / palette |  |
| Resolution / format |  |
| Required glyphs / content |  |
| Allowed license |  |
| Attribution budget |  |
| Deadline |  |

## 2. Candidate Comparison

| Candidate | Author | Source | License | Style fit | Technical fit | Decision |
|---|---|---|---|---|---|---|
| A |  |  |  |  |  |  |
| B |  |  |  |  |  |  |

## 3. Third-Party Notices

Create `THIRD_PARTY_NOTICES.md` at the Unity project root:

| ID | Asset | Author | Source | License | Date | Modified | Attribution |
|---|---|---|---|---|---|---|---|
|  |  |  |  |  |  |  |  |

Keep the original license text in `Assets/ThirdParty/<PackName>/`.

## 4. Import Checklist

- [ ] Sprite Type / Texture Type set correctly
- [ ] Point Filter and compression settings checked
- [ ] Pixels Per Unit matches the grid
- [ ] Font glyph coverage checked (especially Chinese)
- [ ] Audio listened to and volume checked
- [ ] Asset referenced by a scene/prefab
- [ ] `.meta`, license, and notices included in Git
