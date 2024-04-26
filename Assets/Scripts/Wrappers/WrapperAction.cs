using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScriptablesObject
{
    public class WrapperAction : ScriptableObject
    {
        public event Action action;
        public void Call() => action?.Invoke();
    }

    public class WrapperAction<T> : ScriptableObject
    {
        public event Action<T> action;
        public void Call(T t) => action?.Invoke(t);
    }

}