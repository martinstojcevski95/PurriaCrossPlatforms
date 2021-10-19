using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static System.Net.Mime.MediaTypeNames;

namespace Purria
{
    public class MainManager : MonoBehaviour
    {


        public static event Action<string, float> UIInfoOpen;


        private void OnEnable()
        {
            UIInfoOpen += SetUIInfo;
        }

        private void OnDisable()
        {
            UIInfoOpen -= SetUIInfo;
        }

   

        public static void OnUIInfoOpen(string infoText, float destroyTime)
        {

            UIInfoOpen?.Invoke(infoText, destroyTime);
        }


        void SetUIInfo(string infoText, float destroyTime)
        {
            var infoObject = Instantiate(uicontroller().InfoDialogCanvas, new Vector3(0, 180, 0), Quaternion.identity);
            if (infoObject != null)
            {
                var info = infoObject.GetComponent<Info>();
                info.InfoDataSet(infoText, infoObject.gameObject, destroyTime);

            }
        }


        public  static DatabaseReference firebase()
        {
            var firebase = FirebaseDatabase.DefaultInstance.GetReference("USERS").Child(LogInAndRegister.Instance._User.UserName).Child("farmdata");
            return firebase;
        }





        static UIController uicontroller()
        {
            var uicontroller = FindObjectOfType<UIController>();
            return uicontroller;

        }
    }


}


