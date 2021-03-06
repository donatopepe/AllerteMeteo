using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeteoAlert.Services
{
    public interface IAppVersionService
    {
        string Version { get; }
        string MachineName { get; }
    }
}
