using QPortal.Models;
using System.Collections.Generic;
using System.Collections;

namespace QPortal.ViewModels
{
    public class AmbitiViewModel : IEnumerable<Ambiti>
    {
        public List<Ambiti> AmbitoList { get; set; }

        public IEnumerator<Ambiti> GetEnumerator()
        {
            return AmbitoList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return AmbitoList.GetEnumerator();
        }
    }
}