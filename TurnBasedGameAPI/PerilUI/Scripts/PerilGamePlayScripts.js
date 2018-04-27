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
$(function () {
    $('#map-matrix').children('path').filter('.territory').on('mouseenter mouseleave', function () {
        var terrId = $(this).attr('id')

        console.log('Territory Number: ' + terrId);
        $(this).toggleClass('highlighted');

        var i;
        for (i = 0; i < connections[parseInt(terrId)].length; i++) {
            $('#' + connections[parseInt(terrId)][i]).toggleClass('moveable');
        }
    });
});

$(function () {
    $('#map-matrix').children('path').filter('.connection').on('click', function () {
        console.log('Path: ' + $(this).attr('id'));
    })
});

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
// ************************************************************************* TODO: Finish Method***************************************************************************************
$('#.territory').click(function () {
    var gameID = document.getElementById("gameID");

    /* 
        Logic for territory event handler
        1. Get current game state
        2. Use game state to make all variables needed
        3. Check if players turn
            3.1 If players turn, handle different possiblities depending on phases of game
                3.1.1 Setup phase:
                    - Check if territory is neutral (valid)
                        - If checks pass, make api update call with move JSON
                        - Else pop up saying territory already owned or leader already placed, maybe?
                3.1.2 Allocation phase:
                    - Check if player has leader on the board (player.leaderlocation == -1)
                        - Unsure how we're handling purchases still, so allocation is still TODO
                3.1.3 Attack phase:
                    - Check if clicked territory has movable class
                        - If has movable class, it implies 2nd (to) territory selected
                            - If clicked territory owned by player, unknown how to handle yet
                                (possibilities are show pop up saying already owned, or make this territory 1st selection (from) instead of (to), or do nothing, or ...)
                            - Else clicked territory is not owned by player
                                - Set (to) territory id in html to clicked territory id
                                - Remove highlighted class from 'from' territory
                                - Remove movable class from territories using connections and (from) territory id
                                - Open commit modal to begin the attack
                        - Else does not contain movable class, implies either 1st territory clicked on or a territory clicked outside of allowable connections
                            treat both as 1st territory clicked??
                                - Check if territory owned by player
                                    - If owned by player
                                        - Set (from) territory id in html to clicked territory id
                                        - Add highlighted class to territory
                                        - Add movable class to each connecting territories
                3.1.4 Move phase:
                    - Check if owned by player
                        - If owned by player
                            - Check if has movable class
                                - If movable class, it implies 2nd (to) territory selected
                                    - Set (to) territory id in html to clicked territory id
                                    - Remove highlighted class from 'from' territory
                                    - Remove movable class from connecting territories
                                    - Open move modal to determine how many troops to move
                                - Else does not contain movable class
                                    - Set (from) territory id in html to clicked territory id
                                    - Add highlighted class to territory
                                    - Add moveable class to connecting territories
                        - Else not owned by player either do nothing or show pop up of territory info or show pop up saying territory not owned by player ???                 
            3.2 If not players turn show pop up of territory info ??

    */

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
                            // TODO: Add code to allocation phase accounting for leader placement if needed
                            break;
                        case 2: // Attack Phase
                            // Movable so 1st territory is already selected, making this the 2nd (to) territory selected
                            if ($(this).classList.contains('moveable')) {
                                // Territory owned by player
                                if (territoryOwner == playerName) {
                                    // TODO: Unknown how to handle yet, i.e. pop up saying already owned or make this 1st selection (from) instead of (to),...
                                } else { // Territory not owned by player
                                    document.getElementById("toID").innerHTML = territoryID;
                                    var fromID = document.getElementById("fromID");
                                    $('#' + fromID).classList.remove('highlighted');
                                    var i;
                                    for (i = 0; i < connections[fromID].length; i++) {
                                        $('#' + connections[fromID][i]).classList.remove('moveable');
                                    }
                                    // TODO: to and from IDs are both set in html only need to call/open commit modal
                                }
                            } else { // Not moveable so either it is 1st territory selected or new 1st territory selected outside of allowable range of previous 1st
                                // Owned by player
                                if (territoryOwner == playerName) {
                                    document.getElementById("fromID").innerHTML = territoryID;
                                    $('#' + territoryID).classList.add('highlighted');
                                    var i;
                                    for (i = 0; i < connections[territoryID].length; i++) {
                                        $('#' + connections[territoryID][i]).classList.add('moveable');
                                    }
                                } else { // Not owned by player
                                    // TODO: Add pop up for territory info
                                }
                            }

                            break;
                        case 3: // Move Phase
                            // Owned by player
                            if (territoryOwner == playerName) {
                                // Valid 'to' territory. Set toID to territory id & remove highlighted/moveable classes
                                if ($(this).classList.contains('moveable')) {
                                    document.getElementById("toID").innerHTML = territoryID;
                                    var fromID = document.getElementById("fromID");
                                    $('#' + fromID).classList.remove('highlighted');
                                    var i;
                                    for (i = 0; i < connections[fromID].length; i++) {
                                        $('#' + connections[fromID][i]).classList.remove('moveable');
                                    }

                                    // TODO: Add call to move modal
                                } else { // Not valid 'to' territory, set it as 'from'
                                    document.getElementById("fromID").innerHTML = territoryID;
                                    $('#' + territoryID).classList.add('highlighted');
                                    var i;
                                    for (i = 0; i < connections[territoryID].length; i++) {
                                        $('#' + connections[territoryID][i]).classList.add('moveable');
                                    }
                                }
                            } else { // Not owned by player
                                // TODO: Add pop up for territory info
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
        } else { // Not players turn, show territory info in popup maybe?
            // TODO: Add pop up for territory info
        }
    }).fail(function (jqXHR, textStatus, errorThrown) {
        console.log(jqXHR);
    }).always(function () {
        $("body").css("cursor", "auto");
    });
});

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
        var gameID = document.getElementById("gameID");
        var toTerritoryID = document.getElementById("toID");
        var fromTerritoryID = document.getElementById("fromID");
        var gameState = JSON.parse(data.GameState1);
        var moveableTroops = gameState.Territories[fromTerritoryID].Moveable;
        var moveModalBodyTroopsMoveable = "You are allowed to move up to " + moveableTroops + ".";
        var moveModalLeaderBody = "You are allowed to move your leader.";
        var moveModalBodyInsufficientCount = "You do not have enough troops to move from this territory."

        // Set max value in form to the players max allowable troops to move
        document.getElementById("moveTroopsField").max = moveableTroops;

        // Display content in page
        document.getElementById("moveModalHeader").innerHTML = "You are planning to move troops from " + fromTerritoryID + " to " + toTerritoryID + ".";
        document.getElementById("moveModalTroopsBody").innerHTML = moveModalBodyTroopsMoveable;
        document.getElementById("moveModalLeaderBody").innerHTML = moveModalLeaderBody;
        document.getElementById("insufficientTroopCount").innerHTML = moveModalBodyInsufficientCount;

        // Handle leader at 'from' location
        var playerName = gameState.Territories[fromTerritoryID].Owner;
        if (gameState.Players[playerName].LeaderLocation == fromTerritoryID) {
            document.getElementById("moveModalLeaderBody").style.visibility = 'visible';
            document.getElementById("moveLeaderBtn").style.visibility = 'visible';
        } else {
            document.getElementById("moveModalLeaderBody").style.visibility = 'hidden';
            document.getElementById("moveLeaderBtn").style.visibility = 'hidden';
        }

        // Handle insufficient troop amount in modal
        if (moveableTroops == 0) {
            document.getElementById("moveTroopsForm").style.visibility = 'hidden';
            document.getElementById("insufficientTroopCount").style.visibility = 'visible';
            document.getElementById("moveTroopsBtn").disabled = true;
        } else {
            document.getElementById("moveTroopsForm").style.visibility = 'visible';
            document.getElementById("insufficientTroopCount").style.visibility = 'hidden';
            document.getElementById("moveTroopsBtn").disabled = false;
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

}

// James
// Makes API call to handle troop movements
function MoveTroops() {

}

// ***********************************Coded by James: All scripts needed for battle**********************************************
// Auto resize for modals
$("#.modal").on("show.bs.modal", function () {
    $(this).find(".modal-body").css({
        width: 'auto',
        height: 'auto',
        'max-height': '100%'
    });
});

// Coded by James
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
        var gameID = document.getElementById("gameID");
        var toTerritoryID = document.getElementById("toID");
        var fromTerritoryID = document.getElementById("fromID");
        var gameState = JSON.parse(data.GameState1);
        var attackerName = gameState.TerritoryList[fromTerritoryID].Owner;
        var defenderName = gameState.TerritoryList[toTerritoryID].Owner;
        var totalAttackerForce = gameState.TerritoryList[fromTerritoryID].ForceCount - 1;
        var totalDefenderForce = gameState.TerritoryList[toTerritoryID].ForceCount;

        // Vars to use in modal content
        var commitBodyAttackerTroops = attackerName + " is allowed up to " + totalAttackerForce + " troops to attack.";
        var commitBodyDefenderTroops = defenderName + " has " + totalDefenderForce + " troops to defend.";
        var insuficientTroopCount = "You do not have sufficient troops to attack."
        var showInsufficient = document.getElementById("insufficientTroops");
        document.getElementById("OpenAttackBtn").disabled = true;

        // Set max value in form to the attackers starting total available troops
        var maxTroops = document.getElementById("commitTroopField").max = totalAttackerForce;

        // Display content in page
        document.getElementById("commitHeader").innerHTML = attackerName + " is planning to attack " + defenderName + ".";
        document.getElementById("commitAttackerTroops").innerHTML = commitBodyAttackerTroops;
        document.getElementById("commitDefenderTroops").innerHTML = commitBodyDefenderTroops;
        document.getElementById("insufficientTroops").innerHTML = insuficientTroopCount;

        // Handle insufficient attacker troop amount in modal
        if (totalAttackerForce == 0) {
            var hideForm = document.getElementById("commitForm");
            hideForm.style.visibility = 'hidden';
            showInsufficient.style.visibility = 'visible';
            document.getElementById("commitBtn").disabled = true;
        } else {
            showInsufficient.style.visibility = 'hidden';
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
        var gameID = document.getElementById("gameID");
        var toTerritoryID = document.getElementById("toID");
        var fromTerritoryID = document.getElementById("fromID");
        var gameState = JSON.parse(data.GameState1);
        var totalAttackerForce = gameState.TerritoryList[fromTerritoryID].ForceCount - 1;
        var commitMessage = "";
        committedAttackTroops = document.getElementById("commitTroopField").value;

        // Handle validation of user chosen amount of troops committed
        if (committedAttackTroops > 0 && committedAttackTroops <= totalAttackerForce) {
            document.getElementById("OpenAttackBtn").disabled = false;
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
        document.getElementById("troopEntry").innerHTML = commitMessage;
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
    var hideResultBtn = document.getElementById("battleResultsBtn");
    hideResultBtn.style.visibility = 'hidden';
    document.getElementById("battleViewAttackBtn").disabled = false;
    document.getElementById("battleViewRetreatBtn").disabled = false;
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
        dataType: 'json'
		headers: {
            "Authorization": "Bearer " + localStorage.accessToken
        }
    }).done(function (data) {
        var gameID = document.getElementById("gameID");
        var toTerritoryID = document.getElementById("toID");
        var fromTerritoryID = document.getElementById("fromID");
        var gameState = JSON.parse(data.GameState1);
        var attackerName = gameState.TerritoryList[fromTerritoryID].Owner;
        var defenderName = gameState.TerritoryList[toTerritoryID].Owner;
        var committedAttackerForce = gameState.ActiveBattle.Committed;
        var totalDefenderForce = gameState.TerritoryList[toTerritoryID].ForceCount;
        var attackersLost = gameState.ActiveBattle.AttackersLost;
        var defendersLost = gameState.ActiveBattle.DefendersLost;

        document.getElementById("attackVsDefend").innerHTML = attackerName + " is attacking " + defenderName + "!";
        document.getElementById("committedForces").innerHTML = "Started battle with " + committedAttackerForce + " troops";
        document.getElementById("defendingForces").innerHTML = "Started battle with " + totalDefenderForce + " troops";

        var remainingAttackers = gameState.ActiveBattle.RemainingAttackers;
        var remainingDefenders = gameState.ActiveBattle.RemainingDefenders;

        document.getElementById("attackTroopsLost").innerHTML = "Attacker has lost " + attackersLost + " troops";
        document.getElementById("defendTroopsLost").innerHTML = "Defender has lost " + defendersLost + " troops";
        document.getElementById("attackTroopsRemain").innerHTML = "Attacker has " + remainingAttackers + " troops remaining";
        document.getElementById("defendTroopsRemain").innerHTML = "Defender has " + remainingDefenders + " troops remaining";
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
        dataType: 'json'
		headers: {
            "Authorization": "Bearer " + localStorage.accessToken
        }
    }).done(function (data) {
        var attackImg1 = document.getElementById("attack1Img");
        var attackImg2 = document.getElementById("attack2Img");
        var attackImg3 = document.getElementById("attack3Img");
        var defendImg1 = document.getElementById("defend1Img");
        var defendImg2 = document.getElementById("defend2Img");
        var xAttackImg1 = document.getElementById("xAttack1Img");
        var xAttackImg2 = document.getElementById("xAttack2Img");
        var xAttackImg3 = document.getElementById("xAttack3Img");
        var xDefendImg1 = document.getElementById("xDefend1Img");
        var xDefendImg2 = document.getElementById("xDefend2Img");
        var gameID = document.getElementById("gameID");
        var gameState = JSON.parse(data.GameState1);

        // Remove all images from webpage except 1 of each for attacker and defender
        attackImg1.style.visibility = 'visible';
        attackImg2.style.visibility = 'hidden';
        attackImg3.style.visibility = 'hidden';
        defendImg1.style.visibility = 'visible';
        defendImg2.style.visibility = 'hidden';
        xAttackImg1.style.visibility = 'hidden';
        xAttackImg2.style.visibility = 'hidden';
        xAttackImg3.style.visibility = 'hidden';
        xDefendImg1.style.visibility = 'hidden';
        xDefendImg2.style.visibility = 'hidden';

        var remainingAttackers = gameState.ActiveBattle.RemainingAttackers;
        var remainingDefenders = gameState.ActiveBattle.RemainingDefenders;

        // Add correct number of attacker images
        if (remainingAttackers == 2) {
            attackImg2.style.visibility = 'visible';
        }

        if (remainingAttackers >= 3) {
            attackImg2.style.visibility = 'visible';
            attackImg3.style.visibility = 'visible';
        }

        // Add correct number of defending images
        if (remainingDefenders > 1) {
            defendImg2.style.visibility = 'visible';
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
        dataType: 'json'
		headers: {
            "Authorization": "Bearer " + localStorage.accessToken
        }
    }).done(function (data) {
        var gameID = document.getElementById("gameID");
        var toTerritoryID = document.getElementById("toID");
        var fromTerritoryID = document.getElementById("fromID");
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
            var showResultBtn = document.getElementById("battleResultsBtn");
            showResultBtn.style.visibility = 'visible';
            document.getElementById("battleViewAttackBtn").disabled = true;
            document.getElementById("battleViewRetreatBtn").disabled = true;
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
                xAttackImg2.style.visibility = 'visible';
                xAttackImg3.style.visibility = 'visible';
            }

            if (attackLostRound == 1) {
                xAttackImg3.style.visibility = 'visible';
            }
            break;
        case 2:
            if (attackLostRound == 2) {
                xAttackImg1.style.visibility = 'visible';
                xAttackImg2.style.visibility = 'visible';
            }

            if (attackLostRound == 1) {
                xAttackImg2.style.visibility = 'visible';
            }
            break;
        default:
            if (attackLostRound == 1) {
                xAttackImg1.style.visibility = 'visible';
            }
            break;
    }

    // Handles defender losses
    switch (defendedCount) {
        case 2:
            if (defendLostRound == 2) {
                xDefendImg1.style.visibility = 'visible';
                xDefendImg2.style.visibility = 'visible';
            }

            if (defendLostRound == 1) {
                xDefendImg2.style.visibility = 'visible';
            }
            break;
        default:
            if (defendLostRound == 1) {
                xDefendImg1.style.visibility = 'visible';
            }
            break;
    }
}

// Coded by James
// Makes API call to Update to handle attacker retreating from battle
function BattleRetreat() {
    var gameID = document.getElementById("gameID");
    var toTerritoryID = document.getElementById("toID");
    var fromTerritoryID = document.getElementById("fromID");

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
        dataType: 'json'
		headers: {
            "Authorization": "Bearer " + localStorage.accessToken
        }
    }).done(function (data) {
        var gameID = document.getElementById("gameID");
        var toTerritoryID = document.getElementById("toID");
        var fromTerritoryID = document.getElementById("fromID");
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

        var battleResultsHeader = victor + " has won the battle.";
        var battleResultsAttacker = attackerName + " has lost a total of " + attackerLosses + " troops.";
        var battleResultsDefender = defenderName + " has lost a total of " + defenderLosses + " troops.";
        var battleResultsRes = victor + "'s necromancer has successfully risen " + revived + " fallen troops.";

        document.getElementById("batleRsltHeader").innerHTML = battleResultsHeader;
        document.getElementById("batRsltAttacker").innerHTML = battleResultsAttacker;
        document.getElementById("batRsltDefender").innerHTML = battleResultsDefender;
        document.getElementById("batRsltRes").innerHTML = battleResultsRes;
    }).fail(function (jqXHR, textStatus, errorThrown) {
        console.log(jqXHR);
    }).always(function () {
        $("body").css("cursor", "auto");
    });
}