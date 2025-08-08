using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniMapController : MonoBehaviour
{
    public static MiniMapController Instance;
    private RectTransform minimap;
    [SerializeField] private RawImage minimapUI;
    [SerializeField] private Color normalColor;
    [SerializeField] private Color expandColor;
    private Vector2 normalScale = Vector2.one;
    private Vector2 expandScale = new(2, 2);
    private Vector2 targetScale;
    private KeyCode expandKey = KeyCode.Tab;
    [SerializeField] private float expandSpeed = 10;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        minimap = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (Input.GetKey(expandKey))
        {
            targetScale = expandScale;
            minimapUI.color = expandColor;
            MiniMapCam.Instance.ExpandZoom();
        }
        else
        {
            targetScale = normalScale;
            minimapUI.color = normalColor;
            MiniMapCam.Instance.DefaultZoom();
        }
        minimap.localScale = Vector2.Lerp(minimap.localScale, targetScale, expandSpeed * Time.deltaTime);
    }
}
