#include <raylib.h>
#include <math.h>

#include "input.cpp"

const int BAR_WIDTH = 100;
const int BAR_HEIGHT = 30;
const float param_step = 0.05;
Font font;

float al_o = 0.4;
float be_o = 0.75;
float om_o = 0.1;

float al = 0.8;
float be = 0.5;
float om = 0.9;
float* chosen_param = NULL;
float* params[3] = {&al, &be, &om};

float clamp(float x, float min, float max){
    return (x < min) ? min : ((x > max) ? max : x);
}

int sign(float x){
    if(x < 0){return -1;}
    else {return 1;}
}

// Returns a random float between 0 and 1
float r(){
    return (float)rand()/(float)(RAND_MAX);
}

int r_int(int min, int max){
    return int(min + (max-min)*r());
}

struct coefficients {
    float A;
    float B;
    float C;
};

coefficients arrangements[6]{
    {1, 1, -1},
    {1, -1, 1},
    {-1, 1, 1},
    {-1, -1, 1},
    {-1, 1, -1},
    {1, -1, -1}
};

coefficients generate_coeffs(){
    int i = r_int(0,6);
    coefficients c = arrangements[r_int(0, 6)];
    c.A *= r();
    c.B *= r();
    c.C *= fabs((c.A*al_o + c.B*be_o)/om_o);
    return c;
}

int main()
{
    InitWindow(1920, 1080, "Prototype shizzle");
    SetTargetFPS(60);
    
    int frame = 0;

    srand((unsigned int)time(NULL));
    al_o = 0.1 + 0.9*r();
    be_o = 0.1 + 0.9*r();
    om_o = 0.1 + 0.9*r();

    coefficients C1 = generate_coeffs();
    coefficients C2 = generate_coeffs();
    coefficients C3 = generate_coeffs();

    while (!WindowShouldClose())
    {
        if(IsKeyDown(KEY_Q)){chosen_param = &al;}
        else if(IsKeyDown(KEY_W)){chosen_param = &be;}
        else if(IsKeyDown(KEY_E)){chosen_param = &om;}

        bool right_pressed = IsKeyPressed(KEY_D);
        bool left_pressed =  IsKeyPressed(KEY_A);

        if(chosen_param != NULL && left_pressed){
            *chosen_param = clamp(*chosen_param - param_step, 0, 1);
        }
        if(chosen_param != NULL && right_pressed){
            *chosen_param = clamp(*chosen_param + param_step, 0, 1);
        }

        float a1 = C1.A*al + C1.B*be + C1.C*om;
        float a2 = C2.A*al + C2.B*be + C2.C*om;
        float a3 = C3.A*al + C3.B*be + C3.C*om;

        new_frame();

        EndDrawing();
    }

    return 0;
}