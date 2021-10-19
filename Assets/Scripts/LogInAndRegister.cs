using DG.Tweening;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Unity.Editor;
using Purria;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class LogInAndRegister : MonoBehaviour
{


    [Header("Login And Register")]
    public InputField RegistrationEmail, RegistrationPassword, LogInEmail, LogInPassword;

    public User _User;

    public static LogInAndRegister Instance;

    public int AutoLogIn;

    [SerializeField] Toggle toggle;
    [SerializeField] Button logInBtn;


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



    /// <summary>
    /// Remember creds
    /// </summary>
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

    /// <summary>
    /// Remember me toggle
    /// </summary>
    /// <param name="toggle"></param>
    public void RememberMe(Toggle toggle)
    {
        if (toggle.isOn)
            SetLogInCreds();
        else
            RemoveLogInCreds();
    }


    /// <summary>
    /// Firebase log in
    /// </summary>
    /// <param name="_email"></param>
    /// <param name="_password"></param>
    /// <returns></returns>
    private IEnumerator Login(string _email, string _password)
    {
        var LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);

        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

        if (LoginTask.Exception != null)
        {
            //If there are errors handle them
            Debug.LogWarning(message: $"Failed to register task with {LoginTask.Exception}");
            FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "Login Failed!";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Missing Email";
                    break;
                case AuthError.MissingPassword:
                    message = "Missing Password";
                    break;
                case AuthError.WrongPassword:
                    message = "Wrong Password";
                    break;
                case AuthError.InvalidEmail:
                    message = "Invalid Email";
                    break;
                case AuthError.UserNotFound:
                    message = "Account does not exist";
                    break;
                case AuthError.UnverifiedEmail:
                    message = "Please Verify Your Email";
                    break;
            }
            MainManager.OnUIInfoOpen(message, 1f);

        }
        else
        {
            Debug.Log("user logged in");
            FirebaseUser user = LoginTask.Result;
            OnLoginResopnse(user);
        }
    }


    /// <summary>
    /// Firebase register
    /// </summary>
    /// <param name="_email"></param>
    /// <param name="_password"></param>
    /// <returns></returns>
    private IEnumerator Register(string _email, string _password)
    {
        var LoginTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);

        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

        if (LoginTask.Exception != null)
        {
            //If there are errors handle them
            Debug.LogWarning(message: $"Failed to register task with {LoginTask.Exception}");
            FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "Login Failed!";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Missing Email";
                    break;
                case AuthError.MissingPassword:
                    message = "Missing Password";
                    break;
                case AuthError.WrongPassword:
                    message = "Wrong Password";
                    break;
                case AuthError.InvalidEmail:
                    message = "Invalid Email";
                    break;
                case AuthError.EmailAlreadyInUse:
                    message = "Email Already In Use";
                    break;

            }
            MainManager.OnUIInfoOpen(message, 1f);

        }
        else
        {
            FirebaseUser user = LoginTask.Result;
            SetUserToDB(user.UserId);
            UIController.Instance.CloseRegister();
            UIManager.Instance.OpenLogin();
            user.SendEmailVerificationAsync().ContinueWith(t =>
            {
 
            });
            MainManager.OnUIInfoOpen("Please go and  verify your email address", 3f);
        };
    
    }


    /// <summary>
    /// Login a user with the registered credentials from the db
    /// </summary>
    public void LogIn()
    {
        StartCoroutine(Login(LogInEmail.text, LogInPassword.text));
    }


    /// <summary>
    /// Regster a user with the registered credentials from the db
    /// </summary>
    public void SignUp()
    {
        StartCoroutine(Register(RegistrationEmail.text, RegistrationPassword.text));
    }


    void OnLoginResopnse(FirebaseUser user)
    {
        var checkVeriication = user.IsEmailVerified;
       if(checkVeriication == false) { MainManager.OnUIInfoOpen("Email not verified!", 3f);  return; }
        _User.UserName = user.UserId;
        ContractController.Instance.GetDataForAllContracts();
        ContractController.Instance.GridsData();
        UIController.Instance.OpenDashBoard();
    }


    /// <summary>
    /// Set the user into the db
    /// </summary>
    /// <param name="userID"></param>
    public void SetUserToDB(string userName)
    {
        User user = new User();
        user.UserName = userName;
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
    }


    //PRIVATE VARIABLES
    DatabaseReference reference;
    string DATABASEURL = "https://purriafresh.firebaseio.com/";
    FirebaseAuth auth;
}
