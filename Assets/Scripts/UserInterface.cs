using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInterface : MonoBehaviour
{
    PlayerControls controls;

    bool exitGame;

    private void Start()
    {
        controls = new PlayerControls();
        controls.Enable();
    }

    private void Update()
    {
        ReadInput();

        if (exitGame)
            Application.Quit();
    }

    void ReadInput()
    {
        exitGame = controls.UserInterface.ExitGame.IsPressed();
    }
}
