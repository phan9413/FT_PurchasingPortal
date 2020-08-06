using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace XamarinGRN.Services
{
    public interface IQrScanningService
    {
        Task<string> ScanAsync();
    }
}
