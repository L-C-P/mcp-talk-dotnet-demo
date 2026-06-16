# AGENTS.md

Guidance for agents working on the Slidev deck in this directory.

## Scope
- Applies to `slidev/` content.
- Inherits repository defaults from `../AGENTS.md`.
- Always read `../AGENTS.md` at the start of each new session before making changes in `slidev/`.

## Source files
- Deck: `slides.md`
- Styling: `style.css`
- App setup hooks: `setup/main.ts`
- Concept reference: `../mcp-dotnet-talk-concept.md`

## Style system overview (`style.css`)
`style.css` is the central visual layer for the deck. It defines:

- Global theme tokens and typography:
  - `:root` brand color variables (`--brand-primary`, `--slidev-theme-primary`)
  - Base text/headline styling via `.slidev-layout`
  - Global table and marker color behavior
- Slide-specific layout skins via frontmatter classes:
  - `.slidev-layout.title`
  - `.slidev-layout.opening`
  - `.slidev-layout.blm-title`
  - `.slidev-layout.about-me-section`
  - `.slidev-layout.setlist-toc`
- Component-like class blocks used inside slides:
  - `.about-me-content`
  - `.setlist-wrap`, `.setlist-title`, `.setlist-grid`, `.setlist-item`, `.setlist-num`, `.setlist-text`

## Where styles are used in `slides.md`
- `layout: opening` + `class: opening`
  - Used on the opening title slide.
  - `class: opening` picks the background image from `style.css` (`BLMeetingBackground.png`).
- `layout: title`
  - Used on the adesso intro slide.
  - Forces white foreground text on blue background.
- `class: blm-title`
  - Used on the final thank-you slide.
  - Picks background image from `style.css` (`BLMeetingBackground.png`).
- `class: about-me-section`
  - Used on the "About me" slide.
  - Works with inner `.about-me-content` container for centered white text.
- `class: setlist-toc`
  - Used on the "Today's Setlist" slide.
  - Uses `.setlist-*` classes to build the two-column numbered setlist grid.

## Editing guidance for style changes
- Prefer changing `style.css` over adding inline style attributes in `slides.md`.
- Use slide frontmatter `class:` for slide-level visuals; keep class names consistent with existing selectors.
- If you rename or remove a CSS class in `style.css`, update all matching usages in `slides.md` in the same change.
- For quotes in `slides.md`, use Markdown quote syntax (`> ...`). Spacing is controlled centrally in `style.css` via the global `.slidev-layout blockquote` rule.
- Keep Mermaid labels compatible: use `<br/>`, never literal `\n`.

## Presenter countdown behavior (`setup/main.ts`)
- The deck contains a router hook that auto-starts the presenter countdown when the active presenter slide changes.
- Implementation details:
  - Parses presenter route slide ids from paths like `/presenter/<id>`.
  - Runs in `router.afterEach`.
  - Only triggers when both old and new presenter slide ids exist and differ.
  - Tries to click the presenter timer play toggle via DOM query:
    - `.slidev-presenter .grid-section.bottom .i-carbon\:play`
  - Uses a double `requestAnimationFrame` retry to handle delayed DOM rendering.
- If presenter UI structure or icon classes change in Slidev updates, this selector logic may need to be adjusted.

## Quick checks before handoff
- Slides still render with expected backgrounds and text contrast.
- "About me" and "Setlist" layout blocks keep their intended alignment.
- Opening slide still uses `layout: opening` + `class: opening`.
- Adesso intro slide still uses `layout: title`.
- Closing slide still uses `blm-title`.
