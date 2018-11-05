using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class InputUI : MonoBehaviour {
    [System.Serializable]
    public class AnalogStick
    {
        public GameObject container;
        public Image background;
        public Image stick;
        public float maxMagnitude;

        public void RevealAnalogStickAtPoint(Vector2 screenLocation)
        {
            background.transform.localPosition = Vector3.zero;
            stick.transform.localPosition = Vector3.zero;
            container.transform.position = screenLocation;
            container.SetActive(true);
        }

        public void HideAnalogStick()
        {
            container.SetActive(false);
        }

        public void UpdateAnalogStickPosition(Vector2 inputPosition)
        {
            Vector2 center = background.transform.position;
            Vector2 nextPostion = inputPosition;

            if ((inputPosition - center).magnitude > maxMagnitude )
            {
                nextPostion = center + (inputPosition - center).normalized * maxMagnitude;
            }
            stick.transform.position = nextPostion;
        }

        public float GetAnalogStickMagnitude()
        {
            Vector2 backgroundCenter = background.transform.position;
            Vector2 stickCenter = stick.transform.position;
            return (stickCenter - backgroundCenter).magnitude;
        }

    }

    public AnalogStick virtualStick;
}
