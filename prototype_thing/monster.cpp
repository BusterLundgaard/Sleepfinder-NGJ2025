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

void draw_bar(char* title, float x, float y, float param, float param_max){
    DrawTextEx(font, title, Vector2{x-30, y+15}, 30.0, 2.0, BLACK);
    DrawRectangleLines(x, y, BAR_WIDTH, BAR_HEIGHT, BLACK);
    DrawRectangle(x,y, int((param/param_max)*BAR_WIDTH), BAR_HEIGHT, BLUE);    
}

void draw_negative_bar(char* title, float x, float y, float param, float range){
    DrawTextEx(font, title, Vector2{x-40, y+15}, 30.0, 2.0, BLACK);
    DrawRectangleLines(x, y, BAR_WIDTH, BAR_HEIGHT, BLACK);
    DrawRectangle(x, y, int(BAR_WIDTH*abs(param)/range), BAR_HEIGHT, BLUE);
}

float clamp(float x, float min, float max){
    return (x < min) ? min : ((x > max) ? max : x);
}

int sign(float x){
    if(x < 0){return -1;}
    else {return 1;}
}

// float fabs(float x){
//     return (x < 0) ? -x : x;
// }

float dist1(float x){
    return abs(x);
}
float dist2(float x){
    return x*x;
}
float dist3(float x){
    return x*x*x*x;
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
        // int key = GetKeyPressed();
        // while (key > 0) {
        //     if (key == KEY_RIGHT) {
        //         right_pressed = true;
        //         break;
        //     } 
        //     if (key == KEY_LEFT) {
        //         left_pressed = true;
        //         break;
        //     }
        //     key = GetKeyPressed();
        // }

        if(chosen_param != NULL && left_pressed){
            *chosen_param = clamp(*chosen_param - param_step, 0, 1);
        }
        if(chosen_param != NULL && right_pressed){
            *chosen_param = clamp(*chosen_param + param_step, 0, 1);
        }

        if(chosen_param == &al){DrawRectangle(45, 100, 10, 10, BLUE);}
        if(chosen_param == &be){DrawRectangle(45, 250, 10, 10, BLUE);}
        if(chosen_param == &om){DrawRectangle(45, 400, 10, 10, BLUE);}

        //float a2 = 0.7*dist2(al-al_o) + 2.3*dist2(be-be_o) + 0.9*(om - om_o);
        //float a3 = 0.3*dist2(al-al_o) + 0.8*dist2(be-be_o) + 2.2*(om - om_o);

        BeginDrawing();
        
        ClearBackground(RAYWHITE);
        draw_bar("q", 60, 100, al, 1.0);
        draw_bar("w", 60, 250, be, 1.0);
        draw_bar("e", 60, 400, om, 1.0);

        float a1 = C1.A*al + C1.B*be + C1.C*om;
        float a2 = C2.A*al + C2.B*be + C2.C*om;
        float a3 = C3.A*al + C3.B*be + C3.C*om;
        
        //printf("a1 is: %f\n", a1);
        draw_negative_bar("a1", 1400, 100, a1, 2.0);
        draw_negative_bar("a2", 1400, 250, a2, 2.0);
        draw_negative_bar("a3", 1400, 400, a3, 2.0);
        
        //draw_bar("a1", 1400, 100, a1, 3.0);
        //draw_bar("a2", 1400, 250, a2, 3.0);
        //draw_bar("a3", 1400, 400, a3, 3.0);

        frame++;
        new_frame();

        EndDrawing();
    }

    return 0;
}