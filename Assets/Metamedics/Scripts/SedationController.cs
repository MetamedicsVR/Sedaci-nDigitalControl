using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SedationController : MonoBehaviour
{
    [Header("Setup")]
    public GameObject setupView;
    public TextMeshProUGUI roomNameText;
    public TextMeshProUGUI seahorsesTimesText;
    public TextMeshProUGUI blowfishesTimesText;
    public Button confirmButton;

    private int seahorsesTimes;
    private int blowfishesTimes;
    private int defaultSeahorsesTimes;
    private int defaultBlowfishTimes;
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
    public Button startExperienceButton;
    public Button endExperienceButton;

    private bool showSeahorses;
    private bool showBlowfishes;
    public const string seahorsesShowKey = "KEY_SEAHORSES_SHOW";
    public const string blowfishesShowKey = "KEY_BLOWFISH_SHOW";

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
        seahorsesTimes = PlayerPrefs.GetInt(seahorsesShowKey, 5);
        blowfishesTimes = PlayerPrefs.GetInt(blowfishesShowKey, 5);
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
        roomNameText.text = NetworkManager.GetInstance().localRoomName;
        seahorsesTimesText.text = seahorsesTimes.ToString();
        blowfishesTimesText.text = blowfishesTimes.ToString();
        OpenView(View.Setup);
    }

    public void ConfirmSettings()
    {
        if (roomNameText.text != "")
        {
            NetworkManager.GetInstance().localRoomName = roomNameText.text;
            PlayerPrefs.SetString(NetworkManager.localRoomNameKey, roomNameText.text);
            PlayerPrefs.SetInt(seahorsesTimesKey, int.Parse(seahorsesTimesText.text));
            PlayerPrefs.SetInt(blowfishesTimesKey, int.Parse(blowfishesTimesText.text));
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
        endExperienceButton.gameObject.SetActive(false);
        switch (view)
        {
            case View.Setup:
                setupView.SetActive(true);
                break;
            case View.Connecting:
                connectingView.SetActive(true);
                break;
            case View.Playing:
                endExperienceButton.gameObject.SetActive(false);
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

    public void StartExperience()
    {
        ExperienceConnector.GetInstance().StartExperience(showSeahorses ? seahorsesTimes : 0, showBlowfishes ? blowfishesTimes : 0);
        endExperienceButton.gameObject.SetActive(true);
    }

    public void EndExperience()
    {
        ExperienceConnector.GetInstance().EndExperience();
    }
}
