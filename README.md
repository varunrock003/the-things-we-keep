# The Things We Keep

**Paper Comet Studio | GAME 03 | COMP3770 | Fall 2026**

## Project concept
*The Things We Keep* is a proposed 15–20 minute 3D narrative puzzle game made in Unity. The player is the memory of a house on its last evening before being sold. As a small glowing spirit, the player possesses ordinary objects and helps Leena and Theo discover keepsakes. Each object has one verb:

| Room | Object | Verb |
| --- | --- | --- |
| Kitchen | Kettle | Steam |
| Living room | Lamp | Reveal |
| Hallway | Clock | Chime |
| Bedroom | Radio | Play |
| Porch | Chair | Slide |

The intended visual style is a stylised, low-poly **3D dollhouse diorama** with a **fixed camera per room**. The message is that memories become meaningful through attention, care and sharing, rather than through the objects alone.

## Milestone status (October 21, 2026)
**Milestone 1 — concept and design:** The team has prepared the GDD, planning documents, repository setup and task assignments. The submitted Milestone 1 PDF is the primary design record.

**Milestone 2 — implementation:** Unity project creation and scene development are planned for Milestone 2. **No Unity project, gameplay scenes or C# scripts are committed as of this update.** The presence of `unity/` does not mean the engine project exists; it currently contains a setup note only.

See `Docs/milestone-1-repository-audit.md` and `Docs/milestone-2-start-plan.md` for the handoff.

## Team named in submitted GDD
- **Varunteja Katakam** (`varunrock003`) — lead programming, Git, build integration and project management
- **Venkat Preetham Gogineni** (`Gogineng`) — game and puzzle design, room staging and playtesting
- **Shruti Bharat Dadhania** (`ShrutiDadhania`) — 3D art direction, materials, lighting, UI and documentation
- **Hassan Hamadi** (GitHub username not yet verified) — verb effects, hints, integration and testing

The project overview specifies teams of five; this GDD names four. The roster must be confirmed rather than adding an unverified name.

## Repository contents
- `Docs/design-notes-starter.md` — game and puzzle structure
- `Docs/art-ui-notes-starter.md` — style and asset handling
- `COMMIT-PLAN.md` — team's working Git workflow
- `unity/README-Unity-Setup.md` — instructions for creating the Unity project in Milestone 2
- `.gitignore` and `.gitattributes` — Unity exclusions and Git LFS patterns

## Planned Unity setup — Milestone 2, not yet completed
Use Unity 6.3 LTS and a Universal 3D (URP) template as recorded in the team's setup notes. Create the project in `unity/` or a named subfolder inside it, then commit `Assets/`, `Packages/`, `ProjectSettings/` and `.meta` files. Generated `Library/`, `Temp/` and build directories must stay uncommitted. The first planned playable slice is the kitchen and kettle possession mechanic.

## Collaboration and evidence
Use each member's own GitHub account for actual code/documentation changes. Record meaningful commits, tests, design decisions and asset licences. Do not represent planned gameplay as implemented. Follow the course's AI-use disclosure requirements in the relevant milestone deliverables.
