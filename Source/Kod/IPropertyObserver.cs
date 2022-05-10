﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shtoockie.Kod
{
    public interface IPropertyObserver
    {
        bool IsChanged { get; }

        void Check();
    }
}
