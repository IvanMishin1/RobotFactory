
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class GuideManager : MonoBehaviour
{
    public TMP_Text guideText;
    public GameObject guidePanel;
    private string currentPage = "basics";
    
    public void Start()
    {
        guidePanel.SetActive(false);
    }
    
    public void CloseGuide()
    {
        guidePanel.SetActive(false);
    }
    public void OpenGuidePage(string page = null)
    {
        if (string.IsNullOrEmpty(page) && !string.IsNullOrEmpty(currentPage))
            page = currentPage;
        
        guidePanel.SetActive(true);
        
        string path = Application.dataPath + "/Guides/" + page + ".txt";
        if (File.Exists(path))
        {
            guideText.text = File.ReadAllText(path);
        }
        else
        {
            Debug.LogError("Guide page not found: " + path);
        }
    }
}
