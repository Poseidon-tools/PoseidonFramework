namespace Inseminator.Scripts.Example
{
    using TMPro;
    using UnityEngine;

    public interface ITextLogger
    {
        void LogMessage(string message, TMP_Text textContainer);
    }
    public class TestLogger : ITextLogger
    {
        #region Public API
        public void LogMessage(string message, TMP_Text textContainer)
        {
            textContainer.text = message;
        }
        #endregion
    }

    public class GreenTextLogger : ITextLogger
    {
        #region Public API
        public void LogMessage(string message, TMP_Text textContainer)
        {
            textContainer.color = Color.green;
            textContainer.text = message;
        }
        #endregion
    }

    public class CustomLogger : ITextLogger
    {
        private Color color;
        private int fontSize;
        public CustomLogger(Color color, int fontSize)
        {
            this.color = color;
            this.fontSize = fontSize;
        }
        public void LogMessage(string message, TMP_Text textContainer)
        {
            textContainer.color = color;
            textContainer.fontSize = fontSize;
            textContainer.text = message;
        }
    }
}