using UnityEngine;

public class GameContext : MonoBehaviour
{
    public string gameName;
    public bool isNewGame;
    public bool isTutorial;
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
