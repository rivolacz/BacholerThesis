using Project.StateMachines.States;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project
{
    public class Builder : Units.PlayerUnit
    {
        private BuildingState buildingState;
        public void BuildBuilding(ConstructionSite building, Vector3 position)
        {
            Debug.Log("Builder starting build");
            buildingState = new BuildingState(building, position, StateMachine);
            StateMachine.ChangeState(new MoveState(position,StateMachine, 0f, buildingState));
        }
    }
}
