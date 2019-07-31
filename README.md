# [TRPGServer](https://game.keiththor.com)

TRPGServer is an ASP.NET Core and Angular 5 application that hosts a massively-multiplayer online, turn-based role-playing game. Players can battle against one another or the AI in 2 teams of up to 3 players each. They battle using up to 9 characters each that are placed on a grid where positioning plays an important role in survivability.

# About the Infrastructure

The application is split up into 3-layers: the client, the server, and the game. The client displays the game and allows the user to interact with the game and other players. Most interactions the player makes in the game causes the client to send a request to the server, most of which are real-time requests, which handles the requests and forwards them to the game if there are no errors. 

The game exists independently of the server and the client. All players are assigned a unique PlayerEntityManager, which exists as a stand in for the player's representation in the game world. Requests from the client pass into their assigned managers who replicate the player's desired action in the game. Any changes caused by the requests will propagate back to the server through events, which the server should be listening for. The server then sends the results of the requests back to the client.

The application was designed this way because each layer of the application distrusts data coming from the layer above it. The server layer must check to make sure a request coming from the client to make sure it's a valid request. The game layer must check to make sure the request passed from the server is valid in the player's current game state and context. The problem is compounded in an asynchronous and concurrent environment. In the current application design, the game remains the only source of truth and ignores any invalid requests.

## Current State

The game is currently in a playable state. There are a few minor and major bugs that are currently being worked on but for the most part shouldn't affect gameplay too much. The currently implemented features in the game are as follows:

1. The player can create any number of characters with different hair, body, and class combinations.

2. The player can create any number of formations with characters in different positions and in one or more formations at the same time.

3. The player can move around in a game map and move from map to map.

4. The player can talk with other players on the same map or in any map in the game.

5. The player can initiate battle and join battle against the AI or other players.

6. The player can battle against one or more opponents using a variety of abilities.

Most of my time working on this project in the future will be refactoring the code base and adding unit tests, but the planned future features that I have laid the groundwork for are as follows:

1. The player can open their inventory and equip items for each character.

2. An admin login that allows game developers to create new classes, abilities, items, monsters, and game maps that can be implemented immediately without requiring a server restart.

3. The player can invite other players into a party and start a battle with all party members present.

## Installation using Visual Studio

Before installing the project, you must have [Visual Studio 2015+](https://visualstudio.microsoft.com/downloads/) with the .NET Core package and [Node](https://nodejs.org/en/download/) installed.

1. Clone or download the source files above. Extract the files if necessary.

2. Open the TRPGServer.sln file in Visual Studio.

3. In the Quick Launch panel (shortcut Ctrl + Q), type in Package Manager Console and open it.

4. In the Package Manager Console, make sure the Default Project is TRPGServer.

5. Type into the Package Manager Console and execute:
```bash
Update-Database
```

6. After your database is successfully created, open Node and navigate to the root of the project (where TRPGServer.sln is).

7. Type the following into Node to navigate to the client-side code:
```bash
cd TRPGServer/ClientApp
```

8. Type the following into Node to install all the required npm packages:
```bash
npm install
```

9. If there were no issues, you should now have a working installation of the TRPGServer project.

## Installation using Visual Studio Code

Before installing the project, you must have [Visual Studio Code](https://visualstudio.microsoft.com/downloads/), [.NET Core SDK 2.1+](https://dotnet.microsoft.com/download), and [Node](https://nodejs.org/en/download/) installed.

1. Clone or download the source files above. Extract the files if necessary.

2. Open the root folder of the project using Visual Studio Code (you should see TRPGServer.sln).

3. Open a new terminal window within Visual Studio Code (shortcut Ctrl+Shift+`).

4. In the terminal window, type in the following and execute:
```bash
dotnet ef database update
```

5. After your database is successfully created, type in the following in the terminal to navigate to the client-side code:
```bash
cd TRPGServer/ClientApp
```

6. Type the following into the terminal to install all the required npm packages:
```bash
npm install
```

7. If there were no issues, you should now have a working installation of the TRPGServer project.

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

## License
[MIT](https://choosealicense.com/licenses/mit/)
