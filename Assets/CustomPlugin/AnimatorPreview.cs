using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class AnimatorPreview : MonoBehaviour {

	private Animator _anim;

	public Animator anim {
		get {
			if (_anim == null)
				_anim = GetComponent<Animator> ();
			return _anim;
		}
	}

	[Range (0f, 1f)]
	public float speed = 1;

	public bool useNormalizedTime = false;

	[Range (0f, 1f)]
	public float normalizedTime = 0;

	[Tooltip ("ture used state. false used param translate.")]
	public bool passPlay = true;

	public bool passUpdate = true;

	[Range (-1f, 2f)]
	public float updateTimeScale = 1;

	void OnEnable () {
		//		foreach(var a in anim.runtimeAnimatorController.animationClips){
		//			clips.Add (a.name);
		//		}

	}

	void OnDisable () {

	}

	void Update () {

		if (Application.isPlaying)
			return;

		if (!useNormalizedTime) {

			normalizedTime += Time.deltaTime * updateTimeScale;
			if (normalizedTime > 1) {
				normalizedTime = 0;
			}
		}

	}

}