"use strict";
const rollDiceBtn = "rollDiceBtn"
const slot = "slot"
const Player1PawnClass = "Player1"
const Player2PawnClass = "Player2"
let player1Collected = 15
let player2Collected = 15
let dices = [];
let player1Turn = false
let selectedTomove;
let canRollDice = true
//------------------------------connection functions-----------------
const Gameconnection = new signalR.HubConnectionBuilder().withUrl("/backgammonHub").withAutomaticReconnect().build();
Gameconnection.keepAliveIntervalInMilliseconds = 150000
document.getElementById("rollDiceBtn").disabled = true;
Gameconnection.start().then(function () {
    document.getElementById("rollDiceBtn").disabled = false;
    getCurrentGame()
}).catch(function (err) {
    return console.error(err.toString());
});

Gameconnection.on("RollDice", function (dice) {
    canRollDice = false
    dices = []
    var diceHolder = document.getElementById("diceHolder")
    diceHolder.innerHTML = ""
    for (var i = 0; i < dice.length; i++) {

        let number = dice[i];
        let img = document.createElement("img")
        img.id = "dice" + i
        img.src = `../img/dice${number}.png`
        img.classList.add('dice')
        diceHolder.appendChild(img)
        dices.push(new Dice(number, img))
    }
    rollDiceSound()
});
document.getElementById("rollDiceBtn").addEventListener("click", function (event) {
    if (canRollDice) {
        try {
            Gameconnection.invoke("RollDice").catch(function (err) {
                return console.error(err.toString());
            });
            event.preventDefault();

        } catch (e) {
            console.log(e);
        }
    }


});

Gameconnection.on("InitNewGame", function (gameBoard) {
    setGameBoard()
    player1Turn = true
    setAllPawns(gameBoard)
});
document.getElementById("startNewGameBtn").addEventListener("click", function (event) {

    try {
        Gameconnection.invoke("InitNewGame").catch(function (err) {
            return console.error(err.toString());
        });
        event.preventDefault();

    } catch (e) {
        console.log(e);
    }


});

Gameconnection.on("InvalidMove", () => {
    wrongMoveSound();
    alert("InvalidMove");
})
Gameconnection.on("PieceCollected", (from, dice) => {

    var FromSlot = document.getElementById(slot + from)
    var pawn = FromSlot.children[0]

    FromSlot.removeChild(pawn)
    if (player1Turn) {
        player1Collected--
        document.getElementById("player1Left").innerText = player1Collected
    }
    else {
        player2Collected--
        document.getElementById("player2Left").innerText = player2Collected
    }
    useDice(dice)
    IsGameEnd()
})
Gameconnection.on("GameBoardChange", function (from, target, dice) {
    var FromSlot = document.getElementById(slot + from)
    var pawn = FromSlot.children[0]
    FromSlot.removeChild(pawn)

    var targetSlot = document.getElementById(slot + target)
    targetSlot.appendChild(pawn);

    if (dice)
        useDice(dice)
});
Gameconnection.on("NotYourTurn", () => {
    alert("this is not your turn to act");
})
Gameconnection.on("GameOver", (msg) => {
    if (msg) {
        alert(msg)
    }
    else {
        alert("the current game is over, start new one");
    }
})
Gameconnection.on("GiveUpOnTurn", () => {
    togglePlayer();
    clearDice();
})
Gameconnection.on("GetGame", function (gameBoard,playwith) {
    setAllPawns(gameBoard)
    if (playwith) {
        let div = document.createElement("div")
        div.innerText = `you fight vs -- `
        let b = document.createElement("b")
        b.innerText = `${playwith}`
        div.appendChild(b)
        div.classList.add("fightVs")
        document.getElementById("sideGame").appendChild(div)
    }
});
Gameconnection.on("Exception", (e) => {
    alert("some error was made")
    console.error(e)
})
Gameconnection.on("ServerMessage", (msg) => {
    alert(msg)  
})


//------------------------------const functions-----------------
const giveUp = () => {
    let really = confirm("Are you sure you want to give up on game?")
    if (really) {
        Gameconnection.invoke("GiveUp")
    }
}
const IsGameEnd = () => {
    if (player1Collected == 0)
        alert("player 1 win!")
    if (player2Collected == 0)
        alert("player 2 win!")
}
const slotClickEvent = async (boardPart) => {
    // no pawn was select to move
    if (!selectedTomove) {
        if (dices) {
            if (boardPart.children.length > 0) {
                if (player1Turn) {
                    if (boardPart.children[0].classList.contains(Player1PawnClass)) {
                        selectedTomove = boardPart
                    }
                    else {
                        alert("not your pawn")
                    }
                }
                else {
                    if (boardPart.children[0].classList.contains(Player2PawnClass)) {
                        selectedTomove = boardPart
                    }
                    else {
                        alert("not your pawn")
                    }
                }
            }
            if (boardPart.children.length == 0) { alert("no pawn to move") }
        } else {
            alert("roll the dice")
        }
    }
    // try to move
    else {
        try {
            let selectedId = getIdFromSlot(selectedTomove)
            let targetId = getIdFromSlot(boardPart)
            let moved = await activeConnectionMoveEvent(selectedId, targetId)
            if (moved) {
                tapSound();     
            }
            else {
                wrongMoveSound();
            }
            selectedTomove = null
        } catch (e) {
            selectedTomove = null
            console.log(e)
        }
    }

}
const activeConnectionMoveEvent = async (from, target) => {
    return new Promise((res, rej) => {
        var r = Gameconnection.invoke("Move", from, target)
        if (r) {
            r.then((moved) => res(moved))
        }
    })
}
const giveUpOnTurn = () => {
    let really = confirm("Are you sure you want to give up on turn?")
    if (really) {
        Gameconnection.invoke("GiveUpOnTurn")
    }
  
}
const setGameBoard = () => {
    let gamwBoard = document.getElementById('gamwBoard')
    gamwBoard.innerHTML = ""
    let gameBoardTop = document.createElement('div')
    gameBoardTop.className = "gameBoardTop"
    let gameBoardBar = document.createElement('div')
    gameBoardBar.className = "gameBoardBar"
    let gameBoardBot = document.createElement('div')
    gameBoardBot.className = "gameBoardBot"

    player1Collected = 15
    player2Collected = 15

    gamwBoard.appendChild(gameBoardTop)
    gamwBoard.appendChild(gameBoardBar)
    gamwBoard.appendChild(gameBoardBot)

    for (var i = 13; i <= 24; i++) {
        let boardPart = document.createElement("div")
        gameBoardTop.appendChild(boardPart)
        boardPart.addEventListener('click', () => { slotClickEvent(boardPart) })
        boardPart.id = slot + i
        if (i % 2 == 0) {
            boardPart.classList.add("bg-danger")
        }
        else {
            boardPart.classList.add("bg-secondary")
        }
        boardPart.classList.add("boardPart")
        boardPart.classList.add("col-1")
    }
    for (var i = 12; i > 0; i--) {
        let boardPart = document.createElement("div")
        gameBoardBot.appendChild(boardPart)
        boardPart.addEventListener('click', () => { slotClickEvent(boardPart) })
        boardPart.id = slot + i
        if (i % 2 == 0) {
            boardPart.classList.add("bg-secondary")
        }
        else {
            boardPart.classList.add("bg-danger")
        }
        boardPart.classList.add("boardPart")
        boardPart.classList.add("col-1")
    }

    let slot25 = document.createElement("div")
    gameBoardBar.appendChild(slot25)
    slot25.addEventListener('click', () => { slotClickEvent(slot25) })
    slot25.id = slot + 25
    slot25.classList.add("bg-dark")
    slot25.classList.add("boardPartBar")
    slot25.classList.add("col-5")

    let slot0 = document.createElement("div")
    gameBoardBar.appendChild(slot0)
    slot0.addEventListener('click', () => { slotClickEvent(slot0) })
    slot0.id = slot + 0

    slot0.classList.add("bg-dark")
    slot0.classList.add("boardPartBar")
    slot0.classList.add("col-5")

    let PlayerTurn = document.getElementById("PlayerTurn")
        PlayerTurn.classList.add("Player1")
  
}
const getCurrentGame = () => {
    Gameconnection.invoke("GetGame")
}
const setAllPawns = (gameBoard) => {
    if (gameBoard) {

        let PlayerTurn = document.getElementById("PlayerTurn")
        if (player1Turn) {
            PlayerTurn.classList.remove("Player2")
            PlayerTurn.classList.add("Player1")
        }
        else {
            PlayerTurn.classList.remove("Player1")
            PlayerTurn.classList.add("Player2")
        }
        for (var i = 0; i < gameBoard.length; i++) {
            let slotPart = gameBoard[i]
            if (slotPart && slotPart.collection) {
                for (var j = 0; j < slotPart.collection.length; j++) {
                    let elem = slotPart.collection[j]

                    let pawn = createPawn(elem.isFirstPlayer)
                    let slotDiv = document.getElementById(`${slot}${elem.slotAt}`)
                    slotDiv.appendChild(pawn)
                }
            }
        }
    }
}
const togglePlayer = () => {
    player1Turn = !player1Turn
    let PlayerTurn = document.getElementById("PlayerTurn")
    if (player1Turn) {
        PlayerTurn.classList.remove("Player2")
        PlayerTurn.classList.add("Player1")
    }
    else {
        PlayerTurn.classList.remove("Player1")
        PlayerTurn.classList.add("Player2")
    }
    canRollDice = true;
}
const createPawn = (isFirstPlayer) => {
    if (isFirstPlayer) {
        let p1 = document.createElement("div")

        p1.classList.add("pawn")
        p1.classList.add("Player1")
        return p1
    }
    let p2 = document.createElement("div")
    p2.classList.add("pawn")
    p2.classList.add("Player2")
    return p2
}
const getIdFromSlot = (targetSlot) => {
    return Number(targetSlot.id.substring(slot.length))
}
const useDice = (dice) => {
    for (var i = 0; i < dices.length; i++) {
        if (!dices[i].wasUsed) {
            if (dices[i].diceNumber == dice) {
                dices[i].diceImg.classList.add("diceDisable")
                dices[i].wasUsed = true
                break
            }
        }

    }
    for (var i = 0; i < dices.length; i++) {
        if (!dices[i].wasUsed) {
            return;
        }
    }
    // no more dice to use => end turn
    dices = null;
    togglePlayer();
}
const clearDice = () => {
    for (var i = 0; i < dices.length; i++) {
        if (!dices[i].wasUsed) {
            dices[i].diceImg.classList.add("diceDisable")
            dices[i].wasUsed = true
        }

    }
}
const removeSelected = () => {
    selectedTomove = null;
}

class Dice {
    constructor(dicenumber, diceImg) {
        this.diceNumber = dicenumber;
        this.diceImg = diceImg;
        this.wasUsed = false
    }
}


//------------------------------sound-----------------
const tapSound = () => {
    var audio = new Audio('../sound/tap.wav');
    audio.volume = 0.7;
    audio.play();
}
const wrongMoveSound = () => {
    var audio = new Audio('../sound/wrongMove.wav');
    audio.volume = 0.1;
        audio.play();
}
const rollDiceSound = () => {
    var audio = new Audio('../sound/rollDice.wav');
    audio.play();
}





setGameBoard()