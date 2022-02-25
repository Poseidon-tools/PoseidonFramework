using UnityEngine;
using UnityEngine.UI;

namespace Utils
{
    [RequireComponent(typeof(AspectRatioFitter), typeof(Image))]
    public class AspectRatioFitterSetter : MonoBehaviour
    {
        private void Start()
        {
            Texture mainTexture = GetComponent<Image>().mainTexture;
            GetComponent<AspectRatioFitter>().aspectRatio = (float)mainTexture.width / (float)mainTexture.height;
            Destroy(this);
        }
    }
}