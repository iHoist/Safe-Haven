using UnityEngine;
using UnityEngine.UI;

public class TreeObject : MonoBehaviour {
    private GameObject progressBar;
    private double health = 100;

    private void Awake() {
        progressBar = transform.GetChild(1).gameObject;
    }

    private void Update() {
        if (!progressBar.activeInHierarchy && (health < 100 && health > 0)) { progressBar.SetActive(true); }
    }

    public void DamageTree(double damage) {
        health -= damage;
        if (health <= 0) { health = 0; progressBar.SetActive(false); Destroy(gameObject); }
        else { progressBar.transform.GetChild(1).GetComponent<Image>().fillAmount = (float)health / 100; ShakeTree(); }
    }

    public void ShakeTree() {
        GetComponent<Animator>().SetTrigger("Interact");
        if (name[..17].Equals("Pinetree (Winter)")) transform.GetChild(2).gameObject.GetComponent<Animator>().SetTrigger("Interact");
    }
}