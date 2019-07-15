using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CanvasController : Singleton<CanvasController>
{

    public delegate void eventTrigger();
    public static event eventTrigger eventCall;
    public GameObject loadingScreen;
    public GameObject LoginScreen;
    public GameObject HomeScreen;


    public bool loading = false;

    private void OnEnable()
    {
        eventCall += StartGame;
    }

    private void OnDisable()
    {
        eventCall -= StartGame;
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }


    public void StartGame()
    {
        LoginScreen.SetActive(false);
        loadingScreen.SetActive(false);
        HomeScreen.SetActive(true);
    }

    public static void triggerEvent()
    {
        if (eventCall != null)
            eventCall();
    }

    public void LoadScreen()
    {
        loading = true;
        loadingScreen.SetActive(true);
        StartCoroutine(startGame());
    }

    public void CallNotification(string Messenge, float time)
    {

    }

    IEnumerator startGame()
    {
        yield return new WaitForSeconds(2f);
        StartGame();
    }
}
