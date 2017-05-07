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
[{
    "drawRequests": [],
    "id": "4e37dab6-336c-4f02-8124-08d4722da5a3",
    "color": 0,
    "daysPerMove": 100,
    "moves": [{
            "id": "bb559b57-f596-435b-f719-08d47e737ebb",
            "fromX": 4,
            "toX": 4,
            "fromY": 1,
            "toY": 3,
            "piece": 0,
            "color": 0,
            "moveNumber": 1,
            "promoteTo": null,
            "challengeId": "4e37dab6-336c-4f02-8124-08d4722da5a3"
        },
        {
            "id": "0d59e190-240c-46fb-525c-08d47e778646",
            "fromX": 4,
            "toX": 4,
            "fromY": 6,
            "toY": 4,
            "piece": 0,
            "color": 1,
            "moveNumber": 2,
            "promoteTo": null,
            "challengeId": "4e37dab6-336c-4f02-8124-08d4722da5a3"
        },
        {
            "id": "aec1e0f7-0512-4700-ce15-08d47e786105",
            "fromX": 6,
            "toX": 5,
            "fromY": 0,
            "toY": 2,
            "piece": 1,
            "color": 0,
            "moveNumber": 3,
            "promoteTo": null,
            "challengeId": "4e37dab6-336c-4f02-8124-08d4722da5a3"
        },
        {
            "id": "ff0fb4fc-d708-4208-e189-08d47e7c2565",
            "fromX": 1,
            "toX": 2,
            "fromY": 7,
            "toY": 5,
            "piece": 1,
            "color": 1,
            "moveNumber": 4,
            "promoteTo": null,
            "challengeId": "4e37dab6-336c-4f02-8124-08d4722da5a3"
        }
    ]
}]
 ```
Response is an array of the games the player is playing. 
fromX is the file(Number 0 is the a file and number 7 is the e-file.) the piece moves from and toX is the destination file for the piece.
fromY is the zero-based rank from which the piece moves and toY is the zero-based destination rank.
Piece is 0 for Pawn, 1 for Knight, 2 for Bishop, 3 for Rook, 4 for Queen and 5 for King. promoteTo is the same if the move is a pawn promotion.

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
    "piece": 5,
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

###Get opponent

####GET /api/challenge/{id}/opponent

Gets the opponent in a specific game identified by the id for the currently logged in player

response example
{
    "userName": "evahenriksson"
}

### Get chess problem

#### GET /api/problem/

Gets a random chess problem from chessblunders.org and stores it to the database. You can only get one problem at a time.

### propose solving move

#### POST /api/problem/

Same input as for POST /api/move. if the move is correct, the game proceeds to the next move in the problem. If the problem is solved, it is removed from the database and you can get a new random problem.

## Contributors

Let people know how they can dive into the project, include important links to things like issue trackers, irc, twitter accounts if applicable.

## License

A short snippet describing the license (MIT, Apache, etc.)
