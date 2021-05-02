using System;
using System.Collections;
using System.Collections.Generic;

namespace TradesViewer
{
    public class MonthIterator : IEnumerable<DateTime>
    {
        private DateTime _seed;
        private readonly int _number;

        public MonthIterator(DateTime seed, int number = 12)
        {
            _number = number;
            _seed = seed;
        }
        
        public IEnumerator<DateTime> GetEnumerator()
        {
            for (int i = 0; i < _number; i++)
            {
                DateTime currentDateTime = _seed.AddMonths(i);
                yield return currentDateTime;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}