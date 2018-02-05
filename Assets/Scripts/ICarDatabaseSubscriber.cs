using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CAVS.LexusShowcase
{

    public interface ICarDatabaseSubscriber
    {

        void OnCarDatabseLoaded(Car[] cars, Dictionary<Car, Texture> carImages);

    }

}