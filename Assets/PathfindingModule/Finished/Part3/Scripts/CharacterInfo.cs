using UnityEngine;

namespace finished3
{
    public class CharacterInfo : MonoBehaviour
    {
        public OverlayTile standingOnTile;


        private void Update()
        {
          transform.GetChild(0).gameObject. GetComponent<SpriteRenderer>().sortingOrder = 99;
        }

    }

  
}
