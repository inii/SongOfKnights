using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    //void Start()
    //{
    //    IEnumerator enumerator = CoroutineFunc2();
    //    Coroutine coroutine = StartCoroutine(enumerator);
    //    Debug.Log("Test start...");

    //    StopCoroutine(coroutine);
    //}

    //// Update is called once per frame
    //void Update()
    //{
        
    //}

    //IEnumerator CoroutineFunc1()
    //{
    //    Debug.Log("Waiting 2 second...");
    //    yield return new WaitForSeconds(2);
    //    Debug.Log("Done!!");
    //}

    //IEnumerator CoroutineFunc2()
    //{
    //    Debug.Log("第一次进入");
    //    yield return null;
    //    //yield return new WaitForSeconds(2);
    //    Debug.Log("第二次进入");
    //    yield return null;
    //}

    //Unity使用UnitySynchronizationContext将自动收集每帧排队的任何异步代码，并继续在Unity主线程上运行它们！所以Unity的async和await模式中，默认情况下都是在主线程下执行的！
    //public async void AsycnMethod()
    //{
    //    await return waitMethod;
    //}
    //public async void waitMethod()
    //{
    //    Debug.Log("waiting");
    //}

    //public async void AsyncMethod()
    //{
    //    //
    //    await  ()=> { return Debug.Log("wait ......") };
    //    //
    //}
}
