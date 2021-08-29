using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeAnimator : Punchable
{
    public Transform SphereBlob;
    private float GlobWobbleIntensity;

    public override void Start()
    {
        base.Start();

        StartCoroutine( Fucker() );
    }

    public override void Update()
    {
        if (GlobWobbleIntensity > 0.0f)
        {
            GlobWobbleIntensity -= Time.deltaTime;
        
            var globWobbler = 2.0f + ((Mathf.Sin(Time.timeSinceLevelLoad * 18.0f) * 0.5f) * GlobWobbleIntensity);
            var globWobblerB = 2.0f - ((Mathf.Sin(Time.timeSinceLevelLoad * 18.0f) * 0.5f) * GlobWobbleIntensity);

            var globScale = SphereBlob.localScale;
            globScale.x = globWobbler;
            globScale.y = globWobblerB;
            globScale.z = globWobbler;
            SphereBlob.localScale = globScale;
        }
    }
    
    public void GlobWobble()
    {
        GlobWobbleIntensity = Random.Range( 0.7f, 1.1f );
    }

    IEnumerator Fucker()
    {
        yield return new WaitForSeconds( Random.Range( 0.0f, 5.5f ) );

        while ( true )
        {
            yield return new WaitForSeconds( Random.Range( 1.0f, 1.2f ) );

            Punch();
            GlobWobble();
        }
    }
}
