
using System.Collections.Generic;
using UnityEngine;


public struct coonsStruct
{
    public List<Vector3> listPoints;
    public List<GameObject> listGameObjects;
    public LineRenderer line;
    public Material mat;
}

public class CoonsLineConstrutor : MonoBehaviour
{
    public Camera cam;
    //GameObject _sommet;
    public GameObject pointGO;
    
    //public List<Segments> ListSegments = new List<Segments>();
    //public List<Triangles> ListTriangles = new List<Triangles>();

    public List<LineRenderer> lines;
    private float _nearClipPlaneWorldPoint = 0;

    public static float Timer;

    public GameObject clickMenu;
    public GameObject parent;

    public int index = 0;

    public Material[] mat = new Material[4];

    public coonsStruct[] coonsTab = new coonsStruct[4];

    private void Start()
    {
        _nearClipPlaneWorldPoint = cam.nearClipPlane + 1f;
        for (int i = 0; i < 4; i++)
        {
            coonsTab[i] = new coonsStruct();
            coonsTab[i].listGameObjects = new List<GameObject>();
            coonsTab[i].listPoints = new List<Vector3>();
            coonsTab[i].mat = mat[i];
            GameObject newGO = new GameObject();
            newGO.transform.SetParent(parent.transform);
            newGO.name = "sides " + i;
            var line = newGO.AddComponent<LineRenderer>();
            line.startWidth = 0.1f;
            line.endWidth = 0.1f;
            line.positionCount = 0;
            line.material = coonsTab[i].mat;
            coonsTab[i].line = line;
        }
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
                coonsTab[index].listGameObjects.Add(newP);
                coonsTab[index].line.positionCount++;
                coonsTab[index].listPoints.Add(newP.transform.position);
                coonsTab[index].line.SetPosition(coonsTab[index].listPoints.Count-1,
                    new Vector3(point.x,point.y, _nearClipPlaneWorldPoint));
                
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            if (clickMenu.activeSelf)
            {
                clickMenu.SetActive(false);
            }
            else
            {
                if (coonsTab[index].listGameObjects.Count >= 3)
                {
                    if (!clickMenu.activeSelf)
                    {
                        clickMenu.SetActive(true);
                    }
                }
            }
        }
    }

    public void Clear()
    {
        Timer = 0;
        lines.Clear();
        coonsTab[index].listPoints.Clear();
        coonsTab[index].listGameObjects.Clear();
        //ListSegments.Clear();
        //ListTriangles.Clear();
        for(int i = 0; i < parent.transform.childCount; i++)
        {
            Destroy(parent.transform.GetChild(i).gameObject);
        }
        clickMenu.SetActive(false);
    }

    public void GenerateChaikinCurve()
    {
        ChaikinCurveGenerator chGen = gameObject.GetComponent<ChaikinCurveGenerator>();
        chGen.ControlPoints = new List<Vector3>[] { coonsTab[0].listPoints, coonsTab[1].listPoints, coonsTab[2].listPoints, coonsTab[3].listPoints };
        chGen.drawChaikin = true;
    }

    public void NextLine()
    {
        index=(index+1)%4;
        clickMenu.SetActive(false);
    }

    public void GenerateCoons()
    {
        CoonsPatchGenerator coons = gameObject.GetComponent<CoonsPatchGenerator>();
        coons.CurveU0 = coonsTab[0].listPoints;
        coons.CurveU1 = coonsTab[2].listPoints;
        coons.CurveV0 = coonsTab[1].listPoints;
        coons.CurveV1 = coonsTab[3].listPoints;
        coons.GenerateCoonsPatch();
    }
}
