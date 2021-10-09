using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Unity.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LogInAndRegister : MonoBehaviour
{

    [Header("Login And Register")]
    public InputField RegistrationEmail, RegistrationPassword, LogInEmail, LogInPassword;
    public string userID;
    public string UserName;
    public User _User;

    public static LogInAndRegister Instance;

    public string user, password;
    public int AutoLogIn;
    bool isfailed;

    [SerializeField] Toggle toggle;
    [SerializeField] Text LOGTEXT;


    void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

      //  SetGameViewScale();
        CheckForRememberedLogIn();
    }

    // Start is called before the first frame update
    void Start()
    {

        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl(DATABASEURL);
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        Firebase.FirebaseApp.Create();
    }




    
    void CheckForRememberedLogIn()
    {

        if (PlayerPrefs.HasKey("autolog"))
        {
            AutoLogIn = PlayerPrefs.GetInt("autolog");
            LogInEmail.text = PlayerPrefs.GetString("user");
            LogInPassword.text = PlayerPrefs.GetString("password");
            toggle.isOn = true;
        }
    }

    public void RememberMe(Toggle toggle)
    {
        if (toggle.isOn)
            SetLogInCreds();
        else
            RemoveLogInCreds();
    }

    // Update is called once per frame
    void Update()
    {
        if (isfailed)
        {
            if (LogInEmail.text == "" && LogInPassword.text == "")
                RegisterAndLogInLogInfo("Please enter email and password");
            else
                RegisterAndLogInLogInfo("wrong credentials,try again");
        }
    }

    void RegisterAndLogInLogInfo(string log)
    {
        UIController.Instance.DisplayAutomaticLogText(2f, log);
        isfailed = false;
    }



    /// <summary>
    /// Registering a user in the firebase db
    /// </summary>
    public void Registration()
    {


        auth.CreateUserWithEmailAndPasswordAsync(RegistrationEmail.text, RegistrationPassword.text).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }

            // Firebase user has been created.
            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);
            UserName = newUser.UserId;

            SetUserToDB(newUser.UserId);
            UIController.Instance.CloseRegister();
            UIManager.Instance.OpenLogin();

        });
    }



    /// <summary>
    /// Login a user with the registered credentials from the db
    /// </summary>
    public void LogIn()
    {

        auth.SignInWithEmailAndPasswordAsync(LogInEmail.text, LogInPassword.text).ContinueWith(task =>
              {

                  if (task.IsCanceled)
                  {
                      Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                      return;
                  }

                else if (task.IsFaulted)
                  {
                      //  Debug.Log(task.Exception.InnerException.GetBaseException().Message);
                      isfailed = true;
                      return;
                  }


                else if (task.IsCompleted)
                  {
                      Firebase.Auth.FirebaseUser newUser = task.Result;
                      Debug.LogFormat("User signed in successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);
                      UserName = newUser.UserId;
                       ContractController.Instance.GetDataForAllContracts();
                        ContractController.Instance.GridsData();

                  }
    
              });
    }


    /// <summary>
    /// Set the user into the db
    /// </summary>
    /// <param name="userID"></param>
    public void SetUserToDB(string userName)
    {
        User user = new User();
        user.UserName = userName;
        user.UserID = userID;
        string ToJson = JsonUtility.ToJson(user);
        reference.Child("USER").SetValueAsync(ToJson);
    }

    /// <summary>
    /// Set the credentials to be remembered for every next log in
    /// </summary>
    void SetLogInCreds()
    {

        PlayerPrefs.SetString("user", LogInEmail.text);
        PlayerPrefs.SetString("password", LogInPassword.text);
        PlayerPrefs.SetInt("autolog", AutoLogIn = 1);

    }
    /// <summary>
    /// Remove the current credentials
    /// </summary>
    void RemoveLogInCreds()
    {
        PlayerPrefs.DeleteKey("user");
        PlayerPrefs.DeleteKey("password");
        PlayerPrefs.DeleteKey("autolog");
        AutoLogIn = 0;
        toggle.isOn = false;
    }



#if UNITY_EDITOR

    void SetGameViewScale()
    {
        System.Reflection.Assembly assembly = typeof(UnityEditor.EditorWindow).Assembly;
        System.Type type = assembly.GetType("UnityEditor.GameView");
        UnityEditor.EditorWindow v = UnityEditor.EditorWindow.GetWindow(type);

        var defScaleField = type.GetField("m_defaultScale", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

        //whatever scale you want when you click on play
        float defaultScale = 0.1f;

        var areaField = type.GetField("m_ZoomArea", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        var areaObj = areaField.GetValue(v);

        var scaleField = areaObj.GetType().GetField("m_Scale", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        scaleField.SetValue(areaObj, new Vector2(defaultScale, defaultScale));
    }
#endif

    /// <summary>
    /// User Class
    /// </summary>
    [Serializable]
    public class User
    {
        public string UserName;
        public string UserID;
    }


    //PRIVATE VARIABLES
    DatabaseReference reference;
    string DATABASEURL = "https://purriafresh.firebaseio.com/";
    FirebaseAuth auth;
}
