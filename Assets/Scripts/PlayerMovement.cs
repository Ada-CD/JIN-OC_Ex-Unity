using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D _body;
    private Vector2 _dampVelocity = Vector2.zero;

    [SerializeField] private float speed = 5f;
    [SerializeField] private SerialHandler serialHandler;
    [SerializeField] private Rigidbody2D ledArea;
    
    // Start is called before the first frame update
    void Start()
    {
        _body = GetComponentInParent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if ( direction.magnitude == 0)
        {
            _body.velocity = Vector2.SmoothDamp(_body.velocity, Vector2.zero, ref _dampVelocity, 0.05f);
        }
        else
        {
            _dampVelocity = Vector2.zero;
            _body.velocity = Vector2.ClampMagnitude(_body.velocity + direction.normalized * speed, speed);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.attachedRigidbody == ledArea)
        {
            serialHandler.SetLed(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.attachedRigidbody == ledArea)
        {
            serialHandler.SetLed(false);
        }
    }
}
