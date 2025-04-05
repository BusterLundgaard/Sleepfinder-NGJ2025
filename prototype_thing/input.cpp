#include <raylib.h>
#include <iostream>
#include <unordered_map>

// Global variable definition
std::unordered_map<int, bool> P_Is_Key_Pressed = {
    {KEY_LEFT, false},
    {KEY_RIGHT, false},
    {KEY_Q, false},
    {KEY_A, false},
    {KEY_D, false}
};

void new_frame() {
    for (auto iter = P_Is_Key_Pressed.begin(); iter != P_Is_Key_Pressed.end(); ++iter) {
        P_Is_Key_Pressed.insert_or_assign(iter->first, IsKeyDown(iter->first));
    }
}

bool Key_Press(int key) {
    return IsKeyDown(key) && !P_Is_Key_Pressed.at(key);
}

bool Key_Up(int key) {
    return !IsKeyDown(key) && P_Is_Key_Pressed.at(key);
}
