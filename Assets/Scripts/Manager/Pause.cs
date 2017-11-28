using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Pause : MonoBehaviour
{
    [SerializeField] private GameObject pause_panel;
    [SerializeField] private GameObject ui_panel;

    bool is_paused = false;

	// Update is called once per frame
	void Update ()
    {
		if(Input.GetButton("Pause") && !is_paused)
        {
            is_paused = true;
            pause_panel.SetActive(true);
            ui_panel.SetActive(false);
            Time.timeScale = 0;
        }
    }

    public void Resume()
    {
        is_paused = false;
        pause_panel.SetActive(false);
        ui_panel.SetActive(true);
        Time.timeScale = 1;
    }
}
