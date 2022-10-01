using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 5;

    private int currentHealth;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    void TakeDamage(int damage) {
        currentHealth -= damage;
        if (currentHealth <= 0) KillPlayer();
    }

    public void KillPlayer() {
        SceneManager.LoadScene("Game Over", LoadSceneMode.Single);
    }
}
