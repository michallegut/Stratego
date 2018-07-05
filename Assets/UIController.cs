using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController instance;
    public Dropdown player1Dropdown;
    public Dropdown player2Dropdown;
    public Dropdown algorithm1Dropdown;
    public Dropdown algorithm2Dropdown;
    public Dropdown boardHeuristic1Dropdown;
    public Dropdown boardHeuristic2Dropdown;
    public Dropdown nodeHeuristic1Dropdown;
    public Dropdown nodeHeuristic2Dropdown;
    public InputField boardSizeInputField;
    public InputField depthInputField;
    public GameObject startButton;
    public Text startButtonText;
    public GameObject restartButton;
    public Transform gameBoardPanel;
    public RectTransform gameBoardPanelRectTransform;
    public Transform topCenter;
    public Button fieldPrefab;
    public RectTransform fieldPrefabRectTransform;
    public Sprite emptyfieldSprite;
    public Sprite xfieldSprite;
    public Sprite ofieldSprite;
    public Image player1TurnIndicator;
    public Image player2TurnIndicator;
    public Text player1ScoreDisplay;
    public Text player2ScoreDisplay;
    public GameObject finishMessagePanel;
    public Text finishMessageText;
    public GameObject nextMoveButton;
    public GameObject hourglass;
    private Button[,] gameBoard;
    private bool areFieldsDisabled;
    private bool isNextMoveButtonNeeded;
    private float restartButtonCooldown;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

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
        if (boardSizeInputField.isFocused)
        {
            boardSizeInputField.image.color = new Color32(255, 255, 255, 255);
        }
        if (depthInputField.isFocused)
        {
            depthInputField.image.color = new Color32(255, 255, 255, 255);
        }
        restartButtonCooldown -= Time.deltaTime;
    }

    public void onPlayer1DropdownValueChanged()
    {
        if (player1Dropdown.value == 0)
        {
            algorithm1Dropdown.interactable = false;
            boardHeuristic1Dropdown.interactable = false;
            nodeHeuristic1Dropdown.interactable = false;
            if (player2Dropdown.value == 0)
            {
                depthInputField.interactable = false;
            }
        }
        else
        {
            algorithm1Dropdown.interactable = true;
            boardHeuristic1Dropdown.interactable = true;
            nodeHeuristic1Dropdown.interactable = true;
            depthInputField.interactable = true;
        }
    }

    public void onPlayer2DropdownValueChanged()
    {
        if (player2Dropdown.value == 0)
        {
            algorithm2Dropdown.interactable = false;
            boardHeuristic2Dropdown.interactable = false;
            nodeHeuristic2Dropdown.interactable = false;
            if (player1Dropdown.value == 0)
            {
                depthInputField.interactable = false;
            }
        }
        else
        {
            algorithm2Dropdown.interactable = true;
            boardHeuristic2Dropdown.interactable = true;
            nodeHeuristic2Dropdown.interactable = true;
            depthInputField.interactable = true;
        }
    }

    public void onStartButtonClick()
    {
        if (validateInput())
        {
            startGame();
            restartButton.SetActive(true);
            startButton.SetActive(false);
        }
    }

    public void onRestartButtonClick()
    {
        if (restartButtonCooldown <= 0)
        {
            if (validateInput())
            {
                GameController.instance.stopWaitingForThread();
                destroyGameBoard();
                player1ScoreDisplay.text = "0";
                player2ScoreDisplay.text = "0";
                hourglass.SetActive(false);
                finishMessagePanel.SetActive(false);
                startGame();
            }
        }
    }

    private void startGame()
    {
        areFieldsDisabled = true;
        int boardSize = int.Parse(boardSizeInputField.text);
        generateBoard(boardSize);
        if (player1Dropdown.value == 1 && player2Dropdown.value == 1)
        {
            isNextMoveButtonNeeded = true;
            nextMoveButton.SetActive(false);
        }
        else
        {
            isNextMoveButtonNeeded = false;
        }
        player1TurnIndicator.enabled = true;
        player2TurnIndicator.enabled = false;
        GameController.instance.startGame(
                boardSize,
                depthInputField.text.Length == 0 ? 0 : int.Parse(depthInputField.text),
                player1Dropdown.value,
                player2Dropdown.value,
                algorithm1Dropdown.value,
                algorithm2Dropdown.value,
                boardHeuristic1Dropdown.value,
                boardHeuristic2Dropdown.value,
                nodeHeuristic1Dropdown.value,
                nodeHeuristic2Dropdown.value);
    }

    private bool validateInput()
    {
        bool boardSizeInputFieldValidity = validateInputField(boardSizeInputField);
        if (player1Dropdown.value == 1 || player2Dropdown.value == 1)
        {
            bool depthInputFieldValidity = validateInputField(depthInputField);
            return boardSizeInputFieldValidity && depthInputFieldValidity;
        }
        else
        {
            return boardSizeInputFieldValidity;
        }
    }

    private bool validateInputField(InputField inputField)
    {
        bool inputFieldValidity = true;
        if (inputField.text.Length == 0)
        {
            inputFieldValidity = false;
        }
        if (inputFieldValidity && (inputField.text[0] < '1' || inputField.text[0] > '9'))
        {
            inputFieldValidity = false;
        }
        for (int i = 1; inputFieldValidity && i < inputField.text.Length; i++)
        {
            if (inputField.text[i] < '0' || inputField.text[i] > '9')
            {
                inputFieldValidity = false;
            }
        }
        if (!inputFieldValidity)
        {
            inputField.image.color = new Color32(255, 0, 0, 200);
        }
        return inputFieldValidity;
    }

    private void generateBoard(int boardSize)
    {
        gameBoard = new Button[boardSize, boardSize];
        float fieldDimension = Screen.height * 0.8f / boardSize;
        float startingVerticalPosition = -fieldDimension / 2;
        float startingHorizontalPosition;
        if (boardSize % 2 == 0)
        {
            startingHorizontalPosition = -fieldDimension * (boardSize / 2 - 0.5f);
        }
        else
        {
            startingHorizontalPosition = -fieldDimension * (boardSize / 2);
        }
        Vector2 startingPosition = new Vector2(topCenter.transform.position.x + startingHorizontalPosition, topCenter.transform.position.y + startingVerticalPosition);
        fieldPrefabRectTransform.sizeDelta = new Vector2(fieldDimension, fieldDimension);
        for (int i = 0; i < boardSize; i++)
        {
            for (int j = 0; j < boardSize; j++)
            {
                Button newField = Instantiate(fieldPrefab);
                newField.transform.SetParent(gameBoardPanel);
                newField.transform.position = new Vector2(startingPosition.x + i * fieldDimension, startingPosition.y - j * fieldDimension);
                Coordinates newFieldCoordinates = newField.GetComponent<Coordinates>();
                newFieldCoordinates.setX(i);
                newFieldCoordinates.setY(j);
                newField.onClick.AddListener(delegate { fieldOnClick(newField); });
                gameBoard[i, j] = newField;
            }
        }
    }

    public void fieldOnClick(Button field)
    {
        if (!areFieldsDisabled && field.image.sprite == emptyfieldSprite)
        {
            areFieldsDisabled = true;
            Coordinates fieldCoordinates = field.GetComponent<Coordinates>();
            checkField(fieldCoordinates.getX(), fieldCoordinates.getY());
        }
    }

    public void checkField(int xFieldCoordinate, int yFieldCoordinate)
    {
        if (GameController.instance.getCurrentPlayer() == 1)
        {
            gameBoard[xFieldCoordinate, yFieldCoordinate].image.sprite = xfieldSprite;
        }
        else
        {
            gameBoard[xFieldCoordinate, yFieldCoordinate].image.sprite = ofieldSprite;
        }
        if (hourglass.activeSelf)
        {
            if (isNextMoveButtonNeeded)
            {
                nextMoveButton.SetActive(true);
            }
            hourglass.SetActive(false);
        }
        switchHighlightedPlayer();
        GameController.instance.finishTurn(xFieldCoordinate, yFieldCoordinate);
    }

    private void switchHighlightedPlayer()
    {
        player1TurnIndicator.enabled = !player1TurnIndicator.enabled;
        player2TurnIndicator.enabled = !player2TurnIndicator.enabled;
    }

    public void updatePlayer1ScoreDisplay(int score)
    {
        player1ScoreDisplay.text = score.ToString();
    }

    public void updatePlayer2ScoreDisplay(int score)
    {
        player2ScoreDisplay.text = score.ToString();
    }

    public void displayFinishMessage(int winner)
    {
        nextMoveButton.SetActive(false);
        player1TurnIndicator.enabled = false;
        player2TurnIndicator.enabled = false;
        if (winner != 0)
        {
            finishMessageText.text = "The winner is Player " + winner + "!";
        }
        else
        {
            finishMessageText.text = "Draw" + "!";
        }
        finishMessagePanel.SetActive(true);
    }

    public void destroyGameBoard()
    {
        Button[] fields = gameBoardPanel.GetComponentsInChildren<Button>();
        foreach (Button field in fields)
        {
            GameObject.Destroy(field.gameObject);
        }
    }

    public void exitButtonOnClick()
    {
        GameController.instance.stopWaitingForThread();
        Application.Quit();
    }

    public void enableFields()
    {
        areFieldsDisabled = false;
    }

    public void nextMoveButtonOnClick()
    {
        GameController.instance.startNewTurn();
        nextMoveButton.SetActive(false);
    }

    public void displayHourglass()
    {
        hourglass.SetActive(true);
    }

    public void setCooldownOnRestartButton()
    {
        restartButtonCooldown = 2;
    }
}