using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pixeldrawer : MonoBehaviour
{
    public RawImage rawImage;
    public const int width = 475;
    public const int height = 275;

    public int wave_widget_w = 100;
    public int wave_widget_h = 20;
    public int wave_widget_margin_bottom = 20;
    public int wave_widget_margin_left = 20;
    public int wave_widget_spacing = 20;

    public const int H = 12;
    public const int W = 6;

    public const int COLUMNS = width / W;
    public const int ROW = height / H;

    private Texture2D texture;
    // Start is called before the first frame update

    void draw_rectangle_lines(int x, int y, int w, int h, Color color)
    {
        for(int i = 0; i < w; i++)
        {
            DrawPixel(x + i, y, color);
            DrawPixel(x + i, y + h, color);
        }
        for (int i = 0; i < h; i++)
        {
            DrawPixel(x, y + i, color);
            DrawPixel(x + w, y + i, color);
        }
    }

    void draw_rectangle(int x, int y, int w, int h, Color color)
    {
        for(int i = 0; i < w; i++)
        {
            for(int j = 0; j < h; j++)
            {
                DrawPixel(x + i, y + j, color);
            }
        }
    }

    void Start()
    {
        // Create the texture
        texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point; // No smoothing
        texture.wrapMode = TextureWrapMode.Clamp;

        // Clear it with transparent black
        ClearTexture(Color.clear);

        // Assign to the RawImage
        rawImage.texture = texture;

        // Draw something
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                DrawPixel(i, j, Color.black);
            }
        }

        texture.Apply(); // Apply the changes
    }

    void Update()
    {
        draw_rectangle(0, 0, 475, 275, Color.grey);
        draw_rectangle_lines(wave_widget_margin_left, wave_widget_margin_bottom, wave_widget_w, wave_widget_h, Color.green);
        draw_rectangle_lines(wave_widget_margin_left, wave_widget_margin_bottom + wave_widget_spacing + wave_widget_h, wave_widget_w, wave_widget_h, Color.green);
        draw_rectangle_lines(wave_widget_margin_left, wave_widget_margin_bottom + 2*(wave_widget_spacing + wave_widget_h) , wave_widget_w, wave_widget_h, Color.green);

        texture.Apply();
    }

    void DrawPixel(int x, int y, Color color)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            texture.SetPixel(x, y, color);
        }
    }

    void ClearTexture(Color color)
    {
        Color[] fillColorArray = texture.GetPixels();
        for (int i = 0; i < fillColorArray.Length; i++)
        {
            fillColorArray[i] = color;
        }
        texture.SetPixels(fillColorArray);
        texture.Apply();
    }
}
