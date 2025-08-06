using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Builder
{
    public Builder(GameObject _Prefab)
    {
        prefab          = _Prefab;
        subject         = ObjectPool.Instance.GetObject(prefab, false);
        subject.name    = _Prefab.name;
    }

    public Builder Set_Parent(Transform _Parent)
    {
        subject.transform.parent = _Parent;

        return this;
    }

    public Builder Set_Position(Vector3 _Pos)
    {
        subject.transform.position = _Pos;

        return this;
    }

    public Builder Set_Scale(Vector3 _Siz)
    {
        subject.transform.localScale = _Siz;

        return this;
    }

    public Builder Set_Rotation(Quaternion _Rot)
    {
        subject.transform.rotation = _Rot;

        return this;
    }

    public Builder Set_EulerRotation(float _X, float _Y, float _Z)
    {
        subject.transform.rotation =  Quaternion.Euler(_X, _Y, _Z);

        return this;
    }

    public Builder Set_DeleteTime(float _DeleteTime)
    {
        ObjectPool.Instance.ReturnObject(prefab.name, subject, _DeleteTime);

        return this;
    }

    public GameObject Bulild()
    {
        subject.SetActive(true);

        return subject;
    }

    GameObject      prefab;
    GameObject      subject;
}


/*class ItemBuiderBase
{
    public ItemBuiderBase(GameObject _Prefab)
    {
        prefab = _Prefab;
        subject = ObjectPool.Instance.GetObject(prefab, false);
        subject.name = _Prefab.name;
    }

    protected ItemBuiderBase()
    {

    }

    public GameObject Bulild()
    {
        subject.SetActive(true);

        return subject;
    }

    public DefaultBuider Default()
    {
        return new DefaultBuider(subject);
    }

    public DeleteBuider Delete()
    {
        return new DeleteBuider(subject);
    }

    protected GameObject    prefab;
    protected GameObject    subject;
}

class DefaultBuider : ItemBuiderBase
{
    public DefaultBuider(GameObject _Subject)
    {
        subject = _Subject;
    }

    public DefaultBuider Set_Parent(Transform _Parent)
    {
        subject.transform.parent = _Parent;

        return this;
    }

    public DefaultBuider Set_Position(Vector3 _Pos)
    {
        subject.transform.position = _Pos;

        return this;
    }

    public DefaultBuider Set_Scale(Vector3 _Siz)
    {
        subject.transform.localScale = _Siz;

        return this;
    }

    public DefaultBuider Set_Rotation(Quaternion _Rot)
    {
        subject.transform.rotation = _Rot;

        return this;
    }

    public DefaultBuider Set_EulerRotation(float _X, float _Y, float _Z)
    {
        subject.transform.rotation =  Quaternion.Euler(_X, _Y, _Z);

        return this;
    }
}

class DeleteBuider : ItemBuiderBase
{
    public DeleteBuider(GameObject _Subject)
    {
        subject = _Subject;
    }

    public DeleteBuider Set_DeleteTime(float _DeleteTime)
    {
        ObjectPool.Instance.ReturnObject(subject.name, subject, _DeleteTime);

        return this;
    }
}
*/