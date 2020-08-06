using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;
using AnimatorController = UnityEditor.Animations.AnimatorController;

[CustomEditor (typeof (AnimatorPreview))]
public class AnimatorPreviewEditor : Editor {

	private AnimatorPreview ap;

	public int selectIndex;

	//string[] Clips;

	string[] States;

	Dictionary<string, int> stateToLayer = new Dictionary<string, int> ();

	string pre_state;

	void OnEnable () {

		ap = target as AnimatorPreview;

		/*List<string> clips = new List<string> ();

		foreach (var a in ap.anim.runtimeAnimatorController.animationClips) {
			clips.Add (a.name);
		}
		Clips = clips.ToArray ();*/

		CheckController ();

		EditorApplication.update += updateAnimator;

	}

	RuntimeAnimatorController tempContorller;

	void CheckController () {

		if (tempContorller == ap.anim.runtimeAnimatorController) {
			return;
		}

		tempContorller = ap.anim.runtimeAnimatorController;

		if (ap.anim.runtimeAnimatorController == null) {
			return;
		}

		List<string> statesList = new List<string> ();

		var controller = ap.anim.runtimeAnimatorController as AnimatorController; //八成是用interface來實現相容信,所以runtime的部分是原本controller的閹割版...,黑科技阿

		var layers = controller.layers;

		foreach (var layer in layers) {

			var stateMachine = layer.stateMachine;

			var states = stateMachine.states;

			foreach (var state in states) {
				statesList.Add (state.state.name);
				stateToLayer.Add (state.state.name, layer.syncedLayerIndex);
			}
		}

		States = statesList.ToArray ();
	}

	void OnDisable () {
		EditorApplication.update -= updateAnimator;
	}

	public override void OnInspectorGUI () {

		CheckController ();

		if (Application.isPlaying || ap.anim.runtimeAnimatorController == null || ap.enabled == false) {

			if (Application.isPlaying) {
				EditorGUILayout.HelpBox ("game is playing.", MessageType.Warning);
			}

			if (ap.anim.runtimeAnimatorController == null) {
				EditorGUILayout.HelpBox ("controller is null.", MessageType.Warning);
			}

			if (ap.enabled == false) {
				EditorGUILayout.HelpBox ("component is disable.", MessageType.Warning);
			}

			base.OnInspectorGUI ();
			return;
		}

		serializedObject.Update ();

		selectIndex = EditorGUILayout.Popup (selectIndex, States);

		ap.anim.speed = ap.speed;

		ap.anim.updateMode = (AnimatorUpdateMode) EditorGUILayout.EnumPopup ("update mode", ap.anim.updateMode);

		//updateAnimator ();

		//		EditorUtility.SetDirty (ap.transform);

		paramsGUI ();

		//EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene());
		serializedObject.ApplyModifiedProperties ();
		base.OnInspectorGUI ();

	}

	bool paramsFold = false;

	void paramsGUI () {

		var controller = ap.anim.runtimeAnimatorController as AnimatorController;

		var parameters = controller.parameters;

		GUILayout.Label ("params", EditorStyles.boldLabel);

		EditorGUI.indentLevel++;

		paramsFold = EditorGUILayout.Foldout (paramsFold, "params:");

		if (!paramsFold) {
			EditorGUI.indentLevel--;
			return;
		}

		foreach (var parameter in parameters) {
			if (parameter.type == UnityEngine.AnimatorControllerParameterType.Bool) {
				var v = EditorGUILayout.Toggle (parameter.name, ap.anim.GetBool (parameter.name));
				ap.anim.SetBool (parameter.name, v);
				continue;
			}

			if (parameter.type == UnityEngine.AnimatorControllerParameterType.Float) {
				var v = EditorGUILayout.FloatField (parameter.name, ap.anim.GetFloat (parameter.name));
				ap.anim.SetFloat (parameter.name, v);
				continue;
			}

			if (parameter.type == UnityEngine.AnimatorControllerParameterType.Int) {
				var v = EditorGUILayout.IntField (parameter.name, ap.anim.GetInteger (parameter.name));
				ap.anim.SetInteger (parameter.name, v);
				continue;
			}

			if (parameter.type == UnityEngine.AnimatorControllerParameterType.Trigger) {
				var t = EditorGUILayout.Toggle (parameter.name, false, EditorStyles.radioButton);
				//var t = GUILayout.SelectionGrid(parameter.name, false);
				if (t) {
					ap.anim.SetTrigger (parameter.name);
				}
				continue;
			}
		}

		GUILayout.Space (10);

		EditorGUI.indentLevel--;

	}

	void updateAnimator () {

		if (Application.isPlaying || ap.anim.runtimeAnimatorController == null || ap.enabled == false) {
			return;
		}

		//ap.anim.Play (Clips [selectIndex], 0, ap.timeScale);
		var state = States[selectIndex];

		//if(state != pre_state)
		if (ap.passPlay)
			ap.anim.Play (state, stateToLayer[state], ap.normalizedTime);

		if (ap.passUpdate)
			ap.anim.Update (Time.deltaTime * ap.updateTimeScale);

		pre_state = state;
	}
}