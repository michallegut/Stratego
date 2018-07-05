using UnityEngine;
using System.Threading;
using System.Diagnostics;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    private int boardSize;
    private int[,] gameBoard;
    private int depth;
    private int currentPlayer;
    bool isPlayer1AI;
    bool isPlayer2AI;
    Strategy player1Strategy;
    Strategy player2Strategy;
    private int player1Score;
    private int player2Score;
    private int freeFields;
    private bool proceedAutomatically;
    private Thread aiThread;
    private bool waitForThreadAnswer;
    int[] nextMove;
    private int turnNumber;
    private Stopwatch stopwatch;

    private void Update()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        if (waitForThreadAnswer && nextMove != null)
        {
            UnityEngine.Debug.Log("AI move time for player " + currentPlayer + ", turn " + turnNumber + ": " + stopwatch.ElapsedMilliseconds + " ms");
            stopwatch.Reset();
            waitForThreadAnswer = false;
            UIController.instance.checkField(nextMove[1], nextMove[2]);
        }
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void startGame(int boardSize, int depth, int player1, int player2, int algorithm1, int algorithm2, int boardHeuristic1, int boardHeuristic2, int nodeHeuristic1, int nodeHeuristic2)
    {
        this.boardSize = boardSize;
        gameBoard = new int[boardSize, boardSize];
        this.depth = depth;
        currentPlayer = 0;
        isPlayer1AI = player1 == 1;
        isPlayer2AI = player2 == 1;
        proceedAutomatically = !isPlayer1AI || !isPlayer2AI;

        if (isPlayer1AI)
        {
            Algorithm player1Algorithm;
            if (algorithm1 == 0)
            {
                player1Algorithm = new MinimaxAlgorithm();
            }
            else
            {
                player1Algorithm = new AlphaBetaAlgorithm();
            }
            BoardStateEvaluationHeuristic player1BoardStateEvaluationHeuristic;
            if (boardHeuristic1 == 0)
            {
                player1BoardStateEvaluationHeuristic = new OwnAdjacentSymbolsBoardStateEvaluationHeuristic();
            }
            else if (boardHeuristic1 == 1)
            {
                player1BoardStateEvaluationHeuristic = new NoAdjacentOpponentSymbolsBoardStateEvaluationHeuristic();
            }
            else
            {
                player1BoardStateEvaluationHeuristic = new OwnSymbolsInCompletedLinesBoardStateEvaluationHeuristic();
            }
            NodeSelectionHeuristic player1NodeSelectionHeuristic;
            if (nodeHeuristic1 == 0)
            {
                player1NodeSelectionHeuristic = null;
            }
            else if (nodeHeuristic1 == 1)
            {
                player1NodeSelectionHeuristic = new CentralNodeSelectionHeuristic();
            }
            else
            {
                player1NodeSelectionHeuristic = new BorderNodeSelectionHeuristic();
            }
            player1Strategy = new Strategy(player1Algorithm, player1BoardStateEvaluationHeuristic, player1NodeSelectionHeuristic);
        }
        if (isPlayer2AI)
        {
            Algorithm player2Algorithm;
            if (algorithm2 == 0)
            {
                player2Algorithm = new MinimaxAlgorithm();
            }
            else
            {
                player2Algorithm = new AlphaBetaAlgorithm();
            }
            BoardStateEvaluationHeuristic player2BoardStateEvaluationHeuristic;
            if (boardHeuristic2 == 0)
            {
                player2BoardStateEvaluationHeuristic = new OwnAdjacentSymbolsBoardStateEvaluationHeuristic();
            }
            else if (boardHeuristic2 == 1)
            {
                player2BoardStateEvaluationHeuristic = new NoAdjacentOpponentSymbolsBoardStateEvaluationHeuristic();
            }
            else
            {
                player2BoardStateEvaluationHeuristic = new OwnSymbolsInCompletedLinesBoardStateEvaluationHeuristic();
            }
            NodeSelectionHeuristic player2NodeSelectionHeuristic;
            if (nodeHeuristic2 == 0)
            {
                player2NodeSelectionHeuristic = null;
            }
            else if (nodeHeuristic2 == 1)
            {
                player2NodeSelectionHeuristic = new CentralNodeSelectionHeuristic();
            }
            else
            {
                player2NodeSelectionHeuristic = new BorderNodeSelectionHeuristic();
            }
            player2Strategy = new Strategy(player2Algorithm, player2BoardStateEvaluationHeuristic, player2NodeSelectionHeuristic);
        }
        player1Score = 0;
        player2Score = 0;
        freeFields = boardSize * boardSize;
        nextMove = null;
        stopwatch = new Stopwatch();
        turnNumber = 0;
        startNewTurn();
        UIController.instance.setCooldownOnRestartButton();
    }

    public int getCurrentPlayer()
    {
        return currentPlayer;
    }

    public void finishTurn(int xFieldCoordinate, int yFieldCoordinate)
    {
        gameBoard[xFieldCoordinate, yFieldCoordinate] = currentPlayer;
        countScore(xFieldCoordinate, yFieldCoordinate);
        if (--freeFields == 0)
        {
            finishGame();
        }
        else if (proceedAutomatically)
        {
            startNewTurn();
        }
    }

    private void countScore(int xFieldCoordinate, int yFieldCoordinate)
    {
        int totalScore = 0;
        int lineScore = 1;
        if (checkIfHorizontalLineIsFinished(yFieldCoordinate))
        {
            for (int i = xFieldCoordinate - 1; i >= 0 && gameBoard[i, yFieldCoordinate] == currentPlayer; i--)
            {
                lineScore++;
            }
            for (int i = xFieldCoordinate + 1; i < boardSize && gameBoard[i, yFieldCoordinate] == currentPlayer; i++)
            {
                lineScore++;
            }
            if (lineScore > 1)
            {
                totalScore += lineScore;
                lineScore = 1;
            }
        }
        if (checkIfVerticalLineIsFinished(xFieldCoordinate))
        {
            for (int i = yFieldCoordinate - 1; i >= 0 && gameBoard[xFieldCoordinate, i] == currentPlayer; i--)
            {
                lineScore++;
            }
            for (int i = yFieldCoordinate + 1; i < boardSize && gameBoard[xFieldCoordinate, i] == currentPlayer; i++)
            {
                lineScore++;
            }
            if (lineScore > 1)
            {
                totalScore += lineScore;
                lineScore = 1;
            }
        }
        if (checkIfFirstDiagonalLineIsFinished(xFieldCoordinate, yFieldCoordinate))
        {
            for (int i = xFieldCoordinate - 1, j = yFieldCoordinate + 1; i >= 0 && j < boardSize && gameBoard[i, j] == currentPlayer; i--, j++)
            {
                lineScore++;
            }
            for (int i = xFieldCoordinate + 1, j = yFieldCoordinate - 1; i < boardSize && j >= 0 && gameBoard[i, j] == currentPlayer; i++, j--)
            {
                lineScore++;
            }
            if (lineScore > 1)
            {
                totalScore += lineScore;
                lineScore = 1;
            }
        }
        if (checkIfSecondDiagonalLineIsFinished(xFieldCoordinate, yFieldCoordinate))
        {
            for (int i = xFieldCoordinate - 1, j = yFieldCoordinate - 1; i >= 0 && j >= 0 && gameBoard[i, j] == currentPlayer; i--, j--)
            {
                lineScore++;
            }
            for (int i = xFieldCoordinate + 1, j = yFieldCoordinate + 1; i < boardSize && j < boardSize && gameBoard[i, j] == currentPlayer; i++, j++)
            {
                lineScore++;
            }
            if (lineScore > 1)
            {
                totalScore += lineScore;
            }
        }
        if (totalScore > 0)
        {
            if (currentPlayer == 1)
            {
                player1Score += totalScore;
                UIController.instance.updatePlayer1ScoreDisplay(player1Score);
            }
            else
            {
                player2Score += totalScore;
                UIController.instance.updatePlayer2ScoreDisplay(player2Score);
            }
        }
    }

    public void startNewTurn()
    {
        turnNumber++;
        if (++currentPlayer == 3)
        {
            currentPlayer = 1;
        }
        if (currentPlayer == 1)
        {
            if (isPlayer1AI)
            {
                UIController.instance.displayHourglass();
                nextMove = null;
                waitForThreadAnswer = true;
                aiThread = new Thread(() => nextMove = player1Strategy.calculateNextMove(true, 1, 2, depth, boardSize, gameBoard));
                stopwatch.Start();
                aiThread.Start();
            }
            else
            {
                UIController.instance.enableFields();
            }
        }
        else
        {
            if (isPlayer2AI)
            {
                UIController.instance.displayHourglass();
                nextMove = null;
                waitForThreadAnswer = true;
                aiThread = new Thread(() => nextMove = player2Strategy.calculateNextMove(true, 2, 1, depth, boardSize, gameBoard));
                stopwatch.Start();
                aiThread.Start();
            }
            else
            {
                UIController.instance.enableFields();
            }
        }
    }

    private bool checkIfHorizontalLineIsFinished(int yFieldCoordinate)
    {
        for (int i = 0; i < boardSize; i++)
        {
            if (gameBoard[i, yFieldCoordinate] == 0)
            {
                return false;
            }
        }
        return true;
    }

    private bool checkIfVerticalLineIsFinished(int xFieldCoordinate)
    {
        for (int i = 0; i < boardSize; i++)
        {
            if (gameBoard[xFieldCoordinate, i] == 0)
            {
                return false;
            }
        }
        return true;
    }

    private bool checkIfFirstDiagonalLineIsFinished(int xFieldCoordinate, int yFieldCoordinate)
    {
        for (int i = xFieldCoordinate - 1, j = yFieldCoordinate + 1; i >= 0 && j < boardSize; i--, j++)
        {
            if (gameBoard[i, j] == 0)
            {
                return false;
            }
        }
        for (int i = xFieldCoordinate + 1, j = yFieldCoordinate - 1; i < boardSize && j >= 0; i++, j--)
        {
            if (gameBoard[i, j] == 0)
            {
                return false;
            }
        }
        return true;
    }

    private bool checkIfSecondDiagonalLineIsFinished(int xFieldCoordinate, int yFieldCoordinate)
    {
        for (int i = xFieldCoordinate - 1, j = yFieldCoordinate - 1; i >= 0 && j >= 0; i--, j--)
        {
            if (gameBoard[i, j] == 0)
            {
                return false;
            }
        }
        for (int i = xFieldCoordinate + 1, j = yFieldCoordinate + 1; i < boardSize && j < boardSize; i++, j++)
        {
            if (gameBoard[i, j] == 0)
            {
                return false;
            }
        }
        return true;
    }

    private void finishGame()
    {
        int winner;
        if (player1Score > player2Score)
        {
            winner = 1;
        }
        else if (player1Score < player2Score)
        {
            winner = 2;
        }
        else
        {
            winner = 0;
        }
        UIController.instance.displayFinishMessage(winner);
    }

    public void stopWaitingForThread()
    {
        waitForThreadAnswer = false;
        if (aiThread != null)
        {
            aiThread.Join();
        }
    }

    public bool shouldThreadStop()
    {
        return !waitForThreadAnswer;
    }
}