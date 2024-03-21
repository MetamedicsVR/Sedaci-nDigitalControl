using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    public float totalTimeToDestroy;
	public bool countDownOnStart;
	
	private CountDownState countDownState = CountDownState.Stopped;
	private float remainingTime;
	
	private void Start()
	{
		if(countDownOnStart)
		{
			StartCountDown();
		}
	}

    private void Update()
    {
        if(countDownState == CountDownState.Started)
        {
			remainingTime -= Time.deltaTime;
			if (remainingTime <= 0)
			{
				CountDownEnded();
			}
		}
    }

    public void StartCountDown()
	{
		if (countDownState == CountDownState.Stopped)
		{
			remainingTime = totalTimeToDestroy;
		}
		countDownState = CountDownState.Started;
	}

	public void StopCountDown()
	{
		countDownState = CountDownState.Stopped;
	}

	public void PauseCountDown()
	{
		countDownState = CountDownState.Paused;
	}
	
	private void CountDownEnded()
	{
		Destroy(gameObject);
	}
	
	public CountDownState GetCountDownState()
	{
		return countDownState;
	}
	
	public float GetRemainingTime()
	{
		return remainingTime;
	}
	
	public enum CountDownState
	{
		Stopped,
		Started,
		Paused
	}
}
