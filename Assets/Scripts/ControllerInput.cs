using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class ControllerInput {

	public static bool PressSelect() {
        return PressButtonDown() ;
    }

    public static bool PressBack() {
        return PressButtonRight();
    }

    public static void PressMenu() {
        PressMenu(true);
    }

    public static void PressMenu(bool b) {
        if (PressBack() && b) {
            SceneManager.LoadScene(0);
        }
    }

    static bool up, down, left, right;

    public static void ResetToggles() {
        if (Input.GetAxisRaw("DpadY") == 0) {
            up = false;
            down = false;
        }

        if (Input.GetAxisRaw("DpadX") == 0) {
            left = false;
            right = false;
        }
    }

    //button presses
    public static bool PressArrowUp() {
        bool press = Input.GetAxisRaw("DpadY") > 0 && !up;
        if (press) up = true;
        return Input.GetKeyDown(KeyCode.UpArrow) || press;
    }

    public static bool PressArrowDown() {
        bool press = Input.GetAxisRaw("DpadY") < 0 && !down;
        if (press) down = true;
        return Input.GetKeyDown(KeyCode.DownArrow) || press;
    }

    public static bool PressArrowLeft() {
        bool press = Input.GetAxisRaw("DpadX") < 0 && !left;
        if (press) left = true;
        return Input.GetKeyDown(KeyCode.LeftArrow) || press;
    }

    public static bool PressArrowRight() {
        bool press = Input.GetAxisRaw("DpadX") > 0 && !right;
        if (press) right = true;
        return Input.GetKeyDown(KeyCode.RightArrow) || press;
    }

    public static bool PressButtonUp() {
        return Input.GetKeyDown(KeyCode.I)  || Input.GetButtonDown("Button up");
    }

    public static bool PressButtonDown() {
        return Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.K)  || Input.GetButtonDown("Button down");
    }

    public static bool PressButtonLeft() {
        return Input.GetKeyDown(KeyCode.RightShift) || Input.GetKeyDown(KeyCode.J)  || Input.GetButtonDown("Button left");
    }

    public static bool PressButtonRight() {
        return Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.L)  || Input.GetButtonDown("Button right");
    }

    //axis
    public static float GetHorizontal() {
        return Input.GetAxis("DpadX");
    }

    public static float GetVertical() {
        return Input.GetAxis("DpadY");
    }
}
