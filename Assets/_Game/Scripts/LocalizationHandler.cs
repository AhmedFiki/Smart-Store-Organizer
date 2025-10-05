using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class LocalizationHandler : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown languageDropdown;

    private void Awake()
    {
        var locales = LocalizationSettings.AvailableLocales.Locales;
        int currentIndex = locales.IndexOf(LocalizationSettings.SelectedLocale);

        if (languageDropdown != null)
        {
            languageDropdown.value = currentIndex;
            languageDropdown.RefreshShownValue();
        }
    }
    private void OnEnable()
    {
        if (languageDropdown != null)
            languageDropdown.onValueChanged.AddListener(ChangeLocale);
    }
    private void OnDisable()
    {
        if (languageDropdown != null)
            languageDropdown.onValueChanged.RemoveListener(ChangeLocale);
    }
    public void ChangeLocale(int localeID)
    {
        StartCoroutine(SetLocale(localeID));
    }
    private IEnumerator SetLocale(int localeID)
    {
        yield return LocalizationSettings.InitializationOperation;

        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[localeID];

    }
}
