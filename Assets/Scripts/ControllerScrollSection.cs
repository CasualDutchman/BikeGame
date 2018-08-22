using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerScrollSection : MonoBehaviour {

    List<Transform> menuItems = new List<Transform>();

    public float moveSpeed;

    public float beginOffset;
    public float spacing;

    public int maxOnScreen;

    public RectTransform menuItemHolder;
    public RectTransform selectionBox;

    public AnimationCurve curve;

    int topgame = 0;
    int currentSelected = 0;
    bool moving = false;

    int max;
    float objectSize;

    Func<GameObject, object, bool> method;
    GameObject prefab;

    public void Create(GameObject _prefab, object[] list, Func<GameObject, object, bool> setupMethod) {

        if (menuItems.Count > 0) {
            UpdateList(list);
            return;
        }

        max = list.Length;
        objectSize = _prefab.GetComponent<RectTransform>().sizeDelta.y + spacing;

        prefab = _prefab;
        method = setupMethod;

        for (int i = 0; i < list.Length; i++) {
            RectTransform go = Instantiate(_prefab).GetComponent<RectTransform>();
            go.SetParent(menuItemHolder);
            go.anchoredPosition = new Vector3(0, beginOffset - (i * (go.sizeDelta.y + spacing)), 0);
            go.localScale = Vector3.one;

            setupMethod(go.gameObject, list[i]);

            menuItems.Add(go);
        }

        selectionBox.SetAsLastSibling();
        MoveSelection();
    }

    public void UpdateList(object[] list) {
        foreach (Transform child in menuItems) {
            DestroyImmediate(child.gameObject);
        }

        menuItems.Clear();

        max = list.Length;
        currentSelected = 0;
        topgame = 0;

        menuItemHolder.anchoredPosition = new Vector2(0, menuItemHolder.sizeDelta.y * -0.5f);

        for (int i = 0; i < list.Length; i++) {
            RectTransform go = Instantiate(prefab).GetComponent<RectTransform>();
            go.SetParent(menuItemHolder);
            go.anchoredPosition = new Vector3(0, beginOffset - (i * (go.sizeDelta.y + spacing)), 0);
            go.localScale = Vector3.one;

            method(go.gameObject, list[i]);

            menuItems.Add(go);
        }

        selectionBox.sizeDelta = prefab.GetComponent<RectTransform>().sizeDelta;

        selectionBox.SetAsLastSibling();
        MoveSelection();
    }

    public int GetSelected() {
        return currentSelected;
    }

    public void GoDown() {
        if (moving)
            return;

        if (currentSelected + 1 < max) {
            currentSelected += 1;

            if (currentSelected >= topgame + maxOnScreen) {
                StartCoroutine(MoveHolder(true));
            } else {
                MoveSelection();
            }
        }
    }

    public void GoUp() {
        if (moving)
            return;

        if (currentSelected - 1 >= 0) {
            currentSelected -= 1;

            if (currentSelected < topgame) {
                StartCoroutine(MoveHolder(false));
            } else {
                MoveSelection();
            }
        }
    }

    void MoveSelection() {
        selectionBox.position = menuItems[currentSelected].position;
    }

    public void ResetSelection() {
        currentSelected = 0;
        topgame = 0;

        menuItemHolder.anchoredPosition = new Vector2(0, menuItemHolder.sizeDelta.y * -0.5f);

        MoveSelection();
    }

    IEnumerator MoveHolder(bool down) {
        float timer = 0;
        Vector3 oldPos = menuItemHolder.anchoredPosition;
        Vector3 newPos = oldPos + new Vector3(0, objectSize * (down ? 1 : -1), 0);

        moving = true;
        //menuItems[currentSelected].gameObject.SetActive(true);

        while (timer < 10) {
            timer += Time.deltaTime / moveSpeed;

            menuItemHolder.anchoredPosition = Vector3.Lerp(oldPos, newPos, curve.Evaluate(timer));

            if (timer >= 1) {
                menuItemHolder.anchoredPosition = newPos;
                //menuItems[down ? topgame : topgame + 2].gameObject.SetActive(false);
                topgame += down ? 1 : -1;
                MoveSelection();
                moving = false;
                break;
            }

            yield return new WaitForEndOfFrame();
        }
        yield return 0;
    }
}
