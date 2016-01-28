using UnityEngine;
using System.Collections;

public static class LengthText {

	// Use this for initialization
	public static GameObject CreateLengthText(string textVal) {
		var lengthText = new GameObject();	
		var textMesh  = lengthText.AddComponent<TextMesh>();
//		var meshRenderer = lengthText.AddComponent<MeshRenderer>();

		textMesh.text = textVal;
		textMesh.characterSize = 0.01f;
		textMesh.anchor = TextAnchor.MiddleLeft;
		textMesh.alignment = TextAlignment.Left;
		textMesh.fontSize = 50;
		return lengthText;
	}
}
