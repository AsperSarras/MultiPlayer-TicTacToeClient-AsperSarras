using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MyButtonScript : MonoBehaviour
{
    //Script
    public GameObject _logInData;
    public LogInDataScript _logInDataScript;

    //MenuChange
    public GameObject _currentMenu;
    public GameObject _newMenu;

    // for create log in
    public GameObject _inputName;
    public TMP_InputField _logInName;
    public GameObject _inputPass;
    public TMP_InputField _logInPass;





    // Start is called before the first frame update
    void Start()
    {
        if(_logInData != null)
        {
            _logInDataScript = _logInData.GetComponent<LogInDataScript>();
            _logInName= _inputName.GetComponent<TMP_InputField>();
            _logInPass= _inputPass.GetComponent<TMP_InputField>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void _MenuChangeFunction()
    {
         _currentMenu.SetActive(false);
         _newMenu.SetActive(true);
    }

    public void _logInButtonFunction(int type)
    {
        _logInDataScript._string1 = _logInName.text;
        _logInDataScript._string2 = _logInPass.text;
        _logInDataScript._updateUserData(type);
    }

    public void _ReplayMenu()
    {
        _newMenu.SetActive(true);
        NetworkedClient.Instance.SendMessageToHost("11");
    }

    public void _GoBackFromReplay()
    {
        _newMenu.SetActive(false);
    }

    public void _LogOut()
    {
        //NetworkedClient.Instance._sendData("", 3);
        NetworkedClient.Instance.SendMessageToHost("3");
    }

}
