using System;
using System.Collections.Generic;

public class MinimaxAlgorithm : Algorithm
{
    public int[] calculateNextMove(int alpha, int beta, bool currentPlayer, int currentPlayerNumber, int oponentNumber, int depth, int boardSize, int[,] gameBoard, BoardStateEvaluationHeuristic boardStateEvaluationHeuristic, NodeSelectionHeuristic nodeSelectionHeuristic)
    {
        List<int[]> nextMoves = new List<int[]>();
        for (int i = 0; i < boardSize; i++)
        {
            for (int j = 0; j < boardSize; j++)
            {
                if (gameBoard[i, j] == 0)
                {
                    nextMoves.Add(new int[] { i, j });
                }
            }
        }
        if (nodeSelectionHeuristic != null)
        {
            nodeSelectionHeuristic.sortNodes(nextMoves, boardSize);
        }
        int bestScore = currentPlayer ? Int32.MinValue : Int32.MaxValue;
        int currentScore = 0;
        int bestRow = -1;
        int bestColumn = -1;
        if (depth == 0 || nextMoves.Count == 0 || GameController.instance.shouldThreadStop())
        {
            bestScore = boardStateEvaluationHeuristic.evaluateBoardState(currentPlayerNumber, boardSize, gameBoard);
        }
        else
        {
            foreach (int[] move in nextMoves)
            {
                gameBoard[move[0], move[1]] = currentPlayer ? currentPlayerNumber : oponentNumber;
                if (currentPlayer)
                {
                    currentScore = calculateNextMove(alpha, beta, false, currentPlayerNumber, oponentNumber, depth - 1, boardSize, gameBoard, boardStateEvaluationHeuristic, nodeSelectionHeuristic)[0];
                    if (currentScore > bestScore)
                    {
                        bestScore = currentScore;
                        bestRow = move[0];
                        bestColumn = move[1];
                    }
                }
                else
                {
                    currentScore = calculateNextMove(alpha, beta, true, currentPlayerNumber, oponentNumber, depth - 1, boardSize, gameBoard, boardStateEvaluationHeuristic, nodeSelectionHeuristic)[0];
                    if (currentScore < bestScore)
                    {
                        bestScore = currentScore;
                        bestRow = move[0];
                        bestColumn = move[1];
                    }
                }
                gameBoard[move[0], move[1]] = 0;
            }
        }
        return new int[] { bestScore, bestRow, bestColumn };
    }
}