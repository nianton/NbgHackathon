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

        Task<OnboardingState> Update(OnboardingState state);

        Task Delete(Guid id);

        Task<IList<OnboardingState>> GetAll(int page = 1, int itemsPerPage = 50);
    }
}