using UnityEngine;
using UnityEngine.UI;

public class ToastMessage : MonoBehaviour
{
    public static ToastMessage Instance { get; private set; }

    string _message;
    AndroidJavaObject currentActivity;
    AndroidJavaClass unityPlayerClass;
    AndroidJavaObject context;
    AndroidJavaClass toastClass;

    AndroidJavaObject _toastObject;

    void Start()
    {
        // if (Application.platform == RuntimePlatform.Android)
        // {
        //     unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        //     currentActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
        //     context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");
        //     toastClass = new AndroidJavaClass("android.widget.Toast");
        // }
    }

    public void ShowToastMessage(string message)
    {
        // if (Application.platform == RuntimePlatform.Android)
        // {
        //     _message = message;
        //     currentActivity.Call("runOnUiThread", new AndroidJavaRunnable(showToast));
        // }
    }

    public void CancelToast()
    {
        // if (_toastObject != null)
        // {
        //     _toastObject.Call("cancel");
        //     _toastObject = null;
        // }
    }

    void showToast()
    {
        // if (_toastObject != null)
        //     _toastObject.Call("cancel");

        // AndroidJavaObject javaString = new AndroidJavaObject("java.lang.String", _message);
        // _toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", context, javaString, toastClass.GetStatic<int>("LENGTH_SHORT"));
        // _toastObject.Call("show");
    }

    private void Awake()
    {
        if (Instance != null)
            DestroyImmediate(Instance);

        Instance = this;
    }

    private void OnDestroy()
    {
        // if (_toastObject != null)
        // {
        //     _toastObject.Call("cancel");
        //     _toastObject = null;
        // }
    }
}