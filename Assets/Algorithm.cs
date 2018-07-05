public interface Algorithm
{
    int[] calculateNextMove(int alpha, int beta, bool currentPlayer, int currentPlayerNumber, int oponentNumber, int depth, int boardSize, int[,] gameBoard, BoardStateEvaluationHeuristic boardStateEvaluationHeuristic, NodeSelectionHeuristic nodeSelectionHeuristic);
}