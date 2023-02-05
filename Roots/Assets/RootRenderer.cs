using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[RequireComponent(typeof(PathCreator))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class RootRenderer : MonoBehaviour {

    PathCreator creator;
    Path path;

    float pixelDim = 0.1f;
    int pixelsPerScreenX;
    public bool autoUpdate;

    void Start() {
        creator = GetComponent<PathCreator>();
        path = creator.path;
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public Color GetColor(float distFromCurve) {
        return Color.Lerp(Color.black, Color.clear, distFromCurve);
    }    

    public int approximateClosest(Vector2[] points, Vector2 point, int? previousMatch) {
        int closestPoint;
        if(point.y < points[0].y) {
            closestPoint = 0;
        } else if (point.y> points[points.Length-1].y){
            closestPoint = 1;
        } else {

            int low = 0;
            int high = points.Length - 1;

            while (low <= high) {
                int mid = (high + low) / 2;

                if(point.y < points[mid].y) {
                    high = mid - 1;
                } else if (point.y > points[mid].y) {
                    low = mid + 1;
                } else {
                    low = mid + 1;
                    high = mid;
                }
            }
            closestPoint = Vector2.Distance(points[low], point) < Vector2.Distance(point, points[high]) ? low : high;
        }

        if(previousMatch != null && Vector2.Distance(points[(int)previousMatch], point) < Vector2.Distance(points[closestPoint], point)){
            closestPoint = (int) previousMatch;
        }

        while(closestPoint < points.Length - 1 && Vector2.Distance(points[closestPoint + 1], point) < Vector2.Distance(points[closestPoint], point)) {
            closestPoint++;
        }

        while(closestPoint > 0 && Vector2.Distance(points[closestPoint - 1], point) < Vector2.Distance(points[closestPoint], point)) {
            closestPoint--;
        }

        return closestPoint;
    }


    public void UpdateNarrowMesh() {

        Path path = GetComponent<PathCreator>().path;
        Vector2[] points = path.CalculateEvenlySpaced(spacing);
        Array.Sort(points, (pointA, pointB) => pointA.y.CompareTo(pointB.y));


        float minY = points[0].y;
        float maxY = points[points.Length-1].y;
        float minX = points[0].x;
        float maxX = points[0].x;
        for (int i = 1; i < points.Length; i++) {
            if(points[i].x < minX)
                minX = points[i].x;
            if(points[i].x > maxX)
                maxX = points[i].x;
        }

        float margin = 1f;
        


        



        int meshOffsetX = Mathf.FloorToInt((minX - margin)/pixelDim);
        int meshOffsetY = Mathf.FloorToInt((minY - margin)/pixelDim);
        int noPixelsX = Mathf.CeilToInt(((maxX + margin) - (minX - margin))/pixelDim);
        int noPixelsY = Mathf.CeilToInt(((maxY + margin) - (minY - margin))/pixelDim);
        var texture = new Texture2D(
            noPixelsX,
            noPixelsY,
            TextureFormat.ARGB32,
            false
        );

        int? previousClosest = null;
        for (int currentX = 0; currentX < noPixelsX; currentX++) {
            for(int currentY = 0; currentY < noPixelsY; currentY++) {
                Vector2 worldVector = new Vector2((currentX + meshOffsetX)*pixelDim + 0.5f*pixelDim, 0.5f*pixelDim + (currentY + meshOffsetY) * pixelDim);
                int closest = approximateClosest(points, worldVector, previousClosest);
                texture.SetPixel(
                    currentX, 
                    currentY, 
                    GetColor(Vector2.Distance(points[closest], worldVector))
                );
                previousClosest = closest;
            }
        }
        texture.Apply();

        texture.filterMode = FilterMode.Point;
        texture.anisoLevel = 0;

        Vector3[] BBVerts = {
            new Vector3(meshOffsetX*pixelDim, meshOffsetY*pixelDim, 1),
            new Vector3((meshOffsetX + noPixelsX)*pixelDim, meshOffsetY * pixelDim, 1),
            new Vector3(meshOffsetX * pixelDim, (meshOffsetY + noPixelsY)*pixelDim, 1),
            new Vector3((meshOffsetX + noPixelsX)*pixelDim, (meshOffsetY + noPixelsY)*pixelDim, 1)
        };

        Vector2[] uvs = {
            new Vector2(0,0),
            new Vector2(1f,0),
            new Vector2(0,1f),
            new Vector2(1f,1f)
        };

        int[] BBTris = {
            2,
            1,
            0,
            3,
            1,
            2
        };
        
        Mesh BBMesh = new Mesh();
        BBMesh.vertices = BBVerts;
        BBMesh.triangles = BBTris;
        BBMesh.uv = uvs;
        GetComponent<MeshFilter>().mesh = BBMesh;

        var renderer = GetComponent<MeshRenderer>();
        renderer.sharedMaterial.mainTexture = texture;  

    }

    [Range(.05f, 1.5f)]
    public float spacing = 1; 
    public float narrowMeshWidth = 1;
    Mesh CreateNarrowMesh(Vector2[] points) {
        Vector3[] verts = new Vector3[points.Length * 2];
        int[] tris = new int[2* (points.Length - 1) * 3];
        int vertIndex = 0;
        int triIndex = 0;
        
        for (int i = 0; i < points.Length; i++) {
            Vector2 forward = Vector2.zero;
            if (i < points.Length - 1) {
                forward += points[i+1] - points[i];
            }
            if (i > 0) {
                forward += points[i] - points[i - 1];
            }
            forward.Normalize();
            Vector2 left = new Vector2(-forward.y, forward.x);

            verts[vertIndex] = points[i] + left * narrowMeshWidth * 0.5f;
            verts[vertIndex + 1] = points[i] - left * narrowMeshWidth * 0.5f;

            if (i < points.Length - 1) {
                tris[triIndex] = vertIndex;
                tris[triIndex + 1] = vertIndex + 2;
                tris[triIndex + 2] = vertIndex + 1;


                tris[triIndex + 3] = vertIndex + 1;
                tris[triIndex + 4] = vertIndex + 2;
                tris[triIndex + 5] = vertIndex + 3;
            }

            vertIndex += 2;
            triIndex += 6;
        }

        

        Mesh mesh = new Mesh();
        mesh.vertices = verts;
        mesh.triangles = tris;

        return mesh;
    }

    

}
