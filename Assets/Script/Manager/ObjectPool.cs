using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
        : Singleton<ObjectPool>
{
    private class PrefabPool
    {
        private GameObject          prefab;
        private Queue<GameObject>   pool = new Queue<GameObject>();

        public PrefabPool(GameObject _Prefab, ObjectPool _PoolManager)
        {
            prefab      = _Prefab;
        }

        public GameObject GetObject()
        {
            GameObject obj = (0 < pool.Count)
                ? (pool.Dequeue())
                : (CreatNewObject());

            if (null == obj)
            {
                Debug.Log("Err Pool Err");
                return null;
            }

            obj.transform.SetParent(null);
            obj.SetActive(false);

            return obj;
        }

        public void ReturnObject(GameObject _Obj)
        {
            if (!_Obj)
                return;

            _Obj.SetActive(false);
            _Obj.transform.parent = null;
            pool.Enqueue(_Obj);
        }

        public void ReturnObject(GameObject _Obj, float _DelayTime)
        {
            if (!_Obj)
                return;

            ObjectPool.Instance.StartCoroutine(DelayedAction(_Obj, _DelayTime));
        }

        IEnumerator DelayedAction(GameObject _Obj, float _DelayTime)
        {
            yield return new WaitForSeconds(_DelayTime);

            ReturnObject(_Obj);
        }


        private GameObject CreatNewObject()
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            obj.transform.position = Vector3.zero;
            obj.transform.rotation = Quaternion.identity;

            return obj;
        }
    }

    private Dictionary<string, PrefabPool> PoolList = new Dictionary<string, PrefabPool>();

    public GameObject GetObject(GameObject _Prefab, bool _Active)
    {
        if (!_Prefab)
            return null;

        GameObject obj = (Instance.PoolList.ContainsKey(_Prefab.name))
            ? (Instance.PoolList[_Prefab.name].GetObject())
            : (Instance.CreatNewPool(_Prefab).GetObject());

        if (!obj)
            return null;

        obj.transform.SetParent(null);
        obj.SetActive(_Active);

        return obj;
    }

    public void ReturnObject(string _PrefanTag, GameObject _Obj)
    {
        if (null == _Obj)
            return;

        if (!Instance.PoolList.ContainsKey(_PrefanTag))
            CreatNewPool(_Obj);

        Instance.PoolList[_PrefanTag].ReturnObject(_Obj);   
    }

    public void ReturnObject(string _PrefanTag, GameObject _Obj, float _Delay)
    {
        if (null == _Obj)
            return;

        if (!Instance.PoolList.ContainsKey(_PrefanTag))
            CreatNewPool(_Obj);

        Instance.PoolList[_PrefanTag].ReturnObject(_Obj, _Delay);
    }

    private PrefabPool CreatNewPool(GameObject _Prefab)
    {
        PrefabPool NewPool = new PrefabPool(_Prefab, Instance);
        PoolList.Add(_Prefab.name, NewPool);

        return NewPool;
    }
}
