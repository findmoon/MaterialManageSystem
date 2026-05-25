using System;
using System.Threading.Tasks;

namespace MaterialManageSystem.Core.Interfaces;

public interface IWarningDetectionService
{
    Task DetectAndCreateWarningsAsync();
}
