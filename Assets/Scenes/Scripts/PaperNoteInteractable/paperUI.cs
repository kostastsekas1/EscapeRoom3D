using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class paperUI : MonoBehaviour
{
    [Header("Paper UI")]
    [SerializeField] public Image PaperUI;
    [SerializeField] public Image Paper;
    [SerializeField ] public TextMeshProUGUI readableText;
    
    private bool IsTextShown = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {

            if (IsTextShown)
            {

                Hide();
            }
            else
            {

                Show();
            }
        }
    }

   private void Show()
    {
        IsTextShown = true;
        readableText.gameObject.SetActive(true);
        PaperUI.color = new Color(0,0,0, 0.8f);
        Paper.color = new Color(255, 255,255,0.3f);
    }

    private void Hide()
    {
        IsTextShown = false;
        readableText.gameObject.SetActive(false);
        PaperUI.color = new Color(0, 0, 0, 0);
        Paper.color = new Color(255, 255, 255, 1);
    }

}
