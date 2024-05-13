using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ExperienceConnector : MonoBehaviourInstance<ExperienceConnector>
{
    public delegate void DelegateStartExperience(int seahorsesTimes, int blowfishesTimes, LanguageManager.Language language);
    public event DelegateStartExperience StartExperienceEvent;
    public delegate void DelegateEndExperience();
    public event DelegateEndExperience EndExperienceEvent;
    public delegate void DelegateDistraction();
    public event DelegateDistraction DistractionEvent;
    public delegate void DelegateStatus(ExperienceStatus n);
    public event DelegateStatus StatusInfo;

    public enum ExperienceStatus
    {
        NotStarted,
        Starting,
        Started,
        Ending,
        Ended
    }

    public void StartExperience(int seahorsesTimes, int blowfishesTimes, LanguageManager.Language language)
    {
        NetworkManager.GetInstance().RPC(this, nameof(RPCStartExperience), seahorsesTimes, blowfishesTimes, (int)language);
    }

    [PunRPC]
    protected void RPCStartExperience(int seahorsesTimes, int blowfishesTimes, int languageIndex)
    {
        StartExperienceEvent.Invoke(seahorsesTimes, blowfishesTimes, (LanguageManager.Language)languageIndex);
    }

    public void EndExperience()
    {
        NetworkManager.GetInstance().RPC(this, nameof(RPCEndExperience));
    }

    [PunRPC]
    protected void RPCEndExperience()
    {
        EndExperienceEvent.Invoke();
    }

    public void Distraction()
    {
        NetworkManager.GetInstance().RPC(this, nameof(RPCDistraction));
    }

    [PunRPC]
    protected void RPCDistraction()
    {
        DistractionEvent.Invoke();
    }

    public void Status(ExperienceStatus status)
    {
        NetworkManager.GetInstance().RPC(this, nameof(RPCStatus), (int)status);
    }

    [PunRPC]
    protected void RPCStatus(int n)
    {
        StatusInfo.Invoke((ExperienceStatus)n);
    }
}
