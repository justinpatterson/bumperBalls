using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GamePhaseBehavior_Play : GamePhaseBehavior
{
    public GameObject levelPrefab;
    public GameObject characterPrefab;
    public float timer = 0f;

    public enum PlayPhases { waitToBegin, inProgress, pause, end }
    PlayPhases currentPlayPhase = PlayPhases.waitToBegin;

    public delegate void CharacterBehaviorUpdate();
    public static event CharacterBehaviorUpdate OnCharacterUpdate;

    public List<GameObject> players = new List<GameObject>();
    public GameObject currentLevel;

    public override void BeginPhase()
    {
        base.BeginPhase();
        SpawnLevel();
        SpawnCharacterControllers();
        timer = GameManager.instance.currentLevelInfo.waitToStart;
        currentPlayPhase = PlayPhases.waitToBegin;
        GamePhaseUIBehavior_Play castUI = (GamePhaseUIBehavior_Play)phaseUI;
        castUI.ShowPopUpWindow(GamePhaseUIBehavior_Play.PopUpWindow.PopUpTypes.Ready);
    }

    public override void UpdatePhase()
    {
        base.UpdatePhase();
        //phaseUI.UpdateUI();
    }

    public override void FixedUpdatePhase()
    {
        base.FixedUpdatePhase();
        switch (currentPlayPhase)
        {
            case PlayPhases.waitToBegin:
            {
                    timer -= Time.deltaTime;
                    if (timer <= 0f)
                    {
                        timer = GameManager.instance.currentLevelInfo.duration;
                        currentPlayPhase = PlayPhases.inProgress;
                        GamePhaseUIBehavior_Play castUI = (GamePhaseUIBehavior_Play)phaseUI;
                        castUI.ShowPopUpWindow(GamePhaseUIBehavior_Play.PopUpWindow.PopUpTypes.Start);
                        SetAnalogStickVisibility(true);
                    }
                }
            break;
            case PlayPhases.inProgress:
                {
                    timer -= Time.deltaTime;
                    if (OnCharacterUpdate != null)
                    {
                        OnCharacterUpdate();
                    }

                    if (CheckWinState())
                    {
                        currentPlayPhase = PlayPhases.end;
                        InputManager.instance.SetAnalogUIListener(false);
                        GamePhaseUIBehavior_Play castUI = (GamePhaseUIBehavior_Play)phaseUI;
                        castUI.ShowPopUpWindow(GamePhaseUIBehavior_Play.PopUpWindow.PopUpTypes.Finish);
                        foreach (GameObject p in players)
                        {
                            p.GetComponent<CharacterMovementController>().EndCharacterMovement();
                        }
                    }
                }
                break;
            case PlayPhases.end:
                {
                }
                break;
        }
    }

    public override void EndPhase()
    {
        SetAnalogStickVisibility(false);
        RemoveLevel();
        RemovePlayers();
        base.EndPhase();
    }

    public void SetAnalogStickVisibility(bool inputVisibility)
    {
        if (inputVisibility)
        {
            InputManager.instance.SetAnalogUIListener(true);
        }
        else
        {
            InputManager.instance.SetAnalogUIListener(false);
        }
    }

    public void SpawnLevel()
    {
        Vector3 spawnPosition = Vector3.zero; // Camera.main.transform.position + new Vector3(0, -5f, 3f);

        GameObject levelInstance = Instantiate(levelPrefab, spawnPosition, Quaternion.identity) as GameObject;

        Camera.main.transform.position = levelInstance.transform.position + Vector3.up * 15f + Vector3.forward * -4f;
        Camera.main.transform.LookAt(levelInstance.transform);

        currentLevel = levelInstance;
    }

    public void SpawnCharacterControllers()
    {
        int playerCount = GameManager.instance.FetchLevelInfo().players;
        int computerCount = GameManager.instance.FetchLevelInfo().computers;
        List<GameObject> spawnPoints = new List<GameObject>( GameObject.FindGameObjectsWithTag("Spawn") );

        for (int i = 0; i < playerCount; i++)
        {
            if (spawnPoints.Count <= 0) return;
            int selectedSpawnPoint = Random.Range(0, spawnPoints.Count);
            Vector3 spawnPosition = spawnPoints[selectedSpawnPoint].transform.position;
            spawnPoints.RemoveAt(selectedSpawnPoint);
            GameObject characterInstance = Instantiate(characterPrefab, spawnPosition + Vector3.up * 6f, Quaternion.identity) as GameObject;
            characterInstance.name = "Player " + (i + 1);
            players.Add(characterInstance);
            characterInstance.GetComponentInChildren<Renderer>().material.color = Color.red;
            characterInstance.GetComponent<CharacterMovementController>().characterInfo.playerNumber = i;
        }

        for (int i = 0; i < computerCount; i++)
        {
            if (spawnPoints.Count <= 0) return;
            int selectedSpawnPoint = Random.Range(0, spawnPoints.Count);
            Vector3 spawnPosition = spawnPoints[selectedSpawnPoint].transform.position;
            spawnPoints.RemoveAt(selectedSpawnPoint);
            GameObject characterInstance = Instantiate(characterPrefab, spawnPosition + Vector3.up * 6f, Quaternion.identity) as GameObject;
            characterInstance.name = "CP" + (i + 1);
            characterInstance.GetComponent<CharacterMovementController>().controlledBy = CharacterMovementController.ControllerTypes.computer;
            characterInstance.GetComponent<CharacterMovementController>().characterInfo.playerNumber = i;
            players.Add(characterInstance);
        }


       // Vector3 spawnPosition = Vector3.zero; // Camera.main.transform.position + new Vector3(0, -5f, 3f);
       // GameObject characterInstance = Instantiate(characterPrefab, spawnPosition + Vector3.up * 6f, Quaternion.identity) as GameObject;
    }

    void RemoveLevel()
    {
        if (currentLevel) { Destroy(currentLevel); }
        currentLevel = null;
    }
    void RemovePlayers()
    {
        foreach (GameObject g in players)
        {
            g.GetComponent<CharacterMovementController>().QuickDie();
        }
        players.Clear();
    }
    public List<GameObject> GetPlayers()
    {
        return players;
    }

    public void KillPlayer(CharacterMovementController inputPlayer)
    {
        players.Remove(inputPlayer.gameObject);
        inputPlayer.Die();
    }

    bool CheckWinState()
    {
        if (currentPlayPhase == PlayPhases.inProgress)
        {
            return (timer <= 0f || players.Count <= 1);
        }

        return false;
    }
}
