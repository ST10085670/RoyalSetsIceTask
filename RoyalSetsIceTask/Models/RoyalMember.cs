namespace RoyalSetsIceTask.Models
{
    public class RoyalMember
    {
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public bool IsAlive { get; set; }

        public RoyalMember(string name, DateTime dob, bool alive = true)
        {
            Name = name;
            DateOfBirth = dob;
            IsAlive = alive;
        }

        public RoyalMember() { } // For model binding

        public override string ToString()
        {
            return $"{Name} ({DateOfBirth:yyyy-MM-dd}) {(IsAlive ? "" : "†")}";
        }
    }
}
