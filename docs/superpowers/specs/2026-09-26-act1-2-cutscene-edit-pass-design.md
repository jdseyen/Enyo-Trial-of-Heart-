# Act 1–2 Cutscene Edit Pass — Design Doc

**Date:** 2026-09-26
**Status:** Approved design, pending spec review
**Sub-project:** 1 of 3 (see "Decomposition" below)
**Repo:** Enyo: Trial of Heart · branch `main`

---

## 1. Context & Goal

The confidence/affirmation system (sub-project 2) and the branching failure-based
ending with the letter twist (sub-project 3) need their narrative setup planted
early: Enyo's wound (school failure, worth = serving others), the mother's
illness/memory decline as one merged thread, and the thesis lines the ending will
pay off. Act 1 and Act 2 currently carry that setup, but they are ~30% too wordy,
zigzag through time, and contain text bugs.

This pass edits **only** the Act 1 and Act 2 cutscene scenes: restructure,
linearize, trim, fix text bugs, and plant/protect story threads. No code changes,
no new systems, no other scenes.

**Success criteria:** Act 1 = 14 pages (~270 words), Act 2 = 14 pages (~320
words), zero text bugs listed below, story threads aligned, both scenes play
through cleanly with Next/Back/typewriter intact.

---

## 2. Scope

**In scope**
- `Assets/Scenes/ACT 1/1.1_ACT1_CutScene/01_Act1_Story.unity` — page structure, text content, page order
- `Assets/Scenes/ACT 2/2.1_Act2_CutScene/02_Act2_Story.unity` — same
- Scene-level wiring that must change as a result (pager `pages[]` array, `nextText` chains)

**Out of scope (explicitly)**
- Any C# script changes (the pagers already handle any `pages.Length`)
- Act 3/Act 4 cutscenes, ending rewrite (sub-project 3)
- Confidence/affirmation system, journal, Act 3 anxiety timer (sub-project 2; timer fail-state question Q8 still open)
- Main menu, pause, save systems

---

## 3. Verified Current State (audit facts)

Extracted programmatically from the scene files:

| | Act 1 | Act 2 |
|---|---|---|
| Pages | 23 | 22 |
| Total words | 384 (approx) | 435 (approx) |
| Video pages | 2 (Page_01_Video, Page_03_Video) | 0 |
| Target | **14 pages / ~270 w** | **14 pages / ~320 w** |

**Verified bug inventory** (earlier chat claims corrected by re-scan):

| Check | Act 1 | Act 2 |
|---|---|---|
| Literal `\uXXXX`/`\n` escapes rendering as text | 0 | 0 (all in double-quoted YAML → renders correctly) |
| Dangling `nextText` references | 0 of 37 | 0 of 32 |
| Cross-page `nextText` links | 0 | 0 |
| Visible unbalanced quotes | 0 | 6 (see §6) |
| Leading/trailing whitespace in text | 4 | 9 |

**Correction to earlier audit:** the previously reported "3 stale nextText
cross-page links" does not reproduce — both scenes are clean today. The invariant
to preserve: *every `nextText` chain stays within its page and points at a live
object.* Cutting a text object must rewire its predecessor's `nextText` to the
next surviving object (§8).

---

## 4. Act 1 Restructure (23 → 14 pages)

### 4.1 Problem: the timeline zigzags

Current order: present shop (P4–7) → childhood memories (P8–13) → illness fear
(P14–17) → **back to childhood** (P18–19) → present (P20–23). Pages 18–19 loop
back in time after the illness arc has already started.

### 4.2 Linearized target order

Present → childhood → wound → adolescence → illness → stakes. Video pages stay
first and untouched.

| # | Source pages (words) | Directive | Budget |
|---|---|---|---|
| 1 | P1 video (0) | Keep untouched | 0 |
| 2 | P2 "Hey you!" (2) | Keep | 2 |
| 3 | P3 video "Yes you! / Come try! / ok..." (5) | Keep untouched | 5 |
| 4 | P4 + P5 (21+1) | Merge: vibrant-voice shop intro. Cut the bare "wow!!" / "Haha.." exclamations if they are duplicate audio captions; keep if they are on-screen captions for audio (decide at implementation by playing the scene) | ~16 |
| 5 | P6 + P7 (7+25) | Merge: shop warmth (lunchtime, puzzles, laughter). Trim "I won't tell you the answer. Try again!" only if the same line appears as in-scene gameplay text elsewhere | ~24 |
| 6 | P8 + P9 (17+19) | Merge: mother's care + role model. Cut redundancy between "made sure Enyo is well fed" and "always filled with laughter and love" — keep one caring detail | ~24 |
| 7 | P10 + P11 + P12 (14+14+20) | Merge three hardship memories into ONE page. Keep the two strongest details (suggested: "didn't have enough food" + rainstorm/worn raincoat), fold or cut the third | ~25 |
| 8 | P13 (23) | Keep as own page — **this is the wound**: teacher called her a failure. Tighten grammar only (§6) | ~20 |
| 9 | **P18 + P19** (23+30) | **Move here** (from after P17): growing-up-different, stays at the shop. Trim the fragment list ("her peers... her future...") to two beats | ~30 |
| 10 | P14 + P15 (10+17) | Merge: illness/memory decline begins. Apply thread merge (§7.1) | ~22 |
| 11 | P16 + P17 (17+22) | Merge: reassurance + fear of the forgetting day. Cut one of the two repeated "won't forget her / would forget her" phrasings — they currently say the same thing twice | ~28 |
| 12 | P20 (30) | Keep alone (present-day shop scene). Trim narration slightly | ~24 |
| 13 | P21 + P22 (21+27) | Merge: mother's quiet sadness + belief in Enyo + her wish. **Protect** the belief line (§7.3) | ~30 |
| 14 | P23 (19) | Keep — the hook: "time is running out" + cure-sickness wish. Fix `its`→`it's` (§6) | 19 |

**Word math:** 0+2+5+16+24+24+25+20+30+22+28+24+30+19 ≈ **269** ✓

---

## 5. Act 2 Restructure (22 → 14 pages)

No reordering needed — Act 2 is chronological (street → creature → transaction →
question → note → mother → doubt → resolve). Pure merge/trim.

| # | Source pages (words) | Directive | Budget |
|---|---|---|---|
| 1 | P1 (0) | Keep (structural blank/intro) | 0 |
| 2 | P2 + P3 (13+20) | Merge: street reaction + creature appearance | ~24 |
| 3 | P4 + P5 (21+16) | Merge: Enyo's curiosity + creature studying puzzles | ~28 |
| 4 | P6 + P7 (14+14) | Merge: "understood what it felt like to be different" + creature speaks | ~22 |
| 5 | P8 (12) | Keep — "buy them all" beat lands alone | 12 |
| 6 | P9 + P10 (17+23) | Merge: good news to mother + "What guides your kindness?" — note current P9→P10 jumps locations; at merge, keep the packing scene first, then the question (as today) | ~30 |
| 7 | P11 + P12 (9+13) | Merge: "My mama" + "what do you wish for your future?" | ~18 |
| 8 | P13 + P14 (24+28) | Merge: blank future + "not all who wander are lost" + pen request. Trim | ~34 |
| 9 | P15 (35) | Keep alone — **the note** (the creature's written gift). Remove trailing `\n` (§6) | ~30 |
| 10 | P16 + P17 (27+26) | Merge: creature vanishes + spark returns + mother's revelation. Fix unclosed quote (§6.1) | ~34 |
| 11 | P18 + P19 (25+23) | Merge: gem rules + Enyo's sinking heart. **Protect** P19 line (§7.3) | ~32 |
| 12 | P20 (27) | Keep — mother's hug speech. Fix split-quote pair (§6.1) | ~24 |
| 13 | P21 (25) | Keep — "What if I can't do it?" + the opportunities line. **Protect** (§7.3) | ~22 |
| 14 | P22 (17) | Keep — decision to go, "for her mama" | 17 |

**Word math:** 0+24+28+22+12+30+18+34+30+34+32+24+22+17 = **327** ✓ (within §9's ±10% of the ~320 target)

---

## 6. Text Bug Fixes (both scenes)

### 6.1 Quote fixes (Act 2 only — the 6 flagged instances)

1. **P17, "Oh, sweetie…" speech** — opens `“` and is never closed. Add closing
   `”` at "…only to special ones like you."
2. **P20 split pair** — "Remember this, Enyo…" (opens `"`) and "…something in
   you." (closes `"`) live in two text objects that display as one speech. They
   balance across the pair — **do not "fix" one side only.** Normalize both to
   the same style (curly `“ ”` recommended, matching the rest of Act 2).
3. **P15 note split pair** — "You are worthy…" (opens `“`) / "…led you astray."`
   (closes `"`) — same rule: balanced across two objects; normalize style only.
4. **P17 note-style mismatch** — `“But you may wish…only once."` opens curly,
   closes straight. Make both curly.

Rule of thumb for implementation: **a quote mark opening on one text object and
closing on another is intentional** (it's how split-line dialogue was built).
Never balance them within a single object unless they were opened there.

### 6.2 Mechanical fixes (scene-by-scene)

- **13 texts with leading/trailing whitespace** (4 Act 1, 9 Act 2): trim.
  These show as stray spaces in the typewriter output.
- **Act 2 P15**: remove the literal trailing `\n` on both note lines (renders as
  a blank line after the note).

### 6.3 Spelling / grammar fixes

**Act 1:**
| Page | Current | Fix |
|---|---|---|
| P11 | "rice and omelette was a meal they share..." | "were meals they shared..." |
| P12 | "a worned out raincoat... what they share..." | "a worn-out raincoat... what they shared..." |
| P13 | "called Enyo a failure and must repeat another year of school" | "called Enyo a failure and she had to repeat a year of school" |
| P16 | "reasuring" | "reassuring" |
| P20 | "As Enyo's hands dusted with sawdust as she carves a new toy." | "Enyo's hands dusted with sawdust as she carved a new toy." |
| P21 | "believed Enyo is more than capable beyond their tiny old shop" | "believed Enyo was capable of far more than their tiny old shop" |
| P23 | "time feels like its running out" | "time feels like it's running out" |

**Act 2:**
| Page | Current | Fix |
|---|---|---|
| P2 | "to see whats going on" | "what's going on" |
| P3 | "Its presense" | "Its presence" |
| P4 | "Its looks harmless" | "It looks harmless" |
| P13 | "Im not sure...." | "I'm not sure…" |
| P14 | "Thats alright young one...not all who wander are lost" | "That's alright, young one… not all who wander are lost" |
| P15 | "wish…Let your kindness" | "wish… Let your kindness" |
| P17 vs P15 | "cintamani gem" | "Cintamani Gem" (consistent capitalization everywhere) |

- **Tense consistency pass** (both scenes): narration is mixed past/present
  ("lived a young elf" vs "Enyo and her mom owned... while Enyo works hard").
  Target: past tense for narration, present only for spoken dialogue. Apply per
  surviving line after merges.

---

## 7. Story-Thread Directives (the reason this pass exists)

### 7.1 Mother-thread merge: illness → memory decline (one cause)
Act 1 currently states both "her mother's memory declined unexpectedly" (P14)
and "her sick old mother / cure mama's sickness" (P22–23) — but never connects
them. **Edit P14's line so the illness causes the memory decline** (e.g.,
"little by little, the sickness took her memory" — exact wording decided at
implementation). This makes sub-project 3's letter payoff ("the illness and the
memory were the same enemy") retroactively coherent.

### 7.2 Keep the irony intact
Enyo's kindness-first life sets up the "kindness always comes back" theme that
the rigged trials will subvert. Her helping behavior in Act 1 (shop care,
puzzles, staying for mother) must stay visible after trimming — trim *wordiness*,
never *her acts of service*.

### 7.3 Sacred lines (NEVER cut, only typo-fix)
These pay off in later acts and the ending; mark them during trimming:

**Act 1:**
- P13: "when a teacher called Enyo a failure..." (the wound — sub-project 2's confidence root)
- P23: "But to Enyo, time feels like it's running out..." + "If only there was a way to cure mama's sickness..." (letter setup)
- P21: "She always believed Enyo was capable of far more than their tiny old shop" (mother's belief — letter payoff)

**Act 2:**
- P10: "What guides your kindness, young one?" / P11: "My mama" (creature's question — the game's thematic spine)
- P19: "Surely a destiny like this belonged to someone greater than her." (unworthiness belief — the confidence branch target)
- P20: "the world sees something in you" + P21: "Opportunities do not find people who are perfect… they find people…" (the moral — high-confidence ending echo)

---

## 8. Editing Procedure (how to apply this in Unity)

Pages are GameObjects; `Act1StoryPager`/`Act2StoryPager` hold an ordered
`pages[]` array that drives Next/Back. Procedure per merge:

1. **Move, don't recreate.** Drag the text GameObjects from source pages into
   the surviving target page (keeps all TMP settings, TypewriterText refs,
   fonts, and `nextText` components intact).
2. **Reorder text objects** inside the merged page to match the reading order
   given in the tables (sibling order = typewriter order if chained).
3. **Fix `nextText` chains:** after every cut, select each surviving
   TypewriterText and confirm its `nextText` points to the next surviving text
   *on the same page* (or is 0 at page end). A deleted object silently breaks
   the chain — text after the gap will not auto-play.
4. **Delete emptied page GameObjects**, then remove their entries from the
   pager's `pages[]` array in the Inspector (array must end up exactly 14).
5. **Do not touch** video pages (Act 1 pages 1 & 3), the fade overlay, or
   button wiring.
6. **Play the scene end-to-end** after each act: every page's typewriter
   completes, Next advances, Back returns, last page loads the next scene
   (`02_Act2_Story` / `Village Map Scene`).

**Optional tidy:** page names carry stray spaces ("Page_01_Video ", "Page_07 ")
— harmless; fix only if convenient.

---

## 9. Acceptance Criteria

- [ ] Act 1 scene: exactly 14 entries in `pages[]`, plays start→finish, Back works
- [ ] Act 2 scene: exactly 14 entries in `pages[]`, plays start→finish, Back works
- [ ] Act 1 ≈ 270 words, Act 2 ≈ 320 words (±10% acceptable — feel over arithmetic)
- [ ] Act 1 timeline linear (no return to childhood after illness begins)
- [ ] Every bug in §6 fixed; zero new whitespace/quote issues introduced
- [ ] §7.1 illness→memory link present; all §7.3 sacred lines present verbatim (typo-fixes allowed)
- [ ] All `nextText` chains resolve within their page (no dangling, no cross-page)
- [ ] Video pages and fade/button wiring untouched
- [ ] Git: one commit per act (text + structure), pushed only when user approves

---

## 10. Decomposition (why this is sub-project 1 of 3)

| # | Sub-project | Depends on |
|---|---|---|
| **1** | **This doc** — Act 1–2 cutscene edit pass (story setup, no code) | — |
| 2 | Confidence & affirmation system (one persistent value, journal, Act 3 anxiety timer incl. open Q8 fail-state question) | story text from #1 (affirmation word sources) |
| 3 | Branching endings (Act 4 rewrite, active-recall finale, letter scene, open-door coda, opening recontextualization) | #1 setup + #2 word data |

Each sub-project gets its own design doc → implementation plan → build, with
user approval at every gate. Nothing is built, edited, or committed without
explicit user permission.
