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

        public ChallengeParticipant(string name, string email)
        {
            Id = Guid.NewGuid();
            Name = name;
            Email = email;
            FirstChallengeDate = DateTimeHelper.GetBrasiliaTime();
            LastChallengeDate = DateTimeHelper.GetBrasiliaTime();
        }

        public void UpdateLastChallengeDate(DateTime newDate)
        {
            LastChallengeDate = newDate;
        }
    }
}
