﻿using System;
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

        Task<string> UploadPassport(Guid id, string contentType, Stream image);

        Task<string> UploadSelfie(Guid id, string contentType, Stream image);
    }
}