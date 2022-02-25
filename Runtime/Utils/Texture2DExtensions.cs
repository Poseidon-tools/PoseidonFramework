using UnityEngine;

namespace Utils
{
    public static class Texture2DExtensions
    {
        public static Sprite ToSprite(this Texture2D texture2D)
        {
            return Sprite.Create(texture2D, new Rect(0.0f, 0.0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f), 100.0f);
        }
    }
}
