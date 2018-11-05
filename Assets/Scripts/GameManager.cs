using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	public static GameManager instance;

	public enum GamePhaseTypes
	{
		start, ready, play, complete, restart, end
	}

	public GamePhaseTypes currentGamePhaseType = GamePhaseTypes.start;

	[SerializeField]
	public GamePhaseInfo[] gamePhases;

    [SerializeField]
    public LevelInfo currentLevelInfo;

    Dictionary<GamePhaseTypes, GamePhaseBehavior> gamePhaseDictionary = new Dictionary<GamePhaseTypes, GamePhaseBehavior>();

	void Awake()
	{
		if(instance == null)
		{
			instance = this;
			DontDestroyOnLoad(this);
		}
		else 
		{
			Destroy(this.gameObject);
		}

		GenerateGamePhaseDictionary();
	}

    void Start()
    {
        TriggerBeginPhase(GamePhaseTypes.start);
    }

	void Update()
	{
		if(gamePhaseDictionary.ContainsKey( currentGamePhaseType )) 
		{
			gamePhaseDictionary[currentGamePhaseType].UpdatePhase();
		}
	}

    void FixedUpdate()
    {
        if (gamePhaseDictionary.ContainsKey(currentGamePhaseType))
        {
            gamePhaseDictionary[currentGamePhaseType].FixedUpdatePhase();
        }
    }

    void GenerateGamePhaseDictionary()
	{
		foreach(GamePhaseInfo gpi in gamePhases)
		{
			gamePhaseDictionary.Add( gpi.phaseEnum, gpi.phaseBehavior );
		}
	}

    #region Triggers
    public void TriggerBeginPhase( GamePhaseTypes inputPhaseType )
	{
        if (gamePhaseDictionary.ContainsKey(inputPhaseType))
        {
            Debug.Log("Will END " + currentGamePhaseType.ToString() + ", and START " + inputPhaseType.ToString());
            if (currentGamePhaseType != inputPhaseType) { 
			    gamePhaseDictionary[currentGamePhaseType].EndPhase();
            }
            gamePhaseDictionary[inputPhaseType].BeginPhase();
			currentGamePhaseType = inputPhaseType;
		}
		else 
		{
			Debug.Log("No phase behavior for type: " + inputPhaseType.ToString());
		}
	}
    public void TriggerQuit()
    {
        Application.Quit();
    }
    #endregion
    
    #region Fetches
    public float FetchPlayTimerValue()
    {
        GamePhaseBehavior_Play playBehavior = (GamePhaseBehavior_Play)gamePhaseDictionary[GamePhaseTypes.play];
        return playBehavior.timer;
    }

    public LevelInfo FetchLevelInfo()
    {
        return currentLevelInfo;
    }

    public Transform FetchOtherPlayer(GameObject currentPlayer)
    {
        GamePhaseBehavior_Play playBehavior = (GamePhaseBehavior_Play)gamePhaseDictionary[GamePhaseTypes.play];
        List<GameObject> playerList = new List<GameObject>( playBehavior.GetPlayers() );
        playerList.Remove(currentPlayer);
        if (playerList.Count > 0) return playerList[Random.Range(0, playerList.Count)].transform;
        else return null;
    }

    public string FetchResults()
    {
        string results = "";
        GamePhaseBehavior_Play playBehavior = (GamePhaseBehavior_Play)gamePhaseDictionary[GamePhaseTypes.play];
        List<GameObject> playerList = new List<GameObject>(playBehavior.GetPlayers());
        if (playerList.Count > 0)
        {
            if (playerList.Count > 1) { results = "DRAW!";  }
            else { results = playerList[0].name.ToUpper() + " WINS!";  }
        }
        return results;
    }
    #endregion

    #region Reports
    public void ReportPlayerDeath(CharacterMovementController inputPlayer)
    {
        GamePhaseBehavior_Play playBehavior = (GamePhaseBehavior_Play)gamePhaseDictionary[GamePhaseTypes.play];
        playBehavior.KillPlayer(inputPlayer);
    }
    #endregion

}

[System.Serializable]
public class GamePhaseInfo
{
	public GamePhaseBehavior phaseBehavior;
	public GameManager.GamePhaseTypes phaseEnum;
}

[System.Serializable]
public class LevelInfo
{
    public float waitToStart = 3f;
    public float duration = 0f;
    public int players = 1;
    public int computers = 3; 

    public LevelInfo()
    {
        waitToStart = 3f;
        duration = 60f;
        players = 1;
        computers = 3;
    }

}


/* TIME LOG
 * 
 * 9/22
 * 1 HOUR: Game Manager, Game Phase, Game Phase UI Setup
 * 9/23
 * 1 HOUR: Level Instantiate Setup, Basic Player Controls, Basic Character Setup
 * 
 * 10/1
 * 1 HOUR: Text experimentation, play UI
 * 1 HOUR: Lava texture, Character controller edits
 * 1 HOUR: Basic AI
 * 
 * 10/2
 * 1 HOUR: Virtual Controller (Illustrator, Input Manager, Input UI Updates)
 * 1 HOUR: Pop Ups for Game State progression
 * 0.5 HOUR: UI Styling, Game State Progression Bug Fixing
 * 1 HOUR: Android Testing, UI fixing, Minor Bug Fixing
 * 
 */