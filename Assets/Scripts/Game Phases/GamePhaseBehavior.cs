using UnityEngine;
using System.Collections;

[System.Serializable]
public class GamePhaseBehavior : MonoBehaviour {
	public GamePhaseUIBehavior phaseUI;

    public GameManager.GamePhaseTypes nextPhase;

	public virtual void BeginPhase()
	{
		if(phaseUI) { phaseUI.OpenUI();}
	}

	public virtual void UpdatePhase()
	{
		if(phaseUI) { phaseUI.UpdateUI(); }
	}

    public virtual void FixedUpdatePhase()
    {

    }

	public virtual void EndPhase()
	{
		if(phaseUI) { phaseUI.CloseUI(); }
	}

    public virtual void TriggerEndPhase()
    {
        GameManager.instance.TriggerBeginPhase(nextPhase);
    }
}
