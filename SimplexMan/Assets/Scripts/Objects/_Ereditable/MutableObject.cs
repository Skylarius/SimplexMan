using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MutableObject : Recordable {
    public abstract bool ChangeState(bool state);
}
