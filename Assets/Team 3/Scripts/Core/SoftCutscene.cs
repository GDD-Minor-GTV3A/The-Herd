using UnityEngine;
using TMPro;
using System.Collections;

public class SoftCutscene : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textDisplay;

    [Header("Settings governing text")]
    [SerializeField] [ReadOnly] private string[] dialogue;
    [SerializeField] [ReadOnly] private int readTime;
    
    private int currentLine = 0;
    private int maxLines;
    

    public void Initialize(string[] text, int time)
    {
        dialogue = text;
        readTime = time;
        maxLines = text.Length;
        StartCoroutine(ReadText());
    }


    IEnumerator ReadText()
    {
        textDisplay.text = dialogue[currentLine];
        currentLine++;
        yield return new WaitForSeconds(readTime);
        if (currentLine >= maxLines)
            Destroy(this.gameObject);
        StartCoroutine(ReadText());
    }

    private void LateUpdate()
    {
        this.transform.rotation = Quaternion.identity;
    }






}
