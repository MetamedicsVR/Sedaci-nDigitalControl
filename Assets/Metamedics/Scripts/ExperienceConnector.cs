using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ExperienceConnector : MonoBehaviourInstance<ExperienceConnector>
{
    public delegate void DelegateStartExperience(bool showSeaHorses, bool showBlowfishes);
    public event DelegateStartExperience StartExperienceEvent;
    public delegate void DelegateEndExperience();
    public event DelegateEndExperience EndExperienceEvent;
    public delegate void DelegateRestartExperience();
    public event DelegateRestartExperience RestartExperienceEvent;

    private void CreateOrJoinRoom()
    {
        NetworkManager.GetInstance().CreateOrJoinRoom();
    }

    private void RetryCreateOrJoin()
    {
        Invoke(nameof(CreateOrJoinRoom), 5);
    }


    public void StartExperience(bool showSeaHorses, bool showBlowfishes)
    {
        NetworkManager.GetInstance().RPC(this, nameof(RPCStartExperience), showSeaHorses, showBlowfishes);
    }

    [PunRPC]
    protected void RPCStartExperience(bool showSeaHorses, bool showBlowfishes)
    {
        StartExperienceEvent.Invoke(showSeaHorses, showBlowfishes);
    }

    public void EndExperience()
    {
        NetworkManager.GetInstance().RPC(this, nameof(RPCStartExperience));
    }

    [PunRPC]
    protected void RPCEndExperience()
    {
        EndExperienceEvent.Invoke();
    }

    public void RestartExperience()
    {
        NetworkManager.GetInstance().RPC(this, nameof(RPCStartExperience));
    }

    [PunRPC]
    protected void RPCRestartExperience()
    {
        RestartExperienceEvent.Invoke();
    }
}
