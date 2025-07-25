# DoD - Asteroids
This project is a modern take on the classic Asteroids, developed as part of a technical challenge for a hiring process. The main focus was to apply Data-Oriented Design (DOD) principles to create a lean, high-performance, and maintainable system.

## 🎮 About the Project

Inspired by the classic Asteroids, this version allows you to control a spaceship, destroy asteroids, and avoid collisions. The main focus, however, is on the architecture behind the scenes.

You can expand game mechanics by implementing new simulation or rendering agents and registering them into their respective pipelines.

## Controls

- W - Move Up
- S - Move Down
- A - Move Left
- D - Move Right

## 🛠️ Tech Stack
- Unity 2022.3.42f1
- C#
- Data-Oriented Design (not using Unity ECS)


## 📦 Packages used

- [DOTween](https://dotween.demigiant.com/documentation.php) for tween motions, by Demigiant
- [Simple Event System](https://github.com/lourenco-pedro/UnitySimpleEventSystem) for events, by me
- [R3](https://github.com/Cysharp/R3) for UniRx features, by Cysharp 🙇

## 🧠 Execution Architecture

Each game frame is processed in **two main steps**:

### 1. 🧩 Simulation Pipeline (`ISimulationAgent`)

Handles all gameplay logic, independently of Unity. Each simulation step is implemented via an `ISimulationAgent`, which is then registered in the simulation pipeline.

Examples:
- Movement and velocity updates
- Collision detection
- Entity lifespan management
- Spawning logic

To add new mechanics, simply implement `ISimulationAgent` and plug it into the pipeline.

### 2. 🖼️ Render Pipeline (`IRenderAgent`)

Once the simulation step is complete, the render pipeline syncs data with Unity’s scene. Each visual element is managed by an `IRenderAgent`.

To render new elements, implement `IRenderAgent` and register it into the pipeline.

#### 🎭 World Object Representation – BaseWorldObject
All visual entities rendered in the Unity scene must have a corresponding instance derived from the abstract class ``BaseWorldObject``.

These objects serve as the bridge between simulation data and Unity’s GameObjects, encapsulating all the setup needed for rendering.

> ⚠️ It is the developer’s responsibility to manually instantiate these objects when implementing a custom IRenderAgent. Please, refere to [RPlayer.cs](./Assets/Scripts/App/Render/RPlayer.cs) and [WOPlayer.cs](./Assets/Scripts/App/Render/WorldObjects/WOPlayer.cs) for more contexts.

### Implemented simulation and render agents include:

For simulating and render players
- `SPlayer`
- `RPlayer`

For simulating and render asteroids

- `SAsteroids`
- `RAsteroids`

For simulating and render bullets

- `SBullet`
- `RBullet`


---

## 🔁 Frame Execution Flow

```text
[ SIMULATION ]
All ISimulationAgents are executed to update game state

      ↓

[ RENDERING ]
All IRenderAgents are executed to reflect the updated state in Unity
```

## 🗃️ Data Management

All gameplay data is centralized and managed through a structure called DataTable.

The DataTable holds sets of data grouped by type, where each set is identified by an enum ``DataType``. Within each set, individual data entries can be accessed by a unique Id.

This structure acts as the single source of truth during both simulation and rendering phases.

```c#
var playerData = DataTable.FromCollection(DataType.Player).WithId("Player1");
```


**Key Benefits:**

- ✅ Easy and structured access to data across systems
- ✅ Enables decoupling of agents from Unity components
- ✅ Provides flexibility for testing and data injection
- ✅ Facilitates debugging and serialization

This approach reinforces the data-driven nature of the architecture, where all agents (simulation or render) operate on shared, well-defined data without tightly coupling to Unity-specific objects.

---

# Code example

Here are some examples you can find in the ``GameStarter.cs`` class.

**Create a new game instance**

```c#
game = Game.CreateGame();
/* [...] CONFIGURE SIMULATION & RENDER PIPELINES */
game.Start();
```

**Setting up game settings**

```c#
GameConfiguration status = JsonUtility.FromJson<GameConfiguration>(_settings.text);
status.Id = "settings";

DataTable.FromCollection(DataType.GameSettings).Register(status);
```

**Setting up game score**

```c#
Scores gameScores = new Scores
{
    Id = "score",
    asteroidsDestroyed = 0,
    totalScore = 0
};

DataTable.FromCollection(DataType.GameSettings).Register(gameScores);
```

**Setting up player**

```c#
SPlayer.PlayerControllSettings p1Controls = new SPlayer.PlayerControllSettings
{
    Down  = KeyCode.S,
    Up    = KeyCode.W,
    Left  = KeyCode.A,
    Right = KeyCode.D
};

//Adding player simulation agent
//You can add as many players you want. Just make sure they not
//use the same keybindings
game.AddSystem(new SPlayer("player1", p1Controls, Color.white));

 //Add player render to the render pipeline
 //This RPlayer already iterates through all instances of players.
 //But you could also implement an RPlayer that looks for a specific Player ID
 //and updates it. This will lead to add a new RPlayer for each new Player in the
 //Data table 
game.AddRender(new RPlayer());
```

## 🧩 Debug & Visualization – DataTable Viewer

This project includes a custom Editor Window that displays all the data currently registered in the game via the DataTable. It serves as a powerful tool for developers and designers alike.
You can access via `App/Data View` menu item.

## Conclusion

This project showcases a modular and scalable architecture for game development in Unity, built around a data-oriented approach that cleanly separates simulation logic from rendering. With the use of pipelines (``ISimulationAgent``, ``IRenderAgent``), centralized data management (``DataTable``), and rendering through ``BaseWorldObject``, it becomes easier to extend, test, and reason about the game’s behavior.

While the goal has been to decouple systems as much as possible, **some areas may still contain tight coupling**—either because they were overlooked or because no suitable abstraction was found at the time. These parts are candidates for future improvement and refactoring.

> Feel free to explore, extend, and adapt this architecture to fit your own gameplay ideas or development workflows.

# License

DoD - Asteroids is a free project; you can redistribute it and/or modify it under the terms of the MIT license. See LICENSE for details.
