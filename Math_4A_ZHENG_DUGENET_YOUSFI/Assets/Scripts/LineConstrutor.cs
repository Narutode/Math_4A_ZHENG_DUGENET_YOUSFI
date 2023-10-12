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
    public Camera cam;
    GameObject Sommet;
    [FormerlySerializedAs("Line")] public LineRenderer curLine;

    public GameObject pointGO;
    public List<GameObject> points;
    public List<LineRenderer> lines;
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
            if (points.Count > 0)
            {
                GameObject newGO = new GameObject();
                newGO.name = "sides";
                LineRenderer newLine = newGO.AddComponent<LineRenderer>();
                newLine.positionCount = 2;
                newLine.SetPosition(0,points.Last().transform.position);
                newLine.SetPosition(1,newP.transform.position);
                lines.Add(newLine);
            }
            points.Add(newP);
            if (points.Count == 3)
            {
                GameObject newGO = new GameObject();
                newGO.name = "sides";
                LineRenderer newLineEnd = newGO.AddComponent<LineRenderer>();
                newLineEnd.positionCount = 2;
                newLineEnd.SetPosition(0,points.Last().transform.position);
                newLineEnd.SetPosition(1,points.First().transform.position);
                lines.Add(newLineEnd);
                Vector3[] normals = fonctionMath.getNormals(points[0].transform.position, points[1].transform.position, points[2].transform.position);
                int index = 0;
                foreach (var n in normals)
                {
                    GameObject newNormals = new GameObject();
                    newNormals.name = "Normals";
                    LineRenderer newLine = newNormals.AddComponent<LineRenderer>();
                    newLine.positionCount = 2;
                    Vector3 newPos = (points[(index + 1) % 3].transform.position + points[index].transform.position)/2f;
                    newPos.z = points[index].transform.position.z;
                    newLine.SetPosition(0,newPos);
                    newLine.SetPosition(1,newPos+n);
                    lines.Add(newLine);
                    index++;
                }

                foreach (var line in lines)
                {
                    line.startWidth = .05f;
                    line.endWidth = .05f;
                }

                Vector3 centerCircle = fonctionMath.getCenterCircle(points[0].transform.position, points[1].transform.position, points[2].transform.position);
                centerCircle.z = nearClipPlaneWorldPoint;
                pointGO.transform.position = centerCircle;
                GameObject center = Instantiate(pointGO);
                center.name = "center";
            }
        }
    }
}
