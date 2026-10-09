# Art and UI notes

## Look of the game

3D dollhouse. Each room is a small 3D room with the front wall removed and one fixed camera. We are using low-poly models, mostly built from simple shapes and free asset packs, with plain URP materials. Colours are quiet and domestic, warm light in the evening rooms. We are not trying to make it look realistic, a clean stylised look is enough and it keeps all five rooms consistent.

Asset sources and licences will be listed here as we add them, so we don't lose track later.

| Asset | Source | Licence | Where it's used |
| --- | --- | --- | --- |
| (example row, delete when real assets come in) | Kenney | CC0 | furniture |

## Naming

- Rooms and objects: `room_object_state`, for example `kitchen_kettle_idle`
- Possessable objects: `obj_object_verb`, for example `obj_kettle_steam`
- Sounds: `audio_room_event`, for example `audio_kitchen_kettle`

## UI

The UI is kept small on purpose. When you possess an object there is one line at the bottom saying what it can do, like "Steam — fog the window". One line only. A player should be able to guess the verb within about 20 seconds of possessing the object, from how it looks and moves, even before reading the line.

The keepsake prompt comes up in the room, where the family is looking, not in a separate menu screen.
