using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Lookloop.ItemManager
{
public partial class Core
{
    void Awake()
    {
        Application.targetFrameRate = 60;
        canvas = GetComponentInParent<Canvas>();

        var rt = GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;
        rt.SetAsFirstSibling();

        var img = gameObject.AddComponent<Image>();
        img.color = Color.clear;
        img.raycastTarget = true;
    }

    void Start()
    {
        ContainerBuilder.BuildAll(this);
        StartCoroutine(LossTimeLoop());

        DragTool.BuildDragItem(this);
        DragTool.BuildShadow(this);

#if UNITY_EDITOR
        Test.Fill(this);
#endif
    }

    IEnumerator LossTimeLoop()
    {
        yield return new WaitForSeconds(1800f);
        while (true)
        {
            LossTime();
            yield return new WaitForSeconds(1800f);
        }
    }

}
}
