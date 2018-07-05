using System;

public class Strategy
{
    private Algorithm algorithm;
    private BoardStateEvaluationHeuristic boardStateEvaluationHeuristic;
    private NodeSelectionHeuristic nodeSelectionHeuristic;

    public Strategy(Algorithm algorithm, BoardStateEvaluationHeuristic boardStateEvaluationHeuristic, NodeSelectionHeuristic nodeSelectionHeuristic)
    {
        this.algorithm = algorithm;
        this.boardStateEvaluationHeuristic = boardStateEvaluationHeuristic;
        this.nodeSelectionHeuristic = nodeSelectionHeuristic;
    }

    public int[] calculateNextMove(bool currentPlayer, int currentPlayerNumber, int oponentNumber, int depth, int boardSize, int[,] gameBoard)
    {
        return algorithm.calculateNextMove(Int32.MinValue, Int32.MaxValue, currentPlayer, currentPlayerNumber, oponentNumber, depth, boardSize, gameBoard, boardStateEvaluationHeuristic, nodeSelectionHeuristic);
    }
}