using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DoorBehavior : MonoBehaviour
{
    private string id;

    private SpriteRenderer sr;

    private int room1Maximum = 2;
    private int room2Maximum = 2;
    private int room3Maximum = 2;
    private int room4Maximum = 2;
    // Start is called before the first frame update
    void Start()
    {
        sr = gameObject.GetComponent<SpriteRenderer>();
        id = generateId();
        while (RoomManager.instance.Contains(id) == true) {
            id = generateId();
        }
    }


    void OnCollisionEnter2D(Collision2D collision) {
        if(collision.gameObject.tag == "Player") RoomManager.instance.LoadRoom(id);
    }

    string generateId() {
        int roomType = Random.Range(1, 5);
        int roomId = 0;
        switch (roomType)
        {
            case 1:
                sr.color = Color.blue;
                roomId = Random.Range(1, room1Maximum);
                break;
            case 2:
                sr.color = Color.red;
                roomId = Random.Range(1, room2Maximum);
                break;
            case 3:
                sr.color = Color.green;
                roomId = Random.Range(1, room3Maximum);
                break;
            case 4:
                sr.color = Color.yellow;
                roomId = Random.Range(1, room4Maximum);
                break;
            default:
                break;
        }
        string fullId = roomType.ToString() + roomId.ToString();
        if (fullId.Length == 2) fullId = fullId[0] + "0" + fullId[1];
        return fullId;
    }
}
