using System.Collections.Generic;
using UnityEngine;

public class Selector : MonoBehaviour {
    private readonly List<GameObject> selectableObjects = new();
    [HideInInspector] public GameObject selectedObject;

    private void Update() {
        foreach (GameObject obj in selectableObjects) {
            if (selectedObject == null || Vector3.Distance(transform.position, obj.transform.position) < Vector3.Distance(transform.position, selectedObject.transform.position)) SelectObject(obj);
        } if (selectableObjects.Count == 0) SelectObject(null);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Interactable")) selectableObjects.Add(collision.gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Interactable")) selectableObjects.Remove(collision.gameObject);
    }

    private void SelectObject(GameObject obj) {
        if (selectedObject != null) selectedObject.transform.GetChild(0).gameObject.SetActive(false);
        selectedObject = obj;
        if (selectedObject != null) selectedObject.transform.GetChild(0).gameObject.SetActive(true);
    }
}