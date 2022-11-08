using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LogInDataScript : MonoBehaviour
{
    public GameObject _clientGameObject;
    public GameObject _errorMenu;
    public GameObject _errorMessage;
    public TMP_Text _errorMessageText;

    public string _string1;
    public string _string2;

    // Start is called before the first frame update
    void Start()
    {
        _errorMessageText = _errorMessage.GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        _errorMessageText.text = NetworkedClient.Instance.ErrorMessage;

        if (NetworkedClient.Instance.isLogedIn == true)
        {
            SceneManager.LoadScene(1);
        }

        if(NetworkedClient.Instance.ErrorCheck == true)
        {
            _errorMenu.SetActive(true);
        }

    }

    public void _updateUserData(int action)
    {
        string data = action.ToString() + "," +_string1 + "," + _string2;
        if(action == 1)
        {
            NetworkedClient.Instance.UserName = _string1;
        }
        NetworkedClient.Instance.SendMessageToHost(data);

        //switch (action)
        //{
        //    case 0://creation
        //        data = _string1 + "," + _string2;
        //        break;
        //    case 1://login
        //        data = _string1 + "," + _string2;
        //        NetworkedClient.Instance.UserName = _string1;
        //        break;
        //}
        //NetworkedClient.Instance._sendData(data, action);
    }

    public void CloseMessage()
    {
        NetworkedClient.Instance.ErrorCheck = false;
        _errorMenu.SetActive(false);
    }

}
