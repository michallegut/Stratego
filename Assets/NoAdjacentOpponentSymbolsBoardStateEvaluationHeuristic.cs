public class NoAdjacentOpponentSymbolsBoardStateEvaluationHeuristic : BoardStateEvaluationHeuristic
{
    public int evaluateBoardState(int currentPlayerNumber, int boardSize, int[,] gameBoard)
    {
        int value = 0;
        for (int i = 0; i < boardSize; i++)
        {
            for (int j = 0; i < boardSize; i++)
            {
                if (gameBoard[i, j] == currentPlayerNumber)
                {
                    if (i > 0 && (gameBoard[i - 1, j] == currentPlayerNumber || gameBoard[i - 1, j] == 0)) value++;
                    if (i < boardSize - 1 && (gameBoard[i + 1, j] == currentPlayerNumber || gameBoard[i + 1, j] == 0)) value++;
                    if (j > 0 && (gameBoard[i, j - 1] == currentPlayerNumber || gameBoard[i, j - 1] == 0)) value++;
                    if (j < boardSize - 1 && (gameBoard[i, j + 1] == currentPlayerNumber || gameBoard[i, j + 1] == 0)) value++;
                    if (i < boardSize - 1 && j < boardSize - 1 && (gameBoard[i + 1, j + 1] == currentPlayerNumber || gameBoard[i + 1, j + 1] == 0)) value++;
                    if (i > 0 && j > 0 && (gameBoard[i - 1, j - 1] == currentPlayerNumber || gameBoard[i - 1, j - 1] == 0)) value++;
                    if (i < boardSize - 1 && j > 0 && (gameBoard[i + 1, j - 1] == currentPlayerNumber || gameBoard[i + 1, j - 1] == 0)) value++;
                    if (i > 0 && j < boardSize - 1 && (gameBoard[i - 1, j + 1] == currentPlayerNumber || gameBoard[i - 1, j + 1] == 0)) value++;
                }
            }
        }
        return value;
    }
}