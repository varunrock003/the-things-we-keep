# The Things We Keep

Paper Comet Studio — GAME 03, COMP3770

This is our game. It's a short 3D puzzle game in Unity, about 15 to 20 minutes. You play as the memory of a house on the last evening before it gets sold. You can't talk to the family, so you possess objects around the house instead. Every object does one thing: the kettle steams, the lamp reveals, the clock chimes, the radio plays, and the chair slides. Using them at the right time helps Leena and Theo find three keepsakes before they leave.

We went with a dollhouse style for the rooms. The rooms are proper 3D, but the camera stays fixed for each room and the front wall is cut away, so you can see the whole room at once. That made more sense to us than walking around in first person, and it's easier to see how the family reacts to what you do.

## Team

- Varunteja Katakam (varunrock003) — programming, repo and build
- Venkat Preetham Gogineni (Gogineng) — game and room design, playtesting
- Shruti Bharat Dadhania (ShrutiDadhania) — art, UI, documents and sound

## Setup

- Unity 6.3 LTS (6000.3.25f1), 3D (URP), Windows
- Open the project inside `unity/` with Unity Hub
- Run `git lfs install` once after cloning, our art and audio files are stored with LFS

## What's in here

- `unity/` — the Unity project
- `Docs/` — our design notes and art/UI notes

One thing we are careful about: don't commit `Library/`, `Temp/` or `Build/` folders. It's already handled in `.gitignore`, just don't force-add anything.
