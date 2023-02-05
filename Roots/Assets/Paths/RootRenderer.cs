using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[RequireComponent(typeof(PathCreator))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class RootRenderer : MonoBehaviour {

    PathCreator creator;
    public Path path;

    float pixelDim = 0.05f;
    int pixelsPerScreenX;
    public bool autoUpdate;

    void Awake() {
        creator = GetComponent<PathCreator>();
        creator.CreatePath();
        path = creator.path;
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public Color GetColor(float distFromCurve) {

        float coreThickness = 0.15f;
        float barkThickness = 0.15f;
        float crossfadeDist = 0.1f;
        Color coreColor = new Color(0.6784f,0.5647f,0.4824f,1);
        Color barkColor = new Color(0.4902f,0.2353f,0.0471f,1);


        if (distFromCurve > coreThickness + barkThickness + crossfadeDist) {
            return Color.clear;
        } else if (distFromCurve < coreThickness){
            return coreColor;
        } else if (distFromCurve < coreThickness + barkThickness) {
            return Color.Lerp(
                coreColor,
                barkColor,
                (distFromCurve - coreThickness) / (barkThickness)
            );
        } else {
            return Color.Lerp(
                barkColor,
                Color.clear,
                (distFromCurve - (barkThickness + coreThickness)) / (crossfadeDist)
            );
        }
    }    

    public int approximateClosest(Vector2[] points, Vector2 point, int previousMatch, int aboveMatch) {
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

        if(previousMatch >= 0 && Vector2.Distance(points[previousMatch], point) < Vector2.Distance(points[closestPoint], point)){
            closestPoint = previousMatch;
        }

        if(aboveMatch >= 0 && Vector2.Distance(points[aboveMatch], point) < Vector2.Distance(points[closestPoint], point)){
            closestPoint = aboveMatch;
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

        int[,] previousClosests = new int[noPixelsX,noPixelsY];
        int previousClosest = -1;
        for (int currentY = 0; currentY < noPixelsY; currentY++) {
            for(int currentX = 0; currentX < noPixelsX; currentX++) {
                Vector2 worldVector = new Vector2((currentX + meshOffsetX)*pixelDim + 0.5f*pixelDim, 0.5f*pixelDim + (currentY + meshOffsetY) * pixelDim);
                int prevClosest = currentX > 0 ? previousClosests[currentX-1, currentY] : -1;
                int aboveClosest = currentY > 0 ? previousClosests[currentX, currentY - 1] : -1;
                int closest = approximateClosest(points, worldVector, previousClosest, aboveClosest);
                texture.SetPixel(
                    currentX, 
                    currentY, 
                    GetColor(Vector2.Distance(points[closest], worldVector))
                );
                previousClosest = closest;
                previousClosests[currentX, currentY] = closest;
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
            0,
            1,
            2,
            2,
            1,
            3
        };
        
        Mesh BBMesh = new Mesh();
        BBMesh.vertices = BBVerts;
        BBMesh.triangles = BBTris;
        BBMesh.uv = uvs;
        GetComponent<MeshFilter>().mesh = BBMesh;

        var renderer = GetComponent<MeshRenderer>();
        if(renderer == null) {
            Debug.Log("test");
        } else {
            Debug.Log("not null");
        }
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
