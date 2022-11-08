using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLobbyManager : MonoBehaviour
{
    public string _userName;
    public string _roomName;
    public TMP_Text playerName;
    public TMP_Text RoomNameText;
    public TMP_InputField roomName;
    public GameObject WaitingMenu;


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //Update the name
        _userName = NetworkedClient.Instance.UserName;
        playerName.text = _userName;

        if (NetworkedClient.Instance.isLogedIn == false)
        {
            SceneManager.LoadScene(0);
        }

        _roomName = roomName.text;
        RoomNameText.text = _roomName;

        if(NetworkedClient.Instance.PlayerState == StateEnum.WAITING)
        {
            WaitingMenu.SetActive(true);
        }
        else if (NetworkedClient.Instance.PlayerState == StateEnum.LOBY)
        {
            WaitingMenu.SetActive(false);
        }
        else if (NetworkedClient.Instance.PlayerState == StateEnum.GAME)
        {
            SceneManager.LoadScene(2);
        }

    }

    public void CheckLoby()
    {
        string data = "2," + _roomName;
        Debug.Log(_roomName);
        NetworkedClient.Instance.SendMessageToHost(data);
        //NetworkedClient.Instance._sendData(_roomName, 2);
    }

    public void Cancel()
    {
        NetworkedClient.Instance.PlayerState = StateEnum.LOBY;
        NetworkedClient.Instance.SendMessageToHost("4");
        //NetworkedClient.Instance._sendData("", 4);

    }
}
