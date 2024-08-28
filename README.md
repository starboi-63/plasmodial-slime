# Plasmodial Slime Growth Simulation

By Praccho Muna-McQuay, Alaina Lin, and Tanish Makadia; submitted for CSCI 1230 (Computer Graphics) final project at Brown University.
>If you're interested in the math used to model the simulation's slime foraging behavior, consider visiting the [project website](https://tanishmakadia.com/projects/slime-simulation).

- Inspired by Sebastian Lague's [Coding Adventure: Ant and Slime Simulations](https://youtu.be/X-iSQQgOd1A).
- Agent behavior implemented based on [Characteristics of pattern formation and evolution in approximations of physarum transport networks](https://uwe-repository.worktribe.com/output/980579) (Jones).
- Food behavior implemented based on [Stepwise slime mould growth as a template for urban design](https://www.ncbi.nlm.nih.gov/pmc/articles/PMC8789834/) (Kay, Hatton).

## Features

1. **Slime agents** modeling behavior of plasmodial slime molds like _physarum polycephalum_.
   - support for up to four **different slime species** at once.
2. **Foraging behavior** based on an attractor-field of **food sources**.
   - optional **depletion** of food sources with slime agent interactions.
3. **Brushes** to paint food/slime and to erase material on the canvas.
4. **Starting seed library** to begin the simulation, including a "circle," "big-bang," and "starburst".
5. **Rapid** simulation supporting over 1-million concurrent slime agents.
   - utilization of **compute shaders** in Unity to accelerate simulations using parallel computations on the GPU.
6. **Customizable** parameters for nearly every aspect of the simulation displayed on an interactive GUI.

## How to Run

1. Install [Unity 2022.3.14f1](https://unity.com/releases/editor/whats-new/2022.3.14).
2. Clone this repository on your machine.
3. Open the cloned root directory in Unity and double click the `GUI` scene in `Assets/Scenes`.
4. Click the 'play' button at the top of the screen. Alternatively, click "Build and Run" to execute outside the editor.

## Project Structure

### C# Files

In `Assets/Scripts`:

- In `/SlimeSettings`: files containing various species settings.
- `SlimeSimulation.cs`: main entry point running the simulation.
- `SlimeSettings.cs`: global settings for the simulation.
- `SlimeAgent.cs`: definition of a single slime agent.
- `ComputeUtilities.cs`: utils for creating and writing to textures.
- `FoodSource.cs`: definition of a single food source.

### Compute Shaders

In `Assets/Shaders`:

- `SlimeSimulation.compute`: multiple entry points for slime agent logic and reading/writing from textures using the GPU.
