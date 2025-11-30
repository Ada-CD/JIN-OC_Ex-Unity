using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.U2D;

public class ButtonController : MonoBehaviour
{
	private static readonly int ParameterActivated = Animator.StringToHash("Activated");

	// The Animator and SpriteShapeRenderer are assumed to be children of the button, as in the Prefab.
	private Animator _timer;
	private SpriteShapeRenderer _wire;

	void Start()
	{
		_timer = GetComponentInChildren<Animator>();
		_wire = GetComponentInChildren<SpriteShapeRenderer>();
	}

	// The only moving collider is the player, we don't need to check the incoming collider to activate the button.
	private void OnTriggerEnter2D(Collider2D other)
	{
		_wire.color = PowerColors.On;
		_timer.SetBool(ParameterActivated, true);
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		_wire.color = PowerColors.Off;
		_timer.SetBool(ParameterActivated, false);
	}
}
