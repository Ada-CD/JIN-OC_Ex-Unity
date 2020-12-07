using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Transform _transform;

    [SerializeField] private float speed = 5f;
    [SerializeField] private SerialHandler serialHandler;
    [SerializeField] private Rigidbody2D ledArea;
    
    // Start is called before the first frame update
    void Start()
    {
        _transform = GetComponentInParent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"),0);
        _transform.position += direction.normalized * speed * Time.deltaTime;
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
