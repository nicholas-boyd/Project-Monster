using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    public const string ScreenSizeUpdateNotification = "SCREEN_SIZE_CHANGED";

    public const string AlphaDownNotification = "ALPHA_DOWN";
    public const string AlphaHeldNotification = "ALPHA_HELD";
    public const string AlphaUpNotification = "ALPHA_UP";

    public const string NumberDownNotification = "NUMBER_DOWN";
    public const string NumberHeldNotification = "NUMBER_HELD";
    public const string NumberUpNotification = "NUMBER_UP";

    public const string MouseDownNotification = "MOUSE_DOWN";
    public const string MouseHeldNotification = "MOUSE_HELD";
    public const string MouseUpNotification = "MOUSE_UP";

    public const string Movement = "MOVEMENT_REPEATER";
    public const string Scrolling = "SCROLLING_REPEATER";

    class Repeater
    {
        const float threshold = 0f;
        const float rate = 0.1f;
        float _next;
        bool _hold;
        string _axis;
        public Repeater(string axisName)
        {
            _axis = axisName;
        }

        public int Update()
        {
            int retValue = 0;
            int value = Mathf.RoundToInt(Input.GetAxisRaw(_axis));
            if (_axis == "Mouse ScrollWheel")
                value = Mathf.RoundToInt(Mathf.Clamp(Input.GetAxisRaw(_axis) * 10, -1, 1));
            if (value != 0)
            {
                if (Time.time > _next)
                {
                    retValue = value;
                    _next = Time.time + (_hold ? rate : threshold);
                    _hold = true;
                }
            }
            else
            {
                _hold = false;
                _next = 0;
            }
            return retValue;
        }
    }

    Repeater _hor = new Repeater("Horizontal");
    Repeater _ver = new Repeater("Vertical");
    Repeater _wheel = new Repeater("Mouse ScrollWheel");
    Repeater _qe = new Repeater("QE Scroll");

    //TODO Modify Buttons/UI for cards
    string[] _buttons = new string[] { "Fire1", "Fire2", "Fire3" };
    KeyCode[] _numcodes = new KeyCode[] { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5 };
    KeyCode[] _alphacodes = new KeyCode[] { KeyCode.I, KeyCode.R };

    Vector2 resolution;

    void Start()
    {
        resolution = new Vector2(Screen.width, Screen.height);
    }

    void Update()
    {
        int x = _hor.Update();
        int y = _ver.Update();
        if (x != 0 || y != 0)
        {
            this.PostNotification(Movement, new Point(x, y));
        }

        int wheel = _wheel.Update();
        int qe = _qe.Update();
        if (wheel != 0)
            this.PostNotification(Scrolling, wheel);
        if (qe != 0)
            this.PostNotification(Scrolling, qe);

        //TODO Modify Buttons/UI for cards
        for (int i = 0; i < _buttons.Length; ++i)
        {
            if (Input.GetButtonDown(_buttons[i]))
                this.PostNotification(MouseDownNotification, i + 1);

            if (Input.GetButton(_buttons[i]))
                this.PostNotification(MouseHeldNotification, i + 1);

            if (Input.GetButtonUp(_buttons[i]))
                this.PostNotification(MouseUpNotification, i + 1);
        }

        for (int i = 0; i < _numcodes.Length; ++i)
        {
            if (Input.GetKeyDown(_numcodes[i]))
                this.PostNotification(NumberDownNotification, i + 1);

            if (Input.GetKey(_numcodes[i]))
                this.PostNotification(NumberHeldNotification, i + 1);

            if (Input.GetKeyUp(_numcodes[i]))
                this.PostNotification(NumberUpNotification, i + 1);
        }

        for (int i = 0; i < _alphacodes.Length; ++i)
        {
            if (Input.GetKeyDown(_alphacodes[i]))
                this.PostNotification(AlphaDownNotification, _alphacodes[i]);

            if (Input.GetKey(_alphacodes[i]))
                this.PostNotification(AlphaHeldNotification, _alphacodes[i]);

            if (Input.GetKeyUp(_alphacodes[i]))
                this.PostNotification(AlphaUpNotification, _alphacodes[i]);
        }

        if (resolution.x != Screen.width || resolution.y != Screen.height)
        {
            resolution = new Vector2(Screen.width, Screen.height);
            this.PostNotification(ScreenSizeUpdateNotification, resolution);
        }
    }
}
