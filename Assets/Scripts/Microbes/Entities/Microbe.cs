using System.Linq;
using GameBrains.Entities;
using GameBrains.EventSystem;
using Microbes.Movement;
using Microbes.Population_Control;
using UnityEngine;

namespace Microbes.Entities
{
    // TODO for A2 (optional): Clean up this mess. Add better integration with MovingAgent.
    // TODO for A2 (optional): The motor should be moved to Actuators.
    public sealed class Microbe : MovingAgent
    {
        // TODO for A2 (optional): Should sound effects be moved into its own component?
        public AudioClip microbeBirthSound;
        public AudioClip microbeDeathSound;

        // When looking for microbes, don't hit the ground.
        public LayerMask raycastMask;

        public int hungerThreshold = 120;
        int hunger;

        // TODO for A2 (optional): Move motor to actuators.
        MicrobeMotor microbeMotor;
        
        // TODO for A2 (optional): Should attractor and repeller be moved to actuators?
        Attractor attractor;
        Repeller repeller;
        
        LifeSpan lifeSpan;
        BoundsChecker boundsChecker;
        AudioSource audioSource;
        MicrobeTypes foodTypes;
        MicrobeTypes avoidTypes;

        // The type of microbe we are.
        public MicrobeTypes microbeType;

        static readonly MicrobeTypes[] MicrobeTypeArray = 
            { MicrobeTypes.Blue, MicrobeTypes.Red, MicrobeTypes.Green, MicrobeTypes.Yellow };

        public Attractor Attractor
        {
            get => attractor;
            set => attractor = value;
        }

        public Repeller Repeller
        {
            get => repeller;
            set => repeller = value;
        }

        public LifeSpan LifeSpan
        {
            get => lifeSpan;
            set => lifeSpan = value;
        }

        public BoundsChecker BoundsChecker
        {
            get => boundsChecker;
            set => boundsChecker = value;
        }

        public AudioSource AudioSource
        {
            get => audioSource;
            set => audioSource = value;
        }

        // The type of microbes we eat.
        public MicrobeTypes FoodTypes
        {
            get => foodTypes;
            set => foodTypes = value;
        }

        // The type of microbes we avoid.
        public MicrobeTypes AvoidTypes
        {
            get => avoidTypes;
            set => avoidTypes = value;
        }
        
        public MicrobeMotor MicrobeMotor
        {
            get => microbeMotor;
            set => microbeMotor = value;
        }

        public override void Awake()
        {
            base.Awake();

            Hunger = 0;

            MicrobeMotor = gameObject.GetComponent<MicrobeMotor>();

            // make sure we get the Attractor, not the Repeller which is also an Attractor!
            Attractor = gameObject.GetComponents<Attractor>().First<Attractor>(a => !(a is Repeller));

            Repeller = gameObject.GetComponent<Repeller>();
            LifeSpan = gameObject.GetComponent<LifeSpan>();
            BoundsChecker = gameObject.GetComponent<BoundsChecker>();
            AudioSource = gameObject.GetComponent<AudioSource>();
        }

        // TODO for A2 (optional): Hunger uses health bar. Maybe it should have its own indicator.
        // TODO for A2 (optional): Microbes don't take damage so health is not used. Maybe add damage facility. Mines?
        // Gets or sets the microbe's hunger level.
        public int Hunger
        {
            get => hunger;
            set
            {
                hunger = value;
                CurrentHealth = (int)Mathf.Min(100f * value / hungerThreshold, 100);
            }
        }

        // Gets a value indicating whether the microbe is hungry.
        public bool IsHungry => Hunger >= hungerThreshold;

        public override void Start()
        {
            base.Start();

            AudioSource.PlayOneShot(microbeBirthSound);
        }

        public void Die()
        {
            if (IsActive)
            {
                SetInactive();
                AudioSource.PlayOneShot(microbeDeathSound);
                EntityManager.Remove(this);
                
                // TODO for A2 (optional): Add show/hide microbe? Invisibility power-up?
                gameObject.GetComponent<Collider>().isTrigger = true;
                gameObject.GetComponentInChildren<Renderer>().enabled = false;
                var canvasTransform = transform.Find("WeebleCanvas");
                if (canvasTransform != null)
                {
                    canvasTransform.gameObject.SetActive(false);
                }
                
                EventManager.Instance.Fire(Events.Message, $"{name}: I died.");
                Destroy(gameObject, microbeDeathSound.length);
            }
        }

        public static Microbe SpawnRandomTypeAt(GameObject microbePrefab, Vector2 spawnPoint)
        {
            int microbeTypeIndex = Random.Range(0, MicrobeTypeArray.Length);

            return Spawn(microbePrefab, MicrobeTypeArray[microbeTypeIndex], spawnPoint);
        }
        
        // TODO for A2 (optional): Should this be tied to MovingAgent Spawn??
        // TODO for A2: modify the Spawn method to suit your reproductive needs :-)
        // Hint: Besides creating a new microbe of a given type at a given position,
        // you can change its size, food preferences, etc.
        public static Microbe Spawn(GameObject microbePrefab, MicrobeTypes microbeType, Vector2 position)
        {
            if (microbePrefab == null) { return null; }
            
            GameObject microbeObject = Instantiate(microbePrefab, GameObject.Find("Microbes").transform, true);
            microbeObject.SetActive(true);
            microbeObject.transform.position = new Vector3(position.x, 0.5f, position.y);
            microbeObject.GetComponentInChildren<Renderer>().material.color = GetColor(microbeType);

            var microbe = microbeObject.GetComponent<Microbe>();
            microbe.microbeType = microbeType;
            microbe.FoodTypes = GetFoodTypes(microbeType);
            microbe.AvoidTypes = GetAvoidTypes(microbeType);

            // TODO for A2: You may want to change these in certain states.
            microbe.Attractor.AttractTypes = microbe.FoodTypes;
            microbe.Repeller.AttractTypes = microbe.AvoidTypes;

            switch (microbeType)
            {
                case MicrobeTypes.Blue:
                    microbeObject.name = "Blue" + microbe.ID;
                    break;
                case MicrobeTypes.Red:
                    microbeObject.name = "Red" + microbe.ID;
                    break;
                case MicrobeTypes.Green:
                    microbeObject.name = "Green" + microbe.ID;
                    break;
                case MicrobeTypes.Yellow:
                    microbeObject.name = "Yellow" + microbe.ID;
                    break;
                default:
                    microbeObject.name = "Microbe" + microbe.ID;
                    break;
            }

            microbe.ShortName = microbeObject.name;
            microbe.Color = GetColor(microbeType);
            
            EventManager.Instance.Fire(Events.Message, $"{microbe.name}: Happy Birthday.");

            return microbe;
        }

        // TODO for A2: Consider changing these to suit your needs.
        static MicrobeTypes GetFoodTypes(MicrobeTypes microbeType)
        {
            MicrobeTypes foodTypes;

            switch (microbeType)
            {
                case MicrobeTypes.Blue:
                    foodTypes = MicrobeTypes.Red | MicrobeTypes.Green;
                    break;
                case MicrobeTypes.Red:
                    foodTypes = MicrobeTypes.Green | MicrobeTypes.Yellow;
                    break;
                case MicrobeTypes.Green:
                    foodTypes = MicrobeTypes.Yellow | MicrobeTypes.Blue;
                    break;
                case MicrobeTypes.Yellow:
                    foodTypes = MicrobeTypes.Blue | MicrobeTypes.Red;
                    break;
                default:
                    foodTypes = MicrobeTypes.None;
                    break;
            }

            return foodTypes;
        }

        // TODO for A2: Consider changing these to suit your needs.
        static MicrobeTypes GetAvoidTypes(MicrobeTypes microbeType)
        {
            MicrobeTypes avoidTypes;

            switch (microbeType)
            {
                case MicrobeTypes.Blue:
                    avoidTypes = MicrobeTypes.Blue | MicrobeTypes.Green;
                    break;
                case MicrobeTypes.Red:
                    avoidTypes = MicrobeTypes.Red | MicrobeTypes.Yellow;
                    break;
                case MicrobeTypes.Green:
                    avoidTypes = MicrobeTypes.Green | MicrobeTypes.Blue;
                    break;
                case MicrobeTypes.Yellow:
                    avoidTypes = MicrobeTypes.Yellow | MicrobeTypes.Red;
                    break;
                default:
                    avoidTypes = MicrobeTypes.None;
                    break;
            }

            return avoidTypes;
        }
        
        // TODO for A2: Probably need attraction types for dating and mating.
        // TODO for A2: Probably shouldn't eat mate on first contact. Maybe afterward.
        //static MicrobeTypes GetDatingTypes(MicrobeTypes microbeType) { }

        // TODO for A2 (ptional): If you add more microbe types you may need more colors.
        static Color GetColor(MicrobeTypes microbeType)
        {
            switch (microbeType)
            {
                case MicrobeTypes.Blue:
                    return Color.blue;
                case MicrobeTypes.Red:
                    return Color.red;
                case MicrobeTypes.Green:
                    return Color.green;
                case MicrobeTypes.Yellow:
                    return Color.yellow;
                default:
                    return Color.magenta;
            }
        }
    }
}