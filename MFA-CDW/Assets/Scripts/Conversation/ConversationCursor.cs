using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConversationCursor : MonoBehaviour
{
    public Animator cursorAnim;
    public Texture2D cursorDefault;
    public Texture2D cursorHover;

    void Start()
    {
        Cursor.SetCursor(cursorDefault, Vector2.zero, CursorMode.ForceSoftware);
    }


    public void OnMouseEnter()
    {
        cursorAnim.SetBool("isHovering", true);
    }

}
