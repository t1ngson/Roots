using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RootRenderer))]
public class NarrowMeshEditor : Editor {
    RootRenderer renderer;    

    void OnSceneGUI() {
        if(renderer.autoUpdate && Event.current.type == EventType.Repaint) {
            renderer.UpdateNarrowMesh();
        }
    }

    void OnEnable() {
        renderer = (RootRenderer)target;
    }
}
