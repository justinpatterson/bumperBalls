using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {
    public static InputManager instance;

    bool shouldListen_AnalogUI;
    public InputUI inputUI;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void Update()
    {
        if (shouldListen_AnalogUI)
        {
            if (Input.GetMouseButtonDown(0)) { inputUI.virtualStick.RevealAnalogStickAtPoint(GetPointerPosition()); }
            else if (Input.GetMouseButton(0)) { inputUI.virtualStick.UpdateAnalogStickPosition(GetPointerPosition()); }
            else if (Input.GetMouseButtonUp(0)) { inputUI.virtualStick.HideAnalogStick();  }
        }
    }

    public Vector2 GetAnalogInput()
    {
        /*
        #if UNITY_EDITOR
                return new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        #endif
        */
        return new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }

    public Vector2 GetPointerPosition()
    {
        return Input.mousePosition;
    }

    public void SetAnalogUIListener(bool inputListen)
    {
        shouldListen_AnalogUI = inputListen;
        if (inputListen)
        {
            //inputUI.virtualStick.RevealAnalogStickAtPoint(GetAnalogInput());
        }
        else
        {
            inputUI.virtualStick.HideAnalogStick();
        }
    }


    public float FetchAnalogStickMagnitude()
    {
        return inputUI.virtualStick.GetAnalogStickMagnitude();
    }
}
