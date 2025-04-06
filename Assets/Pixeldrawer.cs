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

    public RawImage renderSurface;

    const int ROWS = 20;
    const int COLS = 60;
    const int WAVE_CONTAINER_WIDTH = 39;

    readonly Color FOREGROUND = Color.green;

    float time = 0f;

    // Drawing character approximation using your pixel functions
    void DrawChar(int x, int y, char c, Color color)
    {
        int height = 14;
        int width = 8;
        int X0 = x * width;
        int X1 = (x + 1) * width;
        int Y0 = y * height;
        int Y1 = (y + 1) * height;

        int midY = y * height + height / 2;
        int midX = x * width + width / 2;

        switch (c)
        {
            case '-':
                for(int i = X0+2; i <= X1-2; i++){DrawPixel(i, midY, color);}            
                break;
            case '_':
                for (int i = X0 + 1; i <= X1 - 1; i++) { DrawPixel(i, Y1-1, color); }
                break;
            case '.':
                for(int i = midX - 1; i <= midX + 1; i++) { DrawPixel(i, Y1 - 2, color);}
                for (int i = midX - 1; i <= midX + 1; i++) { DrawPixel(i, Y1 - 1, color); }
                break;
            case '|':
                for(int i = Y0 + 1; i <= Y1-1; i++) { DrawPixel(midX, i, color); }
                break;
            case '*':
                draw_rectangle(X0 + 1, midY - 2, 6, 6, color);
                break;
            case '~':
                draw_rectangle(X0 + 3, midY - 2, 2, 4, color);
                break;
        }
    }

    void DrawBar(int x, int y)
    {
        for (int i = x; i < x + 4; i++)
        {
            DrawChar(i, y, '_', FOREGROUND);
        }

        for (int i = y + 1; i <= y + 5; i++)
        {
            DrawChar(x, i, '|', FOREGROUND);
            DrawChar(x + 3, i, '|', FOREGROUND);
        }
    }

    void DrawWaveContainer(int x, int y)
    {
        for (int i = y; i < y + 5; i++)
        {
            DrawChar(x, i, '|', FOREGROUND);
            DrawChar(x + WAVE_CONTAINER_WIDTH, i, '|', FOREGROUND);
        }

        for (int i = x + 1; i < x + WAVE_CONTAINER_WIDTH; i++)
        {
            DrawChar(i, y, '-', FOREGROUND);
            DrawChar(i, y + 2, '-', FOREGROUND);
            DrawChar(i, y + 4, '-', FOREGROUND);
        }
    }

    void DrawWave(int xStart, int yMid, float amplitude, float frequency, float velocity, Color color)
    {
        for (int i = xStart; i < xStart + WAVE_CONTAINER_WIDTH; i++)
        {
            int val = Mathf.RoundToInt(6 * amplitude * Mathf.Sin(frequency * i + time * velocity));

            char waveChar = '-';
            if (val != 0)
            {
                waveChar = (val % 2 == 0) ? '*' : '~';
            }

            int offset = (val >= 0) ? Mathf.RoundToInt((val - 1) / 2f) : Mathf.RoundToInt((val + 1) / 2f);

            DrawChar(i, yMid + offset, waveChar, color);
        }
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
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                DrawPixel(i, j, Color.red);
            }
        }

        //DrawBar(2, 1);

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
