using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CAVS.Scenes.Showcase
{

    public interface IInteractibleButtonSubscriber
    {

        void OnButtonPress(string buttonName);
        
    }

}