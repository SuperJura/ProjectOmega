using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

    Button btnResumeGame;
    string savePath;

	// Use this for initialization
	void Start () {
        btnResumeGame = GameObject.Find("Canvas/Buttons/Resume").GetComponent<Button>();
        savePath = Application.persistentDataPath + "/save.bin";
        if (!File.Exists(savePath)) btnResumeGame.gameObject.SetActive(false);
	}

    public void StartGame(bool newGame)
    {
        if (newGame && File.Exists(savePath)) File.Delete(savePath);
        SceneManager.LoadScene(1);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void HideItem(GameObject item)
    {
        item.SetActive(false);
    }

    public void ShowItem(GameObject item)
    {
        item.SetActive(true);
    }
}
