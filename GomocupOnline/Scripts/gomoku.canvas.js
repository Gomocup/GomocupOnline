function DrawGomoku(canvas, board, drawSettings)
{   
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
    
    for (var i = 0; i < board.stones.length; i += 1) {
        var x = board.stones[i].X * squareSizeX + squareSizeX / 2 + 1;
        var y = board.stones[i].Y * squareSizeY + squareSizeY / 2 + 1;

        context.beginPath();
        context.arc(x, y, radius, 0, 2 * Math.PI, false);

        if (i % 2 == 0) {
            context.fillStyle = 'white';
        }
        else {
            context.fillStyle = 'black';
        }

        context.fill();
        context.lineWidth = drawSettings.lineWidth;
        context.strokeStyle = 'black';
        context.stroke();
    }

    //footer
    context.fillStyle = 'grey';
    context.fillRect(0, board.m * squareSizeY + 1, width, height);
    //context.fillRect(100,200, 2*width, 2*height);
    
}