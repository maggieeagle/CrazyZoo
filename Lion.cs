using System;

public class Lion
{
    public Lion : Animal
	{
		public Lion(string name, int age) : base(name, age)
        {
        }

        public void MakeSound()
        {
        Console.WriteLine("Arrr!");
        }
    }
}
