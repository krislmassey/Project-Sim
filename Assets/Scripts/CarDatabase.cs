using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace CAVS.LexusShowcase
{

    /// <summary>
    /// Incharge of loading the cars from a CSV file when requested.
    /// 
    /// Will eventually need to be moved over into a singleton that builds a Gameobject
    /// upon request.
    /// </summary>
    public class CarDatabase : MonoBehaviour
    {

        [SerializeField]
        private TextAsset carDb;

        private List<ICarDatabaseSubscriber> subscribers;

        private char lineSeperater = '\n';
        private char fieldSeperator = ',';

        private Dictionary<Car, Texture> allCarImages;
        private Car[] allCars = null;

        /// <summary>
        /// If -1, downloading hasn't begun
        /// If  0, downloading has complete
        /// If >0, currently downloading images
        /// </summary>
        int carImageLoadingCount = -1;

        void Awake()
        {
            subscribers = new List<ICarDatabaseSubscriber>();
            allCars = loadCars(carDb);
            if (allCars == null)
            {
                Debug.LogWarning("Unable to find cars in the database. Probably some stupid parsing error");
                return;
            }

        }

        /// <summary>
        /// Any subscriber passed in will be pushed new information whenever the 
        /// database recieves something.
        /// </summary>
        /// <param name="subscriber"></param>
        public void Subscribe(ICarDatabaseSubscriber subscriber)
        {
            if (subscriber != null)
            {
                subscribers.Add(subscriber);

                // If we've already loaded stuff, push latest information to them
                if (carImageLoadingCount == 0)
                {
                    subscriber.OnCarDatabseLoaded(allCars, allCarImages);
                }
            }
            else
            {
                Debug.LogWarning("Attempting to subscribe to database with null subscriber");
            }
        }
        

        /// <summary>
        /// Called each time a car has completed loading.
        /// </summary>
        private void FinishedLoading()
        {
            carImageLoadingCount -= 1;
            if (carImageLoadingCount == 0)
            {
                foreach (ICarDatabaseSubscriber sub in subscribers)
                {
                    if(sub != null)
                    {
                        sub.OnCarDatabseLoaded(allCars, allCarImages);
                    }
                }
            }
        }


        /// <summary>
        /// Attemps to load all Cars listed in a CSV file.
        /// </summary>
        /// <param name="csvFile"></param>
        /// <returns></returns>
        private Car[] loadCars(TextAsset csvFile)
        {
            if (csvFile == null)
            {
                Debug.LogError("CSV File passed in is null!");
                return null;
            }

            string[] records = csvFile.text.Split(lineSeperater);

            // Doesn't contain any cars (1st row is the column names)
            if (records.Length < 2)
            {
                return null;
            }
            
            Car[] cars = new Car[records.Length - 1];
            allCarImages = new Dictionary<Car, Texture>();

            string[] columnNames = records[0].Split(fieldSeperator);

            allCarImages = new Dictionary<Car, Texture>();
            carImageLoadingCount = records.Length-1;

            // Skip the first line because thats column names
            for (int recordIndex = 1; recordIndex < records.Length; recordIndex ++)
            {
                string[] fields = records[recordIndex].Split(fieldSeperator);

                Dictionary<string, string> extraData = new Dictionary<string,string>(); 

                // Build up any extra data....
                for (int columnIndex = 8; columnIndex < records.Length && columnIndex < fields.Length; columnIndex++)
                {
                    extraData.Add(columnNames[columnIndex], fields[columnIndex]);
                }

                cars[recordIndex - 1] = new Car(
                    int.Parse(fields[0]),
                    fields[1],
                    fields[2],
                    int.Parse(fields[3]),
                    fields[4],
                    fields[5],
                    fields[6],
                    fields[7],
                    extraData
                );

                // Try downloading the image for later use
                StartCoroutine(LoadImage(cars[recordIndex-1].getPictureUrl(), recordIndex, cars[recordIndex-1] ));
            }

            return cars;
        }


        /// <summary>
        /// Starts a seperate thread for making a request to the server for downloading textures,
        /// or if already cached, load from the file system
        /// </summary>
        /// <param name="url"></param>
        /// <param name="index"></param>
        /// <param name="car"></param>
        /// <returns></returns>
        private IEnumerator LoadImage(string url, int index, Car car)
		{
            if (!Directory.Exists("cache"))
            {
                Directory.CreateDirectory("cache");
            }

            
            // Get Md5 hash...
            string hashName = CalculateMD5Hash(url);
            
            string[] chunks = url.Split('.');
            string extension = chunks[chunks.Length-1];
            string filePath = Path.Combine( Path.Combine(Directory.GetCurrentDirectory(), "cache"), hashName + "." + extension);

            // Try loading from memory
            if(File.Exists(filePath))
            {
                Texture2D textureFromCache = new Texture2D(1, 1);
                textureFromCache.LoadImage(File.ReadAllBytes(filePath));
                allCarImages.Add(car, textureFromCache);
			} else if(url != "") {
                // Make a request then to download the source
				Debug.Log("URL: (" + url + ")");
				Debug.Log (url == "");
			    WWW www = new WWW(url);
			    yield return www;
            
                // Load these babies
                allCarImages.Add(car, www.texture);

                // Cache the image on the machine.
                if (www.bytes != null)
                {
                    File.WriteAllBytes(filePath, www.bytes);
                }
            }
            
            FinishedLoading();
		}


        /// <summary>
        /// Used for determining name for caching downloaded content to computer
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private  string CalculateMD5Hash(string input)
        {
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            string sb = "";
            for (int i = 0; i < hash.Length; i++)
            {
                sb += hash[i].ToString("X2");
            }

            return sb.ToString();
        }

    }


}