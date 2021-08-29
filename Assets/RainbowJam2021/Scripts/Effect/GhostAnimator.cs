using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostAnimator : MonoBehaviour
{
    public float Offset = 0.3f;
    public float Divider = 5;

    List<Material> Materials = new List<Material>();

    void Start()
    {
		foreach ( var mesh in GetComponentsInChildren<MeshRenderer>() )
		{
            Materials.Add( mesh.material );
        }
    }

    void Update()
    {
		foreach ( var mat in Materials )
		{
            mat.SetFloat( "_Cutoff", Offset + Mathf.Abs( Mathf.Sin( Time.time + transform.GetSiblingIndex() ) ) / Divider );
            mat.SetTextureOffset( "_MainTex", new Vector2( 1, 1 ) * ( Time.time + transform.GetSiblingIndex() ) );
        }
    }
}
