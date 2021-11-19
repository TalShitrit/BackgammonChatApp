"use strict";

document.getElementById("sendButton").disabled = true;
document.getElementById("loadLastBtn").disabled = true;

const chatConnection = new signalR.HubConnectionBuilder().withUrl("/chatHub").withAutomaticReconnect().build();
chatConnection.keepAliveIntervalInMilliseconds = 300000
chatConnection.start().then(async ()=> {
   await connectionFinished()
}).catch(function (err) {
    return console.error(err.toString());
});

chatConnection.on("ReceiveMessage", function (msg, isme) {
    let li = document.createElement("li");
    li.className = isme ? "Msgin" : "Msgout"
    li.classList.add("liMsg")
    document.getElementById("messagesList").appendChild(li);
    let msgData = msgFormat(msg)
    li.textContent = msgData
    msgSound(isme);
});
chatConnection.on("ProblebInSendinMsg", function (msg) {
    alert(msg)
});
chatConnection.on("LoadLast", async function (user, msgs) {
    for (var i = 0; i < msgs.length; i++) {
        let elem = msgs[i];
        let li = document.createElement("li");
        li.className = user == elem.senderName ? "Msgin" : "Msgout"
        li.classList.add("liMsg")

        let container = document.getElementById("messagesList")
        container.insertBefore(li, container.firstChild);


        let msgData = msgFormat(elem)
        li.textContent = msgData

    }

});
chatConnection.on("serverRespond", function (msg) {
    alert(msg)
});


document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var content = document.getElementById("messageInput").value;
    var destination = document.getElementById("destInput").value;
    document.getElementById("messageInput").value = ""
  
    if (content) {
        chatConnection.invoke("SendMessage", user, content, destination).catch(function (err) {
            console.log(err)
            alert("some problem was made, try refresh the page");
        });
        event.preventDefault();
    }
    else {
        alert("content is empty")
        document.getElementById("messageInput").focus()
        }
    
         
    
});
document.getElementById("loadLastBtn").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var skip = 0
    skip += document.getElementsByClassName("liMsg").length
    var destination = document.getElementById("destInput").value;
    chatConnection.invoke("LoadLast", user, skip, destination).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});
document.getElementById("msgContainerDiv").classList.add("DivWithScroll")
document.getElementById("messageInput").addEventListener('keypress', function (e) {
    if (e.key === 'Enter') {
        document.getElementById("sendButton").click()
    }
})

const connectionFinished =  () => {
    return new Promise((res, rej) => {
        document.getElementById("sendButton").disabled = false;
        document.getElementById("loadLastBtn").disabled = false;  
        document.getElementById("loadLastBtn").click();
    })

}
const getDate = (date) => {
    let msgDate = new Date(date);
    let today = new Date();
    let sec = String(msgDate.getSeconds()).padStart(2, '0');
    let min = String(msgDate.getMinutes()).padStart(2, '0');
    let hh = String(msgDate.getHours()).padStart(2, '0');
    let dd = String(msgDate.getDate()).padStart(2, '0');
    let mm = String(msgDate.getMonth() + 1).padStart(2, '0'); //January is 0!
    let yyyy = msgDate.getFullYear();
    if (today.getFullYear() == yyyy && today.getMonth() + 1 == mm && today.getDate() == dd) {
        return `Today at - ${hh}:${min}:${sec}`
    }
    return `${hh}:${min}:${sec} ; ${dd}/${mm}/${yyyy}`
}
const msgFormat = (msg) => {
    let date = getDate(msg.timeSend)
    return `${msg.senderName} (${date}) \n
${msg.content}`;
}
const msgSound = (isme) => {
    if (isme) {
        var audio = new Audio('../sound/msgSound.wav');
        audio.play();
    }
    else {
        var audio = new Audio('../sound/msgIn.wav');
        audio.volume=.1
        audio.play();
       
    }
}

