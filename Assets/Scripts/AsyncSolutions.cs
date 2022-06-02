using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class AsyncSolutions : MonoBehaviour
{
    public IEnumerator TestCoroutine()
    {
        Uri URL = new Uri($"YourURLHere");
        using (UnityWebRequest www = UnityWebRequest.Get(URL))
        {
            yield return www.SendWebRequest();

            // Check result, do stuff, auto-disposed in the end
        }
    }

    public async void TestAsync1()
    {
        Uri URL = new Uri($"YourURLHere");
        UnityWebRequest www = UnityWebRequest.Get(URL);
        www.SetRequestHeader("Content-Type", "application/json");

        var operation = www.SendWebRequest();
        while (!operation.isDone)
            await Task.Yield();

        // Check result and do stuff, 

        www.Dispose();
    }

    public async void TestAsync1Var2()
    {
        Uri URL = new Uri($"YourURLHere");
        using (UnityWebRequest www = UnityWebRequest.Get(URL))
        {
            www.SetRequestHeader("Content-Type", "application/json");

            var request = www.SendWebRequest();
            while (!request.isDone)
                await Task.Yield();

            // Check result and do stuff
        }
    }

    public void TestAsync2()
    {
        Uri URL = new Uri($"YourURLHere");

        // Must be privately stored to use its info on completion

        UnityWebRequest www = UnityWebRequest.Get(URL);
        UnityWebRequestAsyncOperation request = www.SendWebRequest();
        request.completed += TestRequestDone;

    }

    private void TestRequestDone(AsyncOperation obj)
    {
        // Do stuff here, ask privately stored WebRequest for results
        //Dispose the item here

    }
}
