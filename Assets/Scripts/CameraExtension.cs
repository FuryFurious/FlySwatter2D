using UnityEngine;
using System.Collections;



public static class CameraExtension
{

    public static float UnitsPerPixel(this Camera cam)
    {
        var p1 = cam.ScreenToWorldPoint(Vector3.zero);
        var p2 = cam.ScreenToWorldPoint(Vector3.right);
        //Debug.Log(p1);
        //Debug.Log(p2);
        return Vector3.Distance(p1, p2);
    }

    public static float PixelsPerUnit(this Camera cam)
    {
        return 1.0f / UnitsPerPixel(cam);
    }

}
