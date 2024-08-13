using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrolleySpawner : MonoBehaviour
{
    public GameObject trolleyPrefab;
    public LayerMask groundLayer; 
    public Button spawnButton;
    public bool isSpawned;


    private void Start()
    {
        if (spawnButton != null)
        {
            spawnButton.onClick.AddListener(OnSpawnButtonClick);
        }
    }

    private void Update()
    {
        if (isSpawned)
        {
            if (Input.GetMouseButtonDown(0)) 
            {
                SpawnTrolley();
            }
        }
    }

    void OnSpawnButtonClick()
    {
        isSpawned = true;
    }


    public void SpawnTrolley()
    {
        Vector3 mousePosition = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
        {
            Vector3 spawnPosition = hit.point;
            Instantiate(trolleyPrefab, spawnPosition, Quaternion.LookRotation(spawnPosition));
            isSpawned = false;
        }
    }

}