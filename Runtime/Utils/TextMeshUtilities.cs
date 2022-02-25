namespace Utils
{
    using TMPro;
    using UnityEngine;

    public static class TextMeshUtilities
    {
        public static Rect GetWordRect(this TMP_Text textComponent, int wordIndex)
        {
            var wordInfo = textComponent.textInfo.wordInfo[wordIndex];
            
            if(wordIndex < 0 || wordIndex > textComponent.textInfo.wordInfo.Length - 1) return default;
            
            float maxAscender = -Mathf.Infinity;
            float minDescender = Mathf.Infinity;
            
            float maxX = -Mathf.Infinity;
            float minX = Mathf.Infinity;
            
            Vector3 bottomLeft = Vector3.zero;

            // Iterate through each character of the word
            for(int i = 0; i < wordInfo.characterCount; i++)
            {
                int characterIndex = wordInfo.firstCharacterIndex + i;
                var currentCharInfo = textComponent.textInfo.characterInfo[characterIndex];

                // Track Max Ascender and Min Descender
                maxAscender = Mathf.Max(maxAscender, currentCharInfo.ascender);
                minDescender = Mathf.Min(minDescender, currentCharInfo.descender);
                
                maxX = Mathf.Max(maxX, currentCharInfo.bottomRight.x);
                minX = Mathf.Min(minX, currentCharInfo.bottomLeft.x);

                if(i == 0)
                {
                    bottomLeft = new Vector2(currentCharInfo.bottomLeft.x, currentCharInfo.descender);

                    // If Word is one character
                    if(wordInfo.characterCount == 1)
                    {
                        bottomLeft = textComponent.transform.TransformPoint(new Vector3(bottomLeft.x, minDescender));
                        return new Rect(bottomLeft.x, bottomLeft.y, maxX - minX, maxAscender - minDescender);
                    }
                }

                // Last Character of Word
                if(i != wordInfo.characterCount - 1) continue;
                
                bottomLeft = textComponent.transform.TransformPoint(new Vector3(bottomLeft.x, minDescender));
                return new Rect(bottomLeft.x, bottomLeft.y, maxX - minX, maxAscender - minDescender);
            }

            return default;
        }
    }
}
