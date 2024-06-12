using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvent : MonoBehaviour
{
    private Player Player;

    // Start is called before the first frame update
    void Start()
    {
        Player = GetComponentInParent<Player>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void AnimationTrigger()
    {
        Player.AttackOver();
    }

}
