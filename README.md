# Design Patterns
### This repository contains an implementation of the Maze game for lab-6.
Rules of the game:

The goal of the game is to get from the start point to the end point of the maze.

The player controls a rectangle that represents the player in the maze.

The player can move up, down, left and right using the arrow keys on the keyboard or WASD keys.

The player can only move along the path of the maze. He cannot move through the walls.

The player cannot go outside the maze. He must stay inside the boundary walls.

If the player reaches the end point of the maze, he wins.

The game has a timer that shows the time the player has spent completing the maze.

The game can be paused using the “Pause” button. Pausing the game stops the timer and displays a pause message. The game can be continued after the pause.

The game can be closed using the “Exit” button, which returns the player to the main menu.e you have spent playing the game on the screen.

### Description of the observed programming principles, design patterns, and refactoring techniques

_Programming Principles_

SOLID principles:

Single Responsibility Principle (SRP): The [Level_1 class](https://github.com/Tsaruk-Kate/Maze_game/blob/7518ad18c6c1a0a706e1dd12bb39e422b5247dcf/labyrinth_gam%D0%B5/Views/Level_1.xaml.cs#L34C9-L46C10) is responsible for creating and managing the game level. Its functions include game initialization, maze generation, game display, player control, and event handling. Each method is responsible for a specific task that follows the SRP principle.

Open/Closed Principle (OCP): The Level_1 class can be easily extended or modified without changing the main class. For example, you can add new levels of the game by creating new subclasses or using composition with other classes.

Liskov Substitution Principle (LSP): This code does not use specific descendants or substitutions of base classes, so this principle is less relevant in this case.

Interface Segregation Principle (ISP): There are no explicit interfaces in this code, but classes have well-defined methods that perform specific tasks.

Fail Fast: 

Some methods, such as [MovePlayer](https://github.com/Tsaruk-Kate/Maze_game/blob/7518ad18c6c1a0a706e1dd12bb39e422b5247dcf/labyrinth_gam%D0%B5/Views/Level_1.xaml.cs#L209C8-L221C10), check whether a player's step in the maze is valid. If the step is invalid, the player immediately returns instead of continuing execution.

Composition Over Inheritance:

The code does not use inheritance to implement functionality. Instead, it uses composition, for example, in the [PauseGame](https://github.com/Tsaruk-Kate/Maze_game/blob/7518ad18c6c1a0a706e1dd12bb39e422b5247dcf/labyrinth_gam%D0%B5/Views/Level_1.xaml.cs#L319C9-L327C10) method, where the timer and game object are stopped.

YAGNI (You Ain't Gonna Need It):

The code is simple and efficient, without unnecessary features or complexities. It focuses on a specific task - creating and managing the game level.

Program to Interfaces not Implementations:

In a [few places](https://github.com/Tsaruk-Kate/Maze_game/blob/7518ad18c6c1a0a706e1dd12bb39e422b5247dcf/labyrinth_gam%D0%B5/Views/Level_1.xaml.cs#L339C9-L350C10), abstractions are used (e.g., UserFactory and IUserFactory) to provide flexibility and support for changing implementations without having to change many places in the code.

_Refactoring Techniques_

Composing Methods:

[This code](https://github.com/Tsaruk-Kate/Maze_game/blob/7518ad18c6c1a0a706e1dd12bb39e422b5247dcf/labyrinth_gam%D0%B5/Views/Level_1.xaml.cs) has been refactored using the Composing Methods technique, which consists of splitting functions into smaller code blocks. Here are the main changes:
InitializeWindow() and InitializeTimer() methods: Separate methods have been created to initialize the window and timer. This allows for better code organization and makes the main method easier to understand.
UpdateElapsedTime() method: This method is only responsible for updating the elapsed time display and is called in Timer_Tick. This improves the readability and comprehension of the code.
InitializeMaze() method: A separate method has been created to initialize the maze. This includes placing the entry and exit points, as well as creating walls.
GenerateMazeRecursive() method: This method is responsible for generating the maze. It has been split into several parts: selecting a direction, checking the next position, cutting a path, and calling it recursively. This improves the method structure and its comprehension.
MovePlayer() method: This method has been split into several parts responsible for checking the correctness of the move, updating the player's position, and ending the game if the player reaches the exit. This improves readability and makes the code easier to understand.
These changes make the code more structured, understandable, and maintainable, as each method is now responsible for one specific task.

Simplifying Conditional Expressions:

[The code](https://github.com/Tsaruk-Kate/Maze_game/blob/7518ad18c6c1a0a706e1dd12bb39e422b5247dcf/labyrinth_gam%D0%B5/Views/Level_2.xaml.cs) was refactored using the Simplifying Conditional Expressions technique, which simplifies conditional expressions and makes them more understandable and compact. Here are some of the changes that have been made:
A new IsPaused() method has been introduced to check if the game is currently paused.
The Timer_Tick method uses the IsPaused() method to determine whether the game's downtime should be taken into account.
In the MovePlayer method, new auxiliary methods CheckExitCollision and CheckMovementValidity have been introduced to check the conditions for exit collisions or the possibility of movement in the game.
In the PlayingState.HandleKeyPress method, the same cases are collected (for example, Key.Left and Key.A, Key.Right and Key.D, and so on) to make the code more understandable and compact.
In the PausedState.HandleKeyPress method, the conditional statement is replaced with a simple list of conditions to make the code more understandable.
These changes help make the code more understandable and maintainable.

Moving Features between Objects:

[The code](https://github.com/Tsaruk-Kate/Maze_game/blob/7518ad18c6c1a0a706e1dd12bb39e422b5247dcf/labyrinth_gam%D0%B5/Views/Level_3.xaml.cs) refactoring used the Moving Features between Objects technique, which consists in moving features and functionality between classes to improve the structure and understanding of the code. Here are some of the changes that have been made:
The maze generator and visualizer have been moved to separate classes:
Previously, the functionality for generating the maze and visualizing it was located directly in the Level_3 class. In the refactoring, this functionality was moved to separate classes MazeGenerator and MazeRenderer.
This allowed us to reduce the dependency between classes, make them more independent and understandable.
A separate class for moving the player has been created:
Previously, the logic for moving the player was built directly into the Level_3 class. In the refactoring, this logic was moved to the MovePlayer class, which is responsible for moving the player through the maze.
This simplifies the Level_3 class, making it cleaner and clearer.
Using the Command pattern to control the player's movement:
We used the Command pattern to control the player's movement. Command classes (MoveUpCommand, MoveDownCommand, MoveLeftCommand, MoveRightCommand) are created with an object of the MovePlayer class that performs the player's movement.
This approach makes it easy to add new commands and movements without changing the Level_3 class, which makes the code more flexible and easily extensible.
In general, these changes help to improve the code structure, clarity, and maintainability.

_Design Patterns_

Factory method:

[In this code](https://github.com/Tsaruk-Kate/Maze_game/blob/7518ad18c6c1a0a706e1dd12bb39e422b5247dcf/labyrinth_gam%D0%B5/Views/Labyrinth.xaml.cs), the Factory method pattern is implemented as follows:

There is an IUserFactory interface that defines the CreateUser factory method that returns an object of type User. This interface serves as an abstraction for creating users.

There is a UserFactory class that implements the IUserFactory interface. This class defines the CreateUser method, which creates and returns a new object of type User based on the name and key parameters passed.

In the Labyrinth class, which is a page in a WPF application, there is a private field _userFactory of type IUserFactory, which is used to create user objects.

The constructor of the Labyrinth class takes an object of type IUserFactory as a parameter, which allows you to set the factory when creating a Labyrinth object.

The Button_Click_2 method, which is called when a button is clicked, uses the factory method to create a new user. Instead of creating a User object directly using the new operator, the stored _userFactory is used to create a new user based on the data entered.

Thus, the Factory Method pattern allows you to separate the process of creating User objects from the Labyrinth class, which simplifies its extension and support.

State:

[This code](https://github.com/Tsaruk-Kate/Maze_game/blob/7518ad18c6c1a0a706e1dd12bb39e422b5247dcf/labyrinth_gam%D0%B5/Views/Level_2.xaml.cs) implements the State pattern, which allows objects to change their behavior depending on their state.

The IGameState interface defines the HandleKeyPress method, which will be called to handle key presses in the game. This interface defines a contract for all game states.

The PlayingState and PausedState classes implement the IGameState interface and represent specific game states. Each of these classes has a HandleKeyPress method that handles key presses for the corresponding game state.

The Level_2 class contains a state field that represents the current state of the game. This field is initialized in the constructor of the PlayingState class and is used to handle key presses in the appropriate state.

The Level_2 class also has the PauseGame and ResumeGame methods, which are called to pause and resume the game, respectively. These methods change the current state of the game to PausedState or PlayingState, respectively.

In the Window_KeyDown method of the Level_2 object, the HandleKeyPress method of the current game state is called to handle key presses.

So, the State pattern allows the Level_2 object to change its behavior depending on its game state, which allows you to implement different functionality in different game states.

Command:

[This code](https://github.com/Tsaruk-Kate/Maze_game/blob/7518ad18c6c1a0a706e1dd12bb39e422b5247dcf/labyrinth_gam%D0%B5/Views/Level_3.xaml.cs) implements the Command pattern, which allows you to encapsulate requests or operations in objects that support the execution of actions that are called according to the request.

The main components of the Command pattern in this code:

ICommand interface: This interface defines the Execute() method that performs the action associated with the command.

Specific command implementations (classes MoveUpCommand, MoveDownCommand, MoveLeftCommand, MoveRightCommand): Each of these classes implements the ICommand interface and represents a specific command. Each of them performs a specific action: moving the player up, down, left or right. The Execute() method calls the corresponding Execute() method in the object of the MovePlayer class, which is responsible for performing the movement.

MovePlayer class: This class is responsible for performing player movement actions. It has an Execute() method that is called from a specific command. It accepts input parameters (X and Y axis offsets) and moves the player according to these parameters.

Dictionary _commands: This dictionary is used to map keys to command objects. When a key is pressed, the corresponding command is retrieved from this dictionary and its Execute() method is called.

Window_KeyDown method: This method handles key press events and calls the Execute() method of the corresponding command according to the key pressed.

Thus, the Command pattern allows you to separate the execution of operations from their initiation, and also allows you to easily add new commands without modifying the calling code.

_Студентка: Царук Катерина_

_Група: ІПЗ-22-4_
