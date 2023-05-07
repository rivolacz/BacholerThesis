using Project.StateMachines.States;
using Project.Units;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

namespace Project.StateMachines
{
    public class AttackState : BaseState
    {
        private Transform attackTarget;
        private UnitStats unitStats;
        private float offsetY;
        private LayerMask enemyLayerMask;

        public AttackState(Transform target, StateMachine stateMachine) : base(stateMachine)
        {
            attackTarget = target;
            this.stateMachine = stateMachine;
            unitStats = stateMachine?.unitStats;
        }

        public AttackState(Transform target, float offsetY, StateMachine stateMachine) : base(stateMachine)
        {
            attackTarget = target;
            this.stateMachine = stateMachine;
            unitStats = stateMachine?.unitStats;
            this.offsetY = offsetY;
        }

        public override void StateUpdate() 
        {
            if(attackTarget == null) {
                Debug.Log("Attack target is null");
                var enemyTransform = EnemyFinder.GetEnemyTransform(stateMachine.EnemyLayerMask, stateMachine.transform.position,10);
                if (enemyTransform != null)
                {
                    Debug.Log("Found " + enemyTransform.name);
                    AttackState attackState = new AttackState(enemyTransform, stateMachine);
                    stateMachine.ChangeState(attackState);
                }
                else
                {
                    Debug.Log("NO enemy found");
                    stateMachine.ChangeState(new IdleState(stateMachine));
                }
            }
            MoveToTarget();
        }

        private void MoveToTarget()
        {
            Vector2 directionToTarget = GetDirectionToTarget();
            if (offsetY > 0)
            {
                if (directionToTarget.magnitude < 10 && directionToTarget.x < 1)
                {
                    UnitIsCloseToTarget();
                    return;
                }
            }
            else
            {
                if (directionToTarget.magnitude < 10)
                {
                    UnitIsCloseToTarget();
                    return;
                }
            }
            stateMachine.MoveUnit(directionToTarget);
        }

        private void UnitIsCloseToTarget()
        {
            if (stateMachine == null) return;
            Vector2 directionToCollider = GetDirectionToCollider();
            if (directionToCollider.magnitude < unitStats.AttackRange)
            {
                stateMachine.unitAnimatorValuesSetter.SetAttackTrigger();
                if (directionToCollider.magnitude < unitStats.AttackRange /2)
                {
                    stateMachine.Look(directionToCollider);
                    return;
                }
            }
            stateMachine.MoveUnit(directionToCollider, true);
        }

        private Vector3 GetClosestPointOnCollider()
        {
            if(attackTarget == null) return stateMachine.transform.position;
            Collider2D collider = attackTarget.GetComponent<Collider2D>();
            Vector3 closestPointOnCollider = collider.bounds.ClosestPoint(stateMachine.transform.position);
            Debug.DrawLine(stateMachine.transform.position, closestPointOnCollider, UnityEngine.Color.green,0.1f);
            return closestPointOnCollider;
        }

        private Vector2 GetDirectionToTarget()
        {
            if (attackTarget == null) return Vector2.zero;
            Vector3 attackPosition = attackTarget.position;
            if(offsetY != 0)
            {
                attackPosition.y += offsetY;
            }
            Vector2 direction = attackPosition - stateMachine.transform.position;
            return direction;
        }

        private Vector2 GetDirectionToCollider()
        {
            Vector3 closestPointOnCollider = GetClosestPointOnCollider();
            if(offsetY != 0)
            {
                closestPointOnCollider.y += offsetY;
            }
            Vector2 direction = closestPointOnCollider - stateMachine.transform.position;
            return direction;
        }

    }
}
