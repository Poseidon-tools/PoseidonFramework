using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Utils
{
    public class ButtonView : MonoBehaviour
    {
        [field: SerializeField] public Button Button { get; private set; }
        [field: SerializeField] public Image Background { get; private set; }
        [field: SerializeField] public Image Shadow { get; private set; }
        [field: SerializeField] public TMP_Text Text { get; private set; }
        [field: SerializeField] public Image Image { get; private set; }
    }
}
