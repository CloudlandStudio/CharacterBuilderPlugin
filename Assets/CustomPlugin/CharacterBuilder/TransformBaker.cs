using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformBaker : MonoBehaviour {
    public string parentName;
    public Vector3 localPosition;
    public Quaternion localRotation;
    public Vector3 localScale;
    public bool bakeIt = false;

    /// <summary>
    /// Called when the script is loaded or a value is changed in the
    /// inspector (Called in the editor only).
    /// </summary>
    void OnValidate () {
        if (!bakeIt) return;
        bakeIt = false;
        if (transform.parent != null)
            parentName = transform.parent.name;

        localPosition = transform.localPosition;
        localRotation = transform.localRotation;
        localScale = transform.localScale;

    }
}