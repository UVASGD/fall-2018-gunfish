using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class NamePlate : MonoBehaviour {

    public TextMesh text;

    public PositionConstraint pc;

    public void SetName(string gameName) {
        text.text = gameName;
    }

    public void SetOwner(GameObject owner) {
        ConstraintSource cs = new ConstraintSource();
        cs.sourceTransform = owner.transform;
        cs.weight = 1;
        pc.SetSource(0, cs);
        //pc.translationOffset = new Vector3(0, 1, -1);
        pc.locked = true;
        //pc.translationOffset = Vector3.back;
    }
}
