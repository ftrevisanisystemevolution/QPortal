using QPortal.Models;
using System.Collections.Generic;
using System.Collections;

namespace QPortal.ViewModels
{
    public class FarmsViewModel : IEnumerable<Farms>
    {
        public List<Farms> FarmList { get; set; }

        public IEnumerator<Farms> GetEnumerator()
        {
            return FarmList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return FarmList.GetEnumerator();
        }
    }
}