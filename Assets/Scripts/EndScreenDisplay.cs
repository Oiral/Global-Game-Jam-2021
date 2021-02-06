using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndScreenDisplay : MonoBehaviour
{
    public Text collectableText;
    public Text outOfText;
    public Text timeText;


    private void Start()
    {
        int collectablesCollected = CollectableManager.instance.collectablesCollected;
        int maxCollectables = CollectableManager.instance.maxNumberOfCollectables;


        if (collectablesCollected == maxCollectables)
        {

            collectableText.text = $"Collected all {collectablesCollected} Treasures";

            outOfText.text = "";
        }
        else
        {

            if (collectablesCollected > 1)
            {
                collectableText.text = $"Collected {collectablesCollected} Treasures";
            }
            else
            {
                collectableText.text = $"Collected {collectablesCollected} Treasure";
            }

            outOfText.text = $"Out of {maxCollectables}";
            
        }

        //Lets get the min taken
        float timer = CollectableManager.instance.elapsedTime;

        string minutes = Mathf.Floor(timer / 60).ToString("00");
        string seconds = (timer % 60).ToString("00");

        timeText.text = "Time Taken - " + (string.Format("{0}:{1}", minutes, seconds));

        //Just incase you collected no treasure
        if (collectablesCollected == 0)
        {
            collectableText.text = "You collected no treasure";
            outOfText.text = "";
            timeText.text = "";
        }

        Destroy(CollectableManager.instance.gameObject);
    }
}
