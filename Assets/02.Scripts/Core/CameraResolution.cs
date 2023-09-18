using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraResolution : MonoBehaviour
{
    private Camera _mainCam;

    void Start()
    {
        _mainCam = GetComponent<Camera>();
    }

    private void Update()
    {
        if(_mainCam.rect.width != Screen.width || _mainCam.rect.height != Screen.height)
        {
            Rect rect = _mainCam.rect;

            float scaleheight = ((float)Screen.width / Screen.height) / ((float)9f / 16f);
            float scalewidth = 1f / scaleheight;

            if (scaleheight < 1)
            {
                rect.height = scaleheight;
                rect.y = (1f - scaleheight) / 2f;
            }
            else
            {
                rect.width = scalewidth;
                rect.x = (1f - scalewidth) / 2f;
            }

            _mainCam.rect = rect;
        }
    }
}