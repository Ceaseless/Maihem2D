using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Random;
using Image = UnityEngine.UIElements.Image;


namespace Maihem
{
    public class ScrollingBackground : MonoBehaviour
    {
        
        [SerializeField] private GameObject background;
        
        private float _startpos;
        private float _screenWidth;
        private float _length;
        private float _height;
        private float _speed;
        private void Start()
        {
            if (Camera.main != null) _startpos = Camera.main.transform.position.x;
            _screenWidth = background.GetComponent<RectTransform>().rect.width;
            var rect = GetComponent<RectTransform>().rect;
            _length = rect.width;
            _speed = Range(50, 250);
        }

        
        private void Update()
        {
            var movement = new Vector3(_speed * -1, 0, 0);

            movement *= Time.deltaTime;
            
            
            if (Camera.main != null && transform.position.x+_length*2 < Camera.main.orthographicSize * Screen.width / Screen.height)
            {
                transform.position = new Vector3(_startpos + _screenWidth*1.5f, transform.position.y, 0);
            }
            transform.Translate(movement);
                
        }
    }
}
