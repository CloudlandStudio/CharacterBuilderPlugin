using System.Collections.Generic;
using UnityEngine;

//玩家角色身上的部件,包含服裝,皮膚,各種可以替換的mesh
[CreateAssetMenu (fileName = "CustomPartData",
    menuName = "CharacterBuilder/CustomPartData",
    order = 1)]
public class CustomPartData : ScriptableObject {
    ///<summary>
    ///部件的顯示名稱
    ///</summary>
    public string displayName = "顯示名稱";
    public enum PartType {
        Accessory, //附件,頭上的裝飾物
        Costume, //服裝&和服裝一套的skin
        Eye, //眼睛,瞳孔
        Face, //臉型,包含五官
        Hair, //髮型
        SkinHead, //頭部skin的陰影
    }
    ///<summary>
    ///部件類型
    ///</summary>
    public PartType partType;
    ///<summary>
    ///將skinnedMeshRenderer的Transfrom訊息轉換成字串儲存,
    ///一個部件可能包含多個skinned,比如說Custume就是服裝和skin為一套
    ///</summary>
    public SkinnedBaker[] skinnedBakers;

    public Texture2D previewTexture;
#if UNITY_EDITOR
    public bool doFindSkinnedBaker = false;

    public bool doFindPreview = false;

    // public bool bestMatch = true;

    /// <summary>
    /// Called when the script is loaded or a value is changed in the
    /// inspector (Called in the editor only).
    /// </summary>
    void OnValidate () {
        findSkinnedBaker ();
        findPreview ();
    }

    void findPreview () {
        if (!doFindPreview) return;
        doFindPreview = false;
        var preivewsGuid = UnityEditor.AssetDatabase.FindAssets (this.name);

        for (int i = 0; i < preivewsGuid.Length; i++) {
            string guid = preivewsGuid[i];
            var path = UnityEditor.AssetDatabase.GUIDToAssetPath (guid);
            var obj = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D> (path);
            if (obj != null) {

                previewTexture = obj;
                break;
            }
        }

    }

    void findSkinnedBaker () {

        if (!doFindSkinnedBaker) return;
        doFindSkinnedBaker = false;

        //var bakers = FindObjectsOfType<SkinnedBaker> ();
        //var bakersGuid = UnityEditor.AssetDatabase.FindAssets (this.name + " t:SkinnedBaker");
        List<SkinnedBaker> tempSkinnedBaker = new List<SkinnedBaker> ();
        var bakersGuid = UnityEditor.AssetDatabase.FindAssets (this.name);

        for (int i = 0; i < bakersGuid.Length; i++) {
            string guid = bakersGuid[i];
            var path = UnityEditor.AssetDatabase.GUIDToAssetPath (guid);
            var obj = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject> (path);
            if (obj != null) {
                var baker = obj.GetComponent<SkinnedBaker> ();
                if (baker != null) {
                    //Debug.Log ("[LOG] Find Baker:" + path);
                    tempSkinnedBaker.Add (baker);
                }
            }
        }

        skinnedBakers = tempSkinnedBaker.ToArray ();

        UnityEditor.EditorUtility.SetDirty (this);
        UnityEditor.AssetDatabase.SaveAssets ();
    }
#endif
}