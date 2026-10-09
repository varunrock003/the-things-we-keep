# Design notes

These are our working notes for the puzzles. They will change after playtesting, right now this is the version we are building first.

## Basic structure

Five rooms, and in every room there is one object you can possess:

| Room | Object | What it does |
| --- | --- | --- |
| Kitchen | Kettle | Steams and fogs the window |
| Living room | Lamp | Light reveals writing on the photo |
| Hallway | Clock | Chimes and gets everyone's attention |
| Bedroom | Radio | Plays the recorded song |
| Porch | Chair | Slides under the high hook |

Each room is meant to be a small chain of 2 or 3 steps. Nothing in the game needs fast reactions. If nothing happens for about 90 seconds we show a small hint, something like highlighting the object family, not the full answer.

## Keepsakes

There are three: the recipe card, the photograph, and the recorded song. Our rule is that every keepsake needs two clues before you get it. One clue is in the room itself (a trace, something written, something left behind) and the other one is how Leena or Theo reacts. We did that because if a player misses one clue, the other one can still make the story clear.

## First room

The kitchen teaches the whole game. First you try a door and a drawer and nothing works, so the player learns they can't touch things directly. Then the kettle is the only thing that responds. Steam fogs the window, the writing shows up, Leena reads it and opens the tin. If this room takes more than about four minutes in testing, the chain is probably too long and we will cut a step.
