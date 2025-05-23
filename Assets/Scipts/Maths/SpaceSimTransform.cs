using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = System.Object;

/*
    #==============================================================#
	
	
	
	
*/
public class SpaceSimTransform : MonoBehaviour
{    
//--#
    #region Variables

    [field: SerializeField] public Vector3d position {get; private set;}
    [field: SerializeField] public Vector3d localPosition {get; private set;}
    private SpaceSimTransform m_parent;
    [field: SerializeField] public SpaceSimTransform parent {get; private set;}
    [field: SerializeField] public SpaceSimTransform[] children {get; private set;}

    public Dictionary<Type, Object> componentDict {get; private set;} = new();


    #endregion
//--#



//--#
    #region Positioning


    public void SetLocalPosition(Vector3d _position) {
        localPosition = _position;

        if (parent != null)
            position = _position + parent.position;
        else 
            position = _position;

        foreach (SpaceSimTransform child in children) {
            child.SetLocalPosition(child.localPosition);
        }
    }

    public void ToTransform() {
        transform.position = new Vector3((float)position.x, (float)position.y, (float)position.z);
    }
    public void ToTransform(Vector3d offset) {
        transform.position = new Vector3((float)(position.x + offset.x), (float)(position.y + offset.y), (float)(position.z + offset.z));
    }
    public void ToTransform(SpaceSimTransform centre) {
        ToTransform(-centre.position);
    }



    #endregion
//--#



//--#
    #region Parenting


    public void AddChild(SpaceSimTransform _child) {
        if (children.Contains(_child)) return;

        SpaceSimTransform[] newChildren = new SpaceSimTransform[children.Length + 1];
        for (int i = 0; i < children.Length; i++) {
            newChildren[i] = children[i];
        }
        newChildren[newChildren.Length - 1] = _child;
        children = newChildren;

        _child.parent = this;
    }

    public void RemoveChild(SpaceSimTransform _child) {
        if (!children.Contains(_child)) {
            Debug.Log("Child: ("+ _child +") not found in parent");
            return;
        }

        SpaceSimTransform[] newChildren = new SpaceSimTransform[children.Length - 1];
        for (int i = 0, j = 0; i < children.Length; i++, j++) {
            if (children[i] == _child)
                j--;
            else
                newChildren[j] = children[i];
        }
        children = newChildren;

        if (_child != null) _child.parent = null;
    }

    public void Parent(SpaceSimTransform _parent) {
        if (parent != null)
            parent.RemoveChild(this);

        _parent.AddChild(this);
    }


    #endregion
//--#



//--#
    #region Components


    public void AddSimComponent<T>(T obj) {if (!componentDict.ContainsKey(typeof(T))) componentDict.Add(typeof(T), obj);}
    public void RemoveSimComponent<T>() {if (componentDict.ContainsKey(typeof(T))) componentDict.Remove(typeof(T));}
    public void RemoveSimComponent<T>(T obj) {if (componentDict.ContainsKey(typeof(T))) componentDict.Remove(typeof(T));}
    public bool TryGetSimComponent<T>(out T obj) {
        if (componentDict.TryGetValue(typeof(T), out Object _obj)) {
            obj = (T)(object)_obj;
            return true;
        }
        else {
            obj = default;
            return false;
        }
    }


    #endregion
//--#



//--#
    #region misc


#if UNITY_EDITOR
    private void OnValidate() {
        if (parent != m_parent) {
            m_parent?.RemoveChild(this);
            parent?.AddChild(this);
            m_parent = parent;
        }
        if (children != null) {
            foreach (SpaceSimTransform child in children)
                if (child == null || child.parent != this) RemoveChild(child);
        }
    }
#endif


    #endregion
//--#
}
