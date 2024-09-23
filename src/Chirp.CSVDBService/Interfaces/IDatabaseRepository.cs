﻿using Chirp.CSVDBService.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chirp.CSVDBService.Interfaces
{
    interface IDatabaseRepository<T> where T : IPost
    {
        public Task<List<T>> Read(int? count = null);
        public Task Store(T record);
    }
}
