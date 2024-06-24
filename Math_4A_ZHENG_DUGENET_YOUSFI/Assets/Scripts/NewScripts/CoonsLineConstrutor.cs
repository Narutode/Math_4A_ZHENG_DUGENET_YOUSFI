using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Color = UnityEngine.Color;
//using UnityEngine.EventSystems;
using static FonctionMath;

public class CoonsLineConstrutor : MonoBehaviour
{
    public Camera cam;
    GameObject _sommet;
    [FormerlySerializedAs("Line")] public LineRenderer curLine;
    public GameObject pointGO;
    public List<GameObject> listGameObjects;
    public List<Vector3> listPoints = new List<Vector3>();
    public List<Segments> ListSegments = new List<Segments>();
    public List<Triangles> ListTriangles = new List<Triangles>();

    public List<LineRenderer> lines;
    private float _nearClipPlaneWorldPoint = 0;

    [SerializeField]public static float Timer;

    public GameObject clickMenu;
    public GameObject parent;
    public Material mat;
    public LineRenderer line;

    private void Start()
    {
        _nearClipPlaneWorldPoint = cam.nearClipPlane + 1f;
        GameObject newGO = new GameObject();
        newGO.transform.SetParent(parent.transform);
        newGO.name = "sides";
        line = newGO.AddComponent<LineRenderer>();
        line.startWidth = 0.1f;
        line.endWidth = 0.1f;
        line.positionCount = 0;
        line.materials[0] = mat;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // click gauche
        {
            if (clickMenu.activeSelf == false)
            {        
                Debug.Log("place point");
                Vector3 point = new Vector3();
                Vector2 mousePos = Input.mousePosition;

                point = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, cam.nearClipPlane + 1f));
                pointGO.transform.position = point;
                GameObject newP = Instantiate(pointGO);
                newP.transform.parent = parent.transform;
                listGameObjects.Add(newP);
                line.positionCount++;
                listPoints.Add(newP.transform.position);
                line.SetPosition(listPoints.Count-1,
                    new Vector3(point.x,point.y, _nearClipPlaneWorldPoint));
                
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            if (listGameObjects.Count >= 3)
            {
               
                if (clickMenu.activeSelf == false)
                {
                    Debug.Log("clickmenu is false");
                    clickMenu.SetActive(true);
                }
                else
                {
                    clickMenu.SetActive(false);
                }
            }
        } 
    }
    public void Clear()
    {
        Timer = 0;
        lines.Clear();
        listPoints.Clear();
        listGameObjects.Clear();
        ListSegments.Clear();
        ListTriangles.Clear();
        for(int i = 0; i < parent.transform.childCount; i++)
        {
            Destroy(parent.transform.GetChild(i).gameObject);
        }
        clickMenu.SetActive(false);
    }

    public void GenerateChaikinCurve()
    {
        ChaikinCurveGenerator chGen = gameObject.GetComponent<ChaikinCurveGenerator>();
        chGen.ControlPoints = listPoints; 
        chGen.Subdivide();
    }
}
