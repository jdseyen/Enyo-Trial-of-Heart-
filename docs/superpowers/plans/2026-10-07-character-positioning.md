# Per-Line Character Positioning Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Let any dialogue line optionally slide any on-screen character to a preset or custom screen X the moment the line appears, configured entirely from the DialogueManager inspector.

**Architecture:** Three-file change to the existing VN dialogue system. `DialogueLine` gains a `characterPositions[]` row list (mirrors the existing `characterExpressions` pattern); `CharacterActor` gains a `MoveToX(targetWorldX, duration)` slide method that lerps its root transform; `DialogueManager` gains preset spot fields and applies rows during `ShowLine()`. Positions are absolute world X of the character sprite (camera is orthographic at x 0, orthoSize 5, 16:9, so the screen spans world X ≈ -8.9 to +8.9).

**Tech Stack:** Unity 2022.3.58f1, C#, no new packages, no test framework (project has none — verification is compile checks via Unity MCP plus scripted play-mode assertions and a final playtest checklist).

**Spec:** `docs/superpowers/specs/2026-10-07-character-positioning-design.md`

## Global Constraints

- Only three files may change for code: `Assets/Scripts/Character/DialogueLine.cs`, `Assets/Scripts/Character/CharacterActor.cs`, `Assets/DialogueManager.cs`. No custom property drawers/editors in v1 (default inspector UI only).
- Preset defaults: `leftX = -5.5`, `centerX = 0`, `rightX = 5.0`, `slideDuration = 0.5`.
- Position semantics: absolute world X of the character's sprite; only X changes — each character keeps its own Y and Z.
- No automatic revert: a position persists until a later line changes it. A line with no rows changes nothing.
- Move starts when the line appears (inside `ShowLine()`), sliding with `Time.deltaTime` so pause/`Time.timeScale` is respected.
- `PositionWhere.Unchanged` is the default value of the enum and is a no-op (a freshly added row is inert).
- A new `MoveToX` cancels any slide already in progress on that character (no stacked coroutines).
- Unknown character in a row → `Debug.LogWarning`, skip the row, never throw.
- `duration <= 0` → instant snap (no division by zero).
- Slides must work on inactive (hidden) characters without exceptions.

## Review Focus

Conditions the spec implies but that most easily break — each has a verification step in the task that owns the code:

1. **Sprite child offsets differ** (Enyo sprite local x -5.45, Mother -0.55): `MoveToX` must compute delta from the sprite's *current world X*, or characters land in wrong spots. Verified in Task 2: after a slide, sprite world X ≈ target for both an offset-heavy and a near-zero-offset character.
2. **Rapid dialogue clicking** starts overlapping moves: the second `MoveToX` must fully replace the first. Verified in Task 2: two back-to-back calls with different targets end at the second target only.
3. **Moving a hidden character** (inactive GameObject): must not throw and must apply. Verified in Task 2 on the inactive `Characters/Enyo` root.
4. **Y/Z drift**: repeated slides must leave Y exactly as found. Verified in Task 2: root Y before == after.
5. **Row targeting a speaker not in `characters[]`** must warn and continue, leaving the line playable. Verified in Task 3: row with `SpeakerType.Roberto` (not in VN_ACT1) logs one warning, no exception, later rows still apply.

---

### Task 1: Data model — position rows on DialogueLine

**Files:**
- Modify: `Assets/Scripts/Character/DialogueLine.cs` (append after `CharacterExpressionData`; add field to `DialogueLine`)

**Interfaces:**
- Consumes: existing `SpeakerType` enum (same file).
- Produces (Task 3 relies on these exact names):
  - `public enum PositionWhere { Unchanged, Left, Center, Right, Custom }`
  - `public class CharacterPositionData { public SpeakerType character; public PositionWhere where; public float customX; }` (marked `[System.Serializable]`)
  - `public CharacterPositionData[] characterPositions;` field on `DialogueLine`, with comment matching the file's existing style

- [ ] **Step 1: Add the enum, data class, and field**

In `Assets/Scripts/Character/DialogueLine.cs`: add `PositionWhere` and `CharacterPositionData` exactly as named above (follow the file's comment style — every public field in this file has a plain-English comment), then add `public CharacterPositionData[] characterPositions;` to `DialogueLine` after `characterExpressions`.

- [ ] **Step 2: Compile and verify serialization surfaces**

Run MCP `refresh_unity` (scope scripts, compile request), then `read_console` filtered to errors.
Expected: 0 errors.
Then run `execute_code` (editor mode):

```csharp
var dm = Object.FindObjectOfType<DialogueManager>();
var so = new UnityEditor.SerializedObject(dm);
var p = so.FindProperty("dialogueLines.Array.data[0].characterPositions");
return p != null ? "OK: characterPositions serializes" : "FAIL: property not found";
```
Expected: `OK: characterPositions serializes`

- [ ] **Step 3: Commit**

```bash
git add Assets/Scripts/Character/DialogueLine.cs
git commit -m "Add per-line character position data model"
```

---

### Task 2: CharacterActor.MoveToX slide

**Files:**
- Modify: `Assets/Scripts/Character/CharacterActor.cs`

**Interfaces:**
- Consumes: nothing new (existing `CharacterActor` fields).
- Produces (Task 3 relies on this exact signature):
  - `public void MoveToX(float targetWorldX, float duration)` — slides so the character's sprite lands at `targetWorldX`; cancels any running slide on this actor; `duration <= 0` snaps instantly; safe on inactive GameObjects.

- [ ] **Step 1: Implement `MoveToX`**

Approach the implementer must follow (signature fixed, body is theirs):
- Locate the character's sprite: first active child `SpriteRenderer`; if none active (e.g. hidden character), fall back to the first child `SpriteRenderer` found in children; if there is none at all, log a warning and return.
- Compute `deltaX = targetWorldX - sprite.bounds.center.x` (sprite world X) once, at call time.
- If `duration <= 0`: apply `transform.position += Vector3.right * deltaX` and return.
- Otherwise store slide state (remaining delta, elapsed, duration, running coroutine reference), stop any previously running slide coroutine first, then lerp the root's `position.x` from its start value toward `start + delta` over `duration` using `Time.deltaTime`, completing exactly on target.
- The slide must never touch Y or Z. While the GameObject is inactive, `StartCoroutine` cannot run — in that case apply the delta immediately and return (positions a hidden character for when it later enters).

- [ ] **Step 2: Compile check**

Run MCP `refresh_unity` (compile request) then `read_console` errors.
Expected: 0 errors.

- [ ] **Step 3: Verify Review Focus 1, 2, 3, 4 (play mode)**

Enter play mode (MCP `manage_editor` action `play`). Then three `execute_code` calls with real-time waits between them (bash `Start-Sleep` between calls lets play mode run the slides):

a) Start two moves back-to-back on Mother (Review Focus 2) and note Y (Focus 4):
```csharp
var dm = Object.FindObjectOfType<DialogueManager>();
var mother = Array.Find(dm.characters, c => c.speaker == SpeakerType.Mother);
var enyo = Array.Find(dm.characters, c => c.speaker == SpeakerType.Enyo);
float yBefore = mother.transform.position.y;
mother.MoveToX(5f, 0.4f);
mother.MoveToX(2f, 0.4f);          // must replace the first slide
enyo.MoveToX(-2f, 0.4f);           // enyo root starts inactive: hidden-move (Focus 3)
return "started";
```
Expected: returns `started`, console has no exceptions.

b) Bash: `Start-Sleep -Seconds 1`

c) Assert results:
```csharp
// sprite world X must equal target for both (Focus 1), Y unchanged (Focus 4)
```
Assert with `Mathf.Approximately`-style checks (tolerance 0.05): Mother's active sprite center.x ≈ **2** (second target, not 5), Enyo's sprite center.x ≈ **-2**, `mother.transform.position.y == yBefore`.
Expected: all pass; console shows no errors during the whole sequence.

- [ ] **Step 4: Stop play mode and commit**

MCP `manage_editor` action `stop`.

```bash
git add Assets/Scripts/Character/CharacterActor.cs
git commit -m "Add sliding MoveToX to CharacterActor"
```

---

### Task 3: DialogueManager integration

**Files:**
- Modify: `Assets/DialogueManager.cs` (fields near the top; `ApplyCharacterPositions` near `SetCharacterExpressions`; call inside `ShowLine`)

**Interfaces:**
- Consumes: Task 1 types (`PositionWhere`, `CharacterPositionData`, `DialogueLine.characterPositions`), Task 2 signature (`CharacterActor.MoveToX(float, float)`), existing `GetCharacter(SpeakerType)`.
- Produces (Task 4 relies on these inspector field names):
  - `public PresetSpots presetSpots` where `PresetSpots` is `[System.Serializable]` with `public float leftX = -5.5f; public float centerX = 0f; public float rightX = 5f;`
  - `public float slideDuration = 0.5f;`
  - `private void ApplyCharacterPositions(DialogueLine line)`

- [ ] **Step 1: Add fields and `ApplyCharacterPositions`**

Add the `PresetSpots` class (defined above `DialogueManager` in the same file) and both public fields with comments in the file's style. Implement `ApplyCharacterPositions(DialogueLine line)`:
- Return immediately if `line.characterPositions` is null or empty.
- For each row: look up `GetCharacter(row.character)`; if null, `Debug.LogWarning` naming the speaker and continue (Review Focus 5).
- Resolve target X: `Unchanged` → continue (no-op); `Left/Center/Right` → `presetSpots.leftX/centerX/rightX`; `Custom` → `row.customX`.
- Call `character.MoveToX(targetX, slideDuration)`.

- [ ] **Step 2: Call it from `ShowLine`**

Insert `ApplyCharacterPositions(line);` in `ShowLine()` immediately after `SetCharacterExpressions(line);` (before `ChangeBackground(line);`).

- [ ] **Step 3: Compile check**

MCP `refresh_unity` + `read_console` errors → 0 errors.

- [ ] **Step 4: Verify Review Focus 5 (warning path)**

Editor-mode `execute_code`:
```csharp
var dm = Object.FindObjectOfType<DialogueManager>();
var line = new DialogueLine {
    characterPositions = new[] {
        new CharacterPositionData { character = SpeakerType.Roberto, where = PositionWhere.Left },
        new CharacterPositionData { character = SpeakerType.Mother,  where = PositionWhere.Center },
    }
};
var mother = Array.Find(dm.characters, c => c.speaker == SpeakerType.Mother);
var before = mother.transform.position.x;
// invoke the private method via reflection, then inspect the log
typeof(DialogueManager).GetMethod("ApplyCharacterPositions",
    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
    .Invoke(dm, new object[] { line });
return "invoked";
```
Expected: `invoked`; `read_console` (filter `Roberto`) shows exactly one warning; no exception. (Position application itself is exercised in play mode — Task 2 already proved `MoveToX`; full line-driven flow is Task 4.)

- [ ] **Step 5: Commit**

```bash
git add Assets/DialogueManager.cs
git commit -m "Apply per-line character positions in DialogueManager"
```

---

### Task 4: Configure VN_ACT1 and run the playtest checklist

**Files:**
- Modify: `Assets/Scenes/ACT 1/1.1_VN_SCENE/VN_ACT1.unity` (serialized DialogueManager data only)

**Interfaces:**
- Consumes: all fields from Tasks 1-3 via the scene's DialogueManager component; play-mode behavior proven in Task 2.

- [ ] **Step 1: Add the Mother → Right row to the "silly goose" line**

In editor-mode `execute_code`, find the DialogueManager, locate the `dialogueLines` element whose `dialogue` starts with `"here you silly goose"`, and via `SerializedObject` set its `characterPositions` to one element: `character = Mother`, `where = Right`, `customX = 0`. Save the scene (MCP `manage_scene` `save` with the full path — **use `path`, not `name`, to avoid creating a duplicate scene file**).

Verify with a fresh `execute_code` reading the row back: returns `Mother/Right`.

- [ ] **Step 2: Enter play mode and drive the line-driven flow (Review Focus 1 + timing)**

Enter play mode. Then use `execute_code` to jump `currentLine` to the "silly goose" line by reflection (set `currentLine` field to the found index, invoke private `ShowLine()`), bash `Start-Sleep -Seconds 1`, then assert Mother's active sprite center.x ≈ **5.0** (tolerance 0.05) and that `BackgroundManager` shows the food background.

- [ ] **Step 3: Run the remaining playtest checklist (spec §Testing)**

Walk the spec's 9-step checklist; steps that need eyes (smooth slide vs snap, lines 1-3 visually unchanged, "stays right on later lines") are confirmed by you watching the Game view, with MCP screenshots (`manage_camera` screenshot) as evidence where useful. Scripted assertions cover: later line without rows keeps her at 5.0; a temporary `Custom` row moves Enyo to -3; a `Mother → Center` row returns her sprite x ≈ 0; fast Next-clicking mid-slide ends at the last target.
Record pass/fail per item.

- [ ] **Step 4: Remove any temporary verification rows, save, commit**

Scene must end with exactly one added row (Mother → Right on the silly goose line) or fewer if the user opts out. Save via `manage_scene` `save` with `path`.

```bash
git add "Assets/Scenes/ACT 1/1.1_VN_SCENE/VN_ACT1.unity"
git commit -m "Configure Mother position override on food scene line"
git push origin visual-novel-test-scene
```
