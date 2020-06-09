using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * this class is used to make objects with complex colliders, e.g. a player with colliders on his arms and legs as well as a torso.
 * Without this class, it is impossible to grab such objects either by tractor or by hand.
 * Since the code for grabbing is in Wand.cs and Tractor.cs, only a reference to the parent is needed.
 * When this collider is hit by a tractor beam or grabbed by a hand, the variable public GameObject parent; is grabbed/tractored instead.
 */
public class GrabbableCollider : MonoBehaviour {
	public GameObject parent;
}
