using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AssetFinder : MonoBehaviour {
    public static GameObject FindPrefabByObjectName(string prefabName) {
        string[] guids = AssetDatabase.FindAssets("t:Prefab");

        foreach (var guid in guids) {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (go.name.Equals(prefabName)) {
                Collectable goCollectable = go.GetComponent<Collectable>();
                goCollectable.type = goCollectable.itemSO.type;
                goCollectable.icon = goCollectable.itemSO.icon;
                return go;
            }
        }
        return null;
    }
    public static GameObject FindPrefabBySpriteName(string spriteName) {
        string[] guids = AssetDatabase.FindAssets("t:Prefab");

        foreach (var guid in guids) {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (go.GetComponent<Collectable>().itemSO != null && go.GetComponent<Collectable>().itemSO.icon.name.Equals(spriteName)) {
                Collectable goCollectable = go.GetComponent<Collectable>();
                goCollectable.type = goCollectable.itemSO.type;
                goCollectable.icon = goCollectable.itemSO.icon;
                return go;
            }
        }
        return null;
    }
    public static Tile FindTile(string tileName) {
        string[] guids = AssetDatabase.FindAssets("t:Tile");

        foreach (var guid in guids) {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            Tile tile = AssetDatabase.LoadAssetAtPath<Tile>(path);
            if (tile.sprite.name.Equals(tileName)) {
                return tile;
            }
        }
        return null;
    }
}