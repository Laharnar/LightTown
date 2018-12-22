using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stun : AbilityDecorator {
    public float stunLength = 0;
    public Stun(Stun stun, IDecorator action) : base(action) {
        stunLength = stun.stunLength;
    }

}

