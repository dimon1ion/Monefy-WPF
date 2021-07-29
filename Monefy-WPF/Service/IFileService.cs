using Monefy_WPF.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monefy_WPF.Service
{
    public interface IFileService
    {
        List<Data> Open(string fileName);
        void Save(string fileName, List<Data> actions);
    }
}
