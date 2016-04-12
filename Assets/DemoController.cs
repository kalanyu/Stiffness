using UnityEngine;
using System.Collections;

public class DemoController : MonoBehaviour {
	public float leftMostWeight;
	public float leftMiddleWeight;
	public float leftWeight;
	public float middleWeight;
	public float rightWeight;
	public float rightMiddleWeight;
	public float rightMostWeight;
	public float allHandsStiffness;

	private StiffnessPreventFall[] hands; 

	// Use this for initialization
	void Start () {
		hands = GameObject.Find ("WeightObjects").GetComponentsInChildren<StiffnessPreventFall> ();
	}
	
	// Update is called once per frame
	void Update () {
		foreach (var hand in hands) {
			hand.stiffness = allHandsStiffness;
		}
	}
}
