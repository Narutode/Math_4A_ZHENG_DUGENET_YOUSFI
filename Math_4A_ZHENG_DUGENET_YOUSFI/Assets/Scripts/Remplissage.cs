using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Remplissage : MonoBehaviour
{
    private Texture2D t2d;
    public Camera cam;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)){// click F
            Vector3 point = new Vector3();
            Vector2 mousePos = Input.mousePosition;
            point = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, cam.nearClipPlane));
            RemplissageRegionConnexite(point.x,point.y,point.z,Color.black,Color.red);
         }
                        
    }

    public void RemplissageRegionConnexite(float x,float y,float z,Color ColorContour,Color ColorRemplissage)
    {
        //var colorPixelCourant = t2d.GetPixel(x, y);
        var colorPixelCourant = Color.gray;
        if (ColorContour != colorPixelCourant && ColorRemplissage != colorPixelCourant)
        {
            AffichePixel(x,y,z,ColorRemplissage);
            RemplissageRegionConnexite(x,y-0.001f,z,ColorContour,ColorRemplissage); //Bas
            RemplissageRegionConnexite(x-0.001f,y,z,ColorContour,ColorRemplissage); //Gauche
            RemplissageRegionConnexite(x,y+0.001f,z,ColorContour,ColorRemplissage); //Haut
            RemplissageRegionConnexite(x+0.001f,y,z,ColorContour,ColorRemplissage); //Droite   
        }
    }

    public void RemplissageRegionConnexitePile(int x, int y, Color ColorContour, Color ColorRemplissage)
    {
        
        int[,] pile = {};
        //empiler(x,y)
        while (pile != null)
        {
            //stokage du sommet dans pile
            //dépiler p
            var colorPixelCourant = t2d.GetPixel(x, y);
            if (colorPixelCourant != ColorContour && colorPixelCourant != ColorRemplissage)
            {
                //AffichePixel(x,y,CR);
                
            }

            colorPixelCourant = t2d.GetPixel(x, y - 1);
            if (colorPixelCourant != ColorContour && colorPixelCourant != ColorRemplissage)
            {
                //empiler(x,y-1)
            }
            colorPixelCourant = t2d.GetPixel(x-1, y);
            if (colorPixelCourant != ColorContour && colorPixelCourant != ColorRemplissage)
            {
                //empiler(x-1,y)
            }
            colorPixelCourant = t2d.GetPixel(x, y + 1);
            if (colorPixelCourant != ColorContour && colorPixelCourant != ColorRemplissage)
            {
                //empiler(x,y+1)
            }
            colorPixelCourant = t2d.GetPixel(x+1, y);
            if (colorPixelCourant != ColorContour && colorPixelCourant != ColorRemplissage)
            {
                //empiler(x+1,y)
            }

        }
        
    }

    public void AffichePixel(float i, float j,float z, Color colorCube)
    {
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.transform.position = new Vector3(i, j, z);
        plane.transform.localScale = new Vector3(0.0001f, 0.0001f, 0.0001f);
        plane.transform.Rotate(-90, 0, 0,Space.Self);
        foreach(GameObject gameObj in GameObject.FindObjectsOfType<GameObject>())
        {
            if(gameObj.name == "Plane")
            {
                gameObj.GetComponent<Renderer>().material.color = colorCube;
            }
        }
        

    }
    
}
