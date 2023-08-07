using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explorer : Card
{
    private void Awake()
    {
        this.explorer = true;
    }

    public virtual IEnumerator PlayThis(Player player)
    {
        yield return null;
    }
}