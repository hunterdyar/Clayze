#if ENABLE_INPUT_SYSTEM
using Clayze.Ink;
using UnityEngine;
using UnityEngine.InputSystem;

public class PenInput : MonoBehaviour
{
    private InkCanvas _currentCanvas;
    private Stroke _currentStroke;
    private byte _id;

    private InkPoint NewPointAtCurrent => new InkPoint(Pen.current.position.x.value, Pen.current.position.y.value, Stroke.WidthByteFromFloat(Pen.current.pressure.value));
    [Header("Pen Style Configuration")]
    public Color PenColor = Color.black;
    public float PenThickness = 0.05f;

    [Range(0, 1)] public float PressureControlPercentage = 0;
    //settings
    [Header("Pen Settings")]
    [Tooltip("Distance pen must move in pen (pixel/screen/canvas) space beffore a new point is added.")]
    [SerializeField] private float minRadius = 2;
    [Tooltip("Minimum time after previous point before a new point is added.")]
    [SerializeField] private float _minTime = (1f/50f);
    private float _lastAddTime = Mathf.Infinity;
    
    [Header("References")]
    [SerializeField] private InkManager _manager;

    private void Start()
    {
        //todo: wait until connected
        if (!_manager.TryGetCanvas(0, out _currentCanvas))
        {
            _currentCanvas = _manager.CreateCanvas(0, Screen.width, Screen.height, 0);
        }
        
       _id = _manager.GetUniquePenID();
    }

    // Update is called once per frame
    void Update()
    {
        _lastAddTime += Time.deltaTime;
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

            _currentStroke = _currentCanvas.StartStroke(_id, true, PenColor, PenThickness,PressureControlPercentage);
        }
        
        //2/3 drag. (also first frame)
        
        //if min movement and min time.
       
        if (pen.tip.isPressed && _currentStroke != null)
        {
            if (_lastAddTime >= _minTime)
            {
                if (_currentStroke.Points.Count > 1)
                {
                    if (InkPoint.Distance(_currentStroke.Points[^1], pen.position.value) < minRadius)
                    {
                        goto PenRelease;
                    }
                }
                
                _currentStroke.AddPoint(NewPointAtCurrent);
                _lastAddTime = 0;
            }
            else
            {
                _currentStroke.UpdateLastPoint(NewPointAtCurrent);
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
}

#endif