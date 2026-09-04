using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


// Объявление делегата
public delegate void Notify(string message);

public class TestDelegate1 : MonoBehaviour
{
    public event Notify ShowMessageEvent;
    [System.NonSerialized] public Notify Del = null;
    
    private void Start()
    {
        // Использование
        Del = ShowMessage;
        Del("Delegate at start"); // Вызовет метод ShowMessage
    }

     private void ShowMessage(string message)
     {
         Debug.Log(message);
     }

     void OnMouseDown()
     {
         //Debug.Log("OnMousUp");
         ShowMessageEvent?.Invoke("Event on Mouse Up");
     }
}
