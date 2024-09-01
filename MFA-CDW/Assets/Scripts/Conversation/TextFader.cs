using UnityEngine;
using System.Collections;

//from: https://forum.unity.com/threads/teletype-interfering-with-vertex-jitter.611245/#post-4092415
namespace TMPro.Examples
{
    public class TextFader : MonoBehaviour
    {
        public static TextFader _instance;

        [SerializeField]
        private float FadeSpeed = 1.0F;
        [SerializeField]
        private int RolloverCharacterSpread = 10;
        [SerializeField]
        private Color ColorTint;

        public delegate void FinishedFadeEventDelegate(
            TextMeshProUGUI TMProbject, bool fadingIn);

        public FinishedFadeEventDelegate FadeInFinished;
        public FinishedFadeEventDelegate FadeOutFinished;
        void Awake()
        {
            if(_instance == null)
            {
                _instance = this;
            }
            else
            {
                Destroy(this);
            }
        }
        void Start()
        {
            //FindObjectOfType<FadeableText>().QueueText("This is some test text to see if this worked");
        }

        public void FadeText(TextMeshProUGUI TMProbject, bool fadeIn = false)
        {
            if(fadeIn)
            {
                StartCoroutine("FadeTextIn", TMProbject);
            }
            else
            {
                StartCoroutine("FadeTextOut", TMProbject);
            }
        }

        /// <summary>
        /// Method to animate vertex colors of a TMP Text object.
        /// </summary>
        /// <returns></returns>
        IEnumerator FadeTextIn(TextMeshProUGUI TMProbject)
        {
            TMP_Text currTextComponent = TMProbject.GetComponent<TMP_Text>();
            // Need to force the text object to be generated so we have valid data to work with right from the start.
            currTextComponent.ForceMeshUpdate();
            TMP_TextInfo textInfo = currTextComponent.textInfo;
            Color32[] newVertexColors;
            int currentCharacter = 0;
            int startingCharacterRange = currentCharacter;
            bool isRangeMax = false;
            while (!isRangeMax)
            {
                int characterCount = textInfo.characterCount;
                // Spread should not exceed the number of characters.
                byte fadeSteps = (byte)Mathf.Max(1, 255 / RolloverCharacterSpread);
                for (int i = startingCharacterRange; i < currentCharacter + 1; i++)
                {
                    // Skip characters that are not visible
                    if (!textInfo.characterInfo[i].isVisible) continue;
                    // Get the index of the material used by the current character.
                    int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
                    // Get the vertex colors of the mesh used by this text element (character or sprite).
                    newVertexColors = textInfo.meshInfo[materialIndex].colors32;
                    // Get the index of the first vertex used by this text element.
                    int vertexIndex = textInfo.characterInfo[i].vertexIndex;
                    // Get the current character's alpha value.
                    byte alpha = (byte)Mathf.Clamp(newVertexColors[vertexIndex + 0].a + fadeSteps, 0, 255);
                    // Set new alpha values.
                    newVertexColors[vertexIndex + 0].a = alpha;
                    newVertexColors[vertexIndex + 1].a = alpha;
                    newVertexColors[vertexIndex + 2].a = alpha;
                    newVertexColors[vertexIndex + 3].a = alpha;
                    // Tint vertex colors
                    // Note: Vertex colors are Color32 so we need to cast to Color to multiply with tint which is Color.
                    /*newVertexColors[vertexIndex + 0] = (Color)newVertexColors[vertexIndex + 0] * ColorTint;
                    newVertexColors[vertexIndex + 1] = (Color)newVertexColors[vertexIndex + 1] * ColorTint;
                    newVertexColors[vertexIndex + 2] = (Color)newVertexColors[vertexIndex + 2] * ColorTint;
                    newVertexColors[vertexIndex + 3] = (Color)newVertexColors[vertexIndex + 3] * ColorTint;*/
                    if (alpha == 255)
                    {
                        startingCharacterRange += 1;
                        if (startingCharacterRange == characterCount)
                        {
                            // Update mesh vertex data one last time.
                            currTextComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
                            //yield return new WaitForSeconds(1.0f);
                            // Reset the text object back to original state.
                            //currTextComponent.ForceMeshUpdate();
                            //yield return new WaitForSeconds(1.0f);
                            // Reset our counters.
                            currentCharacter = 0;
                            startingCharacterRange = 0;
                            isRangeMax = true; // Would end the coroutine.
                        }
                    }
                }
                // Upload the changed vertex colors to the Mesh.
                currTextComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
                if (currentCharacter + 1 < characterCount) currentCharacter += 1;
                yield return new WaitForSeconds(0.25f - FadeSpeed * 0.01f);
            }
            FadeInFinished?.Invoke(TMProbject, true);
        }



        /// <summary>
        /// Method to animate vertex colors of a TMP Text object.
        /// </summary>
        /// <returns></returns>
        IEnumerator FadeTextOut(TextMeshProUGUI TMProbject)
        {
            TMP_Text currTextComponent = TMProbject.GetComponent<TMP_Text>();
            // Need to force the text object to be generated so we have valid data to work with right from the start.
            currTextComponent.ForceMeshUpdate();
            TMP_TextInfo textInfo = currTextComponent.textInfo;
            Color32[] newVertexColors;
            int currentCharacter = 0;
            int startingCharacterRange = currentCharacter;
            bool isRangeMax = false;
            while (!isRangeMax)
            {
                int characterCount = textInfo.characterCount;
                // Spread should not exceed the number of characters.
                byte fadeSteps = (byte)Mathf.Max(1, 255 / RolloverCharacterSpread);
                for (int i = startingCharacterRange; i < currentCharacter + 1; i++)
                {
                    // Skip characters that are not visible
                    if (!textInfo.characterInfo[i].isVisible) continue;
                    // Get the index of the material used by the current character.
                    int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
                    // Get the vertex colors of the mesh used by this text element (character or sprite).
                    newVertexColors = textInfo.meshInfo[materialIndex].colors32;
                    // Get the index of the first vertex used by this text element.
                    int vertexIndex = textInfo.characterInfo[i].vertexIndex;
                    // Get the current character's alpha value.
                    byte alpha = (byte)Mathf.Clamp(newVertexColors[vertexIndex + 0].a - fadeSteps, 0, 255);
                    // Set new alpha values.
                    newVertexColors[vertexIndex + 0].a = alpha;
                    newVertexColors[vertexIndex + 1].a = alpha;
                    newVertexColors[vertexIndex + 2].a = alpha;
                    newVertexColors[vertexIndex + 3].a = alpha;
                    // Tint vertex colors
                    // Note: Vertex colors are Color32 so we need to cast to Color to multiply with tint which is Color.
                    newVertexColors[vertexIndex + 0] = (Color)newVertexColors[vertexIndex + 0] * ColorTint;
                    newVertexColors[vertexIndex + 1] = (Color)newVertexColors[vertexIndex + 1] * ColorTint;
                    newVertexColors[vertexIndex + 2] = (Color)newVertexColors[vertexIndex + 2] * ColorTint;
                    newVertexColors[vertexIndex + 3] = (Color)newVertexColors[vertexIndex + 3] * ColorTint;
                    if (alpha == 0)
                    {
                        startingCharacterRange += 1;
                        if (startingCharacterRange == characterCount)
                        {
                            // Update mesh vertex data one last time.
                            currTextComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
                            //yield return new WaitForSeconds(1.0f);
                            // Reset the text object back to original state.
                            //currTextComponent.ForceMeshUpdate();
                            //yield return new WaitForSeconds(1.0f);
                            // Reset our counters.
                            currentCharacter = 0;
                            startingCharacterRange = 0;
                            isRangeMax = true; // Would end the coroutine.
                        }
                    }
                }
                // Upload the changed vertex colors to the Mesh.
                currTextComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
                if (currentCharacter + 1 < characterCount) currentCharacter += 1;
                yield return new WaitForSeconds(0.25f - FadeSpeed * 0.01f);
            }
            FadeOutFinished?.Invoke(TMProbject, false);
        }
    }
}
