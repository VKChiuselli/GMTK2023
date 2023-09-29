using UnityEngine;

namespace finished3
{
    public class CharacterInfo : MonoBehaviour
    {
        public OverlayTile standingOnTile;

        public void SetSortingOrder(int sortingOrder)
        {
            transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sortingOrder = sortingOrder;
        }

    }

  
}
