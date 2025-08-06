using System;
using UnityEngine;
using UnityEngine.Serialization;

public class TweenAnimations : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RectTransform[] buttons;
    [SerializeField] private RectTransform panelHome;
    [SerializeField] private RectTransform panelOptions;
    [SerializeField] private GameObject paneloptions;
    
 
    [Header("Animation Shits")]
    [SerializeField] private float delay = 0.1f;
    [SerializeField] private float popAnimationTime = 0.5f;
    [SerializeField] private float scaleDuration = 0.4f;

    [Header("Camera Movement SHits")]
    [SerializeField] private Transform cameraToMove;
    [SerializeField] private Vector3 cameraOgPosition;
    [SerializeField] private Vector3 cameraTargetPosition;
    [SerializeField] private float cameraMoveDuration = 1.2f;
    [SerializeField] private LeanTweenType cameraEaseType = LeanTweenType.easeInOutQuad;
    
    [Header("Ping-Pong Text Box Shits")]
    [SerializeField] private RectTransform pingPongTextBox;
    [SerializeField] private float pingPongScaleAmount = 2.05f;
    [SerializeField] private float pingPongDuration = 0.6f;

    private Vector3 originalPanelScale;
    private Vector3 OptionsPanelScale;
    
// using tween addon to animate
    private void Awake()
    {
        paneloptions.SetActive(false);
    }

    private void Start()
    {
       SFXManager.instance.PlayRequestedSound("HomeScreenMusic", isLoop: true);// home screen music
      originalPanelScale = panelHome.localScale;
      OptionsPanelScale = panelOptions.localScale;
      StartPingPongAnimation();
    }
    
    private void StartPingPongAnimation()
    {
        if (pingPongTextBox != null)
        {
            LeanTween.scale(pingPongTextBox, Vector3.one * pingPongScaleAmount, pingPongDuration)
                .setEaseInOutSine()
                .setLoopPingPong();
        }
    }
    public void ShowOptions()
    {
        paneloptions.SetActive(true);
        panelOptions.localScale = Vector3.zero;

        LeanTween.scale( panelOptions, originalPanelScale, scaleDuration)
            .setEaseOutBack();
    }
    public void HideOptions()
    {
        LeanTween.scale( panelOptions, Vector3.zero, scaleDuration)
            .setEaseInBack();
    }

    private void AnimateButtons()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            RectTransform b = buttons[i];
            b.localScale = Vector3.zero;

            LeanTween.scale(b, Vector3.one * 2f, popAnimationTime)
                .setEaseOutBack()
                .setDelay(i * delay);
        }
    }
    public void ShowPanel()
    {
        panelHome.localScale = Vector3.zero;

        LeanTween.scale(panelHome, originalPanelScale, scaleDuration)
            .setEaseOutBack();
        cameraToMove.position = cameraOgPosition;

    }
    public void HidePanel()
    {
        Debug.Log("Hide Panel");
        LeanTween.scale(panelHome, Vector3.zero, scaleDuration)
            .setEaseInBack() .setOnComplete(() =>
            {
                MoveCameraToTarget();
                AnimateButtons(); // Wait for panel, then pop in buttons
            });
        
    }

    private void MoveCameraToTarget()
    {
        if (cameraToMove != null)
        {
            LeanTween.move(cameraToMove.gameObject, cameraTargetPosition, cameraMoveDuration)
                .setEase(cameraEaseType);
        }
    }
}
