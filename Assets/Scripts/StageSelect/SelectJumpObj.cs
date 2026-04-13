using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectJumpObj : MonoBehaviour,IPool
{
    [SerializeField] private Image _frame;
    [SerializeField] private TextMeshProUGUI _stageNumText;

    public bool IsGenericUse { get; set; }

    public void SetColor(Color color)
    {
        _frame.color = color;
        _stageNumText.color = color;
    }

    public void SetStageID(int stageID) 
    {
        _stageNumText.text = $"{stageID + 1:000}";
    }

    public void OnRelease()
    {
        _stageNumText.text = string.Empty;
        gameObject.SetActive(false);
    }

    public void OnReuse()
    {
        gameObject.SetActive(true);
    }
}
