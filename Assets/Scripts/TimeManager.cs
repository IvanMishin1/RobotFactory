using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public float timeScale = 1f;
    public int Day = 0;
    public int Hour;
    
    public float decimalHour = 0f; 
    
    void Update()
    {
        decimalHour += (1f/30f) * Time.deltaTime * timeScale;
    
        if (decimalHour >= 24f)
        {
            decimalHour -= 24f;
            Day++;
        }
        Hour = Mathf.FloorToInt(decimalHour);
    }
}
