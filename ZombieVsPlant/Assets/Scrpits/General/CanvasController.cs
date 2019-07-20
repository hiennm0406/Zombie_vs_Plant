using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{

    public delegate void eventTrigger();
    public static event eventTrigger eventCall;
    public GameObject loadingScreen;
    public GameObject LoginScreen;


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
        Debug.Log("done");
        loading = false;
        LoginScreen.SetActive(false);
        loadingScreen.SetActive(false);
    }

    public static void triggerEvent()
    {
        if (eventCall != null)
            eventCall();
    }

    public void LoadScreen()
    {
        Debug.Log("loadscreen");
        loading = true;
        loadingScreen.SetActive(true);
        StartCoroutine(startWait());
    }

    public void CallNotification(string Messenge, float time)
    {

    }

    IEnumerator startWait()
    {
        yield return new WaitForSeconds(2f);
        StartGame();
    }
}
