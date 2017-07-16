using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Game : MonoBehaviour
{
    public string firstLevel;
    public AudioSource levelMusic;
    public AudioMixer gameMixer;

    public static Game instance;

    [HideInInspector] public bool isPaused;
    [HideInInspector] public Level currentLevel;
    [HideInInspector] public PlayerCombat playerCombat;
    [HideInInspector] public PlayerControl playerControl;
    [HideInInspector] public UIManager uiManager;
    [HideInInspector] public PostEffects postEffectManager;

    bool doChangeLevel;
    string nextLevelName;

    void Start()
    {
        instance = this;
        GameObject playerGo = Instantiate(Resources.Load<GameObject>("Player"));
        playerCombat = playerGo.GetComponent<PlayerCombat>();
        playerControl = playerGo.GetComponent<PlayerControl>();
        uiManager = GetComponent<UIManager>();
        postEffectManager = playerGo.transform.Find("Camera").GetComponent<PostEffects>();

        bool saveExists = Save.Init();
        if (saveExists) ChangeLevel(Save.current.mapName, false);
        else ChangeLevel(firstLevel, true);

        playerControl.Init();
        playerCombat.Init();
        uiManager.Init();
        postEffectManager.Init();
        gameMixer.SetFloat("Volume", Save.current.settings.soundLevel);
        if (Save.current.combatData.currentHealth <= 0) isPaused = true;

    }

    private void Update()
    {
        if (doChangeLevel)
        {
            Color color = uiManager.transitionImage.color;
            color.a += Time.deltaTime;
            uiManager.transitionImage.color = color;
            levelMusic.volume -= Time.deltaTime;
            if(color.a >= 1)
            {
                ChangeLevel(nextLevelName, true);
                doChangeLevel = false;
            }
        }
        else if(uiManager.transitionImage.color.a > 0)
        {
            Color color = uiManager.transitionImage.color;
            color.a -= Time.deltaTime;
            uiManager.transitionImage.color = color;
            levelMusic.volume += Time.deltaTime;
        }
    }

    private void OnDisable()
    {
        if (nextLevelName == "End") return;
        Save.SaveCurrent();
    }

    private void ChangeLevel(string name, bool changePosition)
    {
        if (nextLevelName == "End")
        {
            System.IO.File.Delete(Application.persistentDataPath + "/save.bin");
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
        else
        {
            if (name != currentLevel.name) currentLevel.gameObject.SetActive(false);
            currentLevel = ((GameObject)Instantiate(Resources.Load("Levels/" + name))).GetComponent<Level>();
            currentLevel.gameObject.SetActive(true);
            Save.current.mapName = name;
            if (changePosition) playerControl.transform.position = currentLevel.spawnPoint.position;
            else playerControl.transform.position = new Vector3(Save.current.playerPositionX, Save.current.playerPositionY, transform.position.z);

            levelMusic.clip = currentLevel.levelMusic;
            levelMusic.Play();
        }
    }

    public void StartChangingLevel(string name)
    {
        doChangeLevel = true;
        nextLevelName = name;
    }

    public void ChangeMusic(AudioClip newMusic)
    {
        levelMusic.clip = newMusic;
        levelMusic.Play();
    }

    public void ResetMusic()
    {
        levelMusic.clip = currentLevel.levelMusic;
        levelMusic.Play();
    }
}