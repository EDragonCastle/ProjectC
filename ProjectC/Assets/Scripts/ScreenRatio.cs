using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ScreenRatio : MonoBehaviour
{
    public Image titleImage;

    private readonly float baseAspectRatio = 1920f / 1080f;
    public float width1920 = 104f;
    private bool isFullScreen = true;

    private List<Vector2Int> ratioList = new List<Vector2Int>();

    public ScreenRatio()
    {
        ratioList.Add(new Vector2Int(1920, 1080));
        ratioList.Add(new Vector2Int(1600, 1024));
        ratioList.Add(new Vector2Int(1280, 800));
        ratioList.Add(new Vector2Int(1024, 768));
    }

    public void ChangeFullScreen(bool isTrue)
    {
        isFullScreen = isTrue;
    }

    public void ChangeRatio(int index)
    {
        if (index >= ratioList.Count)
            index = 0;

        Screen.SetResolution(ratioList[index].x, ratioList[index].y, isFullScreen);
        SetWidthImage(ratioList[index].x, ratioList[index].y);
    }
    
    private void SetWidthImage(int width, int height)
    {
        if (titleImage == null)
            return;

        float currentAspectRatio = (float)width / height;

        float ratio = currentAspectRatio / baseAspectRatio;
        ratio *= width1920;

        var rectTransform = titleImage.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(ratio, rectTransform.sizeDelta.y);
    }
}
