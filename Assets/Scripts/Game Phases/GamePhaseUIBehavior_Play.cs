using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GamePhaseUIBehavior_Play : GamePhaseUIBehavior {
    public Text timerText;
    [SerializeField]
    public PopUpWindow popUp;
    [SerializeField]
    public PopUpWindow_Confirmation confirmationPopUp;

    public override void OpenUI()
	{
        gameObject.SetActive(true);
        popUp.QuickClose();
        confirmationPopUp.QuickClose();
	}

	public override void UpdateUI()
	{
        float currentTimer = GameManager.instance.FetchPlayTimerValue();
        timerText.text = currentTimer.ToString("00");
	}

	public override void CloseUI()
	{
        gameObject.SetActive(false);
	}

    public void ShowPopUpWindow( PopUpWindow.PopUpTypes inputPopUpType )
    {
        switch (inputPopUpType)
        {
            case PopUpWindow.PopUpTypes.Ready:
                popUp.SetText("Ready");
                popUp.Open();
                StartCoroutine(ClosePopUpWindow());
                break;
            case PopUpWindow.PopUpTypes.Start:
                popUp.SetText("Start!");
                popUp.Open();
                StartCoroutine(ClosePopUpWindow());
                break;
            case PopUpWindow.PopUpTypes.Finish:
                popUp.SetText("Stop!");
                popUp.Open();
                StartCoroutine(ClosePopUpWindow());
                StartCoroutine( OpenPopUpInSeconds(PopUpWindow.PopUpTypes.Confirm, 4f) );
                break;
            case PopUpWindow.PopUpTypes.Confirm:
                confirmationPopUp.Open();
                break;
        }
    }

    IEnumerator ClosePopUpWindow()
    {
        yield return new WaitForSeconds(2f);
        popUp.Close();
    }
    IEnumerator OpenPopUpInSeconds(PopUpWindow.PopUpTypes inputPopUpType, float inputTime)
    {
        yield return new WaitForSeconds(inputTime);
        ShowPopUpWindow(inputPopUpType);
    }

    [System.Serializable]
    public class PopUpWindow
    {
        public enum PopUpTypes { Ready, Start, Finish, Confirm }
        public Text textBox;
        public RectTransform container;

        public void SetText(string inputText)
        {
            textBox.text = inputText;
        }

        public void SetPosition(Vector2 inputPosition)
        {
            container.position = inputPosition;
        }

        public virtual void Open(bool spinText = true)
        {
            container.localScale = Vector3.zero;
            container.gameObject.SetActive(true);
            iTween.ScaleTo
            (
             container.gameObject,
             iTween.Hash("scale", Vector3.one, "time", 0.5f)   
            );

            iTween.RotateBy
           (
            textBox.gameObject,
            iTween.Hash("amount", Vector3.forward * 1f, "time", 1.5f, "easetype", iTween.EaseType.easeOutBack)
           );
            
        }

        public virtual void Close()
        {
            container.localScale = Vector3.one;
            container.gameObject.SetActive(true);
            iTween.ScaleTo
            (
             container.gameObject,
             iTween.Hash("scale", Vector3.zero, "time", 0.5f)
            );
        }

        public void QuickClose()
        {
            container.localScale = Vector3.zero;
            container.gameObject.SetActive(false);
        }
    }

    [System.Serializable]
    public class PopUpWindow_Confirmation : PopUpWindow
    {
        public Button confirmationButton;

        public override void Open(bool spinText = true)
        {
            container.localScale = Vector3.zero;
            container.gameObject.SetActive(true);
            textBox.text = GameManager.instance.FetchResults();
            iTween.ScaleTo
            (
             container.gameObject,
             iTween.Hash("scale", Vector3.one, "time", 0.5f)
            );
        }

        public override void Close()
        {
            container.localScale = Vector3.one;
            container.gameObject.SetActive(true);
            iTween.ScaleTo
            (
             container.gameObject,
             iTween.Hash("scale", Vector3.zero, "time", 0.5f)
            );
        }
    }
}
