using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectableManager : MonoBehaviour
{
    #region singleton
    public static CollectableManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }else if (instance != this)
        {
            Debug.LogWarning("Found and additional Collectable Manager, Deleting", gameObject);
            Destroy(this);
        }

        Arrow.SetActive(false);
        DontDestroyOnLoad(gameObject);
    }
    #endregion


    public List<Collectable> collectables;

    bool showArrow = false;

    public Transform player;

    public Vector3 exitPoint;

    public Text displayText;

    public int maxNumberOfCollectables;

    public int collectablesCollected;

    public GameObject Arrow;

    public float elapsedTime;

    public void AddCollectableToWorld(Collectable item)
    {
        if (collectables.Contains(item)) return;

        collectables.Add(item);

        if (maxNumberOfCollectables < collectables.Count)
        {
            maxNumberOfCollectables = collectables.Count;
        }

        UpdateUI();
    }

    public void CollectedItem(Collectable item)
    {
        if (collectables.Contains(item))
        {
            collectablesCollected += 1;
            //Lets remove it from the list
            collectables.Remove(item);
            //Lets destroy the game object associated with the collectable
            Destroy(item.gameObject);
            //Lets createa a particle effect of the collection
            Debug.LogWarning("Please add a particle for the collection of the item");

            //Lets update the scoreboard

            //If there are only a few collectable left
            //Lets show an arrow towards the players nearest collectable
            if (collectables.Count < 3)
            {
                showArrow = true;
                Arrow.SetActive(true);
            }
        }
        UpdateUI();
    }

    private void LateUpdate()
    {
        elapsedTime += Time.deltaTime;

        if (showArrow)
        {
            //If there is still something to collect show the direction that way
            //Otherwise we want to aim at the exit

            Vector3 aimAt = exitPoint;
            
            if (collectables.Count > 0)
            {
                aimAt = NearestCollectable(player.position).transform.position;
            }

            Arrow.transform.LookAt(aimAt);
        }
    }

    Collectable NearestCollectable(Vector3 pos)
    {
        Collectable nearest = collectables[0];

        for (int i = 1; i < collectables.Count - 1; i++)
        {
            if (Vector3.Distance(pos, collectables[i].transform.position) < Vector3.Distance(pos, nearest.transform.position))
            {
                nearest = collectables[i];
            }
        }

        return nearest;
    }

    public void UpdateUI()
    {
        if (collectables.Count > 0)
        {
            if (collectables.Count == 1)
            {
                displayText.text = $"{collectables.Count} collectable remains";
            }
            else
            {
                displayText.text = $"{collectables.Count} collectables remain";
            }

        }
        else
        {
            displayText.text = "Get to the exit";
        }
    }
}
