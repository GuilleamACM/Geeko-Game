﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    private int targetSceneIndex;
    private DungeonManager dungeonManager;
    // Start is called before the first frame update
    void Start()
    {
        targetSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        dungeonManager = FindObjectOfType<DungeonManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            dungeonManager.UpdatePlayerReference();
            LoadTargetScene loadTargetScene = this.gameObject.AddComponent<LoadTargetScene>();
            loadTargetScene.LoadSceneNum(targetSceneIndex);
        }
    }
}