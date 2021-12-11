using Firebase.Database;
using Firebase.Extensions;
using Firebase.Functions;
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


        public static DatabaseReference firebase()
        {

            var firebase = FirebaseDatabase.DefaultInstance.GetReference("USERS").Child(LogInAndRegister.Instance._User.UserName).Child("farmdata");
            return firebase;
        }





        static UIController uicontroller()
        {
            var uicontroller = FindObjectOfType<UIController>();
            return uicontroller;

        }

        public void OnGrowthCycleCancel()
        {


            var test = FirebaseFunctions.DefaultInstance.GetHttpsCallable("Cancel24HoursCycle");
            test.CallAsync().ContinueWithOnMainThread((response) =>
            {

            });
        }

        public  void Test()
        {
         

            var onSetGrowtnCycle = FirebaseFunctions.DefaultInstance.GetHttpsCallable("GGB");

            onSetGrowtnCycle.CallAsync("test").ContinueWithOnMainThread((response) =>
            {
                Debug.Log("response = " + response.Result.Data.ToString());

                if (response.IsFaulted || response.IsCanceled)
                {
                    Firebase.FirebaseException e = response.Exception.Flatten().InnerExceptions[0] as Firebase.FirebaseException;
                    FunctionsErrorCode error = (FunctionsErrorCode)e.ErrorCode;

                    Debug.LogError("Fault!");
                    Debug.Log("FunctionsErrorCode! = " + error);
                }
                //else
                //{
                //    string returnedName = response.Result.Data.ToString();
                //    if (returnedName == name)
                //    {
                //        //Name already exists in database
                //    }
                //    else if (string.IsNullOrEmpty(returnedName))
                //    {
                //        //Name doesn't exist in database
                //    }
                //}
            });
        }



        public static void OnGrowthCycleStart(string contractid, string fieldid, string dronePlantsCapacity, string droneid)
        {
            var cloudData = new GrowthCycleCloudData();
            cloudData.contractID = contractid;
            cloudData.FieldID = fieldid;
            cloudData.plantsCount = dronePlantsCapacity;
            cloudData.droneID = droneid;

            string json = JsonUtility.ToJson(cloudData);


            var onSetGrowtnCycle = FirebaseFunctions.DefaultInstance.GetHttpsCallable("OnSetGrwothCycle");

            onSetGrowtnCycle.CallAsync(json).ContinueWithOnMainThread((response) =>
            {
                Debug.Log("response = " + response.Result.Data.ToString());

                if (response.IsFaulted || response.IsCanceled)
                {
                    Firebase.FirebaseException e = response.Exception.Flatten().InnerExceptions[0] as Firebase.FirebaseException;
                    FunctionsErrorCode error = (FunctionsErrorCode)e.ErrorCode;

                    Debug.LogError("Fault!");
                    Debug.Log("FunctionsErrorCode! = " + error);
                }
            //else
            //{
            //    string returnedName = response.Result.Data.ToString();
            //    if (returnedName == name)
            //    {
            //        //Name already exists in database
            //    }
            //    else if (string.IsNullOrEmpty(returnedName))
            //    {
            //        //Name doesn't exist in database
            //    }
            //}
        });
        }

    }





    [Serializable]
    public class GrowthCycleCloudData
    {
        public string contractID;
        public string FieldID;
        public string plantsCount;
        public string droneID;
    }
}



