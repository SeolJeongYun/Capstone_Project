using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loading : MonoBehaviour
{
    public GameObject loadingPanel;
    public GameObject InterfacePanel;
    public GameObject joystick;
    void Update()
    {
        GameObject player1 = GameObject.FindWithTag("Player");
        if(player1 != null)
        {
            loadingPanel.SetActive(false);
            InterfacePanel.SetActive(true);
            joystick.SetActive(true);
        }
    }
}
