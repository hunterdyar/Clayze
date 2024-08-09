#if ENABLE_INPUT_SYSTEM
using Clayze.Ink;
using SyncedProperty;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PenInput3D : MonoBehaviour
{
    private InkCanvas _currentCanvas;
    private Stroke3 _currentStroke;
    private byte _id;

    [Header("Pen Style Configuration")] public bool UseSyncColor = true;
    public Color PenColor = Color.black;
    public SyncColor PenSyncColor;
    public Color GetPenColor() => UseSyncColor ? PenSyncColor.Value : PenColor;
    
    public float PenThickness = 0.05f;

    [Tooltip("0, pen will always be 'pen thickness' width. Set to 1, this then the pressure controls the width as a percentage.")]
    [Range(0, 1)] public float PressureControlPercentage = 0;

    [Tooltip("The pressure value from the pen is evalated through this function.")]
    public AnimationCurve PressureCurve = AnimationCurve.Linear(0, 0, 1, 1);
    //settings
    [Header("Pen Settings")]
    [Tooltip("Distance pen must move in world space beffore a new point is added.")]
    [SerializeField] private float minRadius = 0.1f;
    [Tooltip("Minimum time after previous point before a new point is added.")]
    [SerializeField] private float _minTime = (1f/50f);
    private float _lastAddTime = Mathf.Infinity;
    
    [FormerlySerializedAs("_manager")]
    [Header("References")]
    [SerializeField] private InkManager3D manager;

    public bool UsePenTablet = true;
    private void Start()
    {
       _id = manager.GetUniquePenID();
    }

    byte GetCurrentPressure()
    {
        if (PressureControlPercentage == 0)
        {
            return 255;
        }

        var input = PressureCurve.Evaluate(Pen.current.pressure.value);
        var p = 1 - PressureControlPercentage + PressureControlPercentage * (input);
        return Stroke2.WidthByteFromFloat(p);
    }
    // Update is called once per frame
    void Update()
    {
        _lastAddTime += Time.deltaTime;
        if (!UsePenTablet)
        {
            return;
        }
        if (Pen.current == null)
        {
            return;
        }
        var pen = Pen.current;

        if (!pen.wasUpdatedThisFrame)
        {
            return;
        }

        //1/3 press
        if (pen.tip.wasPressedThisFrame)
        {
            if (_currentStroke != null)
            {
                //end
                _currentStroke.Finish();
                _currentStroke = null;
            }

            _currentStroke = manager.StartStroke(_id, true, GetPenColor(), PenThickness);
        }
        
        //2/3 drag. (also first frame)
        
        //if min movement and min time.
       
        if (pen.tip.isPressed && _currentStroke != null)
        {
            if (_lastAddTime >= _minTime)
            {
                float d = 0;
                if (_currentStroke.Points.Count > 1)
                {
                    d = InkPoint3.Distance(_currentStroke.Points[^1], PenToWorld());
                    if (d < minRadius || d == 0)
                    {
                        //don't add new point if we haven't moved enough.
                        goto PenRelease;
                    }
                }

                _currentStroke.AddPoint(NewPointAtCurrent());
                _lastAddTime = 0;
            }
            else
            {
                _currentStroke.UpdateLastPoint(NewPointAtCurrent());
            }
        }

        PenRelease:

        //3/3 release
        if (pen.tip.wasReleasedThisFrame)
        {
            //finish!
            if (_currentStroke != null)
            {
                _currentStroke.Finish();
                _currentStroke = null;
            }
        }
    }

    private InkPoint3 NewPointAtCurrent()
    {
        var p = PenToWorld();
        return new InkPoint3(p, GetCurrentPressure());
    }

    private Vector3 PenToWorld()
    {
       return Camera.main.ScreenToWorldPoint(new Vector3(Pen.current.position.x.value, Pen.current.position.y.value, 5));
    }
}

#endif