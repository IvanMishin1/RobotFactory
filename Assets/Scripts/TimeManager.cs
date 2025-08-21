using UnityEngine;

public class TimeManager : MonoBehaviour
{ public int Day = 0;
    public int Hour;
    
    public float decimalHour = 0f; 
    
    void Update()
    {
        decimalHour += (1f/30f) * Time.deltaTime;
    
        if (decimalHour >= 24f)
        {
            decimalHour -= 24f;
            Day++;
        }
        Hour = Mathf.FloorToInt(decimalHour);
    }
}
