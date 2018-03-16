﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbgHackathon.Domain
{
    public interface IPassportParser
    {
        Task<string> SubmitPassport(string passportImageUri);
        Task<PassportInformation> GetPassportParseResult(string jobId);
    }
}