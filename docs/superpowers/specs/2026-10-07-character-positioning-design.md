# Design: Per-Line Character Positioning

Date: 2026-10-07
Status: Approved
Scope: VN dialogue system (DialogueManager, DialogueLine, CharacterActor)

## Goal

Allow any dialogue line to optionally move any on-screen character to a new
position the moment that line appears, with a smooth slide, configured entirely
from the DialogueManager inspector.

Example use case: on the line "here you silly goose, i made you your favorite!",
the Mother sprite slides to the far right of the screen so the food background
is clearly visible. Her position on all earlier lines is untouched, and she
stays on the right on later lines until another line moves her again.

## Decisions (from brainstorming)

1. **No automatic revert.** A position change persists until a later line
   explicitly changes it again. This matches the existing background behavior
   (`BackgroundType.None` = do not change).
2. **Timing.** The move starts when the line appears (same moment the text and
   background are applied), not after the player advances.
3. **Position specification.** Simplified per-line approach (option B+): each
   line has optional "Move Character" rows with a character dropdown plus a
   where dropdown (Unchanged / Left / Center / Right / Custom). The three named
   presets are simple X numbers defined once at the top of the DialogueManager.
   Rejected alternative: a named "position slot library" with capture buttons
   (option A) — more setup and indirection than this project needs.
4. **Movement style.** Smooth slide (lerp over a configurable duration,
   default 0.5s), not an instant snap.
5. **Multi-character future-proofing.** Rows are a list per line, mirroring the
   existing `characterExpressions` pattern. Any `SpeakerType` may appear in any
   row, multiple characters may move on the same line, and future characters
   need no code changes.

## Inspector design

Top of DialogueManager (defined once):

```
Preset Spots:   Left: -5.5    Center: 0    Right: 5.0
Slide Duration: 0.5
```

Per dialogue line (optional):

```
Move Character:            [+ Add]
   Who:   [Mother ▼]
   Where: [Right ▼]        Unchanged / Left / Center / Right / Custom
   X:     (only used when Where = Custom; always visible in the default
           inspector — no custom property drawer in v1)
```

- A line with no rows behaves exactly as today (nobody moves).
- The first row added defaults to `Where = Unchanged` so it is inert until the
  user picks a spot (safe default, mirrors how `characterAction` defaults to
  `None`).
- Preset values are world X coordinates of the character sprite in the Scene
  view. The camera is orthographic at x = 0 with orthoSize 5 and 16:9 aspect,
  so the visible screen spans world X approx. -8.9 to +8.9; defaults sit
  comfortably inside that range and the user may retune them at any time.

## Runtime behavior

`DialogueManager.ShowLine()` applies the line's position rows after
expressions are set (order: character action, name, text, expressions,
positions, background):

1. For each `CharacterPositionData` row, resolve `where` to a target world X:
   `Left/Center/Right` use `presetSpots`, `Custom` uses the row's own X field,
   `Unchanged` is skipped.
2. Look up the `CharacterActor` via the existing `GetCharacter()` helper.
3. Call `character.MoveToX(targetWorldX, slideDuration)`.

`CharacterActor.MoveToX(float targetWorldX, float duration)`:

- Computes the delta between the target X and the character sprite's current
  world X, then lerps the character's root transform position over `duration`
  seconds (coroutine using `Time.deltaTime`, so the slide respects pause /
  `Time.timeScale`).
- The target is the sprite's absolute screen position, so `Right` produces the
  same visible spot for every character regardless of their sprite child
  offsets (Enyo's sprite sits at local x -5.45, Mother's at -0.55) and
  regardless of where the character was on previous lines.
- Only X moves; each character keeps their own Y and Z.
- Because the root moves, all expression sprite children shift together;
  switching expressions mid-slide or after arrival does not disturb the
  position.
- If a new `MoveToX` arrives while a slide is in progress, the previous
  coroutine is stopped and replaced (rapid dialogue clicking cannot stack
  slides).
- Moving a currently hidden (inactive) character is allowed; the position
  applies when they later enter. If no active sprite child is found, fall back
  to the first `SpriteRenderer` among the children; if there is none at all,
  log a warning and skip.

## Data model changes

`DialogueLine.cs`:

```csharp
public enum PositionWhere { Unchanged, Left, Center, Right, Custom }

[System.Serializable]
public class CharacterPositionData
{
    public SpeakerType character;
    public PositionWhere where;
    public float customX;   // only used when where == Custom
}
```

`DialogueLine` gains one field: `public CharacterPositionData[] characterPositions;`

`CharacterActor.cs` gains `MoveToX(float targetWorldX, float duration)` plus
private slide state (running coroutine reference).

`DialogueManager.cs` gains:

- `public float leftX = -5.5f; public float centerX = 0f; public float rightX = 5f;`
  (grouped as a serializable `PresetSpots` class for a tidy inspector)
- `public float slideDuration = 0.5f;`
- `private void ApplyCharacterPositions(DialogueLine line)` called from
  `ShowLine()`.

## Error handling

- Null/empty `characterPositions` array: skip silently (common case).
- Row references a character not present in `characters[]`: warning log with
  the speaker name, skip the row (same pattern as `HandleCharacterAction`).
- `slideDuration <= 0`: treat as instant snap (guard against division by zero).

## Testing (manual playtest checklist)

The project has no automated test infrastructure; verification is a playtest
of VN_ACT1 in the editor:

1. Lines 1-3 play exactly as before (no movement, no regressions).
2. Adding a row `Mother → Right` on the "here you silly goose..." line slides
   Mother right when that line appears; she was untouched on earlier lines.
3. She remains on the right on subsequent lines that have no rows.
4. A later `Mother → Center` row slides her back.
5. A `Custom` row moves Enyo to an arbitrary X.
6. One line with two rows moves two characters simultaneously.
7. Rapidly clicking Next during a slide does not glitch or stack movement.
8. A row naming a missing character logs one warning and does not break the
   line.
9. `Unchanged` row (freshly added) changes nothing.

## Out of scope

- Vertical (Y) positioning, rotation, scale, or fade-in moves.
- Easing curves beyond linear lerp (can be added later if wanted).
- A custom property-drawer UI; the default inspector list (same style as
  `characterExpressions`) is considered comfortable enough for v1.
- Automatic revert behavior (explicitly rejected by design decision 1).
