function FormatMiliseconds(totalMiliseconds)
{
    var totalSec = totalMiliseconds / 1000;
    var hours = parseInt(totalSec / 3600) % 24;
    var minutes = parseInt(totalSec / 60) % 60;
    var seconds = totalSec % 60;

    if (totalMiliseconds < 10000)
        seconds = Math.round(seconds * 10) / 10;
    else
        seconds = Math.round(seconds);

    var result = (hours < 10 ? "0" + hours : hours) + ":" + (minutes < 10 ? "0" + minutes : minutes) + ":" + (seconds < 10 ? "0" + seconds : seconds);
    return result;
}

function DrawGomoku(canvas, board, drawSettings, moveIndexTo)
{
    var colorPlayer1 = 'white';
    var colorPlayer2 = 'black';
    var stoneStroke = 'black';
    var footerBackground = '#DDDDE6';
    var footerMargin = 2;

    var context = canvas.getContext("2d");

    var width = canvas.width;
    var height = canvas.height;    

    var squareSizeX = (width - 1) / board.Width;
    var squareSizeY = (height - 1) / board.Height;

    squareSize = Math.min(squareSizeX, squareSizeY);
    squareSizeX = squareSize;
    squareSizeY = squareSize;

    context.fillStyle = '#FFFF00';
    context.fillRect(0, 0, width, height);

    context.strokeStyle = 'grey';
    context.lineWidth = drawSettings.lineWidth;

    //grid
    for (var x = 0; x <= board.Width; x += 1) {
        context.moveTo(x * squareSizeX + 1, 0);
        context.lineTo(x * squareSizeY + 1, board.Height * squareSizeX + 1);
    }
    for (var y = 0; y <= board.Height; y += 1) {
        context.moveTo(0, y * squareSizeY + 1, 0);
        context.lineTo(board.Width * squareSizeX + 1, y * squareSizeY + 1);
    }
    context.stroke();

    //last stone
    if (moveIndexTo > 0 && moveIndexTo == board.Moves.length) {
        var last = board.Moves.length - 1;
        var x = (board.Moves[last].X - 1) * squareSizeX + 1;
        var y = (board.Moves[last].Y - 1) * squareSizeY + 1;

        context.fillStyle = 'red';
        context.fillRect(x, y, squareSizeX, squareSizeY);
    }

    //stones
    var radius = Math.min(squareSizeX, squareSizeY) / 2 - 1;
    
    var stoneColors = [colorPlayer1, colorPlayer2];

    var durationTotalMs = 0;

    for (var i = 0; i < moveIndexTo; i += 1) {
        var x = board.Moves[i].X * squareSizeX - squareSizeX / 2 + 1;
        var y = board.Moves[i].Y * squareSizeY - squareSizeY / 2 + 1;

        durationTotalMs = durationTotalMs + board.Moves[i].DurationMS;

        context.beginPath();
        context.arc(x, y, radius, 0, 2 * Math.PI, false);
        context.fillStyle = stoneColors[i % 2];
        context.fill();
        context.lineWidth = drawSettings.lineWidth;
        context.strokeStyle = stoneStroke;
        context.stroke();
    }

    //footer
    context.fillStyle = footerBackground;
    context.fillRect(0, board.m * squareSizeY + 2, width, height);        

    //white player

    var whiteCenterX = squareSizeX / 2 + footerMargin;
    var whiteCenterY = board.Height * squareSizeY + squareSizeY / 2 + 1 + footerMargin;

    context.beginPath();
    context.arc(whiteCenterX, whiteCenterY, radius, 0, 2 * Math.PI, false);
    context.fillStyle = stoneColors[0];
    context.fill();
    context.lineWidth = drawSettings.lineWidth;
    context.strokeStyle = stoneStroke;
    context.stroke();

    //black player

    var blackCenterX = board.Width * squareSizeX - squareSizeX / 2 - footerMargin;
    var blackcenterY = board.Height * squareSizeY + squareSizeY / 2 + 1 + footerMargin;

    context.beginPath();
    context.arc(blackCenterX, blackcenterY, radius, 0, 2 * Math.PI, false);
    context.fillStyle = stoneColors[1];
    context.fill();
    context.lineWidth = drawSettings.lineWidth;
    context.strokeStyle = stoneStroke;
    context.stroke();

    var fontSize = 12;

    context.font = 'bold ' + fontSize + 'px sans-serif';
    context.textAlign = 'left';
    context.fillText(board.Player1, whiteCenterX + squareSizeX, whiteCenterY + fontSize / 2);

    context.textAlign = 'right';
    context.fillText(board.Player2, blackCenterX - squareSizeX, blackcenterY + fontSize / 2);

    //moves, duration
    context.textAlign = 'center';
    var info = 'mov.: ' + moveIndexTo + ', dur.: ' + FormatMiliseconds(durationTotalMs);
    context.fillText(info, (blackCenterX + whiteCenterX) / 2, whiteCenterY + fontSize / 2);

}