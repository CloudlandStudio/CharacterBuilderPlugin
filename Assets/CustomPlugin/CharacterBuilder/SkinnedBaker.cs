using System.Collections.Generic;
using UnityEngine;

public class SkinnedBaker : MonoBehaviour {
    [SerializeField][HideInInspector] SkinnedMeshRenderer skinnedCached;
    public SkinnedMeshRenderer skinned {
        get {
            if (skinnedCached == null)
                skinnedCached = this.GetComponent<SkinnedMeshRenderer> ();
            return skinnedCached;
        }
    }
    public string[] bonesName;
    public string[] bonesFullName;
    public string rootBoneName;
    public bool bakeIt = false;
    public Mesh mesh;
    public Material[] materials;
    /// <summary>
    /// 將baker裡面的資訊回復到skinned裡面
    /// </summary>
    public void LoadToSkinned (Dictionary<string, Transform> map) {
        var boneCount = bonesName.Length;
        Transform[] bones = new Transform[boneCount];
        for (int i = 0; i < boneCount; i++) {
            //bones[i] = map[];
            var boneName = bonesName[i];
            Transform temp;
            if (map.TryGetValue (boneName, out temp)) {
                bones[i] = temp;
            } else {
                Debug.LogError ("[Baker] bone not found. bone:" + boneName + " full:" + bonesFullName[i]);
            }
        }
        skinned.bones = bones;

        skinned.sharedMesh = mesh;
        skinned.sharedMaterials = materials;

        if (!string.IsNullOrEmpty (rootBoneName)) {
            Transform temp;
            if (map.TryGetValue (rootBoneName, out temp)) {
                skinned.rootBone = temp;
            } else {
                Debug.LogError ("[Baker] root bone not found. bone:" + rootBoneName);
            }

        }
    }

    /// <summary>
    /// Called when the script is loaded or a value is changed in the
    /// inspector (Called in the editor only).
    /// </summary>
    void OnValidate () {
        if (!bakeIt) return;
        bakeIt = false;

        if (skinned == null) {
            Debug.Log ("[Bake Failure] skinned mesh renderer not found.", this);
            return;
        }

        mesh = skinned.sharedMesh;
        materials = skinned.sharedMaterials;

        var rootBone = skinned.rootBone;
        var bones = skinned.bones;
        var bl = bones.Length;
        bonesName = new string[bl];
        bonesFullName = new string[bl];
        for (int i = 0; i < bl; i++) {
            var bone = bones[i];
            bonesName[i] = bone.name;
#if UNITY_EDITOR
            var fullPath = UnityEditor.AnimationUtility.CalculateTransformPath (bone, rootBone);
            bonesFullName[i] = fullPath;
#endif
        }
    }
}