using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public float totalDuration = 5;
    public Image mask;

    private float timeRemaining;
    private float timeRatio;
    private float originalSize;

    // Start is called before the first frame update
    void Start()
    {
        timeRemaining = totalDuration;
        originalSize = mask.rectTransform.rect.width;
    }

    // Update is called once per frame
    void Update()
    {
        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0)
        {
            GameObject player = GameObject.FindWithTag("Player");
            player.GetComponent<PlayerHealth>().KillPlayer();
        }
        else
        {
            timeRatio = timeRemaining / totalDuration;
            mask.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, originalSize * timeRatio);
        }
    }
}
