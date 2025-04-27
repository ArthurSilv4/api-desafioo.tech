using api_desafioo.tech.Helpers;

namespace api_desafioo.tech.Models
{
    public class ChallengeParticipant
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Email { get; private set; }

        public DateTime FirstChallengeDate { get; private set; }
        public DateTime LastChallengeDate { get; private set; }

        public ICollection<Challenge> Challenges { get; private set; }
        public Guid ChallengeId { get; private set; }

        public ChallengeParticipant(string name, string email, Guid challengeId)
        {
            Id = Guid.NewGuid();
            Name = name;
            Email = email;
            FirstChallengeDate = DateTime.UtcNow;
            LastChallengeDate = DateTime.UtcNow;
            Challenges = new List<Challenge>();
            ChallengeId = challengeId;
        }

        public void UpdateName(string newName)
        {
            Name = newName;
        }
        public void UpdateLastChallengeDate(DateTime newDate)
        {
            LastChallengeDate = newDate;
        }
    }
}
