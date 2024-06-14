using System.Collections;
using System.Collections.Generic;
using Clayze;
using UnityEngine;

public class MoveToVolume : MonoBehaviour
{
    public enum MoveToVolumeLocation
    {
        Center,
        BottomMiddle,
        TopMiddle
    }

    [SerializeField] private MoveToVolumeLocation _location;
    [SerializeField]
    private Volume _volume;
    // Start is called before the first frame update
    void Start()
    {
        float scale = _volume.Size / _volume.pointsPerUnit;
        transform.localScale = new Vector3(scale, scale, scale);
        var min = _volume.transform.position;
        var max = _volume.transform.position + new Vector3(scale, scale, scale);
        if (_location == MoveToVolumeLocation.Center)
        {
            transform.position = Vector3.Lerp(min, max, .5f);
        }else if (_location == MoveToVolumeLocation.BottomMiddle)
        {
            transform.position = Vector3.Lerp(min, max, .5f) + Vector3.down*scale/2;
        }
        else if (_location == MoveToVolumeLocation.TopMiddle)
        {
            transform.position = Vector3.Lerp(min, max, .5f) + Vector3.up * scale / 2;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
