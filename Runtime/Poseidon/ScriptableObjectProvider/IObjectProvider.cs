namespace Core.ScriptableObjectProvider
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public interface IObjectProvider<T>
    {
        T GetObject(Predicate<T> condition);
        List<T> GetObjects(Predicate<T> condition);
    }
}