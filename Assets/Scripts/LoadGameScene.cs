using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadGameScene : MonoBehaviour {

	// Update is called once per frame
	void Update () 
    {
        if (Input.GetKeyDown(KeyCode.Return))
            SceneManager.LoadScene("default");
	}
}
