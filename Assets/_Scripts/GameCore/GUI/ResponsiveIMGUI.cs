using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResponsiveIMGUI
{
    private static Vector2 mDesignResolution = new Vector2(1024, 768);

    private static float mScaler = 1;

    public static void InitDesignResolution(Vector2 newValue)
    {
        mDesignResolution = newValue;


        if (mDesignResolution.y > mDesignResolution.x)
        { // Portrait 
            mScaler = Screen.width / mDesignResolution.x;
        }
        else
        {    // Landscape 
            mScaler = Screen.height / mDesignResolution.y;
        }

        //Debug.Log("Resolutio Change to " + mDesignResolution + " scale=" + mScaler);
    }

    public static Vector2 DesignResolution
    {
        get
        {
            return mDesignResolution;
        }

        set
        {
            mDesignResolution = value;


            if (mDesignResolution.y > mDesignResolution.x)
            { // Portrait 
                mScaler = Screen.width / mDesignResolution.x;
            }
            else
            {    // Landscape 
                mScaler = Screen.height / mDesignResolution.y;
            }

            //Debug.Log("Resolutio Change to " + mDesignResolution + " scale=" + mScaler);
        }
    }


    // Note: Call this in OnGUI 
    public static void ScaleGUI()
    {
        //Debug.Log("mScaler=" + mScaler);
        ScaleGUIWithValue(mScaler);
    }

    public static void ScaleGUIWithValue(float scale)
    {
        Matrix4x4 _matrix = GUI.matrix;
        _matrix.m00 = scale;
        _matrix.m11 = scale;
        GUI.matrix = _matrix;
    }
}