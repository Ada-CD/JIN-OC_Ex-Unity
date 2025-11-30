using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	private Rigidbody2D _body;
	private Vector2 _dampVelocity = Vector2.zero;

	[SerializeField] private float speed = 5f;

	void Start()
	{
		_body = GetComponentInParent<Rigidbody2D>();
	}

	void Update()
	{
		Vector2 direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
		if (direction.magnitude == 0) {
			_body.linearVelocity = Vector2.SmoothDamp(_body.linearVelocity, Vector2.zero, ref _dampVelocity, 0.05f);
		} else {
			_dampVelocity = Vector2.zero;
			_body.linearVelocity = Vector2.ClampMagnitude(_body.linearVelocity + direction.normalized * speed, speed);
		}
	}
}
