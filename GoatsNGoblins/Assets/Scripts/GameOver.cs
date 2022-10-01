using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOver : MonoBehaviour
{
    public GameObject roomCount;
    // Start is called before the first frame update
    void Start()
    {
        int rooms = RoomManager.instance.GetTotalRooms();
        roomCount.GetComponent<TextMeshProUGUI>().text = "You cleared " + rooms + " rooms!";
        GameObject managers = GameObject.Find("Managers");
        Destroy(managers);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) SceneManager.LoadScene("000", LoadSceneMode.Single);  
    }
}
