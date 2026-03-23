using System.Collections.Generic;
using UnityEngine;

namespace RealMethod
{
    public abstract class GUIManager : MonoBehaviour, IGameManager
    {
        [Header("GUI")]
        [SerializeField]
        protected Vector2 Pivot = new Vector2(10, 10);

        //Implement IGameManager Interface
        MonoBehaviour IGameManager.GetManagerClass()
        {
            return this;
        }

        // Abstract Methods
        public abstract void InitiateManager(bool AlwaysLoaded);
        public abstract void ResolveService(Service service, bool active);
    }
    public abstract class GUIManager<T> : GUIManager where T : IDraw
    {
        [SerializeField, ReadOnly]
        protected List<T> DrawList = new List<T>();
        public int Count => DrawList.Count;

        // Functions
        public virtual void Add(T Drawer)
        {
            if (Drawer == null)
            {
                Debug.LogWarning("The Drawer is not valid!");
                return;
            }

            if (Drawer.Start(this))
                DrawList.Add(Drawer);
        }
        public virtual bool Remove(T Drawer)
        {
            Drawer.End();
            return DrawList.Remove(Drawer);
        }
        public virtual void Remove(int Index)
        {
            DrawList[Index].End();
            DrawList.RemoveAt(Index);
        }
        public virtual T Find(IIdentifier ID)
        {
            foreach (var Drawer in DrawList)
            {
                if (Drawer is IIdentifier id)
                {
                    if (id == ID)
                    {
                        return Drawer;
                    }
                }
            }
            return default;
        }
        public virtual T Find(Name16 Name)
        {
            foreach (var Drawer in DrawList)
            {
                if (Drawer.NameID == Name)
                {
                    return Drawer;
                }
            }
            return default;
        }
        public virtual T Find(int index)
        {
            return DrawList[index];
        }
        public virtual bool TryFind(IIdentifier ID, out IDraw Drawer)
        {
            foreach (var drawer in DrawList)
            {
                if (drawer is IIdentifier id)
                {
                    if (id == ID)
                    {
                        Drawer = drawer;
                        return true;
                    }
                }
            }
            Drawer = null;
            return false;
        }
        public virtual void Clear()
        {
            DrawList.Clear();
        }


#if UNITY_EDITOR || DEVELOPMENT_BUILD
        protected virtual void PreDraw()
        {

        }
        protected virtual void DrawItem(T item, int index)
        {
            if (item.CanDraw())
            {
                item.Draw(Pivot, index);
            }
        }
        protected virtual void PostDraw()
        {

        }
        private void OnGUI()
        {
            PreDraw();
            if (DrawList.Count > 0)
            {
                for (int i = 0; i < DrawList.Count; i++)
                {
                    DrawItem(DrawList[i], i);
                }
            }
            PostDraw();
        }
#endif

    }
}

