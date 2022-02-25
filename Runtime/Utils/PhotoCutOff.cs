namespace Utils
{
    using UnityEngine;

    public static class PhotoCutOff
    {
        public static Texture2D GetTextureCutOff(Texture2D source, Vector2 cutCoords, Vector2 cutSize)
        {
            var pixels = source.GetPixels(Mathf.FloorToInt(cutCoords.x), Mathf.FloorToInt(cutCoords.y),
                Mathf.FloorToInt(cutSize.x),Mathf.FloorToInt(cutSize.y));

            var tex = new Texture2D(Mathf.FloorToInt(cutSize.x),Mathf.FloorToInt(cutSize.y));
            tex.SetPixels(pixels);
            tex.Apply(true,false);
            return tex;
        }
    }
}