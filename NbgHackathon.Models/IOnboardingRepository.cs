using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbgHackathon.Models
{
    public interface IOnboardingRepository
    {
        Task<OnboardingState> Get(Guid id);

        Task<OnboardingState> GetOrCreate(string email);

        Task<OnboardingState> Update(OnboardingState state);

        Task Delete(OnboardingState state);

        Task<IList<OnboardingState>> GetAll(int itemsPerPage = 50);
    }
}