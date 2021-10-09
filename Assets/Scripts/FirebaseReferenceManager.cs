using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Unity.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseReferenceManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl(DATABASEURL);
        reference = FirebaseDatabase.DefaultInstance.RootReference;


    }

    // Update is called once per frame
    void Update()
    {
        
    }



    //PUBLIC VARIABLES
    public static DatabaseReference reference;
    string DATABASEURL = "https://purriafresh.firebaseio.com/";
    public FirebaseAuth auth;
}
