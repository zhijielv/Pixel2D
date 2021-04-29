/*
** Created by fengling
** DateTime:    2021-04-29 11:25:00
** Description: TODO 
*/

using HutongGames.PlayMaker;
using UnityEngine;

namespace Rewired.Integration.PlayMaker
{
    public class CustomPlayerAction
    {
        
    }
    
    [ActionCategory("Rewired")]
    [HutongGames.PlayMaker.Tooltip("Gets the axis value of two Actions.")]
    public class RewiredPlayerGetAxis2dCustom : RewiredPlayerActionGetAxis2DFsmStateAction {
        protected override void DoUpdate() {
            UpdateStoreValue(Player.GetAxis2D(actionNameX.Value, actionNameY.Value));
        }

        [HutongGames.PlayMaker.Tooltip("The comparison operation to perform.")]
        public CompareEqual compareEqual = CompareEqual.None;

        [HutongGames.PlayMaker.Tooltip("The value to which to compare the returned value.")]
        public FsmVector2 compareToValue;

        [HutongGames.PlayMaker.Tooltip("Compare using the absolute values of the two operands.")]
        public FsmBool compareAbsValues;

        [HutongGames.PlayMaker.Tooltip("Event to send when the result of comparison returns true.")]
        public FsmEvent isTrueEvent;

        [HutongGames.PlayMaker.Tooltip("Event to send when the result of comparison returns false.")]
        public FsmEvent isFalseEvent;

        [HutongGames.PlayMaker.Tooltip("Event to send when the value changes.")]
        public FsmEvent valueChangedEvent;

        public override void Reset() {
            base.Reset();
            storeValue = Vector2.zero;
            compareEqual = CompareEqual.None;
            compareToValue = Vector2.zero;
            compareAbsValues = false;
            isTrueEvent = null;
            isFalseEvent = null;
            valueChangedEvent = null;
        }

        protected void UpdateStoreValue(FsmVector2 newValue) {
            if (newValue.Value != storeValue.Value) { // value changed
                // Store new value
                storeValue.Value = newValue.Value;
                TrySendEvent(valueChangedEvent); // send value changed event
            }

            // Compare values
            if (compareEqual != CompareEqual.None) {
                bool result = Compare(newValue.Value);
                if (result) {
                    // send true event
                    TrySendEvent(isTrueEvent);
                } else {
                    // send false event
                    TrySendEvent(isFalseEvent);
                }
            }
        }

        private bool Compare(FsmVector2 value) {
            if (compareEqual == CompareEqual.None) return true;

            FsmVector2 val1 = Vector2.zero, val2 = Vector2.zero;
            if (compareAbsValues.Value) {
                Vector2 tmpVec2 = val1.Value;
                tmpVec2.x = Mathf.Abs(value.Value.x);
                tmpVec2.y = Mathf.Abs(value.Value.y);
                val1.Value = tmpVec2;
                
                tmpVec2 = compareToValue.Value;
                tmpVec2.x = Mathf.Abs(compareToValue.Value.x);
                tmpVec2.y = Mathf.Abs(compareToValue.Value.y);
                compareToValue.Value = tmpVec2;
                
            } else {
                val1 = value;
                val2 = compareToValue.Value;
            }

            switch (compareEqual) {
                case CompareEqual.EqualTo:
                    return val1.Value == val2.Value;
                case CompareEqual.NotEqualTo:
                    return val1.Value != val2.Value;
            }

            return false;
        }
    }
}