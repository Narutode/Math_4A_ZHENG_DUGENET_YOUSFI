using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Remplissage : MonoBehaviour
{
    public Texture2D t2d = null;
    public Camera cam;
    Color ColorBack = Color.black;
    Color colorCube = Color.white;

    private bool[,] tab = new bool[1000,1000];
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            Debug.Log("g press");
            if(t2d == null)
                StartCoroutine(GetScreenshot());
        }

        if (Input.GetKeyDown(KeyCode.F)){// click F
            Debug.Log("f press");
            if (t2d != null)
            {
                Debug.Log("t2d != null");
                //Vector3 point = new Vector3();
                Vector2 mousePos = Input.mousePosition;
                //point = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, cam.nearClipPlane));
                StartCoroutine(RemplissageRegionConnexite((int) mousePos.x, (int) mousePos.y, cam.nearClipPlane));
            }
        }
    }
    
    IEnumerator GetScreenshot()
    {
        yield return new WaitForEndOfFrame();
        Rect viewRect = cam.pixelRect;
        Texture2D tex = new Texture2D( (int)viewRect.width, (int)viewRect.height, TextureFormat.RGB24, false );
        tex.ReadPixels( viewRect, 0, 0, false );
        tex.Apply( false );
        t2d = tex;
    }
    
    public IEnumerator RemplissageRegionConnexite(int x,int y,float z)
    {
        //var colorPixelCourant = Color.gray;
        if (!tab[x, y])
        {
            AffichePixel(x,y,z);
            tab[x, y] = true;
            var colorPixelCourant = t2d.GetPixel(x, y);
            yield return null;
            if (colorPixelCourant.r < 0.15 && colorPixelCourant.g < 0.15 && colorPixelCourant.b < 0.15)
            {
                StartCoroutine(RemplissageRegionConnexite(x, y - 1, z)); //Bas
                StartCoroutine(RemplissageRegionConnexite(x - 1, y, z)); //Gauche
                StartCoroutine(RemplissageRegionConnexite(x, y + 1, z)); //Haut
                StartCoroutine(RemplissageRegionConnexite(x + 1, y, z)); //Droite
            }
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

    public void AffichePixel(float i, float j,float z)
    {
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        Vector3 point = cam.ScreenToWorldPoint(new Vector3(i, j, cam.nearClipPlane));
        plane.transform.position = new Vector3(point.x, point.y, z);
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
    

    public void RemplissageLigne(int x, int y, Color CC, Color CR)
    {
        Color CP, CPd, CPg;
        int xd, xg;
        int[,] pile = {}; /* initialisation de la pile à vide */
        //empiler(x,y)

        while (pile.Length != 0) /* un germe à traiter */
        {
            //(x,y) <- sommetPile(p) /* stockage du sommet */
            //dépiler p
            
            CP = t2d.GetPixel(x, y);
            
            /* On détermine les abscisses extrêmes xg et xd de la ligne de balayage (y) du germe courant */
            /* Recherche de xd : extrême à droite */
            
            xd = x + 1;
            CPd = CP;
            while (CPd != CC)
            {
                xd = xd + 1;
                CPd = t2d.GetPixel(xd, y);
            }
            xd--;
            /* Recherche de xg : extrême à gauche */
            xg = x - 1;
            CPg = CP;
            while (CPg != CC)
            {
                xg--;
                CPg = t2d.GetPixel(xg, y);
            }

            xg++;
            
            /* Affichage de la ligne de balayage de xg à xd, avec la couleur CR */
            
            //afficheLigne(xg,y,xd,y,CR)
            
            /* Recherche de nouveaux germes sur la ligne de balayage au-dessus :
            la recherche s'effectue entre xg et xd */
            x = xd;
            CP = t2d.GetPixel(x, y + 1);

            while (x>xg)
            {
                while ((CP == CC || CP == CR) && x > xg)
                {
                    x--;
                    CP = t2d.GetPixel(x, y + 1);
                }

                if (x > xg && CP != CC && CP != CR)
                {
                    /* On empile le nouveau germe au-dessus trouvé */
                    //empile((x,y+1),p)
                }

                while (CP != CC && x > xg)
                {
                    x--;
                    CP = t2d.GetPixel(x, y + 1);
                }
            }
            
            /* Recherche de nouveaux germes sur la ligne de balayage au-dessous :
            la recherche s'effectue entre xg et xd */

            x = xd;
            CP = t2d.GetPixel(x, y - 1);
            while (x > xg)
            {
                while ((CP == CC || CP == CR) && x > xg)
                {
                    x--;
                    CP = t2d.GetPixel(x, y - 1);
                }

                if (x > xg && CP != CC && CP != CR)
                {
                    /* On empile le nouveau germe au-dessous trouvé */
                    //empile((x,y-1),p)
                }

                while (CP != CC && x > xg)
                {
                    x--;
                    CP = t2d.GetPixel(x, y - 1);
                }
            }

        }
        
        
    }
}
