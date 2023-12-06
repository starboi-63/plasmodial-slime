# Plasmodial Slime Growth Simulation

Praccho Muna-McQuay, Alaina Lin, Tanish Makadia

## How to Run

1. Install [Unity 2022.3.14f1](https://unity.com/releases/editor/whats-new/2022.3.14).
2. Open the root directory in Unity within `Assets/Scenes`.
3. Click the 'play' button at the top of the screen.

## Project Structure (modify as needed)

### C# Files

In `Assets/Scripts`:

- `SlimeSimulation.cs`: main entry point running the simulation
- `SlimeSettings.cs`: global settings for the simulation
- `SlimeAgent.cs`: definition of a single slime agent
- `ComputeUtilities.cs`: utils for creating and writing to textures

### Compute Shaders

In `Assets/Shaders`:

- `SlimeSimulation.compute`: multiple entry points for slime agent logic and reading/writing from textures
