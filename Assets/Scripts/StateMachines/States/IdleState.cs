using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.StateMachines
{
    public class IdleState : BaseState
    {
        public IdleState(StateMachine stateMachine) : base(stateMachine) 
        {
        }

        public override void Enter()
        {
            stateMachine.MoveUnit(Vector2.zero);
        }

        public override void StateUpdate()
        {
            Transform target = EnemyFinder.GetEnemyTransform(stateMachine.EnemyLayerMask, stateMachine.transform.position, 10);
            if(target != null)
            {
                stateMachine.ChangeState(new AttackState(target, stateMachine));
            }
            stateMachine.MoveUnit(Vector2.zero);
        }
    }
}
