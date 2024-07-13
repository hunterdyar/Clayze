#if ENABLE_INPUT_SYSTEM
using Clayze.Ink;
using UnityEngine;
using UnityEngine.InputSystem;

public class PenInput : MonoBehaviour
{
    public Color PenColor = Color.black;
    public float PenThickness = 0.05f;
    [SerializeField] private InkManager _manager;
    
    private InkCanvas _currentCanvas;
    private Stroke _currentStroke;
    
    //settings
    private float minRadius;
    private float minTime = (1f/30f);
    private float lastAddTime = 4;
    private byte _id;
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
        lastAddTime += Time.deltaTime;
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

            _currentStroke = _currentCanvas.StartStroke(_id,true,PenColor,PenThickness);
        }
        
        //2/3 drag. (also first frame)
        
        //if min movement and min time.
       
        if (pen.tip.isPressed && _currentStroke != null)
        {
            if (lastAddTime >= minTime)
            {
                _currentStroke.AddPoint(pen.position.x.value, pen.position.y.value, pen.pressure.value);
                lastAddTime = 0;
            }
            else
            {
                _currentStroke.UpdateLastPoint(pen.position.x.value, pen.position.y.value, pen.pressure.value);
            }
        }
        

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