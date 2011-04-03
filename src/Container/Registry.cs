using System;
using System.Collections.Generic;

namespace Container
{
    internal class Registry : IRegistry
    {

        private readonly Dictionary<string, Registration> _mainDictionary = new Dictionary<string, Registration>();


        public void Add(string key, Registration reg)
        {
            if (Contains(key))
                throw new InvalidOperationException("The key " + key + " is already registered!");
            _mainDictionary.Add(key, reg);
        }

        public bool Contains(string key)
        {
            return _mainDictionary.ContainsKey(key);
        }

        public Registration Get(string key)
        {
            return _mainDictionary[key];
        }
    }
}