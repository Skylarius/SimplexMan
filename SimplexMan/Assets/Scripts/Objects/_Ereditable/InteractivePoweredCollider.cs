using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractivePoweredCollider : InteractiveCollider, IPower {
    
    protected List<Renderer> electricity = new List<Renderer>();
    public bool hasPower = true;

    protected override void Start() {
        base.Start();
        SetPower(hasPower);
    }

    public virtual void SetPower(bool _hasPower) {
        hasPower = _hasPower;
        foreach (Renderer r in electricity) {
            if (hasPower) {
                r.material.EnableKeyword("_EMISSION");
            } else {
                r.material.DisableKeyword("_EMISSION");
            }
        }
    }
}
