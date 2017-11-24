using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager gm_instance = null;

    private void Awake()
    {
        if(gm_instance == null)
        {
            gm_instance = this;
        }
        else if (gm_instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            LoadScene(1);
        }
    }

    public void LoadScene(int scene_id)
    {
        SceneManager.LoadScene(scene_id);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
