function UpdateBoard(id, filename, board) {
    var canvas = document.getElementById(id, filename);

    var jsonQuery = {};
    jsonQuery.tournamentMatch = filename;
    $.getJSON("MatchJSON", jsonQuery, function (data) {

        board = data;
        DrawGomoku(canvas, board, drawSettings, board.Moves.length);
    });
}

function Replay(id, replayState, board) {
    var canvas = document.getElementById(id);

    //var jsonQuery = {};
    //jsonQuery.tournamentMatch = filename;

    DrawGomoku(canvas, board, drawSettings, replayState.moveIndex);

    if (replayState.pause || replayState.moveIndex >= board.Moves.length)
        return; //end of replay

    setTimeout(function () {
        if (replayState.pause)
            return;

        replayState.moveIndex = replayState.moveIndex + 1;
        Replay(id, replayState, board);
    },
    replayPauseMs);
}

function Pause(id, replayState, board) {
    replayState.pause = true;
}

function Continue(id, replayState, board) {
    if (replayState.moveIndex == board.Moves.length)
        replayState.moveIndex = 1;
    replayState.pause = false;
    Replay(id, replayState, board);
}

function Last(id, replayState, board) {
    replayState.moveIndex = board.Moves.length;
    replayState.pause = true;
    Replay(id, replayState, board);
}

function First(id, replayState, board) {
    replayState.moveIndex = 1;
    replayState.pause = true;
    Replay(id, replayState, board);
}

function Next(id, replayState, board) {
    replayState.pause = true;
    if (replayState.moveIndex < board.Moves.length) {
        replayState.moveIndex = replayState.moveIndex + 1;
        Replay(id, replayState, board);
    }
}

function Prev(id, replayState, board) {
    pause = true;
    if (replayState.moveIndex > 0) {
        replayState.moveIndex = replayState.moveIndex - 1;
        Replay(id, replayState, board);
    }
}

function ReplayStart(id, replayState, board) {
    replayState.moveIndex = 1;
    replayState.pause = false;
    Replay(id, replayState, board);
}