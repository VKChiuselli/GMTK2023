using System.Collections.Generic;
using UnityEngine;
using static finished3.ArrowTranslator;

namespace finished3
{
    public class OverlayTile : MonoBehaviour
    {
        public int G;
        public int H;
        public int F { get { return G + H; } }

        public bool isBlocked = false;

        public OverlayTile Previous;
        public Vector3Int gridLocation;
        public Vector2Int grid2DLocation {get { return new Vector2Int(gridLocation.x, gridLocation.y); } }

        public List<Sprite> arrows;

        public SpriteRenderer ArrowSprite;
        public SpriteRenderer ShadowSprite;
        public SpriteRenderer SelectionSprite;

        private void Start()
        {
            ShadowSprite.sortingOrder = SelectionSprite.sortingOrder;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                HideTile();
                ShowShadow();
            }
        }

        public void HideTile()
        {
            SelectionSprite.color = new Color(1, 1, 1, 0);
        }

        public void ShowTile()
        {
            SelectionSprite.color = new Color(1, 1, 1, 1);
        }

        public void ShowShadow()
        {
            ShadowSprite.enabled = true;
        }

        public void HideShadow()
        {
            ShadowSprite.enabled = false;
        }

        public void SetSprite(ArrowDirection d)
        {
            if (d == ArrowDirection.None)
                ArrowSprite.color = new Color(1, 1, 1, 0);
            else
            {
                ArrowSprite.color = new Color(1, 1, 1, 1);
                ArrowSprite.sprite = arrows[(int)d];
                ArrowSprite.sortingOrder = gameObject.GetComponent<SpriteRenderer>().sortingOrder;
            }
        }

    }
}
