# Milestone 3 — Hassan: Steam Stabilization and Lamp Reveal

**Member:** Hassan Hamadi · **Branch:** `milestone3/hassan-effects` · **Role:** gameplay effects, secondary interaction mechanics, technical testing

Handoff for Varunteja (TDD / Doxygen / alpha integration).

## Files

| File | Status | Purpose |
| --- | --- | --- |
| `Assets/Scripts/LampRevealVerb.cs` | New | Lamp's Reveal verb: light and clue fade in, shade glows |
| `Assets/Scripts/KettleSteamVerb.cs` | Changed | Stabilized steam verb (see below) |
| `Assets/Scripts/Editor/FeedbackTestSceneBuilder.cs` | New | Editor menu **Tools → Hassan → Build Feedback Test Scene**; rebuilds the test scene and lamp prefab from KitchenIntegrated |
| `Assets/Scenes/FeedbackTest_Hassan.unity` | New | Test scene: kitchen plus lamp. KitchenIntegrated is unchanged |
| `Assets/Prefabs/PF_LampReveal.prefab` | New | Self-contained lamp; all references are internal |

`PossessableObject`, `PossessionController` and `SpiritMovement` were **not** modified, to avoid conflicts with `milestone3/varun-architecture`.

## Object-verb architecture

Each household object has one verb. Possession and verbs are decoupled through a UnityEvent:

1. `PossessionController` (on the Spirit) holds `objectsInRoom`. **E** possesses the nearest enabled object within `possessionRange` and freezes `SpiritMovement`; E again releases it.
2. **Space** calls `PossessableObject.UseVerb()` on the possessed object, which invokes `onVerbUsed`.
3. A verb component on the same GameObject subscribes to `onVerbUsed` in `Awake` and unsubscribes in `OnDestroy`: `KettleSteamVerb.StartSteam`, `LampRevealVerb.ActivateReveal`.

Adding a new object = `PossessableObject` + a verb script + an entry in the Spirit's `objectsInRoom`. Verb scripts expose their own UnityEvents (`onSteamStarted/Stopped`, `onRevealed/onReset`) for sound, narrative or puzzle hooks without code changes.

## Steam stabilization (changes since Milestone 2)

The play test showed the steam was already **easy to see** from the fixed camera, so the SteamFX particle settings were left unchanged. The stabilization targeted robustness issues found in code review:

| Issue in Milestone 2 | Milestone 3 change |
| --- | --- |
| Every Space press emitted 10 extra particles with no limit; mashing Space flooded the system | New `burstCooldown` (0.5 s) between bursts; the timer still restarts on every press |
| `Play()` called on every press | Only called if not already playing |
| Missing `steamParticles` or renderer caused a NullReferenceException in `Awake`/`Update` | Null checks; a warning is logged instead |
| Listener was never removed from `onVerbUsed` | Removed in `OnDestroy` |
| Disabling the kettle mid-rattle left it tilted and tinted | `OnDisable` restores rotation and clears the tint |
| `MaterialPropertyBlock` could be null after a script reload in Play Mode (seen in testing) | Created lazily |
| No input validation | `[Min]` on durations and counts |
| No documentation | XML doc comments on the class and all public members |

Values kept from Milestone 2: duration 5 s, tail 1.2 s, burst 10, rattle angle 3, rattle speed 24.
SteamFX (unchanged): start lifetime 2.4, start speed 1.8 (clamped to 0.45), start size 0.24, rate over time 20, world space.

## Lamp Reveal

Hierarchy (root unscaled so the child text and light aren't distorted):

```
LampTest            (-2, 0.6, -1)  PossessableObject [Lamp / Reveal], LampRevealVerb, AudioSource
├─ LampBody         cylinder, scale (0.3, 0.6, 0.3), Mat_Wood_Dark
├─ LampShade        sphere, local (0, 0.6, 0), scale (0.45, 0.24, 0.45), Mat_Light_Warm
├─ LampLight        point light, warm, range 5, intensity 3 when on
└─ HiddenMessage    TextMeshPro "Some memories shine when we share them.", starts inactive
```

Behaviour:
- **Reveal:** enables the light, activates the clue, fades both in over 0.6 s, brightens the shade from 55% to a warm glow, logs `Lamp Reveal activated.`, invokes `onRevealed`.
- **Reveal Once (on):** later presses don't reveal again; the lamp flickers for 0.3 s so the press visibly registers.
- **Reset:** `ResetReveal()` (Inspector context menu, or **R** in Play Mode while `debugResetKey` is on) hides everything and re-arms Reveal.

Differences from the original task guide, and why:
- The light is toggled at the **component** level and its GameObject stays active. The guide disabled the GameObject, which the script can't turn back on.
- HiddenMessage is a child of the lamp from the start, so the prefab is self-contained.
- The lamp is an unscaled root with a `LampBody` child instead of a scaled cylinder root.

## Functional test results — 2026-11-18, FeedbackTest_Hassan

Console output read from the Unity Editor log; visual results from the tester's observation.

| Test | Expected | Result |
| --- | --- | --- |
| Possess kettle (E) | `Kettle possessed.` | Pass |
| Steam (Space) | SteamFX plays, rattle and tint | Pass. Steam easy to see |
| Repeat Steam mid-effect (3 presses) | Timer restarts, no errors | Pass |
| Release kettle (E) | Spirit moves again | Pass |
| Possess lamp (E) | `Lamp possessed.` | Pass |
| Reveal (Space) | Light and clue visible | Pass. Clue readable and in frame, glow visible on floor |
| Reveal again (Reveal Once) | No duplicate reveal, flicker | Pass. 11 repeat presses logged `already active` |
| Reset Reveal (R) | Light and clue hidden | Pass. `Lamp Reveal reset.` |
| Reveal after reset | Reveals again | Pass |
| Release lamp (E) | Spirit moves normally | Pass |
| Console | No red errors from gameplay scripts | Pass. One NullReferenceException occurred only when scripts were recompiled during Play Mode; fixed (lazy `MaterialPropertyBlock`) |

## Still to record (do not fill with estimates)

### Performance (Window → Analysis → Profiler, CPU Usage)

| Scenario | CPU frame time (ms) | Notes |
| --- | --- | --- |
| Baseline, no effect | | |
| Kettle Steam active | | |
| Lamp Reveal active | | |

### Playtest (at least 2 people)

| Tester | Noticed Steam? | Noticed Reveal? | Understood controls? | Issues |
| --- | --- | --- | --- | --- |
| | | | | |
| | | | | |

### Audio

| File | Source | Author | Licence | Edits |
| --- | --- | --- | --- | --- |
| `Assets/Audio/lamp_switch_on.wav` | [Lamp String On-Off](https://pixabay.com/sound-effects/household-lamp-string-on-off-38279/) (Pixabay) | FocusBay (Freesound), uploaded by freesound_community | Pixabay Content License | Cut 0.92–1.40 s (first click), +12 dB with limiter (peak −0.9 dB), 80 ms fade-out |
| `Assets/Audio/lamp_switch_off.wav` | same as above | same | Pixabay Content License | Cut 3.18–3.66 s (second click), +12 dB with limiter (peak −0.9 dB), 80 ms fade-out |
| `Assets/Audio/lamp_buzz_loop.wav` | [Lamp Buzz](https://pixabay.com/sound-effects/household-lamp-buzz-6132/) (Pixabay) | an_tony (Freesound), uploaded by freesound_community | Pixabay Content License | +20 dB gain (original averaged −54 dB), 50 ms fade-in / 60 ms fade-out for looping |

Downloaded 2026-11-18; edited with ffmpeg. How it's wired (via **Tools → Hassan → Wire Lamp Audio**, which edits PF_LampReveal):
- AudioSource 1 (switch, one-shot, 2D): `switchOnClip` on Reveal and on a repeat press, `switchOffClip` on Reset.
- AudioSource 2 (buzz, loop, 2D): starts on Reveal, volume follows the light fade up to `buzzVolume` (0.35), stops on Reset.

### Hours (CAD $56/h)

| Task | Estimated | Actual |
| --- | --- | --- |
| Steam stabilization | 3 h | |
| Lamp Reveal | 4 h | |
| Feedback and audio | 2 h | |
| Testing and validation | 2 h | |
| Documentation and GitHub | 1 h | |
| **Total** | **12 h ($672)** | |

## Known limitations
- The lamp exists only in the test scene; integrating it into the alpha scene is Varunteja's step.
- The steam keeps running if the kettle is released mid-effect (by design: the timer finishes on its own).
- Turn off `debugResetKey` on the lamp in the final build.
- The hidden message is a sample clue; the final living-room clue text belongs to the narrative work.
