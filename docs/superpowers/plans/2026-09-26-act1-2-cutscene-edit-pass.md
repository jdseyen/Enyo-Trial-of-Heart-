# Act 1–2 Cutscene Edit Pass Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.
>
> **Project adaptation:** scene surgery happens in the Unity Editor (by the human, or via Unity MCP under the project's standing permission gate). Python audit scripts are the automated test equivalent; play-throughs are the integration tests.

**Goal:** Execute the approved Act 1–2 cutscene edit pass — text fixes, story-thread edits, and the 23→14 / 22→14 page restructures — each verified by an in-repo audit harness before the next task starts.

**Architecture:** Two phases per act. Phase A is pure text correction on existing objects (typos, quotes, whitespace, the illness→memory line). Phase B is structure: move Text GameObjects onto merged pages per the spec's tables, rewire `nextText` chains, update the pager's `pages[]` array, delete emptied pages. Nothing in `Assets/Scripts/` changes — the pagers already tolerate any array length.

**Tech Stack:** Unity Editor (scene files, TMP + TypewriterText components), Python 3 stdlib (audit harness), Git (branch `act1-2-cutscene-edit`).

**Spec:** `docs/superpowers/specs/2026-09-26-act1-2-cutscene-edit-pass-design.md` — this plan argues from the spec; executors read both. References like "§4 row N" point into it; the spec holds the full merge tables and typo lists, so they are not duplicated here.

## Global Constraints

- Edit ONLY these paths: `Assets/Scenes/ACT 1/1.1_ACT1_CutScene/01_Act1_Story.unity`, `Assets/Scenes/ACT 2/2.1_Act2_CutScene/02_Act2_Story.unity`, `tools/cutscene_audit/*`. No C# changes; no other scenes.
- Act 1 result: exactly 14 pages, ~269 words (acceptable band 243–297). Act 2 result: exactly 14 pages, ~327 words (acceptable band 288–352).
- Sacred lines (spec §7.3) survive verbatim — typo fixes allowed, trimming never.
- Act 1 video pages (`Page_01_Video `, `Page_03_Video`) untouched; fade overlay, Next/Back buttons untouched.
- `nextText` invariant: every chain resolves within its page to a live object — 0 dangling, 0 cross-page after every task.
- One commit per task on `act1-2-cutscene-edit`. **NEVER push without the user's explicit approval.**
- Standing gate: the user's approval of this plan authorizes its steps and commits; edits outside this plan's file list still require fresh permission.

## Review Focus

1. **Broken `nextText` chain after deleting a Text object** — later text silently never auto-plays; no error. → dangling check in `scan_bugs` + full-text play check per act (Tasks 3, 5).
2. **Stale `pages[]` entry** pointing at a deleted page — pager skips or throws. → extract script must print exactly `PAGES: 14` (Tasks 3, 5).
3. **Sacred line trimmed during a merge** — later acts lose their setup with no visible symptom. → exact-fragment search step after each restructure (Tasks 3, 5).
4. **Video pages or button/fade wiring disturbed** by hierarchy edits. → end-to-end play-through: videos play, Next/Back/fade work, last page loads the next scene (Tasks 3, 5).
5. **Merged page displays texts out of intended order** (sibling order ≠ reading order). → read-through of every merged page during the play check (Tasks 3, 5).

---

### Task 1: Audit harness in repo (test-first baseline)

**Files:**
- Create: `tools/cutscene_audit/extract_cutscene.py`
- Create: `tools/cutscene_audit/scan_bugs.py`

**Interfaces:**
- Produces: `python tools/cutscene_audit/extract_cutscene.py "<Act1 scene>" "<Act2 scene>"` → per-page word lines plus `PAGES:` / `TOTAL WORDS:` totals.
- Produces: `python tools/cutscene_audit/scan_bugs.py` → six checks per scene: literal-escape bugs, nextText refs/dangling, whitespace offenders (prints each offending text + doc id), visible unbalanced quotes (prints each), cross-page nextText.
- Consumed by: Tasks 2–6 as their only automated verification.

- [ ] **Step 1: Copy the extractor into the repo unchanged**

Copy `C:\Users\Jade\AppData\Local\Temp\opencode\extract_cutscene.py` → `tools/cutscene_audit/extract_cutscene.py`. If the temp file is gone, ask the design-session partner to regenerate it.

- [ ] **Step 2: Create the combined bug scanner**

Merge `C:\Users\Jade\AppData\Local\Temp\opencode\scan_bugs.py` (literal-escape, nextText refs/dangling, whitespace) and `scan2.py` (visible quote balance, cross-page nextText) into one `tools/cutscene_audit/scan_bugs.py`. Import the extractor with `sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))`. The whitespace and quote checks must print each offending text value with its document id, not just counts.

- [ ] **Step 3: Run both scripts and verify the exact baseline**

Run:
`python tools/cutscene_audit/extract_cutscene.py "Assets/Scenes/ACT 1/1.1_ACT1_CutScene/01_Act1_Story.unity" "Assets/Scenes/ACT 2/2.1_Act2_CutScene/02_Act2_Story.unity"`

Expected:
- Act 1: `PAGES: 23`, `TOTAL WORDS: 384`
- Act 2: `PAGES: 22`, `TOTAL WORDS: 435`

Run: `python tools/cutscene_audit/scan_bugs.py`

Expected:
- Act 1: literal-escape 0; nextText 37 refs / 0 dangling; whitespace 4; unbalanced quotes 0; cross-page 0
- Act 2: literal-escape 0; nextText 32 refs / 0 dangling; whitespace 9; unbalanced quotes 6; cross-page 0

- [ ] **Step 4: Commit**

```bash
git add tools/cutscene_audit
git commit -m "Add cutscene audit test harness"
```

---

### Task 2: Act 1 text fixes and story-thread line (pre-structure)

**Files:**
- Modify: `Assets/Scenes/ACT 1/1.1_ACT1_CutScene/01_Act1_Story.unity` (text values only — no object moves, no deletions)

**Interfaces:**
- Consumes: Task 1 scripts.
- Produces: typo-corrected Act 1 scene with the illness→memory link (spec §7.1); consumed by Task 3's merge drafts, which must quote these corrected sacred lines.

- [ ] **Step 1: Apply the seven spelling/grammar fixes from spec §6.3 "Act 1"**

Each row gives current text → fix text. Edit the `m_text` of the named page's Text object in the Inspector.

- [ ] **Step 2: Apply spec §7.1 — rewrite P14's memory-decline line so the illness causes it**

Draft one replacement line, ≤ the original's word count, making sickness the cause of memory decline (spec suggests the direction: "little by little, the sickness took her memory"). Apply it.

- [ ] **Step 3: Trim leading/trailing whitespace on the four Act 1 texts the scanner printed in Step 3 of Task 1**

- [ ] **Step 4: Run `python tools/cutscene_audit/scan_bugs.py` and filter to Act 1**

Expected: whitespace **0**; all other Act 1 checks unchanged (escape 0, nextText 37/0, quotes 0, cross 0).

- [ ] **Step 5: Run the extractor; expected Act 1 still `PAGES: 23`** (word total may drift by ±5)

- [ ] **Step 6: Spot-play Act 1 to page 15**

Expected: page 14 shows the new illness line; the seven fixed lines read as specified; no missing text.

- [ ] **Step 7: Commit**

```bash
git add "Assets/Scenes/ACT 1/1.1_ACT1_CutScene/01_Act1_Story.unity"
git commit -m "Fix Act 1 cutscene text bugs and link illness to memory decline"
```

---

### Task 3: Act 1 restructure to 14 linearized pages

**Files:**
- Modify: `Assets/Scenes/ACT 1/1.1_ACT1_CutScene/01_Act1_Story.unity` (structure + merged copy)

**Interfaces:**
- Consumes: Task 1 scripts, Task 2 corrected text.
- Produces: Act 1 with exactly 14 pages in the spec §4 target order; consumed by Task 6's final verification.

- [ ] **Step 1: Draft the merged-page copy for spec §4 rows 4–14**

For each row: combine the source pages' text, hit the row's word budget, keep §7.3 sacred lines verbatim (Task 2's corrections included), and follow §7.2 — trim wordiness, never Enyo's acts of service. Where a row says a caption may be cut (row 4), play the source page first and decide. **Present all drafts to the user for approval before touching the scene.**

- [ ] **Step 2: Apply the approved copy to the source pages' Text objects**

- [ ] **Step 3: Move Text GameObjects into target pages per the spec §4 "Source pages" column**

This includes the linearization: P18+P19 relocate to target page 9 (after the wound page P13, before the illness pages). Set each merged page's Text objects to sibling order matching the table's reading order. Move — do not recreate (preserves TMP settings and `nextText` components). Do not touch video pages.

- [ ] **Step 4: Rewire every `nextText` chain within each page**

Select each TypewriterText: its `nextText` points to the next surviving Text on the same page, or 0 at page end. A deleted object silently breaks the chain.

- [ ] **Step 5: Delete emptied page GameObjects; set the pager's `pages[]` to exactly 14 entries in table order**

In the Inspector on `Act1StoryPager`. Remove stale array entries.

- [ ] **Step 6: Run the extractor**

Expected: Act 1 `PAGES: 14`, `TOTAL WORDS:` in 243–297.

- [ ] **Step 7: Run the scanner**

Expected Act 1: whitespace 0, quotes 0, escape 0, nextText 0 dangling, cross-page 0.

- [ ] **Step 8: Search the scene file for the three sacred fragments**

Run (PowerShell): `Select-String -Path "Assets\Scenes\ACT 1\1.1_ACT1_CutScene\01_Act1_Story.unity" -Pattern "called Enyo a failure|running out|far more than their tiny old shop"`
Expected: all three found (one hit each minimum).

- [ ] **Step 9: Full play-through of Act 1**

Checklist: both video pages play; every page's typewriter completes fully (no silent missing text — this is the `nextText` test); Next advances, Back returns and the Back button hides on page 1; fade transitions run; page 14's Next loads `02_Act2_Story`.

- [ ] **Step 10: Commit**

```bash
git add "Assets/Scenes/ACT 1/1.1_ACT1_CutScene/01_Act1_Story.unity"
git commit -m "Restructure Act 1 cutscene to 14 linearized pages"
```

---

### Task 4: Act 2 text fixes (quotes, typos, whitespace)

**Files:**
- Modify: `Assets/Scenes/ACT 2/2.1_Act2_CutScene/02_Act2_Story.unity` (text values only)

**Interfaces:**
- Consumes: Task 1 scripts.
- Produces: quote-normalized, typo-free Act 2 text; consumed by Task 5's merge drafts.

- [ ] **Step 1: Apply spec §6.1 quote fixes (four items)**

Close the "Oh, sweetie…" speech; normalize the P20 split pair to matching curly quotes; normalize the P15 note split pair the same way; fix P17's curly-open/straight-close mismatch. Never balance one side of a split pair alone.

- [ ] **Step 2: Apply the seven spelling/grammar fixes from spec §6.3 "Act 2"**

- [ ] **Step 3: Trim whitespace on the nine flagged Act 2 texts; remove the two trailing `\n` sequences on P15's note lines**

- [ ] **Step 4: Run `python tools/cutscene_audit/scan_bugs.py` and filter to Act 2**

Expected: whitespace **0**; unbalanced quotes **exactly 4** — the two intentional split pairs (mother's hug speech, creature's note), each object showing matching style; nextText 32/0; cross-page 0; escape 0. If the count is not 4, an object was balanced wrongly — re-read spec §6.1's rule of thumb.

- [ ] **Step 5: Play Act 2 pages 15–22**

Expected: note reads as two clean lines with no stray blank line after it; quotes look consistent; "Oh, sweetie…" speech closes properly.

- [ ] **Step 6: Commit**

```bash
git add "Assets/Scenes/ACT 2/2.1_Act2_CutScene/02_Act2_Story.unity"
git commit -m "Fix Act 2 cutscene text bugs and normalize quote style"
```

---

### Task 5: Act 2 restructure to 14 pages

**Files:**
- Modify: `Assets/Scenes/ACT 2/2.1_Act2_CutScene/02_Act2_Story.unity` (structure + merged copy)

**Interfaces:**
- Consumes: Task 1 scripts, Task 4 corrected text.
- Produces: Act 2 with exactly 14 pages in spec §5 order; consumed by Task 6.

- [ ] **Step 1: Draft the merged-page copy for spec §5 rows 2–13**

Same rules as Task 3 Step 1 (budgets, sacred lines verbatim, §7.2). Act 2 needs no reordering — pure merge. **Present all drafts to the user for approval before touching the scene.**

- [ ] **Step 2: Apply the approved copy to the source pages' Text objects**

- [ ] **Step 3: Move Text GameObjects into target pages per the spec §5 "Source pages" column; set sibling order to reading order; keep P1's structural page untouched**

- [ ] **Step 4: Rewire every `nextText` chain within each page** (same rule as Task 3 Step 4)

- [ ] **Step 5: Delete emptied page GameObjects; set `Act2StoryPager.pages[]` to exactly 14 entries in table order**

- [ ] **Step 6: Run the extractor**

Expected: Act 2 `PAGES: 14`, `TOTAL WORDS:` in 288–352 (target ≈327).

- [ ] **Step 7: Run the scanner**

Expected Act 2: whitespace 0, escape 0, nextText 0 dangling, cross-page 0, quotes exactly 4 (the two intentional split pairs, style-matched).

- [ ] **Step 8: Search the scene file for the five sacred fragments**

Run: `Select-String -Path "Assets\Scenes\ACT 2\2.1_Act2_CutScene\02_Act2_Story.unity" -Pattern "What guides your kindness|My mama|belonged to someone greater|world sees something in you|Opportunities do not find people"`
Expected: all five found.

- [ ] **Step 9: Full play-through of Act 2**

Checklist: every page's typewriter completes fully; Next/Back/fade work; page 14's Next loads `Village Map Scene`.

- [ ] **Step 10: Commit**

```bash
git add "Assets/Scenes/ACT 2/2.1_Act2_CutScene/02_Act2_Story.unity"
git commit -m "Restructure Act 2 cutscene to 14 pages"
```

---

### Task 6: Final acceptance with the user

**Files:** none modified.

**Interfaces:**
- Consumes: Tasks 1–5 outputs and the spec's §9 checklist.

- [ ] **Step 1: Run both scripts on both scenes; record final outputs**

Expected: Act 1 `PAGES: 14`, words 243–297; Act 2 `PAGES: 14`, words 288–352; both scenes at escape 0 / dangling 0 / cross-page 0 / whitespace 0 / quotes 0 (Act 1) and 4 (Act 2 split pairs).

- [ ] **Step 2: Walk spec §9 acceptance criteria with the user, checkbox by checkbox**

Each box: show the evidence (script output, search hit, or play-through result).

- [ ] **Step 3: Report completion; ask the user whether to push `act1-2-cutscene-edit` to GitHub**

Expected: no push without an explicit yes. Sub-projects 2 and 3 get their own brainstorm → spec → plan cycles; do not start them here.
