using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class PixelColorScript : MonoBehaviour
{

    /*Take in:
    -X, Y for one pixel (int, int)
    -Distance of pixel from the centre of the root (float)
    -total length of root (float)
    -where on the root it is (float from 0-1)

    Return:
    -a hex value of the gradient
    -(x,y)

    USE LAB
    */



    public Color generateColour(int pixel_x, int pixel_y, float distance, float length, float positionOnRoot){
        //darker the further from centre of root of root
        //as the root grows more pixels are 
        //make it darker the further up the root it is
        //darken it as it 

        vector4 pixelColour = (0,0,0,0)
        int starting_width = 5
        int final_width = 50
        float widthCoefficient = 0.1

        //thickness increases by this much per pixel down
        //i.e 0.1 means 10 pixels down = 1 pixel out

        if (distance > final_width){
            return (0,0,0,0)
            //transparent pixel
        }
        elif(position_on_root * length > max_width_at_length){
            
        }

        // linear function to calculate current width of the top of the root, capped at a value
        var currentMaxWidth = math.min(final_width, length*widthCoefficient);

        // linear interpolates between starting width and current max width of the root, at the current position on the root.
        var thisPixelWidth = positionOnRoot*(currentMaxWidth - starting_width) + starting_width;
        
        return
        




        return(Color.clear)
    }

}
