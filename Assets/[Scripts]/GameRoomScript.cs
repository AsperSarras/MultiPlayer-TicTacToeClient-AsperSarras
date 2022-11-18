using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR;

public class GameRoomScript : MonoBehaviour
{
    public TMP_Text PlayerName;
    public TMP_Text PlayerSign;
    public TMP_Text EnemyName;
    public TMP_Text EnemySign;

    public GameObject PlayerLeftRoomMessage;

    public GameObject GameEnd;
    public TMP_Text GameResult;


    public TMP_Text EnemyChatMessage;

    public TMP_Text TurnPlayer;

    public List<TMP_Text> ChatTexts;

    public List<GameObject> BoardSquares;

    public TMP_InputField replayName;
    public GameObject saveReplayMenu;
    public GameObject notificationMenu;
    public TMP_Text notificationText;

    public int i = 2;
    public string TurnSign = "";



    // Start is called before the first frame update
    void Start()
    {
        PlayerName.text = NetworkedClient.Instance.UserName;
        PlayerSign.text = NetworkedClient.Instance.PlayerSign;
        EnemyName.text = NetworkedClient.Instance.EnemyName;
        EnemySign.text = NetworkedClient.Instance.EnemySign;

        NetworkedClient.Instance.winCheck = -1;
        NetworkedClient.Instance.TurnMove = -1;
        NetworkedClient.Instance.serverMessage = -1;

        if(NetworkedClient.Instance.ReplayPlaying == true)
        {
            StartCoroutine(ReplayCoroutine());
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (NetworkedClient.Instance.ReplayPlaying == false)
        {
            notificationText.text = NetworkedClient.Instance.GamePlayNotificationMessage;

            if (NetworkedClient.Instance.TurnPlayer == true)
            {
                TurnPlayer.text = PlayerName.text;
                foreach (GameObject Square in BoardSquares)
                {
                    Square.GetComponent<Button>().interactable = true;
                }
            }
            else
            {
                TurnPlayer.text = EnemyName.text;
                foreach (GameObject Square in BoardSquares)
                {
                    Square.GetComponent<Button>().interactable = false;
                }
            }

            if (NetworkedClient.Instance.newMove == true)
            {
                UpdateGameState();
            }

            if (NetworkedClient.Instance.winCheck != -1)
            {
                EndGame(NetworkedClient.Instance.winCheck);
            }

            if (NetworkedClient.Instance.serverMessage == 1)
            {
                ResetGame();
            }

            if (NetworkedClient.Instance.serverMessage == 2)
            {
                EnemyLeft();
            }

            if (NetworkedClient.Instance.ChatCheck != -1)
            {
                ChatUpdate();
            }

            if (NetworkedClient.Instance.ExpectatorUpdate == true)
            {
                ExpectatorUpdate();
            }

            if (NetworkedClient.Instance.GamePlayNotificationCheck == true)
            {
                notificationMenu.SetActive(true);
            }
        }

        if (NetworkedClient.Instance.PlayerState == StateEnum.LOBY)
        {
            SceneManager.LoadScene(1);
        }

    }

    public void CheckSquare(int Square)
    {
        if(NetworkedClient.Instance.TurnPlayer == true)
        {
            if(BoardSquares[Square].GetComponent <SquareScript>().isUsed == false)
            {
                string data = "5,"+Square.ToString();
                NetworkedClient.Instance.SendMessageToHost(data);
            }

        }
    }

    void UpdateGameState()
    {
        TMP_Text SquareText = BoardSquares[NetworkedClient.Instance.TurnMove].GetComponentInChildren<TMP_Text>();

        if (TurnPlayer.text == PlayerName.text)
        {
            SquareText.text = PlayerSign.text;
            BoardSquares[NetworkedClient.Instance.TurnMove].GetComponent<SquareScript>().isUsed = true;
            NetworkedClient.Instance.TurnPlayer = false;
            NetworkedClient.Instance.newMove = false;
        }
        else if(TurnPlayer.text == EnemyName.text)
        {
            SquareText.text = EnemySign.text;
            BoardSquares[NetworkedClient.Instance.TurnMove].GetComponent<SquareScript>().isUsed = true;
            NetworkedClient.Instance.TurnPlayer = true;
            NetworkedClient.Instance.newMove = false;
        }
    }

    void EndGame(int winer)
    {
        if (winer == 1)
        {
            GameEnd.SetActive(true);
            GameResult.text = "You Win!";
        }
        else if(winer == 0)
        {
            GameEnd.SetActive(true);
            GameResult.text = "You Lose!";
        }
        else if(winer == 2)
        {
            GameEnd.SetActive(true);
            GameResult.text = "Draw!";
        }
    }

    public void GoBackToLobby()
    {
        NetworkedClient.Instance.SendMessageToHost("6"); //6=GetBackToLoby
    }

    public void Replay()
    {
        NetworkedClient.Instance.SendMessageToHost("7"); //7=Replay
    }

    public void ResetGame()
    {
        Debug.Log("CHECK");
        foreach (GameObject Square in BoardSquares)
        {
            SquareScript sScript = Square.GetComponent<SquareScript>();
            TMP_Text SquareText = Square.GetComponentInChildren<TMP_Text>();

            sScript.isUsed = false;
            SquareText.text = "";
        }
        GameEnd.SetActive(false);
        NetworkedClient.Instance.serverMessage = -1;
    }

    public void EnemyLeft()
    {
        PlayerLeftRoomMessage.SetActive(true);
        NetworkedClient.Instance.serverMessage = -1;
    }

    public void SendChatMessage(int i)
    {
        if(NetworkedClient.Instance.ReplayPlaying == false)
        {
            string data = "8," + i.ToString();
            NetworkedClient.Instance.SendMessageToHost(data);
        }
    }

    private void ChatUpdate()
    {
        EnemyChatMessage.text = ChatTexts[NetworkedClient.Instance.ChatCheck].text;
        NetworkedClient.Instance.ChatCheck = -1;
    }

    private void ExpectatorUpdate()
    {
        string[] GameData = NetworkedClient.Instance.ExpectatorData.Split(",");

        for (int i = 0; i < BoardSquares.Count; i++)
        {
            switch (int.Parse(GameData[i+1]))
            {
                case 0:
                    BoardSquares[i].GetComponentInChildren<TMP_Text>().text = "";
                    break;
                case 1:
                    BoardSquares[i].GetComponentInChildren<TMP_Text>().text = NetworkedClient.Instance.PlayerSign;
                    break;
                case 2:
                    BoardSquares[i].GetComponentInChildren<TMP_Text>().text = NetworkedClient.Instance.EnemySign;
                    break;
            }
        }
        NetworkedClient.Instance.ExpectatorUpdate = false;
    }

    public void ToggleSaveMenu()
    {

        if(saveReplayMenu.active == true)
        {
            saveReplayMenu.SetActive(false);
        }
        else
        {
            saveReplayMenu.SetActive(true);
        }
    }

    public void ToggleNotificationMenu()
    {
        notificationMenu.SetActive(false);
        NetworkedClient.Instance.GamePlayNotificationCheck = false;
    }

    public void SaveReplay()
    {
        NetworkedClient.Instance.SendMessageToHost("10," + replayName.text);
    }

    IEnumerator ReplayCoroutine()
    {

        string[] _replayMoves = NetworkedClient.Instance.GameReplayData.Split(",");

        if (TurnSign == "")
        {
            if (int.Parse(_replayMoves[1]) == 1)
            {
                TurnSign = "O";
            }
            else if (int.Parse(_replayMoves[1]) == 2)
            {
                TurnSign = "X";
            }
        }
        else
        {
            if (TurnSign == "O")
            {
                TurnSign = "X";
            }
            else if (TurnSign == "X")
            {
                TurnSign = "O";
            }
        }

        if (i == _replayMoves.Length)
        {
            NetworkedClient.Instance.GameReplayData = "";
            NetworkedClient.Instance.ReplayPlaying = false;
            NetworkedClient.Instance.PlayerState = StateEnum.LOBY;
            //StopAllCoroutines();
        }

        int move = int.Parse(_replayMoves[i]);
        TMP_Text SquareText = BoardSquares[move].GetComponentInChildren<TMP_Text>();
        SquareText.text = TurnSign;

        i++;

        yield return new WaitForSeconds(1);

        StartCoroutine(ReplayCoroutine());

    }

}
