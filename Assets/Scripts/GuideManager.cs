
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class GuideManager : MonoBehaviour
{
    public TMP_Text guideText;
    public GameObject guidePanel;
    private string currentPage = "coding";
    private GameManager gameManager;
    
    public void Start()
    {
        guidePanel.SetActive(false);
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    
    public void CloseGuide()
    {
        guidePanel.SetActive(false);
    }
    public void OpenGuidePage(string page = null)
    {
        gameManager.onGuideOpened.Invoke();
        if (string.IsNullOrEmpty(page) && !string.IsNullOrEmpty(currentPage))
            page = currentPage;
        
        guidePanel.SetActive(true);
        
        TextAsset textAsset = Resources.Load<TextAsset>("Guides/" + page);
        if (textAsset != null)
        {
            guideText.text = textAsset.text;
        }
        else
        {
            Debug.LogError("Guide page not found: " + page);
        }
    }
}
