using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TestDelegate2 : MonoBehaviour
{
    public TestDelegate1 Delegate1;
    
    private void Start()
    {
        if (Delegate1)
            Delegate1.ShowMessageEvent += ShowMessage;
    }
    
    private void OnDestroy()
    {
        if (Delegate1)
            Delegate1.ShowMessageEvent -= ShowMessage;
    }

    public void ShowMessage(string message)
    {
        Debug.Log(message);
    }
    
    void OnMouseDown()
    {
        if (Delegate1 != null && Delegate1.Del != null)
            Delegate1.Del("Delegate from TestDelegate2");
    }
}
