using System;
using System.Reflection;
using UnityEngine;

namespace RealMethod
{
    public abstract class MemberBinding
    {
        [Header("Member Info")]
        [SerializeField]
        private bool UseRefrence = false;
        [SerializeField, ConditionalHide("UseRefrence", true, true)]
        private string gameObjectName;
        [SerializeField, ConditionalHide("UseRefrence", true, false)]
        private GameObject Ref;
        [SerializeField]
        private SoftType<Component> componentType;
        [SerializeField, Slug]
        private string memberName;
        public GameObject SelectedGameObject
        {
            get
            {
                if (UseRefrence)
                {
                    return Ref;
                }

                if (cachedObject == null)
                {
                    cachedObject = FindGameObject();
                }
                return cachedObject;
            }
        }
        public Component SelectedComponent
        {
            get
            {
                if (cachedComponent == null)
                {
                    cachedComponent = FindComponent(SelectedGameObject);
                }
                return cachedComponent;
            }
        }
        public MemberInfo SelectedMember
        {
            get
            {
                if (cachedMember == null)
                {
                    cachedMember = FindMemeber(componentType.Type, memberName);
                }
                return cachedMember;
            }
        }

        private GameObject cachedObject;
        private Component cachedComponent;
        private MemberInfo cachedMember;


        protected virtual GameObject FindGameObject()
        {
            var Target = GameObject.Find(gameObjectName);
            if (Target == null)
            {
                Debug.LogWarning($"GameObject '{gameObjectName}' not found.");
            }
            return null;
        }
        protected virtual Component FindComponent(GameObject target)
        {
            var comp = target.GetComponent(componentType.Type);
            if (comp == null)
            {
                Debug.LogWarning($"Component type '{componentType}' not found.");
            }
            return comp;
        }
        protected virtual MemberInfo FindMemeber(Type type, string memberName)
        {
            MemberInfo Info = type.GetField(memberName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            if (Info == null)
            {
                Info = type.GetProperty(memberName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            }

            return Info;
        }
    }


    [Serializable]
    public class MemberVariable<T> : MemberBinding where T : struct
    {
        public T GetValue()
        {
            if (SelectedMember is FieldInfo FInfo)
            {
                return (T)FInfo.GetValue(SelectedComponent);
            }
            else if (SelectedMember is PropertyInfo PInfo)
            {
                return (T)PInfo.GetValue(SelectedComponent);
            }
            else
            {
                Debug.LogWarning($"Member '{SelectedMember.Name}' not found on component type '{SelectedComponent.GetType()}'.");
                return default;
            }
        }
    }
    [Serializable]
    public class MemberMethod : MemberBinding
    {
        // MemberBinding Methods
        protected override MemberInfo FindMemeber(Type type, string memberName)
        {
            MethodInfo method = type.GetMethod(memberName,
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            if (method == null)
            {
                Debug.LogWarning($"Method '{memberName}' not found on '{type}'.");
                return null;
            }

            return method;
        }

        // Functions
        public void Invoke(params object[] arguments)
        {
            if (SelectedMember is MethodInfo Info)
            {
                if (arguments == null)
                {
                    Info.Invoke(SelectedComponent, null);
                }
                else
                {
                    if (arguments.Length > 0)
                    {
                        Info.Invoke(SelectedComponent, arguments);
                    }
                }
            }
            else
            {
                Debug.LogWarning($"Member '{SelectedMember}' should be [MethodInfo] for Invoking.");
            }
        }

    }




}