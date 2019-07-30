using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour {

    private void OnDestroy()
    {
        if (mDontSave)
            return;
        SaveLastSettings();
    }

    public Settings Settings
    {
        get
        {
            if (mSettings == null)
                LoadLastSettings();
            return mSettings;
        }
        private set
        {
            mSettings = value;
        }
    }
    private Settings mSettings;

    private const string LastSettingsPath = "Last.dat";
    private string PresistentDataPath => Application.persistentDataPath;

    [SerializeField]
    private Text mText;

    public void LoadDefaultSettings()
    {
        Settings = new Settings();
    }

    public void LoadLastSettings()
    {
        try
        {
            if (Application.isPlaying)
                mText.text += PresistentDataPath + "/" + LastSettingsPath;
            Settings = MyJsonSerializer.Deserialize<Settings>(File.ReadAllText(PresistentDataPath + "/" + LastSettingsPath));
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            LoadDefaultSettings();
        }
    }

    public void LoadSettings(string path)
    {
        try
        {
            Settings = MyJsonSerializer.Deserialize<Settings>(path);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    public void SaveLastSettings()
    {
        try
        {
            File.WriteAllText(PresistentDataPath + "/" + LastSettingsPath, MyJsonSerializer.Serialize(Settings));
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    public void SaveSettings(string path)
    {
        try
        {
            File.WriteAllText(path, MyJsonSerializer.Serialize(Settings));
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }


    private bool mDontSave = false;


    public void ResetAndRestart()
    {
        mDontSave = true;
        if (File.Exists(PresistentDataPath + "/" + LastSettingsPath))
            File.Delete(PresistentDataPath + "/" + LastSettingsPath);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ResetAndQuit()
    {
        mDontSave = true;
        if (File.Exists(PresistentDataPath + "/" + LastSettingsPath))
            File.Delete(PresistentDataPath + "/" + LastSettingsPath);
        Application.Quit();
    }

    public void RestartWithOutSave()
    {
        mDontSave = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void RestartWithSave()
    {
        mDontSave = true;
        SaveLastSettings();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitWithOutSave()
    {
        mDontSave = true;
        Application.Quit();
    }

    public void QuitWithSave()
    {
        mDontSave = true;
        SaveLastSettings();
        Application.Quit();
    }

}
