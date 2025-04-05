using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
struct Coefficients
{
    public float A;
    public float B;
    public float C;
    public Coefficients(float a, float b, float c)
    {
        A = a; B = b; C = c;
    }
}

enum Param { None, Alpha, Beta, Omega }

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

    public float paramStep = 0.05f;

    float al_o, be_o, om_o;
    float al = 0.8f, be = 0.5f, om = 0.9f;

    Coefficients A1, A2, A3; 
    Coefficients V1, V2, V3;

    //These are the final computed intensities of audio distortions (a1, a2, a3) and visual distortions respectively (v1, v2, v3)
    //These are computed as dot products between their respective coefficient vectors and (al, be, om)
    // a_i = dot_product(A_i, Vec(al, be, om))
    // v_i = dot_product(V_i, Vec(al, be, om))
    float a1, a2, a3;
    float v1, v2, v3;

    Param selectedParam = Param.None;

    Coefficients[] arrangements = new Coefficients[]
    {
        new Coefficients(1, 1, -1),
        new Coefficients(1, -1, 1),
        new Coefficients(-1, 1, 1),
        new Coefficients(-1, -1, 1),
        new Coefficients(-1, 1, -1),
        new Coefficients(1, -1, -1)
    };

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

        RandomizeBaseParams();
        A1 = GenerateCoeffs();
        A2 = GenerateCoeffs();
        A3 = GenerateCoeffs();
    }

    void Update()
    {
        // =============================
        // LOGIC
        // =============================
        HandleInput();

        a1 = A1.A * al + A1.B * be + A1.C * om;
        a2 = A2.A * al + A2.B * be + A2.C * om;
        a3 = A3.A * al + A3.B * be + A3.C * om;

        v1 = V1.A * al + V1.B * be + V1.C * om;
        v2 = V2.A * al + V2.B * be + V2.C * om;
        v3 = V3.A * al + V3.B * be + V3.C * om;


        // ==============================
        // DRAWING
        // =============================
        draw_rectangle(0, 0, width, height, Color.grey);

        draw_rectangle_lines(wave_widget_margin_left, wave_widget_margin_bottom, wave_widget_w, wave_widget_h, Color.green);
        draw_rectangle_lines(wave_widget_margin_left, wave_widget_margin_bottom, (int)(wave_widget_w*om), wave_widget_h, Color.blue);

        draw_rectangle_lines(wave_widget_margin_left, wave_widget_margin_bottom + wave_widget_spacing + wave_widget_h, wave_widget_w, wave_widget_h, Color.green);
        draw_rectangle_lines(wave_widget_margin_left, wave_widget_margin_bottom + wave_widget_spacing + wave_widget_h, (int)(wave_widget_w * be), wave_widget_h, Color.blue);

        draw_rectangle_lines(wave_widget_margin_left, wave_widget_margin_bottom + 2*(wave_widget_spacing + wave_widget_h) , wave_widget_w, wave_widget_h, Color.green);
        draw_rectangle_lines(wave_widget_margin_left, wave_widget_margin_bottom + 2 * (wave_widget_spacing + wave_widget_h), (int)(wave_widget_w * al), wave_widget_h, Color.blue);

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

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.A)) selectedParam = Param.Alpha;
        if (Input.GetKeyDown(KeyCode.B)) selectedParam = Param.Beta;
        if (Input.GetKeyDown(KeyCode.O)) selectedParam = Param.Omega;

        if (Input.GetKeyDown(KeyCode.LeftArrow)) AdjustSelectedParam(-paramStep);
        if (Input.GetKeyDown(KeyCode.RightArrow)) AdjustSelectedParam(paramStep);
    }

    void AdjustSelectedParam(float delta)
    {
        switch (selectedParam)
        {
            case Param.Alpha: al = Mathf.Clamp01(al + delta); break;
            case Param.Beta: be = Mathf.Clamp01(be + delta); break;
            case Param.Omega: om = Mathf.Clamp01(om + delta); break;
        }
    }

    void RandomizeBaseParams()
    {
        al_o = 0.1f + 0.9f * Random.value;
        be_o = 0.1f + 0.9f * Random.value;
        om_o = 0.1f + 0.9f * Random.value;
    }

    Coefficients GenerateCoeffs()
    {
        Coefficients baseCoeffs = arrangements[Random.Range(0, arrangements.Length)];
        float A = baseCoeffs.A * Random.value;
        float B = baseCoeffs.B * Random.value;
        float C = baseCoeffs.C * Mathf.Abs((A * al_o + B * be_o) / om_o);
        return new Coefficients(A, B, C);
    }
}
