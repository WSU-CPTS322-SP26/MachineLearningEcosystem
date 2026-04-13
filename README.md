
# Machine-Learning Ecosystem
## Project summary
### One-sentence description of the project
The project simulates an ecosystem with machine-learning-based herbivores, carnivores, and producers in a procedurally generated environment.
### Additional information about the project
Creatures will be able to eat, drink, learn to surprise their prey, or stealthily sneak away from predators if it so benefits them. Each ecosystem simulation will run starting with basic creature behavior on a procedurally-generated world while the user is able to watch creatures learn to improve their survival rates. Each new generation of creatures is based on the best performing agent from the previous generation. Each creature in the new generation will get random stat changes (both positive and negative) as well as random mutations to their behavior patterns to simulate evolution and assure the creatures do not often stagnate for many generations.
## Installation
### Prerequisites
Must have Unity version 6000.3.6df1 installed to open the project folder.
serves in your app.
### Installation Steps
* run command: git clone https://github.com/WSU-CPTS322-SP26/MachineLearningEcosystem.git
* Install or open the Unity Hub app and select Add->Add project from disk
* Browse your file explorer to where you cloned the project to and select the subfolder called "MachineLearningEcosystem"
* Open the project and then open the "Main Menu" scene
* Run the project with the play button at the top of the unity editor
## Functionality
Allows the user to enter the procedural generation phase of the simulation, where you can input the size of the map's x and y dimensions (clamped between 40 and 200 units), and generate a map based on the Wave Function Collapse implementation the program uses. You can adjust the animation speed, make the map display everything at once (non-animated), and regenerate the map at the same or a new size. When you have a map you like, select begin to start the program with that map. After selecting begin, creatures and plants will randomly spawn on viable spawning tiles, currently creatures have only random movement and will eat or drink only if they randomly move to water or food. You can pause at any time with the escape key, and you can move the camera with W,A,S, and D to control panning and the mouse scroll wheel for zooming in and out.
## Known Problems
- Performance issues can occur when running terrain generation at max speed or with maximum size (200 x 200)
## Additional Documentation
Sprint reports:
* "Sprint1.md", [Sprint 1 Report Video](https://www.youtube.com/watch?v=MkfPcHMPpAM)
* "Sprint2.md", [Sprint 2 Report Video](https://www.youtube.com/watch?v=GG4l9nynSWY)

User links:
* [Used as a basis for WFC](https://www.uproomgames.com/dev-log/wave-function-collapse)
## License
MIT License
