using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviour
{
    public static RoomManager instance;

    public int arrayLength = 1;

    private string[] previousRooms;
    private int roomsCompleted = 0;

    void Awake() => instance = this;

    // Start is called before the first frame update
    void Start()
    {
        previousRooms = new string[arrayLength];
    }

    public void LoadRoom(string room) {
        roomsCompleted += 1;
        previousRooms[roomsCompleted % arrayLength] = room;
        SceneManager.LoadScene(room, LoadSceneMode.Single);
    }

    public bool Contains(string room) {
        return Array.Exists(previousRooms, element => element == room);
    }

    public int GetTotalRooms() {
        return roomsCompleted;
    }
}
