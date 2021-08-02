using Monefy_WPF.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using System.IO;

namespace Monefy_WPF.Service
{
    class JsonFileService : IFileService
    {
        public void Save(string fileName, List<Data> data)
        {
            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(List<Data>));
            using (FileStream fs = new FileStream(fileName, FileMode.Create))
            {
                jsonSerializer.WriteObject(fs, data);
            }
        }

        List<Data> IFileService.Open(string fileName)
        {
            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(List<Data>));
            try
            {
                using (FileStream fs = new FileStream(fileName, FileMode.Open))
                {
                    return (List<Data>)jsonSerializer.ReadObject(fs);
                }
            }
            catch (Exception)
            {
                return new List<Data>();
            }
        }
    }
}
