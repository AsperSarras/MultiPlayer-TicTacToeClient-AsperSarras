using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameLobbyManager : MonoBehaviour
{
    public string _userName;
    public string _roomName;
    public TMP_Text playerName;
    public TMP_Text RoomNameText;
    public TMP_InputField roomName;
    public GameObject WaitingMenu;
    public GameObject _errorMenu;
    public GameObject _errorMessage;
    public TMP_Text _errorMessageText;

    public GameObject ReplayMenu;
    public TMP_Dropdown ReplayList;
    public string replaySelected;


    // Start is called before the first frame update
    void Start()
    {
        _errorMessageText = _errorMessage.GetComponent<TMP_Text>();

        ReplayList.onValueChanged.AddListener(delegate { ReplayListChange(); });
    }

    // Update is called once per frame
    void Update()
    {
        _errorMessageText.text = NetworkedClient.Instance.ErrorMessage;
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

        if (NetworkedClient.Instance.ErrorCheck == true)
        {
            _errorMenu.SetActive(true);
        }

        if (NetworkedClient.Instance.ReplayUpdate == true)
        {

            UpdateReplayList();
            ReplayList.value = 0;
        }

    }

    public void CheckLoby()
    {
        string data = "2," + _roomName;
        Debug.Log(_roomName);
        NetworkedClient.Instance.SendMessageToHost(data);
    }
    public void Expectate()
    {
        string data = "9," + _roomName;
        NetworkedClient.Instance.SendMessageToHost(data);
    }

    public void Cancel()
    {
        NetworkedClient.Instance.PlayerState = StateEnum.LOBY;
        NetworkedClient.Instance.SendMessageToHost("4");
    }

    private void UpdateReplayList()
    {
        ReplayList.ClearOptions();
        string[] _replayList = NetworkedClient.Instance.replayData.Split(",");

        foreach (string option in _replayList)
        {
            ReplayList.options.Add(new TMP_Dropdown.OptionData(option));
        }
        ReplayList.value = -1;
        NetworkedClient.Instance.ReplayUpdate = false;
    }

    public void ReplayListChange()
    {
        int menuIndex = ReplayList.value;
        List<TMP_Dropdown.OptionData> menuOptions = ReplayList.options;
        replaySelected = menuOptions[menuIndex].text;
    }

    public void PlayReplay()
    {
        if(replaySelected != "")
        {
            NetworkedClient.Instance.SendMessageToHost("12,"+replaySelected);
        }
    }

    public void CloseMessage()
    {
        NetworkedClient.Instance.ErrorCheck = false;
        _errorMenu.SetActive(false);
    }
}
