using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sprite : MonoBehaviour
{
    public static string TypeOfSprite(GameObject gameObject)
    {
        string str = string.Empty;

        if (gameObject.GetComponent<Player>() != null) {
            str = "player";
        }

        else if (gameObject.GetComponent<Block>() != null) {
            str = "block";
        }

        else if (gameObject.GetComponent<Enemy>() != null) {
            str = "enemy";
        }

        else if (gameObject.GetComponent<ObjectM>() != null) {
            str = "object";
        }

        return str;
    }

    public static bool IsCoin(GameObject coin) 
    {
        return (coin.GetComponent<Coin>() != null);
    }

    public static void CreateAsset(GameObject asset, Vector3 position, bool doImageChanges = false, GameObject thisGO = null, bool behindGOLayer = true)
    {
        Transform transform = Instantiate(asset.transform, position, Quaternion.identity);

        if (doImageChanges && thisGO != null) {
            ImageChanges(transform.gameObject, thisGO, behindGOLayer);
        }
    }

    public static Transform CreateAssetWithReturn(GameObject asset, Vector3 position, bool doImageChanges = false, GameObject thisGO = null, bool behindGOLayer = true) 
    {
        Transform transform = Instantiate(asset.transform, position, Quaternion.identity);

        if (doImageChanges && thisGO != null) {
            ImageChanges(transform.gameObject, thisGO, behindGOLayer);
        }

        return transform;
    }

    public static void ImageChanges(GameObject asset, GameObject gameObject, bool behindGOLayer = true)
    {
        SpriteRenderer sr_asset = Sprite.GetSprite(asset);
        SpriteRenderer sr_gameObject = Sprite.GetSprite(gameObject);

        if (sr_asset != null && sr_gameObject) {
            sr_asset.sortingOrder = behindGOLayer ? sr_gameObject.sortingOrder - 5 : sr_gameObject.sortingOrder + 5;
        }
    }

    public static SpriteRenderer GetSprite(GameObject block)
    {
        if (block.GetComponent<SpriteRenderer>() != null) {
            return block.GetComponent<SpriteRenderer>();
        }
        else {
            return null;
        }
    }
}
