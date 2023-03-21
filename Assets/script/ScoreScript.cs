using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;


public class ScoreScript : NetworkBehaviour
{
    TMP_Text p1Text;
    TMP_Text p2Text;
    MainPlayerScript mainPlayer;
    bool delay = false;

    public NetworkVariable<int> scoreP1 = new NetworkVariable<int>(5, NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner);

    public NetworkVariable<int> scoreP2 = new NetworkVariable<int>(5, NetworkVariableReadPermission.Everyone,
    NetworkVariableWritePermission.Owner);

    void Start()
    {
        p1Text = GameObject.Find("P1ScoreText").GetComponent<TMP_Text>();
        p2Text = GameObject.Find("P2ScoreText").GetComponent<TMP_Text>();
        mainPlayer = GetComponent<MainPlayerScript>();
        scoreP1.Value = 5;
        scoreP2.Value = 5;
    }

    private void updateScore()
    {
        if (IsOwnedByServer)
        {
            p1Text.text = $"{mainPlayer.playerNameA.Value} : {scoreP1.Value}";
        }
        else
        {
            p2Text.text = $"{mainPlayer.playerNameB.Value} : {scoreP2.Value}";
        }
    }

    void Update()
    {
        updateScore();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!IsLocalPlayer) return;
        if (collision.gameObject.tag == "Bullet")
        {
            if (IsOwnedByServer && delay == false)
            {
                scoreP1.Value--;
                delay = true;
                StartCoroutine(delaying());
            }
            else if (!IsOwnedByServer && delay == false)
            {
                scoreP2.Value--;
                delay = true;
                StartCoroutine(delaying());
            }
            if (scoreP1.Value == 0)
            {
                scoreP1.Value = 5;
                GetComponent<PlayerSpawnScript>().Respawn();
            }
            else if(scoreP2.Value == 0)
            {               
                scoreP2.Value = 5;
                GetComponent<PlayerSpawnScript>().Respawn();
            }
        }

    }
    IEnumerator delaying()
    {
        yield return new WaitForSeconds(0.5f);
        delay = false;
    }
}
