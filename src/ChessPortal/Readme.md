## Synopsis

This is a chess API where players can create accounts and play chess against each other. It is intended to be used with a frontend for a small chess club. The api can validate moves, store them in a database and inform the players when checkmate and draw occurs. 
It can also fetch chess problems from Chessblunders.org and let the users solve these problems. Users can also look at games they are playing and see the move list. 


## Installation

To Set up this project locally you need the following:
Microsoft Windows (This method for installing the api is only tested on Windows 10)
Visual Studio 2017
Install Visual Studio 2017. Clone the git repo using VS 2017 and build. 
Then hit f5 to start the project. Make sure that ChessPortal.Web is the startup project.


## API Reference

### Create account

#### POST /api/account/register

Creates a new user account on the chess portal with the supplied user name and password.

Request example
 ```
 {
     "userName": "perjansson",
     "password": "Abcd1%efgh2",
     "confirmPassword": "Abcd1%efgh2",
     "email": "anderse77@gmail.com"
 }
 ```
 Password needs to have at least one Captial letter, one number and one special character.

### Login

#### POST /api/account/Login

Sign in to an existing account.
Request example
 ```
 {
     "userName": "perjansson",
     "password": "Abcd1%efgh2"
 }
 ```
Logs the user in if User name and password is correct. The user is required to be logged in to use any of the functions in this api.

### Logout

#### POST /api/account/logout

Logs the user out.

### Create challenge

#### POST /api/challenge

Creates a new challenge for other players to respond to. The user must provide what color he wants to play and the number of days each player has per move.

Request example
 ```
 {
     "color": 0,
     "daysPerMove": 3,
 }
 ```
Color is 0 for white and 1 for black. daysPerMove is the number of days per move.

### Get unanswered challenges

#### GET /api/challenge

Gets all challenges that the currently logged in user can respond to, that is the challenges created by other users but not yet answered

Response example
 ```
[{
    "drawRequests": [],
    "id": "74ce9a8f-9c49-429c-6d60-08d484b11327",
    "color": 1,
    "daysPerMove": 4,
    "moves": []
}]
 ```
Response is an array of the unanswered challenges from other players.

### Accept challenge

#### POST /api/challenge/{id}

Accepts the challenge with he specified id.

### Get games

#### GET /api/game

Gets all games that the currently logged in user is playing.

Response example
 ```
[
  {
    "drawRequests": [],
    "id": "1106168d-dc05-4c57-7d81-08d494c4adc9",
    "daysPerMove": 1,
    "status": 4,
    "moves": [
      {
        "id": "af67cd83-3096-484a-95fb-08d49f65133c",
        "fromX": 4,
        "toX": 4,
        "fromY": 1,
        "toY": 3,
        "piece": 2,
        "color": 0,
        "moveNumber": 1,
        "promoteTo": null,
        "challengeId": "1106168d-dc05-4c57-7d81-08d494c4adc9"
      }
    ],
    "whitePlayer": {
      "userName": "perjansson",
      "numberOfWonGames": 0,
      "numberOfLostGames": 0,
      "numberOfDrawnGames": 0,
      "numberOfProblemsSolved": 2,
      "numberOfProblemsFailed": 1
    },
    "blackPlayer": {
      "userName": "evahenriksson",
      "numberOfWonGames": 0,
      "numberOfLostGames": 0,
      "numberOfDrawnGames": 0,
      "numberOfProblemsSolved": 0,
      "numberOfProblemsFailed": 0
    },
    "whitesTurn": false
  }
]
 ```
Response is an array of the games the player is playing. 
fromX is the file(Number 0 is the a file and number 7 is the e-file.) the piece moves from and toX is the destination file for the piece.
fromY is the zero-based rank from which the piece moves and toY is the zero-based destination rank.
Piece is 2 for Pawn, 3 for Knight, 4 for Bishop, 5 for Rook, 6 for Queen and 7 for King. promoteTo is the same if the move is a pawn promotion.

### Make move

#### POST /api/move

Gets all games that the currently logged in user is playing.

Request example
 ```
{
    "fromX": 4,
    "toX": 6,
    "fromY": 7,
    "toY": 7,
    "piece": 7,
    "color": 1,
    "challengeId": "4e37dab6-336c-4f02-8124-08d4722da5a3"
}
 ```
If move is valid, it is stored in the database and it is the next players turn. If a draw has been requested you reject that request by moving.

### Request draw

#### POST /api/draw/request

Requests a draw

Request example
 ```
{
    "challengeId": "4e37dab6-336c-4f02-8124-08d4722da5a3"
}
 ```
A draw request is made which the other player can see then fetching his or her games.

### Accept draw

#### POST /api/draw/accept

Accepts a requested draw

Request example
 ```
{
    "challengeId": "4e37dab6-336c-4f02-8124-08d4722da5a3"
}
 ```
A draw request is made which the other player can see then fetching his or her games. There is no endpoint for rejecting a draw since a user does that by making a move.

### Get chess problem

#### GET /api/problem/

Gets a random chess problem from chessblunders.org and stores it to the database. You can only get one problem at a time.

### propose solving move

#### POST /api/problem/

Same input as for POST /api/move except tha no challengeId is needed. if the move is correct, the game proceeds to the next move in the problem. If the problem is solved, it is removed from the database and you can get a new random problem.

### Get player stats

#### GET /api/account/player

Gets player stats

response example
{
  "userName": "evahenriksson",
  "numberOfWonGames": 1,
  "numberOfLostGames": 0,
  "numberOfDrawnGames": 8,
  "numberOfProblemsSolved": 0,
  "numberOfProblemsFailed": 0
}
