using Monefy_WPF.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monefy_WPF.Service
{
    class JsonFileService : IFileService
    {
        public void Save(string fileName, List<Data> actions)
        {
            throw new NotImplementedException();
        }

        List<Data> IFileService.Open(string fileName)
        {
            return new List<Data>();
        }
    }
}
