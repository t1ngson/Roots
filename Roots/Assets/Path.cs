using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class Path {
    [SerializeField]
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
    }


    public void AddSegment(Vector2 anchor) {
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
            AutomateAllAffected(correspondingControlIndex);
        }

    }
}