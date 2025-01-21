using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class OpenNotes : MonoBehaviour
{
    [SerializeField] GameObject[] paperNotes = new GameObject[11];
    [SerializeField] Image[] uiNotes = new Image[11];
    bool isReading;
    [SerializeField] float pickUpRange;
    void Start()
    {
        foreach (Image image in uiNotes) 
        { 
            image.gameObject.SetActive(false);    
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!isReading)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, pickUpRange))
                {
                    if (hit.transform.gameObject.CompareTag("Notes"))
                    {
                        Debug.Log("working");
                        for (int i = 0; i < paperNotes.Length; i++)
                        {
                            if (paperNotes[i] == hit.transform.gameObject)
                            {
                                uiNotes[i].gameObject.SetActive(true);
                                isReading = true;
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (Image image in uiNotes)
                {
                    image.gameObject.SetActive(false);
                }
                isReading = false;
            }
        }  
    }
}
