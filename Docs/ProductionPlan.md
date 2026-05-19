# Pixel Dasher Production Plan

Final deadline: August 19, 2026

## Game Concept

Pixel Dasher is a 2D sci-fi / space samurai action platformer inspired by:

- Elden Ring combat feel
- Mario-style level progression
- Fast katana combat
- Boss fights

Core gameplay loop:

- Player traverses levels with enemies and obstacles.
- Small enemy encounters appear throughout each level.
- A final boss fight closes the level.

Player abilities:

- `Q` - Sword slash attack
- `E` - Dash slash through enemies
- `R` - Ultimate ability, to be decided

## Team Ownership

| Team Member | Main Focus | Responsibilities |
| --- | --- | --- |
| Jimmy | Core systems, combat systems, UI systems, integration/debugging | GameManager, combat integration, health/score systems, menus/UI, scene management, ability integration, final integration/testing, documentation support |
| Iskander | Player combat and movement | Player controller, jump/movement system, dash ability, sword attack, combat feel/responsiveness, animation implementation, hit detection/collisions |
| Carlos | Art, UI, and audio support | Space samurai art direction, character/environment sprites, UI visuals, VFX support, audio implementation, environmental assets, combat effect visuals |
| Stephen | Enemy systems, level systems, boss systems | Enemy AI/behavior, obstacle systems, boss fight implementation, difficulty progression, level building, enemy balancing |
| Anthony | QA, testing, and documentation | Playtesting, bug tracking, balance feedback, documentation, build verification, combat testing, boss testing |

## Calendar

This schedule starts from Tuesday, May 19, 2026 and preserves the original phase lengths while adding a buffer/lock period before final delivery.

| Phase | Dates | Goal |
| --- | --- | --- |
| Phase 1 - Core Prototype | May 19-May 25 | Create a playable combat prototype. |
| Phase 2 - Core Gameplay Systems | May 26-June 8 | Complete all major gameplay systems. The game must be fully playable by the end of this phase. |
| Phase 3 - Content Production | June 9-June 22 | Create real game content and polish gameplay. |
| Phase 4 - Polish and Optimization | June 23-July 6 | Turn the game into a stable near-final product. |
| Buffer / Feature Lock | July 7-August 11 | Fix bugs, rebalance, stabilize builds, and avoid new scope unless approved by the team. |
| Phase 5 - Finalization | August 12-August 19 | Prepare final delivery. |

## Milestone Definitions

### Phase 1 - Core Prototype

Goal: Create a playable combat prototype.

End-of-phase success criteria:

- Player can move, jump, attack, and dash slash.
- Enemy can be damaged and killed.
- One test level is functional.
- Basic health UI exists.
- Bugs are being tracked.

### Phase 2 - Core Gameplay Systems

Goal: Complete all major gameplay systems.

End-of-phase success criteria:

- Full playable level exists from start to finish.
- Combat systems are functional.
- Multiple enemies are present.
- Basic boss prototype exists.
- Pause, game over, and health UI are functional.

Important: By the end of this phase, the game must already be fully playable.

### Phase 3 - Content Production

Goal: Create actual game content and polish gameplay.

End-of-phase success criteria:

- Multiple playable levels exist.
- Boss fight is functional.
- Combat gameplay is stable.
- Visual identity is established.
- Audio and VFX are present in core combat moments.

### Phase 4 - Polish and Optimization

Goal: Turn the game into a polished final product.

End-of-phase success criteria:

- Stable near-final build.
- Polished combat experience.
- Major bugs fixed.
- Boss, enemy, and level balance pass completed.

### Phase 5 - Finalization

Goal: Prepare final delivery.

Final success criteria:

- Final build exported.
- Final QA pass completed.
- Documentation completed.
- Submission package prepared before August 19, 2026.

## GitHub Milestones

Create these milestones in GitHub:

| Milestone | Due Date | Description |
| --- | --- | --- |
| Phase 1 - Core Prototype | May 25, 2026 | Playable combat prototype with movement, attacks, enemy damage, and one test level. |
| Phase 2 - Core Gameplay Systems | June 8, 2026 | Fully playable level with core UI, ability cooldowns, multiple enemies, obstacles, and boss prototype. |
| Phase 3 - Content Production | June 22, 2026 | Multiple levels, final combat mechanics, functional boss, visual identity, audio, and VFX. |
| Phase 4 - Polish and Optimization | July 6, 2026 | Near-final stable build with major bugs fixed and balance pass completed. |
| Buffer / Feature Lock | August 11, 2026 | Stabilization, bug fixing, build verification, and documentation progress. |
| Phase 5 - Finalization | August 19, 2026 | Final export, QA verification, submission prep, and final documentation. |

## Recommended GitHub Labels

- `owner:jimmy`
- `owner:iskander`
- `owner:carlos`
- `owner:stephen`
- `owner:anthony`
- `type:system`
- `type:combat`
- `type:movement`
- `type:ui`
- `type:art`
- `type:audio`
- `type:vfx`
- `type:enemy`
- `type:boss`
- `type:level`
- `type:qa`
- `type:docs`
- `priority:high`
- `priority:medium`
- `priority:low`
- `status:blocked`
- `status:needs-review`

## Phase 1 GitHub Issues

These are the immediate issues to create first.

### Jimmy

#### Setup Unity project structure

Labels: `owner:jimmy`, `type:system`, `priority:high`

Milestone: Phase 1 - Core Prototype

Acceptance criteria:

- Script folders are organized by responsibility.
- Scene, prefab, art, audio, and UI folders exist.
- Team can find where to place new work.

#### Implement GameManager prototype

Labels: `owner:jimmy`, `type:system`, `priority:high`

Milestone: Phase 1 - Core Prototype

Acceptance criteria:

- GameManager tracks basic game state.
- GameManager can support playing, paused, game over, and level complete states.
- Other systems can safely reference the GameManager.

#### Implement prototype health system

Labels: `owner:jimmy`, `type:combat`, `type:system`, `priority:high`

Milestone: Phase 1 - Core Prototype

Acceptance criteria:

- Player and enemies can take damage.
- Health reaches zero consistently.
- Death event or callback is available for enemies and player.

#### Implement prototype UI

Labels: `owner:jimmy`, `type:ui`, `priority:medium`

Milestone: Phase 1 - Core Prototype

Acceptance criteria:

- Player health is visible.
- Prototype game over message exists.
- UI works in the prototype scene.

#### Setup GitHub project structure

Labels: `owner:jimmy`, `type:docs`, `priority:medium`

Milestone: Phase 1 - Core Prototype

Acceptance criteria:

- Milestones are created.
- Labels are created.
- Phase 1 issues are assigned.
- README links to the production plan.

### Iskander

#### Prototype player movement

Labels: `owner:iskander`, `type:movement`, `priority:high`

Milestone: Phase 1 - Core Prototype

Acceptance criteria:

- Player can move left and right.
- Player can jump.
- Movement feels responsive enough for combat testing.
- Player collision works on a simple platform level.

#### Prototype sword attack on Q

Labels: `owner:iskander`, `type:combat`, `priority:high`

Milestone: Phase 1 - Core Prototype

Acceptance criteria:

- Pressing `Q` triggers a sword slash.
- Slash has a clear hitbox window.
- Slash can damage a test enemy.
- Slash has a visible placeholder effect or animation.

#### Prototype dash slash on E

Labels: `owner:iskander`, `type:combat`, `type:movement`, `priority:high`

Milestone: Phase 1 - Core Prototype

Acceptance criteria:

- Pressing `E` triggers a forward dash slash.
- Dash moves through or into enemies as designed for the prototype.
- Dash can damage a test enemy.
- Dash has a cooldown or temporary lockout to prevent spam.

#### Setup basic combat collisions

Labels: `owner:iskander`, `type:combat`, `priority:high`

Milestone: Phase 1 - Core Prototype

Acceptance criteria:

- Player attacks detect enemies reliably.
- Enemy hurtboxes are easy to tune.
- Collisions do not repeatedly damage enemies unless intended.

### Carlos

#### Create temporary player and enemy sprites

Labels: `owner:carlos`, `type:art`, `priority:high`

Milestone: Phase 1 - Core Prototype

Acceptance criteria:

- Temporary player sprite exists.
- Temporary enemy sprite exists.
- Sprites are readable in the prototype scene.
- Assets follow the project folder structure.

#### Create placeholder environment assets

Labels: `owner:carlos`, `type:art`, `type:level`, `priority:medium`

Milestone: Phase 1 - Core Prototype

Acceptance criteria:

- Placeholder platform tiles or shapes exist.
- Prototype level can be visually blocked out.
- Assets support a sci-fi / space samurai direction.

#### Research visual direction

Labels: `owner:carlos`, `type:art`, `priority:medium`

Milestone: Phase 1 - Core Prototype

Acceptance criteria:

- Reference board or notes are created.
- Core color, silhouette, and environment ideas are documented.
- Team can agree on the visual direction before final art production.

#### Collect VFX and audio references

Labels: `owner:carlos`, `type:vfx`, `type:audio`, `priority:low`

Milestone: Phase 1 - Core Prototype

Acceptance criteria:

- Reference examples exist for sword slash, dash slash, enemy hit, and boss impact.
- Audio references exist for attack, dash, hit, death, and menu sounds.

### Stephen

#### Prototype simple enemy behavior

Labels: `owner:stephen`, `type:enemy`, `priority:high`

Milestone: Phase 1 - Core Prototype

Acceptance criteria:

- Enemy can idle or patrol.
- Enemy can detect or react to the player.
- Enemy can take damage and die.

#### Create prototype enemy prefab

Labels: `owner:stephen`, `type:enemy`, `priority:high`

Milestone: Phase 1 - Core Prototype

Acceptance criteria:

- Enemy prefab can be placed in a level.
- Enemy has health, hurtbox, and simple behavior.
- Enemy works with player attack and dash slash prototypes.

#### Create prototype test level

Labels: `owner:stephen`, `type:level`, `priority:high`

Milestone: Phase 1 - Core Prototype

Acceptance criteria:

- Level has a start area, combat area, and end area.
- Player can traverse the level.
- Enemy encounter supports combat testing.

#### Test early combat encounters

Labels: `owner:stephen`, `type:enemy`, `type:level`, `priority:medium`

Milestone: Phase 1 - Core Prototype

Acceptance criteria:

- Enemy placement supports basic sword and dash testing.
- Encounter notes are shared with Jimmy and Iskander.
- Major combat blockers are reported as issues.

### Anthony

#### Setup bug tracking process

Labels: `owner:anthony`, `type:qa`, `priority:high`

Milestone: Phase 1 - Core Prototype

Acceptance criteria:

- Bug report format is documented.
- Bugs include steps to reproduce, expected result, actual result, and severity.
- Team knows where to log bugs.

#### Test combat feel

Labels: `owner:anthony`, `type:qa`, `type:combat`, `priority:high`

Milestone: Phase 1 - Core Prototype

Acceptance criteria:

- Sword attack responsiveness is tested.
- Dash slash responsiveness is tested.
- Movement plus combat flow is tested.
- Feedback is written as actionable issues.

#### Record prototype gameplay issues

Labels: `owner:anthony`, `type:qa`, `priority:medium`

Milestone: Phase 1 - Core Prototype

Acceptance criteria:

- Issues are logged with reproduction steps.
- Combat, movement, enemy, UI, and level bugs are separated clearly.
- Blockers are marked with high priority.

#### Create documentation structure

Labels: `owner:anthony`, `type:docs`, `priority:medium`

Milestone: Phase 1 - Core Prototype

Acceptance criteria:

- Documentation folders or files exist for controls, systems, known bugs, and build notes.
- Phase goals are included.
- Documentation can be updated throughout the project.

## Later Phase Backlog

Use these as starting issues for future milestones.

### Phase 2 - Core Gameplay Systems

- Jimmy: Implement UI menus.
- Jimmy: Implement health UI.
- Jimmy: Implement pause and game over systems.
- Jimmy: Implement ability cooldown systems.
- Jimmy: Integrate and debug core gameplay systems.
- Iskander: Polish combat responsiveness.
- Iskander: Add movement polish.
- Iskander: Implement combo feel.
- Iskander: Improve dash mechanics.
- Iskander: Begin ultimate ability on `R`.
- Carlos: Create environment assets.
- Carlos: Create enemy visuals.
- Carlos: Polish UI visuals.
- Carlos: Add sound effects and music.
- Carlos: Add combat VFX.
- Stephen: Add enemy variations.
- Stephen: Create obstacle systems.
- Stephen: Begin boss fight prototype.
- Stephen: Implement difficulty progression.
- Stephen: Implement checkpoint systems.
- Anthony: Continue testing.
- Anthony: Provide combat balancing feedback.
- Anthony: Report bugs.
- Anthony: Update documentation.

### Phase 3 - Content Production

- Jimmy: Complete final gameplay integration.
- Jimmy: Polish UI.
- Jimmy: Implement save/progression systems.
- Jimmy: Support optimization.
- Iskander: Finalize combat mechanics.
- Iskander: Finalize ultimate ability.
- Iskander: Balance combat.
- Iskander: Polish animations.
- Carlos: Complete final art pass.
- Carlos: Polish combat effects.
- Carlos: Polish audio.
- Carlos: Polish environments.
- Carlos: Finalize UI visuals.
- Stephen: Build additional levels.
- Stephen: Finalize boss fight.
- Stephen: Balance enemies.
- Stephen: Balance difficulty.
- Stephen: Design encounters.
- Anthony: Complete full playtesting.
- Anthony: Create QA reports.
- Anthony: Continue documentation.
- Anthony: Verify bug fixes.

### Phase 4 - Polish and Optimization

- Jimmy: Complete final integration.
- Jimmy: Complete final debugging.
- Jimmy: Stabilize build.
- Iskander: Complete final combat polish.
- Iskander: Complete animation polish.
- Iskander: Complete gameplay balancing.
- Carlos: Complete final VFX polish.
- Carlos: Complete final audio polish.
- Carlos: Complete UI polish.
- Carlos: Complete environment polish.
- Stephen: Balance boss.
- Stephen: Balance enemies.
- Stephen: Balance levels.
- Anthony: Complete intensive testing.
- Anthony: Complete final QA verification.
- Anthony: Complete final documentation.

### Phase 5 - Finalization

- Jimmy: Export final build.
- Jimmy: Prepare submission.
- Jimmy: Complete final integration checks.
- Iskander: Complete final gameplay fixes.
- Carlos: Verify final visuals and audio.
- Stephen: Verify final boss and levels.
- Anthony: Complete final documentation.
- Anthony: Complete final QA testing.

## Bug Report Template

Use this format for QA issues:

```md
## Summary

Short description of the bug.

## Steps to Reproduce

1. Start from...
2. Press...
3. Observe...

## Expected Result

What should happen.

## Actual Result

What happened instead.

## Severity

Blocker / High / Medium / Low

## Build or Scene

Unity scene name, build version, or branch.

## Notes

Screenshots, clips, or extra context.
```

## Definition of Done

An issue is done when:

- The feature works in the Unity editor.
- The feature works in the current playable scene.
- The owner has tested the main success path.
- Any known bugs are logged.
- Related documentation or notes are updated when needed.

For combat, enemy, boss, and level issues, Anthony should verify the change before it is treated as final.
