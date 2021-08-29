using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeAnimator : Punchable
{
    public override void Start()
    {
        base.Start();

        StartCoroutine( Fucker() );
    }

    IEnumerator Fucker()
    {
        yield return new WaitForSeconds( Random.Range( 0.0f, 5.5f ) );

        while ( true )
        {
            yield return new WaitForSeconds( 1 );

            Punch();
        }
    }
}
