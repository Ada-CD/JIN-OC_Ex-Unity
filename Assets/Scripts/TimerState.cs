using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void TimerUpdate();

// The timer Animator has three states : off, active, counting down. The only one whose state change matters for
// behaviours is off : when we enter off, we want things to be deactivated, when we exit off, we want things to be
// activated.
// Thus, this is the logic we follow here : when we enter the state, PowerOff is invoked, and PowerOn when we exit.
// This should only be linked to the "Off" state in the Animator.
public class TimerBehaviour : StateMachineBehaviour
{
	public event TimerUpdate PowerOn;
	public event TimerUpdate PowerOff;

	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		PowerOff?.Invoke();
	}

	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		PowerOn?.Invoke();
	}
}
