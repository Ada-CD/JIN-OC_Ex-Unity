using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class LightController : MonoBehaviour
{
    public bool lightState = false;
    private static readonly int ParameterTimerSpeed = Animator.StringToHash("TimerSpeed");
    
    [SerializeField] private SerialHandler serialHandler;
    
    [SerializeField] private SpriteShapeRenderer leftWire;
    [SerializeField] private SpriteShapeRenderer rightWire;

    [SerializeField] private Animator leftTimer;
    private bool _leftState = false;
    [SerializeField] private Animator rightTimer;
    private bool _rightState = false;

    [SerializeField] private Sprite offSprite;
    [SerializeField] private Sprite offOnSprite;
    [SerializeField] private Sprite onOffSprite;
    [SerializeField] private Sprite onSprite;
    private SpriteRenderer _spriteRenderer;

    private enum Side
    {
        Left,
        Right
    };
    
    // Start is called before the first frame update
    void Start()
    {
        leftTimer.GetBehaviour<TimerBehaviour>().PowerOn += () => { LightStateChecker(Side.Left, true); };
        leftTimer.GetBehaviour<TimerBehaviour>().PowerOff += () => { LightStateChecker(Side.Left, false); };
        rightTimer.GetBehaviour<TimerBehaviour>().PowerOn += () => { LightStateChecker(Side.Right, true); };
        rightTimer.GetBehaviour<TimerBehaviour>().PowerOff += () => { LightStateChecker(Side.Right, false); };
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    public void UpdateTimerMultiplier(float timerMultiplier)
    {
        leftTimer.SetFloat(ParameterTimerSpeed, timerMultiplier);
        rightTimer.SetFloat(ParameterTimerSpeed, timerMultiplier);
    }

    void LightStateChecker(Side side, bool state)
    {
        switch (side)
        {
            case Side.Left:
                _leftState = state;
                leftWire.color = _leftState ? PowerColors.On : PowerColors.Off;
                break;
            case Side.Right:
                _rightState = state;
                rightWire.color = _rightState ? PowerColors.On : PowerColors.Off;
                break;
        }
        
        // Detect a change of state : this is when we send a message to the Arduino and update lightState.
        bool newState = _leftState && _rightState;
        if (!lightState && newState)
        {
            _spriteRenderer.sprite = onSprite;
            serialHandler.SetLed(true);
            lightState = true;
            // There is only one "on" case : we are done.
            return;
        }
        if (lightState && !newState)
        {
            serialHandler.SetLed(false);
            lightState = false;
            // There are multiple "off" cases : check below.
        }
        
        // Update the sprite according to which, if any, side is still powered on at this point.
        if (_leftState)
        {
            _spriteRenderer.sprite = onOffSprite;
            return;
        }
        if (_rightState)
        {
            _spriteRenderer.sprite = offOnSprite;
            return;
        }
        _spriteRenderer.sprite = offSprite;
    }
}
