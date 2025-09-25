using System;
using UnityEngine;

namespace TJ.Scripts
{
    public class EnergyManager : Singleton<EnergyManager>
    {
    private const int MAX_ENERGY = 50;
    private const int ENERGY_RECOVERY_TIME = 1200; // 20分钟 = 1200秒

    private int currentEnergy;
    private DateTime lastEnergyUpdate;

    private void Start()
    {
        //PlayerPrefs.DeleteKey(PlayerPrefsManager.PlayerEnergy);
        LoadEnergyData();
        UpdateEnergyText();
        InvokeRepeating(nameof(RecoverEnergy), 1f, 1f); // 每秒检查一次体力恢复
    }

    private void LoadEnergyData()
    {
        // 加载当前体力值
        currentEnergy = PlayerPrefs.GetInt(PlayerPrefsManager.PlayerEnergy, 50); // 默认满体力

        // 加载上次体力更新时间
        string lastUpdateString = PlayerPrefs.GetString(PlayerPrefsManager.EnergyLastUpdate, "");
        if (!string.IsNullOrEmpty(lastUpdateString))
        {
            if (DateTime.TryParse(lastUpdateString, out DateTime parsedTime))
            {
                lastEnergyUpdate = parsedTime;
                // 计算离线恢复的体力
                CalculateOfflineEnergyRecovery();
            }
            else
            {
                lastEnergyUpdate = DateTime.Now;
            }
        }
        else
        {
            lastEnergyUpdate = DateTime.Now;
        }

        // 确保体力不超过上限
        if (currentEnergy > MAX_ENERGY)
            currentEnergy = MAX_ENERGY;
    }

    private void CalculateOfflineEnergyRecovery()
    {
        DateTime now = DateTime.Now;
        TimeSpan timePassed = now - lastEnergyUpdate;
        int energyToAdd = (int)(timePassed.TotalSeconds / ENERGY_RECOVERY_TIME);

        if (energyToAdd > 0)
        {
            currentEnergy = Mathf.Min(currentEnergy + energyToAdd, MAX_ENERGY);
            lastEnergyUpdate = now;
            SaveEnergyData();
        }
    }

    private void RecoverEnergy()
    {
        DateTime now = DateTime.Now;
        TimeSpan timePassed = now - lastEnergyUpdate;

        if (timePassed.TotalSeconds >= ENERGY_RECOVERY_TIME && currentEnergy < MAX_ENERGY)
        {
            currentEnergy = Mathf.Min(currentEnergy + 1, MAX_ENERGY);
            lastEnergyUpdate = now;
            SaveEnergyData();
            UpdateEnergyText();
        }
    }

    private void SaveEnergyData()
    {
        PlayerPrefs.SetInt(PlayerPrefsManager.PlayerEnergy, currentEnergy);
        PlayerPrefs.SetString(PlayerPrefsManager.EnergyLastUpdate, lastEnergyUpdate.ToString());
        PlayerPrefs.Save();
    }

    public bool UseEnergy()
    {
        if (currentEnergy >= 5)
        {
            currentEnergy -= 5;
            lastEnergyUpdate = DateTime.Now; // 重置恢复计时
            SaveEnergyData();
            UpdateEnergyText();
            return true;
        }
        return false; // 体力不足
    }

    public void AddEnergy(int amount=5)
    {
        currentEnergy = Mathf.Min(currentEnergy + amount, MAX_ENERGY);
        SaveEnergyData();
        UpdateEnergyText();
    }

    public int GetCurrentEnergy()
    {
        return currentEnergy;
    }

    public int GetMaxEnergy()
    {
        return MAX_ENERGY;
    }

    public float GetEnergyRecoveryProgress()
    {
        DateTime now = DateTime.Now;
        TimeSpan timePassed = now - lastEnergyUpdate;
        return Mathf.Clamp01((float)(timePassed.TotalSeconds / ENERGY_RECOVERY_TIME));
    }

    public TimeSpan GetTimeUntilNextEnergy()
    {
        DateTime now = DateTime.Now;
        TimeSpan timePassed = now - lastEnergyUpdate;
        TimeSpan timeUntilNext = TimeSpan.FromSeconds(ENERGY_RECOVERY_TIME) - timePassed;
        return timeUntilNext > TimeSpan.Zero ? timeUntilNext : TimeSpan.Zero;
    }

    private void UpdateEnergyText()
    {
        if (EnergyPanel.instance != null)
        {
            EnergyPanel.instance.UpdateEnergyText(currentEnergy);
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (!pause) // 恢复游戏时重新加载数据
        {
            LoadEnergyData();
            UpdateEnergyText();
        }
        else // 暂停游戏时保存数据
        {
            SaveEnergyData();
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus) // 恢复焦点时重新加载数据
        {
            LoadEnergyData();
            UpdateEnergyText();
        }
        else // 失去焦点时保存数据
        {
            SaveEnergyData();
        }
    }

    private void OnApplicationQuit()
    {
        SaveEnergyData();
    }

    private void OnDestroy()
    {
        SaveEnergyData();
    }
}
}

