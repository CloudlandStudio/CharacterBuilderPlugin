using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class AutoLoadPart : MonoBehaviour {
    public CharacterBuilder characterBuilder;
    public bool autoLoad = false;
    // Update is called once per frame
    void Update () {
        //這個組件只在編輯器下起作用
        if (Application.isPlaying) return;

        if (!autoLoad) return;
        if (characterBuilder == null) return;
        //auto init
        if (!characterBuilder.isInit)
            characterBuilder.Init ();

        characterBuilder.AutoBuildUpdate ();
    }
}