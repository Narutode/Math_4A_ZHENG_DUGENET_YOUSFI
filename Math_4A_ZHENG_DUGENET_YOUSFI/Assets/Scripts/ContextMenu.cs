using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ContextMenu : MonoBehaviour
{
    public GameObject menuPanel;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (menuPanel.activeSelf == false)
            {
                menuPanel.SetActive(true);
                
            }
            else
            {
                menuPanel.SetActive(false);
            }
            
        }
    }

    public void Couleurs()
    {
        Debug.Log("Couleurs");
    }

    public void Polygone()
    {
        Debug.Log("Polygone");
    }

    public void Tracé()
    {
        Debug.Log("Tracé");
    }

    public void Fenêtrage()
    {
        Debug.Log("Fenêtrage");
    }

    public void Remplissage()
    {
        Debug.Log("Remplissage");
    }
}
