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

    public static RectTransform AddSpacer(RectTransform parent, float height)
    {
        GameObject go = new GameObject("spacer");
        go.transform.SetParent(parent, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        LayoutElement le = go.AddComponent<LayoutElement>();
        le.minHeight = height;
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
}