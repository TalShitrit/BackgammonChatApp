"use strict";

const userConnection = new signalR.HubConnectionBuilder().withUrl("/userHub").withAutomaticReconnect().build();
userConnection.keepAliveIntervalInMilliseconds = 500000
userConnection.serverTimeoutInMilliseconds = 120000;

userConnection.start().then(function  () {
    let promise = new Promise((res, rej) => {
        getLoggedIn()
        getLoggedOut()
    })
    promise.then()
 
}).catch(function (err) {
    return console.error(err.toString());
});


userConnection.on("AskForAGame", function (sender) {
    let res = confirm(`the user ${sender} want to play with you :) 
do you agree?`)
    if (res) {
        userConnection.invoke("StartGame", sender).catch(function (err) {
            return console.error(err.toString());
        });
    } else {
        //let target = sender
        userConnection.invoke("GameRefused", sender).catch(function (err) {
            return console.error(err.toString());
        });
    }
});
userConnection.on("GameRefused", function () {
    alert("you game request were refused")
});
userConnection.on("StartGame", function () {
    window.location.replace("Home/Game");
});
userConnection.on("serverRespond", function (msg) {
    alert(msg)
});
const askForAGame = () => {
    let target = document.getElementById("gameTargetInput").value
    if (target) {
        userConnection.invoke("AskForAGame", target).catch(function (err) {
            return console.error(err.toString());
        });
    }
}


const getLoggedIn = () => {
    let ulLoggedIn = document.getElementById("ulLoggedIn")
    if (ulLoggedIn) {
        ulLoggedIn.innerHTML = "Loading..."
        $.get("User/GetLoggedIn", function (data) {
            ulLoggedIn.innerHTML = ""
            for (var i = 0; i < data.length; i++) {
                let li = document.createElement('li')
                let user = data[i]
                li.innerText = user
                li.classList.add("liUser")
                li.addEventListener("click", () => selectUser(user))
                ulLoggedIn.appendChild(li)
            }

        });
    }
};
const getLoggedOut = () => {
    let ulLoggedOut = document.getElementById("ulLoggedOut")
    if (ulLoggedOut) {
        ulLoggedOut.innerHTML = "Loading..."
        $.get("User/GetLoggedOut", function (data) {

            ulLoggedOut.innerHTML = ""
            for (var i = 0; i < data.length; i++) {
                let li = document.createElement('li')
                let user = data[i]
                li.innerText = user
                li.classList.add("liUser")
                li.addEventListener("click", () => selectUser(user))
                ulLoggedOut.appendChild(li)
            }
        });
    }
};

const selectUser = (value) => {

    let chatTargetInput = document.getElementById("chatTargetInput")
    if (chatTargetInput) {
        chatTargetInput.value = value
    }
    let gameTargetInput = document.getElementById("gameTargetInput")
    if (gameTargetInput) {
        gameTargetInput.value = value
    }
}