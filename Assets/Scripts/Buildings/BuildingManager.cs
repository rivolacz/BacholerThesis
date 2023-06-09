using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Windows;

namespace Project
{
    public class BuildingManager : MonoBehaviour
    {
        [SerializeField]
        private ContactFilter2D contactFilter;
        [SerializeField]
        private GameObject buildingConfirmation;
        [SerializeField]
        private Image buyingImage;
        [SerializeField]
        private TMP_Text buildingCostText;
        [SerializeField]
        private Sprite blackTick;
        [SerializeField]
        private Sprite redTick;
        private PlayerInput input;
        private Vector2 buildingPosition = Vector2.zero;
        private GameObject buildingObject = null;
        List<Collider2D> colliders = new List<Collider2D>();
        private ConstructionSite constructionSite = null;
        private BoxCollider2D boxCollider;
        private float buildingCost = 0;
        private bool alreadyBuilding = false;
        private RangeVisualizer rangeVisualizer = null;
        private Building buildingToBeBuild = null;
        private void Awake()
        {
            input = new PlayerInput();
            input.Enable();
        }

        private void Update()
        {
            if (!alreadyBuilding || buildingObject == null) return;
            UpdateGraphics();
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                Vector2 tapPosition = input.Player.FirstTouchPosition.ReadValue<Vector2>();
                buildingPosition = Camera.main.ScreenToWorldPoint(tapPosition);
                buildingObject.transform.position = buildingPosition;
            }
            boxCollider.Overlap(contactFilter, colliders);
            if (colliders.Count > 0)
            {
                constructionSite.SetColorToBlueprint(Color.red);
            }
            else
            {
                constructionSite.SetColorToBlueprint(Color.green);
            }
        }

        public void UpdateGraphics()
        {
            if (GameData.CanAfford(buildingCost))
            {
                buyingImage.sprite = blackTick;
            }
            else
            {
                buyingImage.sprite = redTick;
            }
        }

        public void StartBuilding(Building building)
        {
            if (alreadyBuilding) return;
            Settings.PlayerIsBuilding = true;
            alreadyBuilding = true;
            buildingConfirmation.SetActive(true);
            UpdateGraphics();
            Vector2 middleOfTheScreen = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height) / 2;
            buildingPosition = Camera.main.ScreenToWorldPoint(middleOfTheScreen);
            buildingObject = Instantiate(building.BuildingPrefab, buildingPosition, Quaternion.identity);
            rangeVisualizer = buildingObject.GetComponentInChildren<RangeVisualizer>();
            if(rangeVisualizer != null)
            {
                var turret = buildingObject.GetComponentInChildren<ArcherTurret>();
                if (turret != null)
                {
                    rangeVisualizer.SetRange(turret.Range);
                }
            }
            constructionSite = buildingObject.GetComponent<ConstructionSite>();
            constructionSite.DisableShooting();
            constructionSite.isBlueprint = true;
            boxCollider = buildingObject.GetComponentInChildren<BoxCollider2D>(false);
            boxCollider.isTrigger = true;
            buildingCost = building.BuildingCost;
            buildingCostText.text = buildingCost.ToString();
            buildingToBeBuild = building;
        }

        public void CancelBuilding()
        {
            alreadyBuilding = false;
            Settings.PlayerIsBuilding = false;
            if(buildingObject != null)
            {
                Destroy(buildingObject);
            }
        }

        public void FinishBuilding()
        {
            if (GameData.CanAfford(buildingCost))
            {
                boxCollider.Overlap(contactFilter, colliders);
                if (colliders.Count > 0) return;
                GameData.Buy(buildingCost);
                Settings.PlayerIsBuilding = false;
                constructionSite.isBlueprint = false;
                alreadyBuilding = false;
                constructionSite.SetColorToBlueprint(Color.white);
                constructionSite.StartBuilding(buildingToBeBuild);
                boxCollider.isTrigger = false;
                buildingConfirmation.SetActive(false);
                rangeVisualizer.gameObject.SetActive(false);
                buildingObject = null;
            }
        }
    }
}
