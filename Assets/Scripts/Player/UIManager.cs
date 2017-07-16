using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public float InGameUIFadeTime;
    public EventSystem eventSystem;

    public CanvasGroup inGameUI;
    public CanvasGroup escapeUI;
    public CanvasGroup diedUI;
    public RectTransform canvasRect;
    public Image transitionImage;

    Transform[] magicsSprites;
    Transform[] magicsButtonSprites;
    Image[] timeToNextAmmo;

    public void Init()
    {

        magicsSprites = new Transform[]
        {
            inGameUI.transform.Find("Hotbar/Magic0"),
            inGameUI.transform.Find("Hotbar/Magic1"),
            inGameUI.transform.Find("Hotbar/Magic2"),
            inGameUI.transform.Find("Hotbar/Magic3"),
        };
        timeToNextAmmo = new Image[Save.current.combatData.magic.Length];
        for (int i = 0; i < Save.current.combatData.magic.Length; i++)
        {
            timeToNextAmmo[i] = magicsSprites[i].Find("Info/Time").GetComponent<Image>();
        }
        
        magicsButtonSprites = new Transform[]
        {
            inGameUI.transform.Find("Hotbar/Magic0/Button"),
            inGameUI.transform.Find("Hotbar/Magic1/Button"),
            inGameUI.transform.Find("Hotbar/Magic2/Button"),
            inGameUI.transform.Find("Hotbar/Magic3/Button"),
        };

        ToggleCanvasGroup(inGameUI, true);
        ToggleCanvasGroup(escapeUI, false);
        if(Save.current.combatData.currentHealth > 0)   ToggleCanvasGroup(diedUI, false);
        else                                            ToggleCanvasGroup(diedUI, true);

        UpdateMagicUI();
        UpdateHealthUI();
        UpdateExpUI();
        UpdateEscapeUI();
        UpdateDiedUI();
    }

    public void Update()
    {
        if (InGameUIFadeTime > 0)
        {
            inGameUI.alpha -= Time.deltaTime;
            if (inGameUI.alpha < 0.5f) inGameUI.alpha = 0.5f;

            InGameUIFadeTime -= Time.deltaTime;
            if (InGameUIFadeTime < 0) InGameUIFadeTime = 0;
        }
        else
        {
            inGameUI.alpha += Time.deltaTime;
            if (inGameUI.alpha > 1) inGameUI.alpha = 0;
        }
        
        for (int i = 0; i < magicsSprites.Length; i++)
        {
            if (i == (int)Save.current.combatData.selectedMagic) magicsButtonSprites[i].Rotate(Vector3.forward, 45 * Time.deltaTime);
            else magicsButtonSprites[i].rotation = Quaternion.identity;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnEscape_Click();
        }
    }

    #region InGameUI
    public void OnMagic_Click(int slotIndex)
    {
        Game.instance.playerCombat.ChangeMagic(slotIndex);
        UpdateMagicUI();
    }

    public void OnEscape_Click()
    {
        if (Save.current.combatData.currentHealth > 0)
        {
            ToggleCanvasGroup(escapeUI, !escapeUI.interactable);
            Game.instance.isPaused = escapeUI.interactable;
            Game.instance.playerControl.ToggleWalkingSound(false);
        }
    }

    public void UpdateMagicUI()
    {
        for (int i = 0; i < 4; i++)
        {
            if (!Save.current.combatData.enabledMagics[i])  magicsSprites[i].Find("Button").GetComponent<Button>().interactable = false;
            else                                            magicsSprites[i].Find("Button").GetComponent<Button>().interactable = true;
            string amountDisplayText = Save.current.combatData.magic[i].amount.ToString();
            magicsSprites[i].Find("Info/Amount").GetComponent<Text>().text = amountDisplayText;
            magicsSprites[i].Find("Info/Keybind").GetComponentInChildren<Text>().text = Save.current.settings.hotbarKeyCodes[i].ToString();
            if (i == (int)Save.current.combatData.selectedMagic)    magicsSprites[i].localScale = new Vector3(1.1f, 1.1f, 1.1f);
            else                                                    magicsSprites[i].localScale = new Vector3(1f, 1f, 1f);
        }
    }

    public void UpdateNextAmmoUI()
    {
        for (int i = 0; i < timeToNextAmmo.Length; i++)
        {
            timeToNextAmmo[i].fillAmount = Game.instance.playerCombat.timesToNextAmmos[i] / PlayerCombat.MAX_TIME_TO_NEXT_AMMO;
        }
    }

    public void UpdateHealthUI()
    {
        float maxHealth = Save.current.combatData.maxHealth;
        float currentHealth = Save.current.combatData.currentHealth;
        if (currentHealth < 0) currentHealth = 0;
        inGameUI.transform.Find("HPBar/HPBarValue").GetComponent<Image>().fillAmount = currentHealth / maxHealth;
        inGameUI.transform.Find("HPValue").GetComponent<Text>().text = currentHealth + "/" + maxHealth;
    }

    public void UpdateExpUI()
    {
        float maxExp = Save.current.combatData.nextLevelExp;
        float currentExp = Save.current.combatData.currentExp;
        if(Save.current.combatData.currentLevel == Save.MAX_LEVEL)
        {
            maxExp = 0;
            currentExp = 0;
        }
        inGameUI.transform.Find("ExpBar/ExpBarValue").GetComponent<Image>().fillAmount = currentExp / maxExp;
        inGameUI.transform.Find("ExpValue").GetComponent<Text>().text = currentExp + "/" + maxExp;
        inGameUI.transform.Find("CurrentLevel").GetComponent<Text>().text = Save.current.combatData.currentLevel.ToString();
    }

    public void UpdateDiedUI()
    {
        if (Save.current.combatData.currentHealth <= 0)
        {
            ToggleCanvasGroup(escapeUI, false);
            ToggleCanvasGroup(diedUI, true);
        }
    }

    void UpdateEscapeUI()
    {
        escapeUI.transform.Find("VolumeSlider").GetComponent<Slider>().value = Save.current.settings.soundLevel;
    }
    #endregion

    #region EscapeUI
    public void BackBtn_Click()
    {
        ToggleCanvasGroup(escapeUI, false);
        Game.instance.isPaused = false;
    }

    public void VolumeSlider_Changed(Slider sender)
    {

        Save.current.settings.soundLevel = sender.value;

        Game.instance.gameMixer.SetFloat("Volume", sender.value);
    }

    public void SaveGameBtn_Click()
    {
        Save.SaveCurrent();
        ToggleCanvasGroup(escapeUI, false);
        Game.instance.isPaused = false;
    }

    public void ExitMenuBtn_Click()
    {
        Save.SaveCurrent();
        SceneManager.LoadScene(0);
    }

    public void ExitGameBtn_Click()
    {
        Application.Quit();
    }
    #endregion

    #region DiedUI
    public void LoadLastSave_Click()
    {
        SceneManager.LoadScene(1);
    }
    #endregion

    void ToggleCanvasGroup(CanvasGroup group, bool enable)
    {
        group.alpha = enable ? 1 : 0;
        group.blocksRaycasts = group.interactable = enable;
    }
}