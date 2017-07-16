using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicAttack : MonoBehaviour {
    
    Animator anim;

    public void Init(Vector3 position) {
        anim = GetComponent<Animator>();
        transform.position = position;
    }

    void Update() {
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !anim.IsInTransition(0))
        {
            Destroy(gameObject);
        }
    }
}
