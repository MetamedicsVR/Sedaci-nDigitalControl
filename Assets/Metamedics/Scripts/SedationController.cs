using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SedationController : MonoBehaviour
{
    [Header("Setup")]
    public GameObject setupView;
    public TMP_InputField roomNameText;
    public TMP_InputField seahorsesTimesText;
    public TMP_InputField blowfishesTimesText;
    public Button confirmButton;

    private int seahorsesTimes;
    private int blowfishesTimes;
    private const int defaultSeahorsesTimes = 5;
    private const int defaultBlowfishTimes = 5;
    public const string seahorsesTimesKey = "KEY_SEAHORSES_TIMES";
    public const string blowfishesTimesKey = "KEY_BLOWFISH_TIMES";

    [Header("Connecting")]
    public GameObject connectingView;
    public TextMeshProUGUI connectingStatusText;

    private bool gotRoomNames;

    [Header("Playing")]
    public GameObject playingView;
    public Toggle showSeahorsesToggle;
    public Toggle showBlowfishesToggle;
    public Toggle spanishToggle;
    public Toggle englishToggle;
    public Button startExperienceButton;
    public Button distractionButton;
    public Button endExperienceButton;

    private bool showSeahorses;
    private bool showBlowfishes;
    public const string seahorsesShowKey = "KEY_SEAHORSES_SHOW";
    public const string blowfishesShowKey = "KEY_BLOWFISH_SHOW";

    private List<float> settingsButtonPressed = new List<float>();

    private enum View
    {
        Setup,
        Connecting,
        Playing
    }

    private void Start()
    {
        NetworkManager.GetInstance().EventConnected += () => UpdateConnectingStatusText("Connected to server");
        NetworkManager.GetInstance().EventJoinedLobby += () => UpdateConnectingStatusText("Connected to lobby");
        NetworkManager.GetInstance().EventRoomListUpdated += () => GotRoomNames();
        NetworkManager.GetInstance().EventCreatedRoom += (name) => UpdateConnectingStatusText("Waiting for headset");
        NetworkManager.GetInstance().EventJoinRoom += OpenPlaying;
        NetworkManager.GetInstance().EventOtherEnteredRoom += OpenPlaying;
        NetworkManager.GetInstance().EventCreateRoomFailed += () => RetryCreateOrJoin("Room creation failed");
        NetworkManager.GetInstance().EventJoinRoomFailed += () => RetryCreateOrJoin("Room join failed");

        showSeahorsesToggle.gameObject.SetActive(false);
        showBlowfishesToggle.gameObject.SetActive(false);
        seahorsesTimes = PlayerPrefs.GetInt(seahorsesTimesKey, defaultSeahorsesTimes);
        blowfishesTimes = PlayerPrefs.GetInt(blowfishesTimesKey, defaultBlowfishTimes);
        showSeahorsesToggle.gameObject.SetActive(true);
        showBlowfishesToggle.gameObject.SetActive(true);

        if (NetworkManager.GetInstance().localRoomName != "")
        {
            OpenConnecting();
        }
        else
        {
            OpenSetup();
        }
    }

    private void OpenSetup()
    {
        if (NetworkManager.GetInstance().localRoomName != "")
        {
            roomNameText.text = NetworkManager.GetInstance().localRoomName;
        }
        else
        {
            roomNameText.text = "test";
        }
        seahorsesTimesText.text = seahorsesTimes.ToString();
        blowfishesTimesText.text = blowfishesTimes.ToString();
        OpenView(View.Setup);
    }

    public void ConfirmSettings()
    {
        if (roomNameText.text != "")
        {
            NetworkManager.GetInstance().localRoomName = roomNameText.text;
            string fixedRoomName = Regex.Replace(roomNameText.text, @"[^a-zA-Z0-9]", "");
            PlayerPrefs.SetString(NetworkManager.localRoomNameKey, fixedRoomName);
            string fixedSeahorseTimes = Regex.Replace(seahorsesTimesText.text, @"[^0-9]", "");
            PlayerPrefs.SetInt(seahorsesTimesKey, fixedSeahorseTimes == "" ? 0 : int.Parse(fixedSeahorseTimes));
            string fixedBlowfishesTimes = Regex.Replace(blowfishesTimesText.text, @"[^0-9]", "");
            PlayerPrefs.SetInt(blowfishesTimesKey, fixedBlowfishesTimes == "" ? 0 : int.Parse(fixedBlowfishesTimes));
        }
        OpenConnecting();
    }

    private void OpenConnecting()
    {
        OpenView(View.Connecting);
        connectingView.SetActive(true);
        NetworkManager.GetInstance().Connect();
    }

    private void UpdateConnectingStatusText(string s)
    {
        print(s);
        connectingStatusText.text = s;
    }

    private void GotRoomNames()
    {
        if (!gotRoomNames)
        {
            gotRoomNames = true;
            CreateOrJoinRoom();
        }
    }

    private void CreateOrJoinRoom()
    {
        UpdateConnectingStatusText("Joining");
        NetworkManager.GetInstance().CreateOrJoinRoom();
    }

    private void RetryCreateOrJoin(string errorText)
    {
        UpdateConnectingStatusText(errorText + ", trying in 5 seconds");
        Invoke(nameof(CreateOrJoinRoom), 5);
    }

    private void OpenPlaying()
    {
        showSeahorses = PlayerPrefs.GetInt(seahorsesShowKey, 1) == 1;
        showBlowfishes = PlayerPrefs.GetInt(blowfishesShowKey, 1) == 1;
        showSeahorsesToggle.isOn = showSeahorses;
        showBlowfishesToggle.isOn = showBlowfishes;
        OpenView(View.Playing);
    }

    private void OpenView(View view)
    {
        setupView.SetActive(false);
        connectingView.SetActive(false);
        playingView.SetActive(false);
        switch (view)
        {
            case View.Setup:
                setupView.SetActive(true);
                break;
            case View.Connecting:
                connectingView.SetActive(true);
                break;
            case View.Playing:
                endExperienceButton.transform.parent.gameObject.SetActive(false);
                distractionButton.transform.parent.gameObject.SetActive(false);
                playingView.SetActive(true);
                break;
        }
    }

    public void ChangeShowSeahorses(bool b)
    {
        showSeahorses = b;
        PlayerPrefs.SetInt(seahorsesShowKey, showSeahorses ? 1 : 0);

    }

    public void ChangeShowBlowfishes(bool b)
    {
        showBlowfishes = b;
        PlayerPrefs.SetInt(blowfishesShowKey, showBlowfishes ? 1 : 0);
    }

    public LanguageManager.Language GetSelectedLanguage()
    {
        if (spanishToggle.isOn) return LanguageManager.Language.Spanish;
        if (englishToggle.isOn) return LanguageManager.Language.English;
        return LanguageManager.GetInstance().GetCurrentLanguage();
    }

    public void StartExperience()
    {
        ExperienceConnector.GetInstance().StartExperience(showSeahorses ? seahorsesTimes : 0, showBlowfishes ? blowfishesTimes : 0, GetSelectedLanguage());
        startExperienceButton.transform.parent.gameObject.SetActive(false);
        distractionButton.transform.parent.gameObject.SetActive(true);
        endExperienceButton.transform.parent.gameObject.SetActive(true);
    }

    public void Distraction()
    {
        ExperienceConnector.GetInstance().Distraction();
    }

    public void EndExperience()
    {
        ExperienceConnector.GetInstance().EndExperience();
    }

    public void SettingsButtonPressed()
    {
        if (playingView.activeSelf)
        {
            settingsButtonPressed.Add(Time.time);
        }
    }

    private void Update()
    {
        if (playingView.activeSelf)
        {
            CheckSettingsButton();
        }
    }

    private void CheckSettingsButton()
    {
        while (settingsButtonPressed.Count > 0 && Time.time - settingsButtonPressed[0] > 4)
        {
            settingsButtonPressed.RemoveAt(0);
        }
        if (settingsButtonPressed.Count >= 10)
        {
            settingsButtonPressed = new List<float>();
            OpenView(View.Setup);
        }
    }
}
