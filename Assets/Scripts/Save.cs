using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class Save
{
    public static SaveData current;
    static string savePath;

    public static readonly int MAX_LEVEL = 4;
    public static readonly float MAX_VOLUME = 20;
    public static readonly float MIN_VOLUME = -80;

    //bool oznacava dali je postojao save (true) ili se novi napravio (false)
    public static bool Init()
    {
        savePath = Application.persistentDataPath + "/save.bin";
        if (File.Exists(savePath))
        {
            FileStream fs = new FileStream(savePath, FileMode.Open);
            BinaryFormatter bf = new BinaryFormatter();
            current = (SaveData)bf.Deserialize(fs);
            fs.Close();
            return true;
        }
        else
        {
            current = new SaveData()
            {
                mapName = "",
                playerPositionX = 0,
                playerPositionY = 0,
                combatData = new CombatData()
                {
                    currentHealth = 5,
                    maxHealth = 5,
                    currentExp = 0,
                    nextLevelExp = 100,
                    currentLevel = 1,
                    selectedMagic = MagicType.Fire,
                    magic = new MagicData[]
                    {
                        new MagicData(){type = MagicType.Fire,      amount = 5},
                        new MagicData(){type = MagicType.Lightning, amount = 0},
                        new MagicData(){type = MagicType.Air,       amount = 0},
                        new MagicData(){type = MagicType.Water,     amount = 0},
                    },
                    enabledMagics = new bool[]
                    {
                        true,       //Fire
                        false,
                        false,
                        false
                    }
                },
                settings = new Settings()
                {
                    hotbarKeyCodes = new KeyCode[] { KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R },
                    inventoryKeyCode = KeyCode.I,
                    soundLevel = 1
                }
            };
            return false;
        }
    }

    public static void SaveCurrent()
    {
        if (current.combatData.currentHealth <= 0) return;
        BinaryFormatter bf = new BinaryFormatter();
        FileStream fs = new FileStream(savePath, FileMode.Create);
        bf.Serialize(fs, current);
        fs.Close();
    }

    public static void LevelUp()
    {
        current.combatData.nextLevelExp += 100;

        current.combatData.enabledMagics[current.combatData.currentLevel] = true;
        current.combatData.currentLevel += 1;

        current.combatData.maxHealth += 5;
        current.combatData.currentHealth = current.combatData.maxHealth;

        Game.instance.playerCombat.PlayLevelUpSound();
    }

    [Serializable]
    public struct SaveData
    {
        public string mapName;
        public float playerPositionX;
        public float playerPositionY;
        public CombatData combatData;
        public Settings settings;
    }

    [Serializable]
    public struct CombatData
    {
        public int currentHealth;
        public int maxHealth;
        public int currentExp;
        public int nextLevelExp;
        public int currentLevel;
        public MagicType selectedMagic;
        public MagicData[] magic;
        public bool[] enabledMagics;
    }

    [Serializable]
    public struct MagicData
    {
        public MagicType type;
        public int amount; //Koliko igrac ima mane za odredeni type
    }

    [Serializable]
    public struct Settings
    {
        public KeyCode[] hotbarKeyCodes;
        public KeyCode inventoryKeyCode;
        public float soundLevel;
    }

    public enum MagicType
    {
        Fire,
        Lightning,
        Air,
        Water
    }
}
