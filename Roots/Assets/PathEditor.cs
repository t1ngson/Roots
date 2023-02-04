using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(PathCreator))]
public class PathEditor : Editor
{
    PathCreator creator;
    Path path;


    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        
        bool autoSetControlPoints = GUILayout.Toggle(path.AutoSetControlPoints, "Auto Set Control Points");
        if (autoSetControlPoints != path.AutoSetControlPoints) {
            Undo.RecordObject(creator, "Toggle auto set control points.");
            path.AutoSetControlPoints = autoSetControlPoints;
        }
    }

    void OnSceneGUI() {
        Input();
        Draw();
}

    void Input() {
        Event guiEvent = Event.current;
        Vector2 mousePos = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition).origin;

        if(guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.shift){
            Undo.RecordObject(creator, "Add segment");
            path.AddSegment(mousePos);
        }
    }

    void Draw() {

        Handles.color = Color.black;
        for(int i = 0; i < path.NumSegments; i++) {
            Vector2[] points = path.GetPointsInSegment(i);
            Handles.DrawLine(points[1], points[0]);
            Handles.DrawLine(points[2], points[3]);
            Handles.DrawBezier(points[0], points[3], points[1], points[2], Color.green, null, 2f);
        }

        Handles.color = Color.red;
        for (int i = 0; i < path.NumPoints; i++) {
            Vector2 newPosn = Handles.FreeMoveHandle(path[i], Quaternion.identity, .1f, Vector2.zero, Handles.CylinderHandleCap);
            if (newPosn != path[i]) {
                Undo.RecordObject(creator, "Move point");
                path.MovePoint(i, newPosn);
            }
        }
    }

    void OnEnable() {
        creator = (PathCreator)target;
        if(creator.path == null) {
            creator.CreatePath();
        }
        path = creator.path;
    }
}
