using Project.StateMachines;
using Project.Upgrades;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project
{
    public class BuildingState : BaseState
    {
        BuilderUpgrades builderUpgrades;
        private ConstructionSite constructionSite { get; set; }
        private Vector3 BuildingPosition { get; set; }
        float buildSpeed = 1;

        public BuildingState(ConstructionSite building, Vector3 position, StateMachine stateMachine) : base(stateMachine)
        {
            constructionSite = building;
            BuildingPosition = position;
            builderUpgrades = UpgradesManager.Upgrades.BuilderUpgrades;
        }

        public override void Enter()
        {
            Debug.Log("Building a building with cost of " + constructionSite.name);
            if (builderUpgrades.BuildingSpeedMultiplierBought)
            {
                buildSpeed *= builderUpgrades.BuildingSpeedMultiplier;
            }
            if (builderUpgrades.InstantlyBuildBuildingsBought)
            {
                buildSpeed = 99999999;
            }
            Debug.Log("Build speed " + buildSpeed);
        }

        public override void StateUpdate()
        {
            Debug.Log("Building");
            bool finished = constructionSite.ProgressWithBuild(Time.deltaTime * buildSpeed);
            if(finished)
            {
                stateMachine.ChangeState(new IdleState(stateMachine));
            }
        }
    }
}
