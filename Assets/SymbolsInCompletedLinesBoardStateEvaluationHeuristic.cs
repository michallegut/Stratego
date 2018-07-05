public class OwnSymbolsInCompletedLinesBoardStateEvaluationHeuristic : BoardStateEvaluationHeuristic
{
    public int evaluateBoardState(int currentPlayerNumber, int boardSize, int[,] gameBoard)
    {
        int value = 0;
        for (int i = 0; i < boardSize; i++)
        {
            bool isHorizontalLineCompleted = true;
            bool isVerticalLineCompleted = true;
            bool isFirstDiagonalLineCompleted = true;
            bool isSecondDiagonalLineCompleted = true;
            int horizontalValue = 0;
            int verticalValue = 0;
            int firstDiagonalValue = 0;
            int secondDiagonalValue = 0;
            int partialHorizontalValue = 0;
            int partialVerticalValue = 0;
            int partialFirstDiagonalValue = 0;
            int partialSecondDiagonalValue = 0;
            for (int j = 0; j < boardSize; j++)
            {
                if (isHorizontalLineCompleted)
                {
                    if (gameBoard[i, j] == currentPlayerNumber)
                    {
                        partialHorizontalValue++;
                    }
                    else if (gameBoard[i, j] == 0)
                    {
                        isHorizontalLineCompleted = false;
                        horizontalValue = 0;
                        partialHorizontalValue = 0;
                    }
                    else
                    {
                        if (partialHorizontalValue > 1)
                        {
                            horizontalValue += partialHorizontalValue;
                            partialHorizontalValue = 0;
                        }
                    }
                }
                if (isVerticalLineCompleted)
                {
                    if (gameBoard[j, i] == currentPlayerNumber)
                    {
                        partialVerticalValue++;
                    }
                    else if (gameBoard[j, i] == 0)
                    {
                        isVerticalLineCompleted = false;
                        verticalValue = 0;
                        partialVerticalValue = 0;
                    }
                    else
                    {
                        if (partialVerticalValue > 1)
                        {
                            verticalValue += partialVerticalValue;
                            partialVerticalValue = 0;
                        }
                    }
                }
                if (isFirstDiagonalLineCompleted && i + j < boardSize)
                {
                    if (gameBoard[i + j, j] == currentPlayerNumber)
                    {
                        partialFirstDiagonalValue++;
                    }
                    else if (gameBoard[i + j, j] == 0)
                    {
                        isFirstDiagonalLineCompleted = false;
                        firstDiagonalValue = 0;
                        partialFirstDiagonalValue = 0;
                    }
                    else
                    {
                        if (partialFirstDiagonalValue > 1)
                        {
                            firstDiagonalValue += partialFirstDiagonalValue;
                            partialFirstDiagonalValue = 0;
                        }
                    }
                }
                if (isSecondDiagonalLineCompleted && i - j >= 0)
                {
                    if (gameBoard[i - j, j] == currentPlayerNumber)
                    {
                        partialSecondDiagonalValue++;
                    }
                    else if (gameBoard[i - j, j] == 0)
                    {
                        isSecondDiagonalLineCompleted = false;
                        secondDiagonalValue = 0;
                        partialSecondDiagonalValue = 0;
                    }
                    else
                    {
                        if (partialSecondDiagonalValue > 1)
                        {
                            secondDiagonalValue += partialSecondDiagonalValue;
                            partialSecondDiagonalValue = 0;
                        }
                    }
                }
            }
            value += horizontalValue + verticalValue + firstDiagonalValue + secondDiagonalValue + partialHorizontalValue > 1 ? partialHorizontalValue : 0 + partialVerticalValue > 1 ? partialVerticalValue : 0 + partialFirstDiagonalValue > 1 ? partialFirstDiagonalValue : 0 + partialSecondDiagonalValue > 1 ? partialSecondDiagonalValue : 0;
        }
        return value;
    }
}