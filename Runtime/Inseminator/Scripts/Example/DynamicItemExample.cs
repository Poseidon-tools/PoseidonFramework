namespace Inseminator.Scripts.Example
{
    using TMPro;
    using UnityEngine;

    public class DynamicItemExample : MonoBehaviour
    {
        #region Inspector
        [SerializeField] private TMP_Text textRenderer;
        #endregion
        #region Private Variables
        [InseminatorAttributes.Inseminate] private ITextLogger textLogger;
        #endregion
        #region Unity Methods
        private void OnEnable()
        {
            textLogger.LogMessage($"{name}", textRenderer);
        }
        #endregion
        #region Public API
        public void OverrideText(string text)
        {
            textLogger.LogMessage(text, textRenderer);
        }
        #endregion
    }
}