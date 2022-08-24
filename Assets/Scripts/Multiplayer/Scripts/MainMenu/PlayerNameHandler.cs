using UnityEngine;
using TMPro;
using UnityEngine.UI;


namespace PopEm.Multiplayer.Lobby{ 
    public class PlayerNameHandler : MonoBehaviour{
        [SerializeField] TMP_InputField inputField;
        [SerializeField] Button confirmButton;


        public static string DISPLAY_NAME { get; private set; }  
    
    
        private void Start() => SetupInputField();



        string playerPrefsKeys = "PlayerName";
        void SetupInputField(){
            if (!PlayerPrefs.HasKey(playerPrefsKeys)) { return; }
            string savedName = PlayerPrefs.GetString(playerPrefsKeys);

            inputField.text = savedName;
            IsValidName(savedName);
        }


        public void IsValidName(string name) => confirmButton.interactable = !string.IsNullOrEmpty(name);


        public void SaveName(){
            DISPLAY_NAME = inputField.text;
            PlayerPrefs.SetString(playerPrefsKeys, DISPLAY_NAME);
        }
    }
}
