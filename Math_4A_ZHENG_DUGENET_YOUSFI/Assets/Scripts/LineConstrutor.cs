using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Color = UnityEngine.Color;
using UnityEngine.EventSystems;

public class LineConstrutor : MonoBehaviour
{
    private Camera cam;
    GameObject Sommet;
    [FormerlySerializedAs("Line")] public LineRenderer curLine;

    public GameObject pointGO;

    private float nearClipPlaneWorldPoint = 0;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // click gauche
        {
            Debug.Log("place point");
            Vector3 point = new Vector3();
            Vector2 mousePos = Input.mousePosition;

            point = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, cam.nearClipPlane + 1f));
            if (nearClipPlaneWorldPoint == 0)
                nearClipPlaneWorldPoint = point.z;
            pointGO.transform.position = point;
            GameObject newP = Instantiate(pointGO);
            newP.tag = "Extru";
        }
    }
}
