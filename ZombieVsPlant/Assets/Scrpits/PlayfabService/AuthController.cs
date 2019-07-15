using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab.ClientModels;
using PlayFab;
using UnityEngine.Events;
using System;
using System.Text.RegularExpressions;

public class AuthController : MonoBehaviour
{

    private const string emailPattern = @"^([0-9a-zA-Z]([\+\-_\.][0-9a-zA-Z]+)*)+@(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]*\.)+[a-zA-Z0-9]{2,17})$";
    // used for device ID
    public string android_id = string.Empty; // device ID to use with PlayFab login
    public string ios_id = string.Empty; // device ID to use with PlayFab login
    public string custom_id = string.Empty; // custom id for other platforms        

    public CanvasController canvas;

    public Text user;
    public Text email;
    public Text loginUser;
    public Text loginPass;
    public Text pass1;
    public Text pass2;

    public string saveUser;
    public string savePass;
    public bool save = true;

    public GameObject reg;
    public GameObject login;
    public GameObject Autologin;

    private void Start()
    {
        saveUser = PlayerPrefs.GetString("Username");
        savePass = PlayerPrefs.GetString("Password");
    }

    private void Update()
    {
        if (save)
        {
            save = false;
            if(saveUser != String.Empty && savePass != String.Empty)
            {
                Autologin.SetActive(true);
                var request = new LoginWithPlayFabRequest
                {
                    Username = saveUser,
                    Password = savePass,
                    TitleId = PlayFabSettings.TitleId
                };

                //DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.GenericLogin);
                PlayFabClientAPI.LoginWithPlayFab(request, OnLoginSuccess, OnLoginError);
            }else if (saveUser != String.Empty && savePass == String.Empty)
            {
                Autologin.SetActive(true);
                LoginWithDeviceId();
            }
        }
    }


    /// <summary>
    /// Registers the new PlayFab account.
    /// </summary>
    public void RegisterNewPlayfabAccount()
    {
        if (user.text.Length == 0 || pass1.text.Length == 0 || pass2.text.Length == 0)
        {

            //OnLoginFail("All fields are required.", MessageDisplayStyle.error);
            canvas.CallNotification("All field is need", 2f);
            
            return;
        }

        var passwordCheck = ValidatePassword(pass1.text, pass2.text);
        var emailCheck = ValidateEmail(email.text);
        if (!passwordCheck)
        {
            canvas.CallNotification("Pass not match or not enough 5 character", 2f);            
            return;
        }
        else if (!emailCheck)
        {
            canvas.CallNotification("Email incorrect", 2f);            
            return;
        }
        else
        {
            var request = new RegisterPlayFabUserRequest
            {
                TitleId = PlayFabSettings.TitleId,
                Username = user.text,
                Email = email.text,
                Password = pass1.text
            };
            Debug.Log(request);
//            DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.GenericLogin);
            PlayFabClientAPI.RegisterPlayFabUser(request, onSucces => {
                canvas.CallNotification("New account registed", 2f);
                login.SetActive(true);
                reg.SetActive(false);
            } , (PlayFabError error) => 
            {               
                Debug.Log(error.ErrorMessage);
                if(error.Error == PlayFabErrorCode.EmailAddressNotAvailable)
                {
                    canvas.CallNotification("Email has used", 2f);
                }
            });
        }
    }

    public void LoginWithEmail()
    {
        if (loginUser.text.Length > 0 && loginPass.text.Length > 0)
        {
            //LoginMethodUsed = LoginPathways.pf_email;
            var request = new LoginWithPlayFabRequest
            {
                Username = loginUser.text,
                Password = loginPass.text,
                TitleId = PlayFabSettings.TitleId
            };

            PlayerPrefs.SetString("Username", loginUser.text);
            PlayerPrefs.SetString("Password", loginPass.text);
            //DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.GenericLogin);
            PlayFabClientAPI.LoginWithPlayFab(request, OnLoginSuccess, OnLoginError);

        }
        else
        {
            canvas.CallNotification("All field is need", 2f);
        }
    }


    public void LoginWithDeviceId()
    {
        Action<bool> processResponse = (bool response) =>
        {
            if (response && GetDeviceId())
            {
                if (!string.IsNullOrEmpty(android_id))
                {
                    Debug.Log("Using Android Device ID: " + android_id);
                    var request = new LoginWithAndroidDeviceIDRequest
                    {
                        AndroidDeviceId = android_id,
                        TitleId = PlayFabSettings.TitleId,
                        CreateAccount = true
                    };
                    PlayerPrefs.SetString("Username", android_id);
                    PlayerPrefs.SetString("Password", null);
                    PlayFabClientAPI.LoginWithAndroidDeviceID(request, OnLoginSuccess, (PlayFabError error) =>
                    {
                        if (error.Error == PlayFabErrorCode.AccountNotFound)
                        {
                            canvas.CallNotification("Account not found", 2f);
                        }
                        else
                        {
                            OnLoginError(error);
                        }

                    });
                }
                else if (!string.IsNullOrEmpty(ios_id))
                {
                    Debug.Log("Using IOS Device ID: " + ios_id);
                    var request = new LoginWithIOSDeviceIDRequest
                    {
                        DeviceId = ios_id,
                        TitleId = PlayFabSettings.TitleId,
                        CreateAccount = true
                    };

                    PlayerPrefs.SetString("Username", ios_id);
                    PlayerPrefs.SetString("Password", null);
                    // DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.GenericLogin);
                    PlayFabClientAPI.LoginWithIOSDeviceID(request, OnLoginSuccess, (PlayFabError error) =>
                    {
                        if (error.Error == PlayFabErrorCode.AccountNotFound)
                        {
                            canvas.CallNotification("Account not found", 2f);
                        }
                        else
                        {
                            OnLoginError(error);
                        }
                    });
                }
            }
            else
            {
                Debug.Log("Using custom device ID: " + custom_id);
                var request = new LoginWithCustomIDRequest
                {
                    CustomId = custom_id,
                    TitleId = PlayFabSettings.TitleId,
                    CreateAccount = true
                };

                PlayerPrefs.SetString("Username", custom_id);
                PlayerPrefs.SetString("Password", null);
                //DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.GenericLogin);
                PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, error =>
                {
                    if ( error.Error == PlayFabErrorCode.AccountNotFound)
                    {
                        canvas.CallNotification("Account not found", 2f);
                    }
                    else
                    {
                        OnLoginError(error);
                    }
                });
            }
        };

        processResponse(true);
        //DialogCanvasController.RequestConfirmationPrompt("Login With Device ID", "Logging in with device ID has some issue. Are you sure you want to contine?", processResponse);
    }


    /// <summary>
    /// Gets the device identifier and updates the static variables
    /// </summary>
    /// <returns><c>true</c>, if device identifier was obtained, <c>false</c> otherwise.</returns>
    public bool GetDeviceId(bool silent = false) // silent suppresses the error
    {
        if (CheckForSupportedMobilePlatform())
        {
#if UNITY_ANDROID
            //http://answers.unity3d.com/questions/430630/how-can-i-get-android-id-.html
            AndroidJavaClass clsUnity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject objActivity = clsUnity.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject objResolver = objActivity.Call<AndroidJavaObject>("getContentResolver");
            AndroidJavaClass clsSecure = new AndroidJavaClass("android.provider.Settings$Secure");
            android_id = clsSecure.CallStatic<string>("getString", objResolver, "android_id");
#endif

#if UNITY_IPHONE
			ios_id = UnityEngine.iOS.Device.vendorIdentifier;
#endif
            return true;
        }
        else
        {
            custom_id = SystemInfo.deviceUniqueIdentifier;
            return false;
        }
    }

    /// <summary>
    /// Check to see if our current platform is supported (iOS & Android)
    /// </summary>
    /// <returns><c>true</c>, for supported mobile platform, <c>false</c> otherwise.</returns>
    public bool CheckForSupportedMobilePlatform()
    {
        return Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer;
    }


    /// <summary>
    /// Raises the login error event.
    /// </summary>
    /// <param name="error">Error.</param>
    private void OnLoginError(PlayFabError error) //PlayFabError
    {
        Autologin.SetActive(false);
        string errorMessage;
        if (error.Error == PlayFabErrorCode.InvalidParams && error.ErrorDetails.ContainsKey("Password"))
            errorMessage = "Invalid Password";
        else if (error.Error == PlayFabErrorCode.InvalidParams && error.ErrorDetails.ContainsKey("Username") || (error.Error == PlayFabErrorCode.InvalidUsername))
            errorMessage = "Invalid Username";
        else if (error.Error == PlayFabErrorCode.AccountNotFound)
            errorMessage = "Account Not Found, you must have a linked PlayFab account. Start by registering a new account or using your device id";
        else if (error.Error == PlayFabErrorCode.AccountBanned)
            errorMessage = "Account Banned";
        else if (error.Error == PlayFabErrorCode.InvalidUsernameOrPassword)
            errorMessage = "Invalid Username or Password";
        else
            errorMessage = string.Format("Error {0}: {1}", error.HttpCode, error.ErrorMessage);

        //if (OnLoginFail != null)
        //OnLoginFail(errorMessage, MessageDisplayStyle.error);
        canvas.CallNotification(errorMessage, 2f);
        
    }

    public void clear()
    {
        user.text = "";
        email.text = "";
        loginUser.text = "";
        loginPass.text = "";
        pass1.text = "";
        pass2.text = "";
    }

 
    /// Validates the email.
    /// </summary>
    /// <returns><c>true</c>, if email was validated, <c>false</c> otherwise.</returns>
    /// <param name="em">Email address</param>
    public static bool ValidateEmail(string em)
    {
        return Regex.IsMatch(em, emailPattern);
    }

    public static bool ValidatePassword(string p1, string p2)
    {
        return ((p1 == p2) && p1.Length > 5);
    }

    public void OnLoginSuccess(LoginResult result)
    {        
        if (result.NewlyCreated)
        {
            Debug.Log("Create new");
            ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest()
            {
                FunctionName = "makeFirstCall",
                GeneratePlayStreamEvent = true
            };

            PlayFabClientAPI.ExecuteCloudScript(request, success => { }, error => { });
        }
        else
        {
            //SetUserDataStatic();
            Debug.Log("Logined");
        }
        //GameController.RequestRefeshData();
        canvas.LoadScreen();
    }

}
