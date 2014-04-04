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

function DrawGomoku(canvas, board, drawSettings)
{
    var colorPlayer1 = 'white';
    var colorPlayer2 = 'black';
    var stoneStroke = 'black';
    var footerBackground = '#DDDDE6';
    var footerMargin = 2;

    var context = canvas.getContext("2d");

    var width = canvas.width;
    var height = canvas.height;    

    var squareSizeX = (width - 1) / board.n;
    var squareSizeY = (height - 1) / board.m;

    squareSize = Math.min(squareSizeX, squareSizeY);
    squareSizeX = squareSize;
    squareSizeY = squareSize;

    context.fillStyle = '#FFFF00';
    context.fillRect(0, 0, width, height);

    context.strokeStyle = 'grey';
    context.lineWidth = drawSettings.lineWidth;

    //grid
    for (var x = 0; x <= board.n; x += 1) {
        context.moveTo(x * squareSizeX + 1, 0);
        context.lineTo(x * squareSizeY + 1, board.m * squareSizeX + 1);
    }
    for (var y = 0; y <= board.m; y += 1) {
        context.moveTo(0, y * squareSizeY + 1, 0);
        context.lineTo(board.n * squareSizeX + 1, y * squareSizeY + 1);
    }
    context.stroke();

    //last stone
    if (board.stones.length > 0) {
        var last = board.stones.length - 1;
        var x = board.stones[last].X * squareSizeX + 1;
        var y = board.stones[last].Y * squareSizeY + 1;

        context.fillStyle = 'red';
        context.fillRect(x, y, squareSizeX, squareSizeY);
    }

    //stones
    var radius = Math.min(squareSizeX, squareSizeY) / 2 - 1;
    
    var stoneColors = [colorPlayer1, colorPlayer2];

    var durationTotalMs = 0;

    for (var i = 0; i < board.stones.length; i += 1) {
        var x = board.stones[i].X * squareSizeX + squareSizeX / 2 + 1;
        var y = board.stones[i].Y * squareSizeY + squareSizeY / 2 + 1;

        durationTotalMs = durationTotalMs + board.stones[i].DurationMS;

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
    var whiteCenterY = board.m * squareSizeY + squareSizeY / 2 + 1 + footerMargin;

    context.beginPath();
    context.arc(whiteCenterX, whiteCenterY, radius, 0, 2 * Math.PI, false);
    context.fillStyle = stoneColors[0];
    context.fill();
    context.lineWidth = drawSettings.lineWidth;
    context.strokeStyle = stoneStroke;
    context.stroke();

    //black player

    var blackCenterX = board.n * squareSizeX - squareSizeX / 2 - footerMargin;
    var blackcenterY = board.m * squareSizeY + squareSizeY / 2 + 1 + footerMargin;

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
    context.fillText(board.player1, whiteCenterX + squareSizeX, whiteCenterY + fontSize / 2);

    context.textAlign = 'right';
    context.fillText(board.player2, blackCenterX - squareSizeX, blackcenterY + fontSize / 2);

    //moves, duration
    context.textAlign = 'center';
    var info = 'moves: ' + board.stones.length + ', duration: ' + FormatMiliseconds(durationTotalMs);
    context.fillText(info, (blackCenterX + whiteCenterX) / 2, whiteCenterY + fontSize / 2);

}