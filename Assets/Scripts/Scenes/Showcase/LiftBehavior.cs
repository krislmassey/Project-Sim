using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CAVS.Scenes.Showcase
{

    public class LiftBehavior : MonoBehaviour
    {

        enum LiftState
        {
            Raise,
            Lower,
            Idle
        }

        [SerializeField]
        private GameObject objectToLift;

        [SerializeField]
        private Vector3 startingPosition;

        [SerializeField]
        private Vector3 endPosition;

        [SerializeField, Range(0, 1)]
        private float percent;

        private LiftState currentState;

        [SerializeField]
        private InteratibleButtonBehavior liftToggleButton;

        void Start()
        {
            liftToggleButton.Subscribe(ToggleLift);
        }

        // Update is called once per frame
        void Update()
        {
            if (objectToLift == null)
            {
                return;
            }
            objectToLift.transform.position = startingPosition + ((endPosition - startingPosition) * percent);

            switch (currentState)
            {
                case LiftState.Lower:
                    percent = Mathf.Max(0, percent - Time.deltaTime);
                    if (percent == 0)
                    {
                        currentState = LiftState.Idle;
                    }
                    break;

                case LiftState.Raise:
                    percent = Mathf.Min(1, percent + Time.deltaTime);
                    if (percent == 1)
                    {
                        currentState = LiftState.Idle;
                    }
                    break;
            }

        }

        private void ToggleLift()
        {
            switch (currentState)
            {
                case LiftState.Lower:
                    currentState = LiftState.Raise;
                    break;

                case LiftState.Raise:
                    currentState = LiftState.Lower;
                    break;

                case LiftState.Idle:
                    if (percent == 1)
                    {
                        Lower();
                    } else {
                        Raise();
                    }
                    break;
            }
        }

        public void Raise()
        {
            if (percent == 1)
            {
                return;
            }
            currentState = LiftState.Raise;
        }

        public void Lower()
        {
            if (percent == 0)
            {
                return;
            }
            currentState = LiftState.Lower;
        }
    }

}