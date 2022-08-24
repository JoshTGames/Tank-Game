using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class SettingsMenu : MonoBehaviour{
    [SerializeField] Toggle fullscreenToggle;
    [SerializeField] TMP_Dropdown resolutionDropdown, graphicsDropdown;
    Resolution[] resolutions;

    #region LOADING
    int SetupRes(){
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        resolutions = Screen.resolutions;

        int curIndex = 0;
        bool found = false;
        foreach(Resolution res in resolutions){
            string option = $"{res.width} x {res.height} | {res.refreshRate}hz";
            options.Add(option);            
            if(res.width == resolutions[PlayerPrefs.GetInt(Settings.ResolutionIndex.ToString())].width 
            && res.height == resolutions[PlayerPrefs.GetInt(Settings.ResolutionIndex.ToString())].height 
            && res.refreshRate == resolutions[PlayerPrefs.GetInt(Settings.ResolutionIndex.ToString())].refreshRate 
            && !found){
                found = true;
                resIndex = curIndex;
            }
            else if(res.width == Screen.currentResolution.width 
            && res.height == Screen.currentResolution.height 
            && res.refreshRate == Screen.currentResolution.refreshRate
            && !found)
            {
                found = true; 
                resIndex = curIndex;   
            }
            curIndex++;
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.RefreshShownValue();
        return resIndex;
    }
    private void Start() { LoadSettings(); }
    #endregion

    #region SETTINGS
    bool isFullscreen;
    int resIndex, graphicsIndex;
    #endregion

    public void SetFullscreen(bool _isFullscren){
        isFullscreen = _isFullscren;
        PauseMenuHandler.hasSettingsChanged = true;
    }
    public void SetResolution(int _resIndex){
        resIndex = _resIndex;
        PauseMenuHandler.hasSettingsChanged = true;
    }
    public void SetGraphics(int _graphicsIndex){
        graphicsIndex = _graphicsIndex;
        PauseMenuHandler.hasSettingsChanged = true;
    }



    public void ApplySettings(){
        if(PauseMenuHandler.hasSettingsChanged){ SaveSettings(); }
        PauseMenuHandler.hasSettingsChanged = false;       

        Resolution resolution = resolutions[resIndex];
        Screen.SetResolution(resolution.width, resolution.height, isFullscreen, resolution.refreshRate);
        QualitySettings.SetQualityLevel(graphicsIndex, true);
    }


    public void SaveSettings(){        
        PlayerPrefs.SetString(Settings.IsFullscreen.ToString(), isFullscreen.ToString()); // FULLSCREEN
        PlayerPrefs.SetInt(Settings.GraphicsLevel.ToString(), graphicsIndex); // GRAPHICS
        PlayerPrefs.SetInt(Settings.ResolutionIndex.ToString(), resIndex); // RESOLUTION 
    }
    public void LoadSettings(){
        isFullscreen = (PlayerPrefs.GetString(Settings.IsFullscreen.ToString()) == "False")? false : true;        
        fullscreenToggle.isOn = isFullscreen;

        
        resolutionDropdown.value = SetupRes(); 
        graphicsIndex = (PlayerPrefs.HasKey(Settings.GraphicsLevel.ToString())) ? PlayerPrefs.GetInt(Settings.GraphicsLevel.ToString()) : QualitySettings.GetQualityLevel();
        graphicsDropdown.value = graphicsIndex;

        ApplySettings();
    }

    enum Settings{
        IsFullscreen,
        GraphicsLevel,
        ResolutionIndex
    } 
}
