using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbgHackathon.Models
{
    public class OnboardingRepository : IOnboardingRepository
    {

        public OnboardingRepository(string storageConnectionString)
        {

        }

        public Task Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<OnboardingState> Get(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IList<OnboardingState>> GetAll(int page = 1, int itemsPerPage = 50)
        {
            throw new NotImplementedException();
        }

        public Task<OnboardingState> Update(OnboardingState state)
        {
            throw new NotImplementedException();
        }
    }
}
