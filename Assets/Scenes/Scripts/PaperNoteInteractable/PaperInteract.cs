using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class PaperInteract : InteractableObject
{
    [Header("Paper UI")]
    [SerializeField] public GameObject PaperUI;



    private Vector3 PaperPosition;
    private Quaternion PaperRotation;
    private Vector3 PaperScale;
    private static bool IsFocused = false;
    public override void Awake()
    {
        gameObject.layer = 6;
    }

    public override void OnInteract(Transform pickUpSlot)
    {
        Time.timeScale = 0f;
        PaperUI.SetActive(true);

    }

    public override void OnFocus()
    {
        return;
    }

    public override void OnLoseFocus()
    {
        return;
    }

    public override void OnDrop()
    {
        PaperUI.SetActive(false);
        Time.timeScale = 1f;
    }
}
