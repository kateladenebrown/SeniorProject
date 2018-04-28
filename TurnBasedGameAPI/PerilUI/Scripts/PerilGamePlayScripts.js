// Scripts coded by Garrick & James

"use strict"

//********************************* Code by Garrick: Scripts for map *************************************************
var transMatrix = [0.5, 0, 0, 0.5, 0, 0];
var origMatrix = [0.5, 0, 0, 0.5, 0, 0];

function init(evt) {
    if (window.svgDocument == null) {
        svgDoc = evt.target.ownerDocument;
    }
    mapMatrix = svgDoc.getElementById("map-matrix");
    width = evt.target.getAttributeNS(null, "width");
    height = evt.target.getAttributeNS(null, "height");
}

function pan(dx, dy) {
    transMatrix[4] += dx;
    transMatrix[5] += dy;

    newMatrix = "matrix(" + transMatrix.join(' ') + ")";
    mapMatrix.setAttributeNS(null, "transform", newMatrix);
}

//Written by Garrick Evans
function zoom(scale) {
    for (var i = 0; i < transMatrix.length; i++) {
        transMatrix[i] *= scale;
    }
    transMatrix[4] += (1 - scale) * width / 2;
    transMatrix[5] += (1 - scale) * height / 2;

    newMatrix = "matrix(" + transMatrix.join(' ') + ")";
    mapMatrix.setAttributeNS(null, "transform", newMatrix);
}

//Resets the map's transform matrix
//Written by Garrick Evans
function centerMap() {
    for (var i = 0; i < transMatrix.length; i++) {
        transMatrix[i] = origMatrix[i];
    }
    mtx = "matrix(" + origMatrix.join(' ') + ")";
    mapMatrix.setAttributeNS(null, "transform", mtx);
}

// Marks connected territories yellow on mouseover
//Written by Garrick Evans
//$(function () {
//    $('#map-matrix').children('path').filter('.territory').on('mouseenter mouseleave', function () {
//        var terrId = $(this).attr('id')

//        console.log('Territory Number: ' + terrId);
//        $(this).toggleClass('highlighted');

//        var i;
//        for (i = 0; i < connections[parseInt(terrId)].length; i++) {
//            $('#' + connections[parseInt(terrId)][i]).toggleClass('moveable');
//        }
//    });
//});

//$(function () {
//    $('#map-matrix').children('path').filter('.connection').on('click', function () {
//        console.log('Path: ' + $(this).attr('id'));
//    })
//});

//Reads the connections JSON
//Written by Garrick Evans
var connections;
$(function () {
    $.ajax({
        type: 'GET',
        url: "http://localhost:8000/Map1Connections.json",
        dataType: 'json'
    }).done(function (data) {
        connections = data;
    }).fail(function (jqXHR, textStatus, errorThrown) {
        console.log(textStatus);
        console.log(errorThrown);
        console.log(jqXHR);
    })
});

// Coded by James
// Method to handle click event for territories
$('#.territory').click(function () {
    var gameID = $("#gameID").val;

    // GET request to game controllers GetGame(int id) for retrieving current game state
    $.ajax({
        method: 'GET',
        url: '', // TODO: Add URL to call
        headers: {
            "Authorization": "Bearer " + localStorage.accessToken
        }
    }).done(function (data) {
        var gameState = JSON.parse(data.GameState1);
        var currentTurnID = gameState.CurrentTurn;
        var playerID = 0; // TODO: Need to add player ID. Unsure how to get players ID here
        var playerName = ""; // TODO: Need to add players name.

        // Current players turn, process territory info & possibly start battle
        if (currentTurnID == playerID) {
            // Get JSON of all territory connections
            $(function () {
                $.ajax({
                    type: 'GET',
                    url: "http://localhost:8000/Map1Connections.json",
                    dataType: 'json'
                }).done(function (data) {
                    var connections = data;
                    var territoryID = $(this).attr('id');
                    var territoryOwner = gameState.Territories[territoryID].Owner;

                    switch (gameState.Phase) {
                        case 0: // Setup Phase
                            // If territory is neutral, make API call update to place leader
                            if (territoryOwner == null && gameState.Players[playerName].LeaderLocation == -1) {
                                // Create move JSON
                                var move = {
                                    "HowMany": -1,
                                    "From": -1,
                                    "To": territoryID
                                };

                                // POST request to game controllers Update(int id, [FromBody] string requestedTurn)
                                $.ajax({
                                    method: 'POST',
                                    url: '', // TODO: Add URL to AJAX call
                                    data: JSON.stringify(move),
                                    contentType: 'application/json',
                                    headers: {
                                        "Authorization": "Bearer " + localStorage.accessToken
                                    }
                                }).done(function () {
                                    // TODO: done? unsure if needed
                                }).fail(function (jqXHR, textStatus, errorThrown) {
                                    console.log(jqXHR);
                                }).always(function () {
                                    $("body").css("cursor", "auto");
                                });
                            } else { // Territory is already owned
                                // TODO: pop up saying unable to place leader or territory already owned?
                            }
                            break;
                        case 1: // Allocation Phase
                            // Player is buying leader
                            if ($("#buyLeader").val == 1) {
                                if (territoryOwner == playerName &&
                                    gameState.Players[playerName].LeaderLocation == -1 &&
                                    $(this).classList.contains('hightlighted')) {
                                    CompleteLeaderPurchase();

                                    // Remove highlighted and blackedOut territories
                                    for (var territory in gameState.Territories) {
                                        if (territory.value.Owner == playerName) {
                                            $('#' + territory.key).classList.remove('highlighted');
                                        } else {
                                            $('#' + territory.key).classList.remove('blackedOut');
                                        }
                                    }
                                }
                            } else { // Not buying leader
                                // Load territory information modal
                                $('#territoryModal').on('show.bs.modal', function () {
                                    ShowTerritoryInformation(territoryID);
                                });
                                $('#territoryModal').modal('show');                             
                            }
                            break;
                        case 2: // Attack Phase
                            // Territory is 'selected', 'highlighted', 'blackout', or none of those
                            if ($(this).classList.contains('selected')) {
                                // Undo coloring (selected/highlighted/blackedOut) of each territory accordingly
                                for (var territory in gameState.Territories) {
                                    if (territory.key == territoryID) {
                                        $(this).classList.remove('selected');
                                    } else if (territory.value.Owner != playerName && connections[territoryID].contains(territory.key)) {
                                        $('#' + territory.key).classList.remove('highlighted');
                                    } else {
                                        $('#' + territory.key).classList.remove('blackedOut');
                                    }
                                }
                            } else if ($(this).classList.contains('highlighted')) {
                                $('#toID').val = territoryID;

                                // Undo coloring (selected/highlighted/blackedOut) of each territory accordingly
                                for (var territory in gameState.Territories) {
                                    if (territory.key == territoryID) {
                                        $(this).classList.remove('selected');
                                    } else if (territory.value.Owner != playerName && connections[territoryID].contains(territory.key)) {
                                        $('#' + territory.key).classList.remove('highlighted');
                                    } else {
                                        $('#' + territory.key).classList.remove('blackedOut');
                                    }
                                }

                                // Load commit modal
                                $('#commitModal').on('show.bs.modal', function () {
                                    PopulateCommitModal();
                                });
                                $('#commitModal').modal('show');
                            } else if ($(this).classList.contains('blackedOut')) {
                                // Load territory information modal
                                $('#territoryModal').on('show.bs.modal', function () {
                                    ShowTerritoryInformation(territoryID);
                                });
                                $('#territoryModal').modal('show');
                            } else {
                                $('#fromID').val = territoryID;

                                // Color (selected/highlighted/blackedOut) each territory accordingly
                                for (var territory in gameState.Territories) {
                                    if (territory.key == territoryID) {
                                        $(this).classList.add('selected');
                                    } else if (territory.value.Owner != playerName && connections[territoryID].contains(territory.key)) {
                                        $('#' + territory.key).classList.add('highlighted');
                                    } else {
                                        $('#' + territory.key).classList.add('blackedOut');
                                    }
                                }
                            }
                            break;
                        case 3: // Move Phase
                            // Territory is 'selected', 'highlighted', 'blackedout', or none of those
                            if ($(this).classList.contains('selected')) {
                                // Undo coloring (selected/highlighted/blackedOut) of each territory accordingly
                                for (var territory in gameState.Territories) {
                                    if (territory.key == territoryID) {
                                        $(this).classList.remove('selected');
                                    } else if (territory.value.Owner == playerName && connections[territoryID].contains(territory.key)) {
                                        $('#' + territory.key).classList.remove('highlighted');
                                    } else {
                                        $('#' + territory.key).classList.remove('blackedOut');
                                    }
                                }
                            } else if ($(this).classList.contains('highlighted')) {
                                $('#toID').val = territoryID;

                                // Undo coloring (selected/highlighted/blackedOut) of each territory accordingly
                                for (var territory in gameState.Territories) {
                                    if (territory.key == territoryID) {
                                        $(this).classList.remove('selected');
                                    } else if (territory.value.Owner == playerName && connections[territoryID].contains(territory.key)) {
                                        $('#' + territory.key).classList.remove('highlighted');
                                    } else {
                                        $('#' + territory.key).classList.remove('blackedOut');
                                    }
                                }

                                // Load move modal
                                $('#moveModal').on('show.bs.modal', function () {
                                    PopulateMoveModal();
                                });
                                $('#moveModal').modal('show');
                            } else if ($(this).classList.contains('blackedOut')) {
                                // Load territory information modal
                                $('#territoryModal').on('show.bs.modal', function () {
                                    ShowTerritoryInformation(territoryID);
                                });
                                $('#territoryModal').modal('show');
                            } else {
                                $('#fromID').val = territoryID;

                                // Color (selected/highlighted/blackedOut) each territory accordingly
                                for (var territory in gameState.Territories) {
                                    if (territory.key == territoryID) {
                                        $(this).classList.add('selected');
                                    } else if (territory.value.Owner == playerName && connections[territoryID].contains(territory.key)) {
                                        $('#' + territory.key).classList.add('highlighted');
                                    } else {
                                        $('#' + territory.key).classList.add('blackedOut');
                                    }
                                }
                            }
                            break;
                        default: // Do nothing?
                            break;
                    }
                }).fail(function (jqXHR, textStatus, errorThrown) {
                    console.log(textStatus);
                    console.log(errorThrown);
                    console.log(jqXHR);
                })
            });
        } else { // Not players turn
            // Load territory information modal
            $('#territoryModal').on('show.bs.modal', function () {
                ShowTerritoryInformation(territoryID);
            });
            $('#territoryModal').modal('show');
        }
    }).fail(function (jqXHR, textStatus, errorThrown) {
        console.log(jqXHR);
    }).always(function () {
        $("body").css("cursor", "auto");
    });
});

// James
// Populates territory information modal
function ShowTerritoryInformation(id) {
    var gameID = $("#gameID").val;

    // GET request to game controllers GetGame(int id)
    $.ajax({
        method: 'GET',
        url: '', // TODO: Add URL to call
        headers: {
            "Authorization": "Bearer " + localStorage.accessToken
        }
    }).done(function (data) {
        var gameState = JSON.parse(data.GameState1);

        // Modal content
        $('#territoryIDModalHeader').innerHTML = "Territory ID: " + id;
        $('#territoryOwnerBody').innerHTML = "Territory Owner: " + gameState.Territories[id].Owner;
        $('#territoryTroopCountBody').innerHTML = "Territory Troops: " + gameState.Territories[id].ForceCount;
        $('#territoryPowerValueBody').innerHTML = "Territory Value: " + gameState.Territories[id].PowerValue;
    }).fail(function (jqXHR, textStatus, errorThrown) {
        console.log(jqXHR);
    }).always(function () {
        $("body").css("cursor", "auto");
    });
}

// James
// Sets buyLeader to 1 indicating player is purchasing a leader and highlight each territory allowed to place leader
function BuyLeader() {
    $("#buyLeader").val = 1;
    var gameID = $("#gameID").val;

    // GET request to game controllers GetGame(int id)
    $.ajax({
        method: 'GET',
        url: '', // TODO: Add URL to call
        headers: {
            "Authorization": "Bearer " + localStorage.accessToken
        }
    }).done(function (data) {
        var gameState = JSON.parse(data.GameState1);
        var playerName = ""; // TODO: Add current players name somewhow

        for (var territory in gameState.Territories) {
            if (territory.value.Owner == playerName) {
                $('#' + territory.key).classList.add('highlighted');
            } else {
                $('#' + territory.key).classList.add('blackedOut');
            }
        }
    }).fail(function (jqXHR, textStatus, errorThrown) {
        console.log(jqXHR);
    }).always(function () {
        $("body").css("cursor", "auto");
    });
}

// James
// Opens modal and populates it with costs associated with players leader
function ShowBuyLeaderModal() {
    var gameID = $("#gameID").val;
    var toTerritoryID = $("#toID").val;
    // GET request to game controllers GetGame(int id)
    $.ajax({
        method: 'GET',
        url: '', // TODO: Add URL to call
        headers: {
            "Authorization": "Bearer " + localStorage.accessToken
        }
    }).done(function (data) {
        var gameState = JSON.parse(data.GameState1);
        var playerName = ""; // TODO: Add current players name somewhow
        var currentPower = gameState.Players[playerName].PowerTotal;
        var necroCost = gameState.Players[playerName].LeaderCost;

        // Modal content
        $("#buyLeaderModalHeader").innerHTML = "Purchase Necromancer.";
        $("#buyLeaderPowerBody").innerHTML = "You currently have " + currentPower + " power.";
        $("#buyLeaderCostBody").innerHTML = "The cost to purchase a necromancer is " + necroCost + " power.";
        $("#buyLeaderInstructionBody").innerHTML = "To puchase a necromancer, click on the purchase button then click on a territory that you own.";
        $("#insufficientPower").innerHTML = "You do not have enough power to purchase a necromancer.";

        // Conditional modal content
        if (currentPower < necroCost) {
            $("#buyLeaderInstructionBody").style.visibility = 'hidden';
            $("#insufficientPower").style.visibility = 'visible';
            $("#purchaseLeaderBtn").disabled = true;
        } else {
            $("#buyLeaderInstructionBody").style.visibility = 'visible';
            $("#insufficientPower").style.visibility = 'hidden';
            $("#purchaseLeaderBtn").disabled = false;
        }
    }).fail(function (jqXHR, textStatus, errorThrown) {
        console.log(jqXHR);
    }).always(function () {
        $("body").css("cursor", "auto");
    });
}

// James
// Handles values and calls to purcahse leader
function CompleteLeaderPurchase() {
    var gameID = $("#gameID").val;
    // Create move JSON
    var move = {
        "HowMany": -1,
        "From": -1,
        "To": $("#toID").val
    };
    // POST request to game controllers Update(int id, [FromBody] string requestedTurn)
    $.ajax({
        method: 'POST',
        url: '', // TODO: Add URL to AJAX call
        data: JSON.stringify(move),
        contentType: 'application/json',
        headers: {
            "Authorization": "Bearer " + localStorage.accessToken
        }
    }).done(function () {
        $("#buyLeader").val = -1;
    }).fail(function (jqXHR, textStatus, errorThrown) {
        console.log(jqXHR);
    }).always(function () {
        $("body").css("cursor", "auto");
    });
}

// James
// Opens modal and populates it with costs associated with players troops
function ShowBuyTroopsModal() {
    var gameID = $("#gameID").val;

    // GET request to game controllers GetGame(int id)
    $.ajax({
        method: 'GET',
        url: '', // TODO: Add URL to call
        headers: {
            "Authorization": "Bearer " + localStorage.accessToken
        }
    }).done(function (data) {
        var gameState = JSON.parse(data.GameState1);
        var playerName = ""; // TODO: Add current players name somewhow
        var currentPower = gameState.Players[playerName].PowerTotal;
        // TODO: PerilGameState does not contain troop cost amount, only PerilLogic. Currently at 3
        var troopsCost = 3;
        var totalTroopsAllowed = Math.floor(currentPower / troopsCost);
        var results = "";

        $("#confirmPurchaseTroopsBtn").disabled = true;
        $("#purchaseResults").style.visibility = 'hidden';
        $("#buyTroopsField").max = totalTroopsAllowed;

        // Modal content
        $("#buyTroopsModalHeader").innerHTML = "Purchase Troops.";
        $("#buyTroopsPowerBody").innerHTML = "You have " + currentPower + " power available to spend on troops.";
        $("#buyTroopsCostBody").innerHTML = "The cost for each troop is " + troopsCost + " power.";

        // Conditional modal content
        if (currentPower < troopsCost) {
            $("#buyTroopsInstructionBody").innerHTML = "You do not have enough power to purchase any troops.";
            $("#purchaseTroopsBtn").disabled = true;
            $("#buyTroopsForm").style.visibility = 'hidden';
        } else {
            $("#buyTroopsInstructionBody").innerHTML = "Enter the number of troops you wish to purchase between 1 and " + totalTroopsAllowed +
                ", then click the purchase button. You will be given a chance to change your selection or confirm the entered amount. The troops will " +
                "automatically be added to the territory that contains your necromancer.";
            $("#purchaseTroopsBtn").disabled = false;
            $("#buyTroopsForm").style.visibility = 'visible';
        }
    }).fail(function (jqXHR, textStatus, errorThrown) {
        console.log(jqXHR);
    }).always(function () {
        $("body").css("cursor", "auto");
    });
}

// James
// Validates players troop purchase selection then makes API call to change amounts
function AddPurchasedTroops() {
    var gameID = $("#gameID").val;
    // GET request to game controllers GetGame(int id)
    $.ajax({
        method: 'GET',
        url: '', // TODO: Add URL to call
        headers: {
            "Authorization": "Bearer " + localStorage.accessToken
        }
    }).done(function (data) {
        var gameState = JSON.parse(data.GameState1);
        var playerName = ""; // TODO: Add players name
        var leaderLocation = gameState.Players[playerName].LeaderLocation;
        var attemptedPurchase = $("#buyTroopsField").value;
        var result = "";
        var maxAllowed = $("#buyTroopsField").max;

        // Valid user input
        if (attemptedPurchase > 0 && attemptedPurchase <= maxAllowed) {
            $("#confirmPurchaseTroopsBtn").disabled = false;
            $("#purchaseResults").style.visibility = 'visible';

            // Create move JSON
            var move = {
                "HowMany": attemptedPurchase, // Only how many matters here
                "From": -1,
                "To": -1
            };

            // POST request to game controllers Update(int id, [FromBody] string requestedTurn)
            $.ajax({
                method: 'POST',
                url: '', // TODO: Add URL to AJAX call
                data: JSON.stringify(move),
                contentType: 'application/json',
                headers: {
                    "Authorization": "Bearer " + localStorage.accessToken
                }
            }).done(function () {
                result = attemptedPurchase + " troops have been added."
            }).fail(function (jqXHR, textStatus, errorThrown) {
                console.log(jqXHR);
            }).always(function () {
                $("body").css("cursor", "auto");
            });
        } else { //Invalid user input
            $("#confirmPurchaseTroopsBtn").disabled = true;
            result = "Invalid entry. Please enter a number between 1 and " + maxAllowed + ".";
        }
    }
}

// Coded by James
// Fills out all of the vars in the modal that allows the user to choose how many troops to move
function PopulateMoveModal() {
    // GET request to game controllers GetGame(int id)
    $.ajax({
        method: 'GET',
        url: '', // TODO: Add URL to call
        headers: {
            "Authorization": "Bearer " + localStorage.accessToken
        }
    }).done(function (data) {
        var gameID = $("#gameID").val;
        var toTerritoryID = $("#toID").val;
        var fromTerritoryID = $("#fromID").val;
        var gameState = JSON.parse(data.GameState1);
        var moveableTroops = gameState.Territories[fromTerritoryID].Moveable;

        // Set max value in form to the players max allowable troops to move
        $("#moveTroopsField").max = moveableTroops;

        // Display content in page
        $("#moveModalHeader").innerHTML = "You are planning to move troops from " + fromTerritoryID + " to " + toTerritoryID + ".";
        $("#moveModalTroopsBody").innerHTML = "You are allowed to move up to " + moveableTroops + ".";
        $("#moveModalLeaderBody").innerHTML = "You are allowed to move your leader.";
        $("#insufficientTroopCount").innerHTML = "You do not have enough troops to move from this territory."

        // Handle leader at 'from' location
        var playerName = gameState.Territories[fromTerritoryID].Owner;
        if (gameState.Players[playerName].LeaderLocation == fromTerritoryID && !gameState.Players[playerName].LeaderMove &&
            gameState.Territories[toTerritoryID].Owner == playerName) {
            $("#moveModalLeaderBody").style.visibility = 'visible';
            $("#moveLeaderBtn").style.visibility = 'visible';
        } else {
            $("#moveModalLeaderBody").style.visibility = 'hidden';
            $("#moveLeaderBtn").style.visibility = 'hidden';
        }

        // Handle insufficient troop amount in modal
        if (moveableTroops < 1) {
            $("#moveTroopsForm").style.visibility = 'hidden';
            $("#insufficientTroopCount").style.visibility = 'visible';
            $("#moveTroopsBtn").disabled = true;
        } else {
            $("#moveTroopsForm").style.visibility = 'visible';
            $("#insufficientTroopCount").style.visibility = 'hidden';
            $("#moveTroopsBtn").disabled = false;
        }
    }).fail(function (jqXHR, textStatus, errorThrown) {
        console.log(jqXHR);
    }).always(function () {
        $("body").css("cursor", "auto");
    });
}

// James
// Makes API call to handle moving leader
function MoveLeader() {
    var gameID = $("#gameID").val;

    // Create move JSON
    var move = {
        "HowMany": -1,
        "From": $("#fromID").val,
        "To": $("#toID").val
    };

    // POST request to game controllers Update(int id, [FromBody] string requestedTurn)
    $.ajax({
        method: 'POST',
        url: '', // TODO: Add URL to AJAX call
        data: JSON.stringify(move),
        contentType: 'application/json',
        headers: {
            "Authorization": "Bearer " + localStorage.accessToken
        }
    }).done(function () {
        // TODO: Anything?
    }).fail(function (jqXHR, textStatus, errorThrown) {
        console.log(jqXHR);
    }).always(function () {
        $("body").css("cursor", "auto");
    });
}

// James
// Makes API call to handle troop movements
function MoveTroops() {
    var gameID = $("#gameID").val;
    var requestedMove = $("#moveTroopsField").value;

    // GET request to game controllers GetGame(int id)
    $.ajax({
        method: 'GET',
        url: '', // TODO: Add URL to call
        headers: {
            "Authorization": "Bearer " + localStorage.accessToken
        }
    }).done(function (data) {
        var gameState = JSON.parse(data.GameState1);
        var allowableMove = gameState.Territories[$("#fromID").val].Moveable;
        var playerName = ""; // TODO: Add players name

        // Valid user input
        if (requestedMove > 0 && requestedMove <= allowableMove) {
            // Create move JSON
            var move = {
                "HowMany": requestedMove,
                "From": $("#fromID").val,
                "To": $("#toID").val
            };

            // POST request to game controllers Update(int id, [FromBody] string requestedTurn)
            $.ajax({
                method: 'POST',
                url: '', // TODO: Add URL to AJAX call
                data: JSON.stringify(move),
                contentType: 'application/json',
                headers: {
                    "Authorization": "Bearer " + localStorage.accessToken
                }
            }).done(function () {
                // TODO: Anything?
            }).fail(function (jqXHR, textStatus, errorThrown) {
                console.log(jqXHR);
            }).always(function () {
                $("body").css("cursor", "auto");
            });
        }
    }
}

// James
// Auto resize for modals
$(".modal").on("show.bs.modal", function () {
    $(this).find(".modal-body").css({
        width: 'auto',
        height: 'auto',
        'max-height': '100%'
    });
});

// James
// Fills out all of the vars in the modal that allows the user to choose how many to commit to battle
function PopulateCommitModal() {
    // GET request to game controllers GetGame(int id)
    $.ajax({
        method: 'GET',
        url: '', // TODO: Add URL to call
        headers: {
            "Authorization": "Bearer " + localStorage.accessToken
        }
    }).done(function (data) {
        var gameID = $("#gameID").val;
        var toTerritoryID = $("#toID").val;
        var fromTerritoryID = $("#fromID").val;
        var gameState = JSON.parse(data.GameState1);
        var attackerName = gameState.TerritoryList[fromTerritoryID].Owner;
        var defenderName = gameState.TerritoryList[toTerritoryID].Owner;
        var totalAttackerForce = gameState.TerritoryList[fromTerritoryID].ForceCount - 1;
        var totalDefenderForce = gameState.TerritoryList[toTerritoryID].ForceCount;

        $("#OpenAttackBtn").disabled = true;

        // Set max value in form to the attackers starting total available troops
        $("#commitTroopField").max = totalAttackerForce;

        // Display content in page
        $("#commitHeader").innerHTML = attackerName + " is planning to attack " + defenderName + ".";
        $("#commitAttackerTroops").innerHTML = attackerName + " is allowed up to " + totalAttackerForce + " troops to attack.";
        $("#commitDefenderTroops").innerHTML = defenderName + " has " + totalDefenderForce + " troops to defend.";
        $("#insufficientTroops").innerHTML = "You do not have sufficient troops to attack."

        // Handle insufficient attacker troop amount in modal
        if (totalAttackerForce < 1) {
            $("#commitForm").style.visibility = 'hidden';
            $("#insufficientTroops").style.visibility = 'visible';
            $("#commitBtn").disabled = true;
        } else {
            $("#commitForm").style.visibility = 'visible';
            $("#insufficientTroops").style.visibility = 'hidden';
            $("#commitBtn").disabled = false;
        }
    }).fail(function (jqXHR, textStatus, errorThrown) {
        console.log(jqXHR);
    }).always(function () {
        $("body").css("cursor", "auto");
    });
}

// Coded by James
// Determines if value entered in form is valid and outputs a message accordingly
function ValidateTroopCount() {
    // GET request to game controllers GetGame(int id)
    $.ajax({
        method: 'GET',
        url: '', // TODO: Add URL to call
        headers: {
            "Authorization": "Bearer " + localStorage.accessToken
        }
    }).done(function (data) {
        var gameID = $("#gameID").val;
        var toTerritoryID = $("#toID").val;
        var fromTerritoryID = $("#fromID").val;
        var gameState = JSON.parse(data.GameState1);
        var totalAttackerForce = gameState.TerritoryList[fromTerritoryID].ForceCount - 1;
        var commitMessage = "";
        committedAttackTroops = $("#commitTroopField").value;

        // Handle validation of user chosen amount of troops committed
        if (committedAttackTroops > 0 && committedAttackTroops <= totalAttackerForce) {
            $("#OpenAttackBtn").disabled = false;
            commitMessage = committedAttackTroops + "/" + totalAttackerForce +
                " troops have been committed. Click \"Begin Attack\" to enter battle or change the number of troops to commit by reentering a new value and clicking \"Commit\".";

            // Create move JSON
            var move = {
                "HowMany": committedAttackTroops,
                "From": fromTerritoryID,
                "To": toTerritoryID
            };

            // POST request to game controllers Update(int id, [FromBody] string requestedTurn)
            $.ajax({
                method: 'POST',
                url: '', // TODO: Add URL to AJAX call
                data: JSON.stringify(move),
                contentType: 'application/json',
                headers: {
                    "Authorization": "Bearer " + localStorage.accessToken
                }
            }).done(function () {
                // TODO: done
            }).fail(function (jqXHR, textStatus, errorThrown) {
                console.log(jqXHR);
            }).always(function () {
                $("body").css("cursor", "auto");
            });
        } else {
            commitMessage = "Invalid entry. Please enter a number between 1 and " + totalAttackerForce;
        }
        $("#troopEntry").innerHTML = commitMessage;
    }).fail(function (jqXHR, textStatus, errorThrown) {
        console.log(jqXHR);
    }).always(function () {
        $("body").css("cursor", "auto");
    });
}

// Coded by James
// Makes the calls needed to populate the active battle modal
function InitBattleValues() {
    // Hide and disable buttons until needed
    $("#battleResultsBtn").style.visibility = 'hidden';
    $("#battleViewAttackBtn").disabled = false;
    $("#battleViewRetreatBtn").disabled = false;
    UpdateBattlePage();
    SetBattleImages();
}

// Coded by James
// Update troop counts on webpage
function UpdateBattlePage() {
    // GET request to game controllers GetGame(int id)
    $.ajax({
        method: 'GET',
        url: '', // TODO: Add URL to call
        headers: {
            "Authorization": "Bearer " + localStorage.accessToken
        }
    }).done(function (data) {
        var gameID = $("#gameID").val;
        var toTerritoryID = $("#toID").val;
        var fromTerritoryID = $("#fromID").val;
        var gameState = JSON.parse(data.GameState1);
        var attackerName = gameState.TerritoryList[fromTerritoryID].Owner;
        var defenderName = gameState.TerritoryList[toTerritoryID].Owner;
        var committedAttackerForce = gameState.ActiveBattle.Committed;
        var totalDefenderForce = gameState.TerritoryList[toTerritoryID].ForceCount;
        var attackersLost = gameState.ActiveBattle.AttackersLost;
        var defendersLost = gameState.ActiveBattle.DefendersLost;

        $("#attackVsDefend").innerHTML = attackerName + " is attacking " + defenderName + "!";
        $("#committedForces").innerHTML = "Started battle with " + committedAttackerForce + " troops";
        $("#defendingForces").innerHTML = "Started battle with " + totalDefenderForce + " troops";

        var remainingAttackers = gameState.ActiveBattle.RemainingAttackers;
        var remainingDefenders = gameState.ActiveBattle.RemainingDefenders;

        $("#attackTroopsLost").innerHTML = "Attacker has lost " + attackersLost + " troops";
        $("#defendTroopsLost").innerHTML = "Defender has lost " + defendersLost + " troops";
        $("#attackTroopsRemain").innerHTML = "Attacker has " + remainingAttackers + " troops remaining";
        $("#defendTroopsRemain").innerHTML = "Defender has " + remainingDefenders + " troops remaining";
    }).fail(function (jqXHR, textStatus, errorThrown) {
        console.log(jqXHR);
    }).always(function () {
        $("body").css("cursor", "auto");
    });
}

// James
// Sets correct battle images to display on battle modal
function SetBattleImages() {
    // GET request to game controllers GetGame(int id)
    $.ajax({
        method: 'GET',
        url: '', // TODO: Add URL to call
        headers: {
            "Authorization": "Bearer " + localStorage.accessToken
        }
    }).done(function (data) {
        var gameID = $("#gameID").val;
        var gameState = JSON.parse(data.GameState1);

        // Remove all images from webpage except 1 of each for attacker and defender
        $("#attack1Img").style.visibility = 'visible';
        $("#attack2Img").style.visibility = 'hidden';
        $("#attack3Img").style.visibility = 'hidden';
        $("#defend1Img").style.visibility = 'visible';
        $("#defend2Img").style.visibility = 'hidden';
        $("#xAttack1Img").style.visibility = 'hidden';
        $("#xAttack2Img").style.visibility = 'hidden';
        $("#xAttack3Img").style.visibility = 'hidden';
        $("#xDefend1Img").style.visibility = 'hidden';
        $("#xDefend2Img").style.visibility = 'hidden';

        var remainingAttackers = gameState.ActiveBattle.RemainingAttackers;
        var remainingDefenders = gameState.ActiveBattle.RemainingDefenders;

        // Add correct number of attacker images
        if (remainingAttackers == 2) {
            $("#attack2Img").style.visibility = 'visible';
        }

        if (remainingAttackers >= 3) {
            $("#attack2Img").style.visibility = 'visible';
            $("#attack3Img").style.visibility = 'visible';
        }

        // Add correct number of defending images
        if (remainingDefenders > 1) {
            $("#defend2Img").style.visibility = 'visible';
        }
    }).fail(function (jqXHR, textStatus, errorThrown) {
        console.log(jqXHR);
    }).always(function () {
        $("body").css("cursor", "auto");
    });
}

// Coded by James
// Handles calls to server side game logic to set and get values
function HandleAttacks() {
    // GET request to game controllers GetGame(int id)
    $.ajax({
        method: 'GET',
        url: '', // TODO: Add URL to call
        headers: {
            "Authorization": "Bearer " + localStorage.accessToken
        }
    }).done(function (data) {
        var gameID = $("#gameID").val;
        var toTerritoryID = $("#toID").val;
        var fromTerritoryID = $("#fromID").val;
        var gameState = JSON.parse(data.GameState1);

        SetBattleImages();

        var attCount = gameState.ActiveBattle.RemainingAttackers >= 3 ? 3 : gameState.ActiveBattle.RemainingAttackers;
        var defCount = gameState.ActiveBattle.RemainingDefenders >= 2 ? 2 : gameState.ActiveBattle.RemainingDefenders;
        var originalAttLost = gameState.ActiveBattle.AttackersLost;
        var originalDefLost = gameState.ActiveBattle.RemainingDefenders;

        // Create move JSON
        var move = {
            "HowMany": gameState.ActiveBattle.RemainingAttackers,
            "From": fromTerritoryID,
            "To": toTerritoryID
        };

        // POST request to game controllers Update(int id, [FromBody] string requestedTurn)
        $.ajax({
            method: 'POST',
            url: '', // TODO: Add URL to AJAX call
            contentType: 'application/json',
            data: JSON.stringify(move),
            headers: {
                "Authorization": "Bearer " + localStorage.accessToken
            }
        }).done(function () {
            // TODO: done
        }).fail(function (jqXHR, textStatus, errorThrown) {
            console.log(jqXHR);
        }).always(function () {
            $("body").css("cursor", "auto");
        });

        var newAttLost = gameState.ActiveBattle.AttackersLost;
        var newDefLost = gameState.ActiveBattle.RemainingDefenders;
        var attLost = newAttLost - originalAttLost;
        var defLost = newDefLost - originalDefLost;
        SetXdOutTroopImages(attCount, attLost, defCount, defLost);
        UpdateBattlePage();

        if (gameState.BattleResult != null) {
            $("#battleResultsBtn").style.visibility = 'visible';
            $("#battleViewAttackBtn").disabled = true;
            $("#battleViewRetreatBtn").disabled = true;
        }
    }).fail(function (jqXHR, textStatus, errorThrown) {
        console.log(jqXHR);
    }).always(function () {
        $("body").css("cursor", "auto");
    });
}

// Coded by James
// Handles which battle images to cross out based on how many were involved in last battle round and how many of each were lost
function SetXdOutTroopImages(attackedCount, attackLostRound, defendedCount, defendLostRound) {
    // Handles attacker losses
    switch (attackedCount) {
        case 3:
            if (attackLostRound == 2) {
                $("#xAttack2Img").style.visibility = 'visible';
                $("#xAttack3Img").style.visibility = 'visible';
            }

            if (attackLostRound == 1) {
                $("#xAttack3Img").style.visibility = 'visible';
            }
            break;
        case 2:
            if (attackLostRound == 2) {
                $("#xAttack1Img").style.visibility = 'visible';
                $("#xAttack2Img").style.visibility = 'visible';
            }

            if (attackLostRound == 1) {
                $("#xAttack2Img").style.visibility = 'visible';
            }
            break;
        default:
            if (attackLostRound == 1) {
                $("#xAttack1Img").style.visibility = 'visible';
            }
            break;
    }

    // Handles defender losses
    switch (defendedCount) {
        case 2:
            if (defendLostRound == 2) {
                $("#xDefend1Img").style.visibility = 'visible';
                $("#xDefend2Img").style.visibility = 'visible';
            }

            if (defendLostRound == 1) {
                $("#xDefend2Img").style.visibility = 'visible';
            }
            break;
        default:
            if (defendLostRound == 1) {
                $("#xDefend1Img").style.visibility = 'visible';
            }
            break;
    }
}

// Coded by James
// Makes API call to Update to handle attacker retreating from battle
function BattleRetreat() {
    var gameID = $("#gameID").val;
    var toTerritoryID = $("#toID").val;
    var fromTerritoryID = $("#fromID").val;

    // Create move JSON
    var move = {
        "HowMany": -3,
        "From": fromTerritoryID,
        "To": toTerritoryID
    };

    // POST request to game controllers Update(int id, [FromBody] string requestedTurn)
    $.ajax({
        method: 'POST',
        url: '', // TODO: Add URL to AJAX call
        contentType: 'application/json',
        data: JSON.stringify(move),
        headers: {
            "Authorization": "Bearer " + localStorage.accessToken
        }
    }).done(function () {
        // TODO: done
    }).fail(function (jqXHR, textStatus, errorThrown) {
        console.log(jqXHR);
    }).always(function () {
        $("body").css("cursor", "auto");
    });

    BattleResults();
}

// Coded by James
// Populate the results modal for the victory/results of the battle outcome
function BattleResults() {
    // GET request to game controllers GetGame(int id)
    $.ajax({
        method: 'GET',
        url: '', // TODO: Add URL to call
        headers: {
            "Authorization": "Bearer " + localStorage.accessToken
        }
    }).done(function (data) {
        var gameID = $("#gameID").val;
        var toTerritoryID = $("#toID").val;
        var fromTerritoryID = $("#fromID").val;
        var gameState = JSON.parse(data.GameState1);

        // vars from gameState JSON
        var attackerName = gameState.BattleResult.Attacker;
        var defenderName = gameState.BattleResult.Defender;
        var attackerLosses = gameState.BattleResult.AttLost;
        var defenderLosses = gameState.BattleResult.DefLost;
        var revived = gameState.BattleResult.NumRevived;
        var victor = "";

        if (gameState.BattleResult.AttackerWon) {
            victor = attackerName;
        } else {
            victor = defenderName;
        }

        $("#batleRsltHeader").innerHTML = victor + " has won the battle.";
        $("#batRsltAttacker").innerHTML = attackerName + " has lost a total of " + attackerLosses + " troops.";
        $("#batRsltDefender").innerHTML = defenderName + " has lost a total of " + defenderLosses + " troops.";
        $("#batRsltRes").innerHTML = victor + "'s necromancer has successfully risen " + revived + " fallen troops.";
    }).fail(function (jqXHR, textStatus, errorThrown) {
        console.log(jqXHR);
    }).always(function () {
        $("body").css("cursor", "auto");
    });
}