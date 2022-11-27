using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Remplissage : MonoBehaviour
{
    private Texture2D t2d;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)){// click gauche
            RemplissageRegionConnexite(0,0,Color.black,Color.red);
         }
                        
    }

    public void RemplissageRegionConnexite(int x,int y,Color ColorContour,Color ColorRemplissage)
    {
        var colorPixelCourant = t2d.GetPixel(x, y);
        if (ColorContour != colorPixelCourant && ColorRemplissage != colorPixelCourant)
        {
            //AffichePixel(x,y,CR);
            RemplissageRegionConnexite(x,y-1,ColorContour,ColorRemplissage); //Bas
            RemplissageRegionConnexite(x-1,y,ColorContour,ColorRemplissage); //Gauche
            RemplissageRegionConnexite(x,y+1,ColorContour,ColorRemplissage); //Haut
            RemplissageRegionConnexite(x+1,y,ColorContour,ColorRemplissage); //Droite
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
}
