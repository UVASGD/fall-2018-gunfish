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

    public void ChangeNameColor () {
        StartCoroutine(ColorChange());
    }

    private IEnumerator ColorChange () {
        text.color = Color.yellow;

        yield return new WaitForSeconds(2f);

        float t = 0f;
        while (t < 1) {
            print("Color: " + text.color);
            t += Time.deltaTime / 2f;
            text.color = Color.Lerp(Color.yellow, Color.white, t);
            yield return new WaitForEndOfFrame();
        }
        text.color = Color.white;
    }
}
