using UnityEngine;
using Clayze;
using Clayze.Marching.Operations;
using TMPro;
using UnityEditor;
using UnityEngine.Experimental.Rendering;
using UnityEngine.VFX;

public class VolumeVFXGraphSDFProvider : MonoBehaviour
{
    private Volume _volume;

    private VisualEffect _visualEffect;
    //Start is called before the first frame update
    public Texture3D _sdf;
    public string SFDPropertyName = "SDF";

    private float _textureRange = 32f;
    public float multiplier = 1;
    public float offset = 0;

    [Header("Debug")] public bool DrawVolumeGizmo;
    public Gradient GizmoGradient;
    void Awake()
    {
        _volume = GetComponent<Volume>();
        _visualEffect = GetComponent<VisualEffect>();
        InitSDF();
    }

    private void InitSDF()
    {
        _sdf = new Texture3D(_volume.Size, _volume.Size, _volume.Size, TextureFormat.RHalf,false);
        _textureRange = _volume.Size;
        _sdf.Apply();
    }

    private void OnEnable()
    {
        _volume.OnChange+= OnChange;
    }

    private void OnDisable()
    {
        _volume.OnChange -= OnChange;
    }

    private void OnChange(Vector3Int min, Vector3Int max)
    {
        Debug.Log("Updating Texture");
        for (int x = min.x; x < max.x; x++)
        {
            for (int y = min.y; y < max.y; y++)
            {
                for (int z= min.z; z < max.z; z++)
                {
                    float p = _volume.SamplePoint(x, y, z) / _textureRange;
                    _sdf.SetPixel(x,y,z,new Color(multiplier*p+offset,0,0));   
                }
            }
        }
        _sdf.Apply();
        //if we did getTexture and updated it directly, would that be faster?
        _visualEffect.SetTexture(SFDPropertyName,_sdf);
        _visualEffect.Reinit();
    }

    private void Bake()
    {
        
    }

    private void OnDrawGizmos()
    {
        if (DrawVolumeGizmo)
        {
            Handles.DoPositionHandle(transform.position, transform.rotation);
            Handles.DrawTexture3DSDF(_sdf,1,4, GizmoGradient);
        }
    }
}
