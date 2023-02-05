using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class Path {
    [SerializeField, HideInInspector]
    List<Vector2> points;
    [SerializeField, HideInInspector]
    bool autoSetControlPoints;

    public Path(Vector2 centre) {
        points = new List<Vector2> {
            centre + Vector2.left,
            centre + (Vector2.left + Vector2.up)*0.5f,
            centre + (Vector2.right + Vector2.down)*0.5f,
            centre + Vector2.right
        };
        Debug.Log("points length initial: " + points.Count);
    }


    public void AddSegment(Vector2 anchor) {
        Debug.Log("points length: " + points.Count);
        points.Add(points[points.Count - 1]*2f - points[points.Count-2]);
        points.Add((points[points.Count - 1] + anchor)*0.5f);
        points.Add(anchor);
        if(autoSetControlPoints)
            AutomateAnchor(points.Count-1);

    }

    public void AutomateAllAffected(int index) {
        for(int i = Math.Max(0, index-3); i <= Math.Min(points.Count-1, index + 3); i+=3) {
            AutomateAnchor(i);
        }
        AutomateAnchor(0);
        AutomateAnchor(points.Count-1);
    }

    public void AutomateAllAnchors() {
        for (int i = 0; i < points.Count; i+= 3) { //start at second segment, move one segment at a time till penultimate segment
            AutomateAnchor(i);            
        }
    }

    public void AutomateAnchor(int index) {
        bool atStart = index == 0;
        bool atEnd = index == points.Count - 1;
        Vector2 current = points[index];
        if (!atStart && !atEnd) {
            Vector2 next = points[index+3];
            Vector2 prev = points[index-3];

            //calculate direction of control points
            Vector2 normalisedToNext = (next - current).normalized;
            Vector2 normalisedToPrev = (prev - current).normalized;
            Vector2 dir = (normalisedToNext - normalisedToPrev).normalized;

            var magnitudeToNext = Mathf.Abs((next-current).magnitude)*0.5f;
            var magnitudeToPrev = Mathf.Abs((prev - current).magnitude)*0.5f;

            points[index-1] = points[index] -dir * magnitudeToPrev;
            points[index+1] = points[index] + dir * magnitudeToNext;
        } else if (atStart) {
            points[index+1] = (points[index] + points[index+2])*0.5f;
        } else if (atEnd) {
            points[index-1] = (points[index] + points[index-2])*0.5f;
        }
    }
    public Vector2[] GetPointsInSegment(int i){
        return new Vector2[]{points[i*3], points[i*3+1], points[i*3 + 2], points[i*3 + 3]};
    }

    public int NumSegments {
        get {
            return (points.Count - 4) / 3 + 1;
        }
    }
    
    public int NumPoints {
        get {
            return points.Count;
        }
    }

    public bool AutoSetControlPoints {
        get {
            return autoSetControlPoints;
        }
        set {
            if(autoSetControlPoints != value) {
                autoSetControlPoints = value;
                if(autoSetControlPoints) {
                    AutomateAllAnchors();
                }
            }
        }
    }

    public Vector2 this[int index] {
        get {
            return points[index];
        }
    }

    public void MovePoint(int index, Vector2 newPosition) {
        Vector2 deltaMove = newPosition - points[index];
        points[index] = newPosition;

        if (index % 3 == 0) { //moving an anchor point
            if(index + 1 < points.Count) {
                points[index + 1] += deltaMove;
            } 
            if (index - 1 >= 0) {
                points[index-1] += deltaMove;
            }
            if(autoSetControlPoints) 
                AutomateAllAffected(index);
        } else { //moving a control point
            bool nextPointIsAnchor = (index+1) % 3 == 0;
            int correspondingControlIndex = nextPointIsAnchor ? index + 2 : index - 2; 
            int anchorIndex = nextPointIsAnchor ? index + 1 : index - 1;

            if(correspondingControlIndex >= 0 && correspondingControlIndex < points.Count) {
                float dist = (points[anchorIndex] - points[correspondingControlIndex]).magnitude;
                Vector2 dir = (points[anchorIndex] - newPosition).normalized;
                points[correspondingControlIndex] = points[anchorIndex] + dir*dist;
            }
            if(autoSetControlPoints)
                AutomateAllAffected(anchorIndex);
        }

    }


    public static Vector2 EvaluateQuadratic(Vector2 a, Vector2 b, Vector2 c, float t) {
        Vector2 p0 = Vector2.Lerp(a, b, t);
        Vector2 p1 = Vector2.Lerp(b, c, t);
        return Vector2.Lerp(p0, p1, t);
    }

    public static Vector2 EvaluateCubic(Vector2 a, Vector2 b, Vector2 c, Vector2 d, float t) {
        Vector2 p1Quadratic = EvaluateQuadratic(a, b, c, t);
        Vector2 p2Quadratic = EvaluateQuadratic(b, c, d, t);
        return Vector2.Lerp(p1Quadratic, p2Quadratic, t);
    }

    public Vector2[] CalculateEvenlySpaced(float spacing, float resolution = 1) {
        List<Vector2> evenlySpacedPoints = new List<Vector2>();
        evenlySpacedPoints.Add(points[0]);
        Vector2 previousPoint = points[0];
        float dstSinceLastPoint = 0;
        for(int segmentIndex = 0; segmentIndex < NumSegments; segmentIndex++) {
            Vector2[] p = GetPointsInSegment(segmentIndex);
            float controlNetLength = Vector2.Distance(p[0], p[1]) + Vector2.Distance(p[1], p[2]) + Vector2.Distance(p[2], p[3]);
            float estimatedCurveLength = Vector2.Distance(p[0], p[3]) + controlNetLength/2;
            int divisions = Mathf.CeilToInt(estimatedCurveLength * resolution * 10);
            float t = 0;
            while (t <= 1) {
                t += 1f/divisions;
                Vector2 pointOnCurve = EvaluateCubic(p[0], p[1], p[2], p[3], t);
                dstSinceLastPoint += Vector2.Distance(previousPoint, pointOnCurve);

                while (dstSinceLastPoint >= spacing) {
                    float overshootDst = dstSinceLastPoint - spacing;
                    Vector2 newEvenlySpacedPoint = pointOnCurve + (previousPoint - pointOnCurve).normalized * overshootDst;
                    evenlySpacedPoints.Add(newEvenlySpacedPoint);
                    dstSinceLastPoint = overshootDst;
                    previousPoint = newEvenlySpacedPoint;
                }

                previousPoint = pointOnCurve;
            }
        }
        return evenlySpacedPoints.ToArray();
    }
}