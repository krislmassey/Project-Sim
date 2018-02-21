using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace CAVS.Scenes.Showcase
{

    public class InteratibleButtonBehavior : MonoBehaviour
    {

        private List<Action> subscribers;

        [SerializeField]
        [Range(0.1f, 5f)]
        private float minimumProximityForAnimation;

        [SerializeField]
        private GameObject[] controllers;

        [SerializeField]
        private GameObject buttonPiece;

        [SerializeField]
        private GameObject proximityPiece;

        public void Subscribe(Action sub)
        {
            if(sub != null)
            {
                if (subscribers == null)
                {
                    subscribers = new List<Action>();
                }
                subscribers.Add(sub);
            }
        }


        void OnTriggerEnter(Collider other)
        {
            proximityPiece.GetComponent<MeshRenderer>().material.color = Color.green;
            GetComponent<BoxCollider>().size = new Vector3(0.6f, 3f, 0.6f);
            buttonPiece.transform.Translate(Vector3.up / 15f);
            if (subscribers != null)
            {
                foreach(Action sub in subscribers)
                {
                    if(sub != null)
                    {
                        sub();
                    }
                }
            }
        }

        void OnTriggerExit(Collider other)
        {
            GetComponent<BoxCollider>().size = new Vector3(0.6f, 2f, 0.6f);
            buttonPiece.transform.Translate(Vector3.down / 15f);
            proximityPiece.GetComponent<MeshRenderer>().material.color = Color.blue;
        }


        // Update is called once per frame
        void Update()
        {

            if (controllers == null) 
            {
                return;
            }

            GameObject closest = null;
            float closestDistance = float.MaxValue;
            foreach(GameObject controller in controllers)
            {
                if (controller == null)
                {
                    continue;
                }
                float dist = Vector3.Distance(controller.transform.position, this.buttonPiece.transform.position);
                if(dist < closestDistance){
                    closestDistance = dist;
                    closest = controller;
                }
            }

            Vector3 newProximityScale;
            if (closestDistance <= minimumProximityForAnimation)
            {
                float scale = 1.1f - (closestDistance / minimumProximityForAnimation);
                newProximityScale = new Vector3(scale, 1f, scale);
            }
            else
            {
                newProximityScale = Vector3.zero;
            }
            proximityPiece.transform.localScale = newProximityScale;
        }

     
    }

}