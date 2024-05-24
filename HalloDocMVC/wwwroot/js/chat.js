"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//Disable the send button until connection is established.

connection.on("ReceiveMessage", function (user, message, reqid) {

    var now = new Date();

    var request = document.getElementById("requestid").value;
    if (request != null && reqid == request) {
        console.log(message);
        var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
        var encodedMsg = msg;
        var div = document.createElement("div");
        var div2 = document.createElement("div");
        div.classList.add('text-start', 'text-info', 'h5');
        div2.classList.add('text-start', 'small', 'text-muted');
        div.textContent = encodedMsg;
        div2.textContent = now.toLocaleTimeString();
        $("#messagesList").append(div);
        $("#messagesList").append(div2);
    }

});

connection.start().then(function () {
    connection.invoke("GetConnectionId").then(function (id) {
        //document.getElementById("connectionId").innerHTML = id;
        console.log(id);
    });
}).catch(function (err) {
    return console.error(err.toString());
});

function sendMSG() {
    var sender = document.getElementById("userInput").value;
    console.log(sender);
    var message = document.getElementById("messageInput").value;
    console.log(message);
    var receiver = document.getElementById("receiverInput").value;
    console.log(receiver);
    if (receiver != "") {
        //send to a user

        connection.invoke("SendToUser", sender, receiver, message, reqid, receiverid, receivertype, receivername).then(function () {
            var now = new Date();
            var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
            var encodedMsg = msg;
            var div = document.createElement("div");
            var div2 = document.createElement("div");
            div.classList.add('text-end', 'text-secondary', 'h5');
            div2.classList.add('text-end', 'small', 'text-muted');
            div.textContent = encodedMsg;
            div2.textContent = now.toLocaleTimeString();

            $("#messagesList").append(div);
            $("#messagesList").append(div2);
            document.getElementById("messageInput").value = '';
        }).catch(function (err) {
            return console.error(err.toString());
        });
    }
    event.preventDefault();
}

