using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine : MonoBehaviour
{

    void InitGame()
    {
        print(Utils.Floor(1.0f));
        print(Utils.Floor(2.1f));
        print(Utils.Floor(3.2f));
        print(Utils.Floor(4.3f));
        print(Utils.Floor(5.4f));
        print(Utils.Floor(6.5f));
        print(Utils.Floor(7.6f));
        print(Utils.Floor(8.7f));
        print(Utils.Floor(9.8f));
        print(Utils.Floor(0.9f));
    }

    // Start is called before the first frame update
    void Start()
    {
        InitGame();      
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
