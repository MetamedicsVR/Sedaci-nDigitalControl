using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ExperienceConnector : MonoBehaviourInstance<ExperienceConnector>
{
    public delegate void DelegateStartExperience(int seahorsesTimes, int blowfishesTimes);
    public event DelegateStartExperience StartExperienceEvent;
    public delegate void DelegateEndExperience();
    public event DelegateEndExperience EndExperienceEvent;

    private void CreateOrJoinRoom()
    {
        NetworkManager.GetInstance().CreateOrJoinRoom();
    }

    private void RetryCreateOrJoin()
    {
        Invoke(nameof(CreateOrJoinRoom), 5);
    }


    public void StartExperience(int seahorsesTimes, int blowfishesTimes)
    {
        NetworkManager.GetInstance().RPC(this, nameof(RPCStartExperience), seahorsesTimes, blowfishesTimes);
    }

    [PunRPC]
    protected void RPCStartExperience(int seahorsesTimes, int blowfishesTimes)
    {
        StartExperienceEvent.Invoke(seahorsesTimes, blowfishesTimes);
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
}
