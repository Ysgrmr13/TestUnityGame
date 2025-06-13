using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public static VirtualJoystick Instance { get; private set; }
    
    [Header("Joystick Settings")]
    public RectTransform joystickBackground;
    public RectTransform joystickHandle;
    public float handleRange = 1f;
    
    private Vector2 inputVector = Vector2.zero;
    private Vector2 joystickCenter;
    private Camera uiCamera;
    
    private void Awake()
    {
        Instance = this;
    }
    
    private void Start()
    {
        joystickCenter = joystickBackground.position;
        uiCamera = GetComponentInParent<Canvas>().worldCamera;
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 position;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            joystickBackground, eventData.position, uiCamera, out position))
        {
            position.x = (position.x / joystickBackground.sizeDelta.x);
            position.y = (position.y / joystickBackground.sizeDelta.y);
            
            inputVector = new Vector2(position.x * 2, position.y * 2);
            inputVector = (inputVector.magnitude > 1.0f) ? inputVector.normalized : inputVector;
            
            joystickHandle.anchoredPosition = 
                new Vector2(inputVector.x * (joystickBackground.sizeDelta.x / 2), 
                           inputVector.y * (joystickBackground.sizeDelta.y / 2));
        }
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        inputVector = Vector2.zero;
        joystickHandle.anchoredPosition = Vector2.zero;
    }
    
    public Vector2 GetInputVector()
    {
        return inputVector;
    }
}