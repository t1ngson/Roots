using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateRoot : MonoBehaviour
{
    public List<Color> generateGradient(){
        int rMax = Color.FromArgb(77,45,10)
        int rMin = Color.FromArgb(199,199,199)
        int size = 9
        var colorList = new List<Color>();

        for(int i=0; i<size; i++)
        {
            var rAverage = rMin + (int)((rMax - rMin) * i / size);
            var gAverage = gMin + (int)((gMax - gMin) * i / size);
            var bAverage = bMin + (int)((bMax - bMin) * i / size);
            colorList.Add(Color.FromArgb(rAverage, gAverage, bAverage));
        }

        return colorList;
    }
    
    public void generateRoot(int centrePosX, int centrePosY){
        // Create a 9x1 texture of the root
        var texture = new Texture2D(9, 1, TextureFormat.ARGB32, false);

        colorListFinal = generateGradient()
        
        for(int j=0; j<size; j++)
        {
            texture.SetPixel(j, 0, colorList[j]);
        }

        // Apply all SetPixel calls
        texture.Apply();
 
        // connect texture to material of GameObject this script is attached to
        renderer.material.mainTexture = texture;
    }
}
