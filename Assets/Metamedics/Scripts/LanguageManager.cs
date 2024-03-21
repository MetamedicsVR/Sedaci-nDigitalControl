using System;
using UnityEngine;

public class LanguageManager : MonoBehaviourInstance<LanguageManager>
{
    public delegate void LanguageChanged(Language language);
    public event LanguageChanged OnLanguageChanged;

    private Language currentLanguage;
    private bool settedLanguage;

    protected override void OnInstance()
    {
        DontDestroyOnLoad(gameObject);
    }

    public Language GetCurrentLanguage()
    {
        if (!settedLanguage)
        {
            currentLanguage = GameManager.GetInstance().GetSettings().targetLanguage;
            settedLanguage = true;
        }
        return currentLanguage;
    }

    public void ChangeLanguage(Language language)
    {
        if (currentLanguage != language)
        {
            currentLanguage = language;
            if (OnLanguageChanged != null)
            {
                OnLanguageChanged(language);
            }
        }
    }

    public void ChangeLanguage(string language)
    {
        ChangeLanguage((Language)Enum.Parse(typeof(Language), language));
    }

    public static string GetLanguageCode(Language language)
    {
        switch (language)
        {
            case Language.English:
                return "EN";
            case Language.Spanish:
                return "ES";
        }
        return "??";
    }

    public enum Language
    {
        English,
        Spanish
    }
}
