using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIHelper : MonoBehaviour
{
    public static RectTransform Create(RectTransform prefab, RectTransform parent)
    {
        RectTransform rt = Instantiate(prefab);
        rt.SetParent(parent);
        rt.localScale = Vector2.one;
        rt.localPosition = Vector3.zero;
        return rt;
    }

    public static void SetNavigationVertical(List<RectTransform> rts, Navigation startNav, Navigation middleNav, Navigation endNav)
    {
        for (int i = 0; i < rts.Count; i++)
        {
            Selectable current = rts[i].GetComponentInChildren<Selectable>();

            if (i == 0)
            {
                startNav.selectOnDown = rts[i + 1].GetComponentInChildren<Selectable>();
                current.navigation = startNav;
            }
            else if (i == rts.Count - 1)
            {
                endNav.selectOnUp = rts[i - 1].GetComponentInChildren<Selectable>();
                rts[i].GetComponentInChildren<Selectable>().navigation = endNav;
            }
            else
            {
                Navigation nav = middleNav;
                nav.selectOnUp = rts[i - 1].GetComponentInChildren<Selectable>();
                nav.selectOnDown = rts[i + 1].GetComponentInChildren<Selectable>();
                current.navigation = nav;
            }
        }
    }

    public static RectTransform SetAnchors(RectTransform rt, SpriteAlignment alignment)
    {
        Vector2 vec = Vector2.zero;

        switch (alignment)
        {
            case SpriteAlignment.BottomLeft:
                vec = new Vector2(0.0f, 0.0f);
                break;
            case SpriteAlignment.BottomCenter:
                vec = new Vector2(0.5f, 0.0f);
                break;
            case SpriteAlignment.BottomRight:
                vec = new Vector2(1.0f, 0.0f);
                break;
            case SpriteAlignment.LeftCenter:
                vec = new Vector2(0.0f, 0.5f);
                break;
            case SpriteAlignment.Center:
                vec = new Vector2(0.5f, 0.5f);
                break;
            case SpriteAlignment.RightCenter:
                vec = new Vector2(1.0f, 0.5f);
                break;
            case SpriteAlignment.TopLeft:
                vec = new Vector2(0.0f, 1.0f);
                break;
            case SpriteAlignment.TopCenter:
                vec = new Vector2(0.5f, 1.0f);
                break;
            case SpriteAlignment.TopRight:
                vec = new Vector2(1.0f, 1.0f);
                break;
        }

        rt.anchoredPosition = Vector2.zero;
        rt.anchorMin = vec;
        rt.anchorMax = vec;
        rt.pivot = vec;

        return rt;
    }

    public static RectTransform SetSize(RectTransform rt, int width, int height)
    {
        LayoutElement le = rt.GetComponent<LayoutElement>();

        if (le != null)
        {
            le.preferredWidth = width;
            le.preferredHeight = height;
        }

        return rt;
    }
}