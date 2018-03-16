using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbgHackathon.Domain
{
    public interface IOnboardingRepository
    {
        Task<OnboardingState> Get(Guid id);

        Task<OnboardingState> GetOrCreate(string email, string botSessionId);

        Task<OnboardingState> Update(OnboardingState state);

        Task Delete(OnboardingState state);

        Task<IList<OnboardingState>> GetAll(int itemsPerPage = 50);

        /// <summary>
        /// Uploads the passport image to Blob storage.
        /// </summary>
        /// <param name="id">The onboarding identifier</param>
        /// <param name="contentType">The images contenttype</param>
        /// <param name="image">The image's stream</param>
        /// <returns>The Uri of the uploaded image.</returns>
        Task<string> UploadPassport(Guid id, string contentType, Stream image);

        /// <summary>
        /// Uploads the selfie to Blob storage.
        /// </summary>
        /// <param name="id">The onboarding identifier</param>
        /// <param name="contentType">The images contenttype</param>
        /// <param name="image">The image's stream</param>
        /// <returns>The Uri of the uploaded image.</returns>
        Task<string> UploadSelfie(Guid id, string contentType, Stream image);
    }
}