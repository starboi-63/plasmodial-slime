# Plasmodial Slime Growth Simulation

Praccho Muna-McQuay, Alaina Lin, Tanish Makadia

## How to Run

1. Install [Unity 2022.3.14f1](https://unity.com/releases/editor/whats-new/2022.3.14)
2. Click Play?

## Project Structure

In Assets/Scripts:

### C# Files

- `SlimeSimulation.cs`: main entry point running the simulation
- `SlimeSettings.cs`: global settings for the simulation
- `SlimeAgent.cs`: definition of a single slime agent
- `SlimeFileReader.cs`: logic for parsing custom `.slime` files

### Compute Shaders

- TBD (need to render scene and do per-agent calculations)
