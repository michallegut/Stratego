public interface BoardStateEvaluationHeuristic
{
    int evaluateBoardState(int currentPlayerNumber, int boardSize, int[,] gameBoard);
}