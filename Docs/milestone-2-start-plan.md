# Milestone 2 handoff plan — proposed work, not yet done

**Project:** The Things We Keep  
**Course:** COMP3770, GAME 03  
**Overview deadline:** October 21, 2026  
**Official overview focus:** Initial Mechanics, Architecture; environment, materials, lighting and camera setup; methodology report plus deliverable.

## Priority 1 — Unity project foundation
- Create Universal 3D (URP) project inside `unity/`.
- Save `Kitchen.unity`, commit `Assets`, `Packages`, `ProjectSettings` and `.meta` files.
- Open it on a second team member's machine, capture version and errors.
- Keep every source file and Git commit attributable to the actual contributor.

## Priority 2 — first kitchen diorama
- Cutaway three-wall room; kitchen floor, counter, window, table, kettle and blue tin.
- Reusable URP materials; warm evening lighting and legible shadows.
- Fixed view covering the whole room, including consequences of player actions.

## Priority 3 — minimal architecture and gameplay (time permitting)
- Glowing spirit mote, WASD/arrow movement within the room.
- E toggles possession/release of nearby kettle; Space uses its single verb.
- Simple steam/fog state and window text clue.
- Kitchen state flag connecting the clue to the recipe-card result.
- Placeholder character reaction allowed if accurately marked as incomplete.

## Architecture outline
```
Unity scene: Kitchen
  Environment (floor, walls, window, counter)
  PuzzleProps (kettle, blue tin, recipe card)
  Spirit
  Main Camera (fixed)
  Directional Light
  KitchenRoomController

Scripts (proposed, filenames may change)
  SpiritController
  PossessionController
  PossessableObject / IObjectVerb
  KettleSteamVerb
  KitchenRoomController
```

## Proof and methodology report
Capture scene hierarchy, Game view, lighting, Project folders, Play Mode console, and any interaction that actually works. Document architectural choices, asset licence records, tests/defects, per-member Git links and work left for Milestone 3.

## Collaboration
- Varunteja: Unity foundation, input/possession architecture, Git integration
- Venkat: room and clue layout, kitchen puzzle design
- Shruti: palette, materials, lights and UI prompts
- Hassan: kettle verb effects and testing
Confirm team size with instructor: overview requests five members, current submitted GDD lists four.

**AI declaration:** Course overview asks for a tool/prompt/verification declaration in each milestone deliverable. Do not silently postpone it until the final phase; keep an accurate log of any assistance and include it at the required stage.
