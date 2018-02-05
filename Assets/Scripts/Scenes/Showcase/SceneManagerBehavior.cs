using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CAVS.LexusShowcase;

namespace CAVS.Scenes.Showcase
{

    /// <summary>
    /// Meant managing the entire carshowcase scene.
    /// </summary>
    public class SceneManagerBehavior : MonoBehaviour, ICarDatabaseSubscriber, IInteractibleButtonSubscriber
    {

        /// <summary>
        /// The database we're pulling car data from.
        /// </summary>
        [SerializeField]
        private CarDatabase carDatabase;

        /// <summary>
        /// The screens we're going to render the current car to
        /// </summary>
        [SerializeField]
        private RawImage[] carImageScreens;

        /// <summary>
        /// All text displays that will list the car's name currently being displayed
        /// </summary>
        [SerializeField]
        private Text[] carNameDisplays;


        [SerializeField]
        private GameObject extraInfoContent;


        [SerializeField]
        private GameObject extraInfoContentEntry;


        [SerializeField]
        private InteratibleButtonBehavior[] buttons;

      
        /// <summary>
        /// Images associated with the car we want to display. (Grabbed from the database)
        /// </summary>
        private Dictionary<Car, Texture> carImages;

        
        /// <summary>
        /// All the cars we're going to display information about. (Grabbed from the database)
        /// </summary>
        private Car[] cars;


        /// <summary>
        /// The current 3d model in the scene which is the car that players can interact with in
        /// different ways
        /// </summary>
        private GameObject currentCarGameObject; 


        /// <summary>
        /// The quality we want to render the cars at, set in accordance to the device that is running
        /// the showcase.
        /// </summary>
        private CarQuality qualityToRender;


        void Start()
        {
            if (buttons != null)
            {
                foreach(InteratibleButtonBehavior button in buttons)
                {
                    button.Subscribe(this);
                }
            }
            if(carDatabase != null)
            {
                carDatabase.Subscribe(this);
            }
            else
            {
                Debug.LogWarning("Unable to subscribe to database because it's null! Check if it's been set in the inspector");
            }
        }


        /// <summary>
        /// Called by buttons placed in the scene
        /// </summary>
        public void OnButtonPress(string buttonName)
        {
            if(buttonName == "Next")
            {
                this.DisplayNextCar();
            }

            if (buttonName == "Previous")
            {
                this.DisplayPreviousCar();
            }
        }

        /// <summary>
        /// Called whenever the Database has car information for us
        /// </summary>
        /// <param name="cars"></param>
        /// <param name="carImages"></param>
        public void OnCarDatabseLoaded(Car[] cars, Dictionary<Car, Texture> carImages)
        {
            this.carImages = carImages;
            this.cars = cars;
            this.DisplayNextCar();
        }


        private int carBeingDisplayedIndex = -1;

        public void DisplayPreviousCar()
        {
            // Make sure we even have cars...
            if (cars == null)
            {
                return;
            }

            // Increment Index
            carBeingDisplayedIndex = Mathf.Clamp(carBeingDisplayedIndex - 1, 0, this.cars.Length);

            DisplayCar(cars[carBeingDisplayedIndex]);
        }

        
        public void DisplayNextCar()
        {
            // Make sure we even have cars...
            if(cars == null)
            {
                return;
            }
            
            // Increment Index
            carBeingDisplayedIndex = Mathf.Clamp(carBeingDisplayedIndex + 1, 0, this.cars.Length);

            DisplayCar(cars[carBeingDisplayedIndex]);
        }


        /// <summary>
        /// Set up the entire scene to be rendering information about the specific
        /// car passed in.
        /// </summary>
        /// <param name="carToDisplay">Car to display info about</param>
        private void DisplayCar(Car carToDisplay)
        {
            // Update All The Screens
            if (carImageScreens != null)
            {
                foreach (RawImage screen in carImageScreens)
                {
                    screen.texture = carImages[carToDisplay];
                }
            }

            // Update all name displays
            if (carNameDisplays != null)
            {
                foreach (Text nameDisplay in carNameDisplays)
                {
                    nameDisplay.text = carToDisplay.ToString();
                }
            }

            // Delete all past car information..
            for (int i = extraInfoContent.transform.childCount - 1; i >= 0; i--)
            {
                Transform child = extraInfoContent.transform.GetChild(i);
                child.SetParent(null);
                Destroy(child.gameObject);
            }

            // Add extra information about car..
            foreach (KeyValuePair<string, string> entry in carToDisplay.getAllExtraData())
            {
                GameObject uiEntry = Instantiate<GameObject>(extraInfoContentEntry, extraInfoContent.transform);
                uiEntry.transform.Find("DataName").GetComponent<Text>().text = entry.Key;
                uiEntry.transform.Find("Value").GetComponent<Text>().text = entry.Value;
            }

            // Delete old car model
            if(currentCarGameObject != null)
            {
                Destroy(currentCarGameObject);
            }

            // Display Car
            currentCarGameObject = Instantiate<GameObject>(
                LoadCarModelReference(carToDisplay, qualityToRender), 
                Vector3.up, 
                Quaternion.identity
            );

        }


        /// <summary>
        /// This will change the quality in which you render the car models.
        /// Meant for rendering on different devices, such as Hololens, which
        /// can't keep up with higher quality models.
        /// </summary>
        /// <param name="newQuality"></param>
        public void SetQuality(CarQuality newQuality)
        {
            // Don't do anything if we're not changing shit
            if (newQuality == qualityToRender)
            {
                return;
            }

            // Update what we're currently rendering.
            this.qualityToRender = newQuality;
            DisplayCar(cars[carBeingDisplayedIndex]);
        }


        /// <summary>
        /// Loads a reference to a car model from the Resources folder to be instantiated and displayed in the scene
        /// 
        /// TODO: ACTUALLY IMPLEMENT THIS
        /// </summary>
        /// <returns>A reference of a car to be instantiated</returns>
        private GameObject LoadCarModelReference(Car car, CarQuality quality)
        {
            switch(quality)
            {
                case CarQuality.Lowest:
                    return Resources.Load<GameObject>("Low Quality Car");
                case CarQuality.Medium:
                    return Resources.Load<GameObject>("Low Medium Car");
                case CarQuality.Highest:
                    return Resources.Load<GameObject>("Low Highest Car");
                default:
                    return Resources.Load<GameObject>("Low Highest Car");
            }

        }

    }

}