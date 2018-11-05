using UnityEngine;
using System.Collections;

public class GamePhaseUIBehavior : MonoBehaviour {

    public virtual void OpenUI()
	{
        gameObject.SetActive(true);
	}

	public virtual void UpdateUI()
	{
	}

	public virtual void CloseUI()
	{
        gameObject.SetActive(false);
	}
}
