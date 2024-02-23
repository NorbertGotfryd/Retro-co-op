using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    public Color[] playerColors;
    public List<PlayerController> players = new List<PlayerController>();
    public Transform[] spawnPoints;
    public GameObject deathEffectPrefab;
    public GameObject playerContainer;
    public Transform playerContainerParent;

    //singleton
    public static PlayerManager instance;

    private void Awake()
    {
        instance = this;
    }

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        //set the player color
        playerInput.GetComponentInChildren<SpriteRenderer>().color = playerColors[players.Count];

        //create the player container ui
        PlayerContainerUi containerUi = Instantiate(playerContainer, playerContainerParent).GetComponent<PlayerContainerUi>();
        playerInput.GetComponent<PlayerController>().containerUi = containerUi;
        containerUi.Initialize(playerColors[players.Count]);

        players.Add(playerInput.GetComponent<PlayerController>());
        playerInput.transform.position = spawnPoints[Random.Range(0, spawnPoints.Length)].position;

    }

    public void OnPlayerDeath(PlayerController player, PlayerController attacker)
    {
        //spawn the death effect for player
        Destroy(Instantiate(deathEffectPrefab, player.transform.position, Quaternion.identity), 1.5f);

        //increase attacker's score
        if(attacker != null)
        {
            attacker.score++;

            //update score UI
            attacker.containerUi.UpdateScoreText(attacker.score);
        }

        //respawn the player
        player.Respawn(spawnPoints[Random.Range(0, spawnPoints.Length)].position);
    }
}
