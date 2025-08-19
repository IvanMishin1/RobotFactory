using UnityEngine;

public class GameContext : MonoBehaviour
{
    public string gameName;
    public bool isNewGame;
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
