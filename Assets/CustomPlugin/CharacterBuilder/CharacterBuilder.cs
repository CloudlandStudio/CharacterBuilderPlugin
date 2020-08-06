using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class CharacterBuilder : MonoBehaviour {
    //animator設置成always animate好像可以避免換裝的時候造成的動畫停止撥放
    public Dictionary<string, Transform> bonesMap = new Dictionary<string, Transform> ();
    Transform cachedTransform;
    [Serializable]
    public class CustomPart {
        [SerializeField][HideInInspector]
        CustomPartData cachedPart;
        public CustomPartData customPart;
        public CustomPartData.PartType partType;
        ///<summary>
        ///儲存載入的部件實體化成的遊戲物件
        ///重新載入部件會(或必須)銷毀舊物件
        ///</summary>
        [SerializeField][HideInInspector]
        List<Transform> subPartList = new List<Transform> ();
        public bool slotTypeCheck () {
            return partType == customPart.partType;
        }
        public bool isMatchCache () {
            return customPart == cachedPart;
        }
        public void CacheIt () {
            cachedPart = customPart;
        }
        public void Load (CharacterBuilder builder) {
            var bakers = this.customPart.skinnedBakers;
            for (int i = 0; i < bakers.Length; i++) {
                var baker = bakers[i];
                var transBaker = baker.GetComponent<TransformBaker> ();

                var insBaker = MonoBehaviour.Instantiate (baker, builder.cachedTransform);

                if (transBaker != null) {
                    //這邊要處理物件是綁在骨架內部的情況
                    //處理方式目前計畫用unity內部的綁定工具(原本是為了rig設計)

                    var parentBone = builder.bonesMap[transBaker.parentName];

                    var constraint = insBaker.gameObject.AddComponent<ParentConstraint> ();
                    constraint.AddSource (new ConstraintSource () {
                        sourceTransform = parentBone,
                            weight = 1
                    });
                    insBaker.transform.SetPositionAndRotation (
                        Vector3.zero, Quaternion.identity
                    );

                    constraint.SetRotationOffset (0, transBaker.localRotation.eulerAngles);
                    constraint.SetTranslationOffset (0, transBaker.localPosition);
                    constraint.rotationAtRest = transBaker.localRotation.eulerAngles;
                    constraint.translationAtRest = transBaker.localPosition;
                    constraint.constraintActive = true;
                    constraint.locked = true;

                }

                insBaker.LoadToSkinned (builder.bonesMap);
                subPartList.Add (insBaker.transform);

            }
        }
        public void UnLoad () {
            for (int i = subPartList.Count - 1; i >= 0; i--) {
                if (subPartList[i] != null) {
                    if (Application.isPlaying) {
                        Destroy (subPartList[i].gameObject);
                    } else {
                        DestroyImmediate (subPartList[i].gameObject);
                    }
                }
            }
            subPartList.Clear ();
        }

    }
    public CustomPart AccessoryPart;
    public CustomPart CostumePart;
    public CustomPart EyePart;
    public CustomPart FacePart;
    public CustomPart HairPart;
    public CustomPart SkinHeadPart;
    void OnEnable () {
        Init ();
    }
    public bool isInit { get { return bonesMap.Count != 0; } }
    public void Init () {
        cachedTransform = this.transform;
        bonesMap.Clear ();
        //將骨骼映射到對應的名稱上
        //這樣才可以在載入skinned的時候分配正確的權重
        Transform[] allChildren = GetComponentsInChildren<Transform> ();
        foreach (Transform bone in allChildren) {
            if (bone == cachedTransform)
                continue;
            bonesMap.Add (bone.name, bone);
        }
    }
    void LoadPart (CustomPart part) {
        part.UnLoad ();
        part.Load (this);
    }

    // Update is called once per frame
    void Update () {
        //autoLoad
        AutoBuildUpdate ();
    }

    public void AutoBuildUpdate () {
        AutoLoad (AccessoryPart);
        AutoLoad (CostumePart);
        AutoLoad (EyePart);
        AutoLoad (FacePart);
        AutoLoad (HairPart);
        AutoLoad (SkinHeadPart);
    }

    void AutoLoad (CustomPart cp) {
        //不載入空物件,不然一定出事 = ="
        if (cp.customPart == null) return;

        //不載入錯誤型態的部件,不然會重複
        //TODO:在手動安裝部件的時候也要檢查這個
        if (!cp.slotTypeCheck ()) {
            return;
        }

        if (!cp.isMatchCache ()) {
            cp.CacheIt ();
            LoadPart (cp);
        }
    }

#if UNITY_EDITOR
    [Header ("Gizmos")]
    public bool drawGizmos = true;
    public Color gizmosColor;
    public bool drawBone;
    public Transform rootNode;
    public float boneWidth = 0.125f;
    Transform[] childNodes;
    public bool drawMesh;
    public Mesh gizmosMesh;
    void OnDrawGizmos () {
        if (drawGizmos)
            Gizmos.DrawIcon (transform.position, "CharacterBuilder Icon.png", true, gizmosColor);
    }
    void OnDrawGizmosSelected () {

        if (!drawGizmos) return;

        if (drawBone) {
            DrawSkeleton ();
        }
        if (drawMesh) {
            DrawMesh ();
        }
    }

    void DrawMesh () {
        if (gizmosMesh == null) return;
        Gizmos.DrawMesh (gizmosMesh, transform.position, transform.rotation);
    }

    void DrawSkeleton () {
        if (rootNode != null) {
            if (childNodes == null || (childNodes.Length == 0 && rootNode.childCount != 0)) {
                childNodes = rootNode.GetComponentsInChildren<Transform> ();
            }

            foreach (Transform child in childNodes) {

                if (child == rootNode) {
                    //list includes the root, if root then larger, green cube
                    // Gizmos.color = gizmosColor;
                    // Gizmos.DrawCube (child.position, new Vector3 (.1f, .1f, .1f));
                } else {
                    Gizmos.color = gizmosColor;
                    DrawBone (child.parent, child.parent.position, child.position);
                    // Gizmos.DrawCube (child.position, new Vector3 (.01f, .01f, .01f));
                }
            }

        } else {
            rootNode = transform;
        }
    }

    void DrawBone (Transform parent, Vector3 from, Vector3 to) {

        var dis = Vector3.Distance (from, to);
        var scale = Mathf.Clamp01 (dis);

        var a1 = from + parent.up * boneWidth * scale;
        Gizmos.DrawLine (a1, to);
        var a2 = from - parent.up * boneWidth * scale;
        Gizmos.DrawLine (a2, to);
        var a3 = from + parent.forward * boneWidth * scale;
        Gizmos.DrawLine (a3, to);
        var a4 = from - parent.forward * boneWidth * scale;
        Gizmos.DrawLine (a4, to);

        Gizmos.DrawLine (a1, a3);
        Gizmos.DrawLine (a3, a2);
        Gizmos.DrawLine (a4, a1);
        Gizmos.DrawLine (a4, a2);

    }

#endif
}