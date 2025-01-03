using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shutdown : MonoBehaviour
{
    void Update()
    {
        // Check if the 'Q' key is pressed
        if (Input.GetKeyDown(KeyCode.Q))
        {
            // Quit the application
            Application.Quit();
        }
    }
}
