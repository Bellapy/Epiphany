using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

// Não precisa mais ser um Singleton
public class ItemAcquiredDisplay : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemDescriptionText;

    // O método Awake() não é mais necessário para o Singleton.

    public void ShowItem(ItemData item)
    {
        if (item == null) return;

        itemIcon.sprite = item.icon;
        itemNameText.text = item.itemName;
        itemDescriptionText.text = item.description;
        
        // Ativa o objeto pai para garantir que a corrotina possa rodar.
        gameObject.SetActive(true); 
        StartCoroutine(ShowItemRoutine());
    }

    private IEnumerator ShowItemRoutine()
    {
        // Garante que o estado inicial seja invisível
        canvasGroup.alpha = 0;

        // Fade In
        float timer = 0f;
        while (timer < 0.5f)
        {
            timer += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, timer / 0.5f);
            yield return null;
        }
        canvasGroup.alpha = 1;

        // Wait for 3 seconds
        yield return new WaitForSecondsRealtime(3.0f);

        // Fade Out
        timer = 0f;
        while (timer < 0.5f)
        {
            timer += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(1, 0, timer / 0.5f);
            yield return null;
        }
        canvasGroup.alpha = 0;

        // Desativa o objeto ao final para limpar a tela.
        gameObject.SetActive(false);
    }
}