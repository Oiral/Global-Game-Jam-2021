using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyKill : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            //Lets lose the game
            Destroy(other.gameObject.GetComponentInParent<Player>());
            StartCoroutine(LoseGame());
        }
    }

    IEnumerator LoseGame()
    {
        yield return 0;
        GetComponentInParent<AI>().animator.SetTrigger("Kill Player");
        GetComponentInParent<AI>().agent.isStopped = true;
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(3);
    }
}
