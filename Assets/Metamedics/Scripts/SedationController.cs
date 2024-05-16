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
    private string[] debugLines = new string[20];

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
        NetworkManager.GetInstance().EventJoinedLobby += ConnectedToLobby;
        NetworkManager.GetInstance().EventRoomListUpdated += GotRoomNames;
        NetworkManager.GetInstance().EventCreatedRoom += (name) => UpdateConnectingStatusText("Waiting for headset");
        NetworkManager.GetInstance().EventJoinRoom += OpenPlaying;
        NetworkManager.GetInstance().EventLeftRoom += LeftRoom;
        ExperienceConnector.GetInstance().StatusInfo += StatusInfo;

        showSeahorsesToggle.gameObject.SetActive(false);
        showBlowfishesToggle.gameObject.SetActive(false);
        seahorsesTimes = PlayerPrefs.GetInt(seahorsesTimesKey, defaultSeahorsesTimes);
        blowfishesTimes = PlayerPrefs.GetInt(blowfishesTimesKey, defaultBlowfishTimes);
        showSeahorsesToggle.gameObject.SetActive(true);
        showBlowfishesToggle.gameObject.SetActive(true);

        showSeahorsesToggle.transform.parent.gameObject.SetActive(true);
        showBlowfishesToggle.transform.parent.gameObject.SetActive(true);
        startExperienceButton.transform.parent.gameObject.SetActive(false);
        endExperienceButton.transform.parent.gameObject.SetActive(false);
        distractionButton.transform.parent.gameObject.SetActive(false);

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
            string fixedRoomName = Regex.Replace(roomNameText.text, @"[^a-zA-Z0-9]", "");
            NetworkManager.GetInstance().localRoomName = fixedRoomName;
            PlayerPrefs.SetString(NetworkManager.localRoomNameKey, fixedRoomName);
            string fixedSeahorseTimes = Regex.Replace(seahorsesTimesText.text, @"[^0-9]", "");
            PlayerPrefs.SetInt(seahorsesTimesKey, fixedSeahorseTimes == "" ? 0 : int.Parse(fixedSeahorseTimes));
            string fixedBlowfishesTimes = Regex.Replace(blowfishesTimesText.text, @"[^0-9]", "");
            PlayerPrefs.SetInt(blowfishesTimesKey, fixedBlowfishesTimes == "" ? 0 : int.Parse(fixedBlowfishesTimes));
        }
        if (NetworkManager.GetInstance().IsConnected() && gotRoomNames)
        {
            if (NetworkManager.GetInstance().CurrentRoomName() == NetworkManager.GetInstance().localRoomName && NetworkManager.GetInstance().localRoomName != "")
            {
                OpenPlaying();
            }
            else
            {
                NetworkManager.GetInstance().LeaveRoom();
                showSeahorsesToggle.transform.parent.gameObject.SetActive(true);
                showBlowfishesToggle.transform.parent.gameObject.SetActive(true);
                startExperienceButton.transform.parent.gameObject.SetActive(false);
                endExperienceButton.transform.parent.gameObject.SetActive(false);
                distractionButton.transform.parent.gameObject.SetActive(false);
                OpenConnecting();
            }
        }
        else
        {
            OpenConnecting();
        }
    }

    private void OpenConnecting()
    {
        OpenView(View.Connecting);
        connectingView.SetActive(true);
        if (NetworkManager.GetInstance().IsConnected())
        {
            if (gotRoomNames)
            {
                JoinRoom();
            }
        }
        else
        {
            NetworkManager.GetInstance().Connect();
        }
    }

    public void Log(string message)
    {
        for (int i = debugLines.Length - 1; i > 0; i--)
        {
            debugLines[i] = debugLines[i - 1];
        }
        debugLines[0] = message;
        string allLines = "";
        for (int i = 0; i < debugLines.Length; i++)
        {
            allLines += debugLines[i] + "\r\n";
        }
        connectingStatusText.text = allLines;
    }

    private void UpdateConnectingStatusText(string s)
    {
        Logger.GetInstance().Log(s);
    }

    private void ConnectedToLobby()
    {
        gotRoomNames = false;
        UpdateConnectingStatusText("Connected to lobby");
    }

    private void GotRoomNames()
    {
        if (!gotRoomNames)
        {
            gotRoomNames = true;
        }
        if (NetworkManager.GetInstance().CurrentRoomName() == "")
        {
            JoinRoom();
        }
    }

    private void JoinRoom()
    {
        if (NetworkManager.GetInstance().RoomExist(NetworkManager.GetInstance().localRoomName))
        {
            Logger.GetInstance().Log("Room: " + NetworkManager.GetInstance().localRoomName);
            NetworkManager.GetInstance().JoinRoom(NetworkManager.GetInstance().localRoomName);
        }
        else
        {
            UpdateConnectingStatusText("Waiting for headset");
        }
    }

    private void RetryJoin(string errorText)
    {
        UpdateConnectingStatusText(errorText + ", trying in 5 seconds");
        Invoke(nameof(JoinRoom), 5);
    }

    private void LeftRoom()
    {
        if (playingView.activeSelf)
        {
            OpenConnecting();
        }
    }

    private void OpenPlaying()
    {
        UpdateConnectingStatusText("Joined room " + NetworkManager.GetInstance().localRoomName);
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
        if (!setupView.activeSelf)
        {
            settingsButtonPressed.Add(Time.time);
        }
    }

    private void Update()
    {
        if (!setupView.activeSelf)
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
            OpenSetup();
        }
    }

    private void StatusInfo(ExperienceConnector.ExperienceStatus experienceStatus)
    {
        switch (experienceStatus)
        {
            case ExperienceConnector.ExperienceStatus.NotStarted:
                showSeahorsesToggle.transform.parent.gameObject.SetActive(true);
                showBlowfishesToggle.transform.parent.gameObject.SetActive(true);
                startExperienceButton.transform.parent.gameObject.SetActive(true);
                endExperienceButton.transform.parent.gameObject.SetActive(false);
                //distractionButton.transform.parent.gameObject.SetActive(false);
                break;
            case ExperienceConnector.ExperienceStatus.Starting:
            case ExperienceConnector.ExperienceStatus.Started:
                showSeahorsesToggle.transform.parent.gameObject.SetActive(false);
                showBlowfishesToggle.transform.parent.gameObject.SetActive(false);
                startExperienceButton.transform.parent.gameObject.SetActive(false);
                endExperienceButton.transform.parent.gameObject.SetActive(true);
                //distractionButton.transform.parent.gameObject.SetActive(true);
                break;
            case ExperienceConnector.ExperienceStatus.Ending:
            case ExperienceConnector.ExperienceStatus.Ended:
                showSeahorsesToggle.transform.parent.gameObject.SetActive(false);
                showBlowfishesToggle.transform.parent.gameObject.SetActive(false);
                startExperienceButton.transform.parent.gameObject.SetActive(false);
                endExperienceButton.transform.parent.gameObject.SetActive(false);
                //distractionButton.transform.parent.gameObject.SetActive(false);
                break;
        }
    }
}
