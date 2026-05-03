# Sprint 3 Report (4/3/2026 – 5/2/26)
## What's New (User Facing)
- Feature: Improved General UI
- Feature: Display Creature Averages 
- Feature: Settings Menu
- Feature: Machine Learning for Creatures
- Feature: Simulation Graphic Filters
- Feature: Web Application – Itch.io

## Work Summary (Developer Facing)
-	We implemented many new functionalities and updates during this sprint. The first is improved UI. This feature is split across many smaller changes: a simulation timer, a settings menu with sound and graphics sliders, a box to display a specific creature’s stats and the average for the simulation, and more. The next feature that was added was machine learning, which uses PPO to allow our agents to become better at pathing to food and water for survival. We also improved graphics by altering the Universal Render Pipeline (URP) and main camera globals. The URP changes will not be visible on the web version of the game. There is now a web application version of the simulation on itch.io.

## Unfinished Work
- Fix lag spikes in saving and loading neural networks
- Fix rewards reaching infinity for some creatures
- Music
- Allow Creatures to Attack
- Modify stats based on the elite creature’s stats

## Completed Issues/User Stories
Here are links to the issues that were completed in this sprint:
* https://github.com/orgs/WSU-CPTS322-SP26/projects/39

## Incomplete Issues/User Stories
Here are links to issues we worked on but did not complete in this sprint:
- Saving/Loading Lag: https://github.com/WSU-CPTS322-SP26/MachineLearningEcosystem/issues/48
- Allow Agents to Attack: https://github.com/WSU-CPTS322-SP26/MachineLearningEcosystem/issues/58
- Background Music: https://github.com/WSU-CPTS322-SP26/MachineLearningEcosystem/issues/57

## Code Files for Review
Please review the following code files, which were actively developed during this sprint, for quality:
* Item 1: Machine Learning: https://github.com/WSU-CPTS322-SP26/MachineLearningEcosystem/pull/43/changes
* Item 2: Sound Effects Manager: https://github.com/WSU-CPTS322-SP26/MachineLearningEcosystem/pull/46/changes
* Item 3: Menu UI: https://github.com/WSU-CPTS322-SP26/MachineLearningEcosystem/pull/49/changes
* Item 4: Settings UI: https://github.com/WSU-CPTS322-SP26/MachineLearningEcosystem/pull/59/changes

## Retrospective Summary
Here's what went well:
* Item 1: Improved and added UI is very helpful
* Item 2: The graphical changes improve user experience
* Item 3: The web application is playable

Here's what we'd like to improve:
* Item 1: Fix lagging and various bugs with Machine Learning
* Item 2: Add speed slider to the UI

Here are changes we plan to implement in the next sprint:
* Item 1: Music
* Item 2: Allow creatures to attack
* Item 3: Modify stats each generation based on the elite creature’s stats 


