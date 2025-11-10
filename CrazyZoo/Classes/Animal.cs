using  CrazyZoo.Properties;
using System.Printing;
using System.Windows.Threading;
using static CrazyZoo.ZooViewModel;

namespace CrazyZoo
{
    public abstract class Animal
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }

        public string Description { get; set; }

        public abstract string PreferableFood { get; }

        public int EnclosureId { get; set; }

        public string Type
        {
            get
            {
                return this.GetType().Name;
            }
        }

        public event Action<string>? LogGenerated;

        public DispatcherTimer EatProgressTimer;
        protected int timeBetweenBites = 5;
        protected DateTime lastBiteTime = DateTime.MinValue;
        protected int bitesUntilFull = 5;
        protected int bitesMade = 0;
        public string FoodDropped = String.Empty;

        protected Animal(string name, int age, string description, int enclosureId)
        {
            Name = name;
            Age = age;
            if (!String.IsNullOrEmpty(description))
            {
                Description = description;
            } else
            {
                Description = Describe();
            }
            EnclosureId = enclosureId;
            setEatProgressTimer();
        }

        public virtual string Describe()
        {
            return string.Format(Resource1.describeDefault, Name, Age);
        }

        public abstract string MakeSound();
        public virtual string EatFood(string food)
        {
            string cleanFood = food.Trim().ToLower();
            string cleanPreferableFood = PreferableFood.Trim().ToLower();
            if (cleanFood != cleanPreferableFood) {
                return string.Format(Resource1.ateWithConsequence, cleanFood, RandomConsequence());
            }
            return string.Format(Resource1.ate, cleanFood);
        }

        protected static string RandomConsequence()
        {
            Random random = new Random();
            int number = random.Next(0, 3);
            string consequence;

            switch (number)
            {
                case 0:
                    consequence = Resource1.consequenceDissatisfied;
                    break;
                case 1:
                    consequence = Resource1.consequenceFeelsBad;
                    break;
                default:
                    consequence = Resource1.consequenceWantsMore;
                    break;

            }
            return consequence;
        }
        public abstract void OnAnimalJoinedInSameEnclosure (Animal animal);
        public abstract void OnFoodDropped(string food);

        protected void Log(string log)
        {
            LogGenerated?.Invoke(log);
        }
        protected void setEatProgressTimer()
        {
            EatProgressTimer = new DispatcherTimer();
            EatProgressTimer.Interval = TimeSpan.FromSeconds(1);
            EatProgressTimer.Tick += EatProgressTimer_Tick;
            EatProgressTimer.Start();
        }
        protected void EatProgressTimer_Tick(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(FoodDropped) && (DateTime.Now - lastBiteTime).TotalSeconds > timeBetweenBites)
            {
                string progressBar = String.Empty;
                for (int i = 0; i < bitesUntilFull; i++)
                {
                    if (i <= bitesMade) progressBar += Resource1.progressBarFilledElement;
                    else progressBar += Resource1.progressBarEmptyElement;
                }

                Log(string.Format(Resource1.isEating, FoodDropped, progressBar));

                bitesMade += 1;
                if (bitesMade == bitesUntilFull)
                {
                    bitesMade = 0;
                    FoodDropped = String.Empty;
                }
                lastBiteTime = DateTime.Now;
            }
        }
    }
}

