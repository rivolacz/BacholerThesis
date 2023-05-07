using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Project
{
    public class SpeedUpManager : MonoBehaviour
    {
        bool speedUp = false;
        [SerializeField]
        private TMP_Text SpeedUpText;

        public void ChangeGameSpeed()
        {
            speedUp = !speedUp;
            if (speedUp)
            {
                Time.timeScale = 2;
                SpeedUpText.text = "2x";
            }
            else
            {
                Time.timeScale = 1;
                SpeedUpText.text = "1x";
            }
        }
    }
}
