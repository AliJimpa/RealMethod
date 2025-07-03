using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RealMethod
{
    [Serializable]
    public struct PCGData
    {
        public string CodeName;
        public int PrefabID { get; private set; }
        public int InstanceCount { get; private set; }
        public int SelfIndex { get; private set; }
        public Vector3 Position;
        public Vector3 Rotation;
        public Vector3 Scale;

        public PCGData(int sourceindex, int instancecount, int dataindex)
        {
            PrefabID = sourceindex;
            InstanceCount = instancecount;
            SelfIndex = dataindex;
            CodeName = $"{dataindex} {sourceindex}-{InstanceCount}";
            Position = Vector3.zero;
            Rotation = Vector3.zero;
            Scale = Vector3.one;
        }

        public PCGSourceLayer GetLayer(PCGResourceAsset Resource)
        {
            return Resource.GetSource(PrefabID).Layer;
        }

    }

    [Serializable]
    public struct PCGOrder
    {
        public string Request;
        public string StringParam;
        public float FloatParam;
        public Vector3 VectorParam;
    }

    public abstract class PCGBaseGenerationAsset : DataAsset
    {
        [Header("PreProcess")]
        [SerializeField, Range(0, 100)]
        private float Generat_BG = 100;
        [SerializeField, Range(0, 100)]
        private float Generat_MG = 100;
        [SerializeField, Range(0, 100)]
        private float Generat_FG = 100;
        [SerializeField]
        private string[] LabelFilter;
        [Header("Process")]
        [SerializeField]
        private List<PCGOrder> Background = new List<PCGOrder>();
        [SerializeField]
        private List<PCGOrder> Middleground = new List<PCGOrder>();
        [SerializeField]
        private List<PCGOrder> Foreground = new List<PCGOrder>();
        [Header("PostProcess")]
        [SerializeField]
        private List<PCGOrder> FullItem = new List<PCGOrder>();

        private PCGResourceAsset MyResource;


        public PCGData[] GetFullProcess(PCGResourceAsset resource)
        {
            PCGData[] Result = PreProcess(resource);
            Process(ref Result);
            PostProcess(ref Result);
            return Result;
        }
        public PCGData[] PreProcess(PCGResourceAsset resource)
        {
            MyResource = resource;
            List<PCGData> Result = new List<PCGData>();
            for (int i = 0; i < MyResource.GetLength(); i++)
            {
                PCGSource source = MyResource.GetSource(i);
                foreach (var TLabel in LabelFilter)
                {
                    if (source.Label == TLabel)
                    {
                        continue;
                    }
                }

                for (int j = 0; j < source.Count; j++)
                {
                    switch (source.Layer)
                    {
                        case PCGSourceLayer.Background:
                            if (i < (source.Count * (Generat_BG / 100)))
                            {
                                Result.Add(new PCGData(i, j, Result.Count));
                            }
                            break;
                        case PCGSourceLayer.Middleground:
                            if (i < (source.Count * (Generat_MG / 100)))
                            {
                                Result.Add(new PCGData(i, j, Result.Count));
                            }
                            break;
                        case PCGSourceLayer.Foreground:
                            if (i < (source.Count * (Generat_FG / 100)))
                            {
                                Result.Add(new PCGData(i, j, Result.Count));
                            }
                            break;
                    }

                }
            }
            return Result.ToArray();
        }
        public void Process(ref PCGData[] DataObjects)
        {
            for (int i = 0; i < DataObjects.Length; i++) // Iterate using index to modify the list directly
            {
                PCGData tempData = DataObjects[i];
                switch (DataObjects[i].GetLayer(MyResource))
                {
                    case PCGSourceLayer.Background:
                        DataObjects[i] = Stage_Background(DataObjects[i]);
                        break;
                    case PCGSourceLayer.Middleground:
                        DataObjects[i] = Stage_Middleground(DataObjects[i]);
                        break;
                    case PCGSourceLayer.Foreground:
                        DataObjects[i] = Stage_Foreground(DataObjects[i]);
                        break;
                }
            }
        }
        public void PostProcess(ref PCGData[] DataObjects)
        {
            for (int i = 0; i < DataObjects.Length; i++) // Iterate using index to modify the list directly
            {
                PCGData tempData = DataObjects[i];
                DataObjects[i] = Stage_PostProcess(DataObjects[i]);
            }
        }



        // Process
        private PCGData Stage_Background(PCGData Data)
        {
            PCGData temporary = Data;
            int RequestResult = 0;
            PCGRequest MyRequest = null;
            foreach (var order in Background)
            {
                if (order.Request != null)
                {
                    Type RequestType = Type.GetType(order.Request);
                    if (RequestType == null)
                    {
                        Debug.LogError($"Request type '{order.Request}' not found.");
                        return Data;
                    }
                    MyRequest = (PCGRequest)Activator.CreateInstance(RequestType);
                    var requestInterface = MyRequest as ICommandInitiator;
                    if (requestInterface != null)
                    {
                        if (!requestInterface.Initiate(this, this))
                        {
                            Debug.LogWarning($"{this.name}> ICommandInitiator.Initiate failed for {MyRequest.GetType().Name}");
                        }
                    }
                    RequestResult = (int)MyRequest.Process(ref temporary, order.StringParam, order.FloatParam, order.VectorParam);
                }
                if (RequestResult == 2)
                {
                    Debug.LogWarning($"{this.name}> The Request({MyRequest} is Faield)");
                }
                if (RequestResult == 3)
                {
                    return Data;
                }
            }
            return temporary;
        }
        private PCGData Stage_Middleground(PCGData Data)
        {
            PCGData temporary = Data;
            int RequestResult = 0;
            PCGRequest MyRequest = null;
            foreach (var Oerder in Middleground)
            {
                if (Oerder.Request != null)
                {
                    Type RequestType = Type.GetType(Oerder.Request);
                    if (RequestType == null)
                    {
                        Debug.LogError($"Request type '{Oerder.Request}' not found.");
                        return Data;
                    }
                    MyRequest = (PCGRequest)Activator.CreateInstance(RequestType);
                    var requestInterface = MyRequest as ICommandInitiator;
                    if (requestInterface != null)
                    {
                        if (!requestInterface.Initiate(this, this))
                        {
                            Debug.LogWarning($"{this.name}> ICommandInitiator.Initiate failed for {MyRequest.GetType().Name}");
                        }
                    }
                    RequestResult = (int)MyRequest.Process(ref temporary, Oerder.StringParam, Oerder.FloatParam, Oerder.VectorParam);
                }
                if (RequestResult == 2)
                {
                    Debug.LogWarning($"{this.name}> The Request({MyRequest} is Faield)");
                }
                if (RequestResult == 3)
                {
                    return Data;
                }
            }
            return temporary;
        }
        private PCGData Stage_Foreground(PCGData Data)
        {
            PCGData temporary = Data;
            int RequestResult = 0;
            PCGRequest MyRequest = null;
            foreach (var Oerder in Foreground)
            {
                if (Oerder.Request != null)
                {
                    Type RequestType = Type.GetType(Oerder.Request);
                    if (RequestType == null)
                    {
                        Debug.LogError($"Request type '{Oerder.Request}' not found.");
                        return Data;
                    }
                    MyRequest = (PCGRequest)Activator.CreateInstance(RequestType);
                    var requestInterface = MyRequest as ICommandInitiator;
                    if (requestInterface != null)
                    {
                        if (!requestInterface.Initiate(this, this))
                        {
                            Debug.LogWarning($"{this.name}> ICommandInitiator.Initiate failed for {MyRequest.GetType().Name}");
                        }
                    }
                    RequestResult = (int)MyRequest.Process(ref temporary, Oerder.StringParam, Oerder.FloatParam, Oerder.VectorParam);
                }
                if (RequestResult == 2)
                {
                    Debug.LogWarning($"{this.name}> The Request({MyRequest} is Faield)");
                }
                if (RequestResult == 3)
                {
                    return Data;
                }
            }
            return temporary;
        }
        private PCGData Stage_PostProcess(PCGData Data)
        {
            PCGData temporary = Data;
            int RequestResult = 0;
            PCGRequest MyRequest = null;
            foreach (var Oerder in FullItem)
            {
                if (Oerder.Request != null)
                {
                    Type RequestType = Type.GetType(Oerder.Request);
                    if (RequestType == null)
                    {
                        Debug.LogError($"Request type '{Oerder.Request}' not found.");
                        return Data;
                    }
                    MyRequest = (PCGRequest)Activator.CreateInstance(RequestType);
                    var requestInterface = MyRequest as ICommandInitiator;
                    if (requestInterface != null)
                    {
                        if (!requestInterface.Initiate(this, this))
                        {
                            Debug.LogWarning($"{this.name}> ICommandInitiator.Initiate failed for {MyRequest.GetType().Name}");
                        }
                    }
                    RequestResult = (int)MyRequest.Process(ref temporary, Oerder.StringParam, Oerder.FloatParam, Oerder.VectorParam);
                }
                if (RequestResult == 1)
                {
                    Debug.LogWarning($"{this.name}> The Request({MyRequest} is Faield)");
                }
                if (RequestResult == 2)
                {
                    return Data;
                }
            }
            return temporary;
        }
    }


    [CreateAssetMenu(fileName = "PCG_Generation", menuName = "RealMethod/PCG/Generation", order = 2)]
    public class PCGGenerationAsset : PCGBaseGenerationAsset
    {
        [Header("Advance")]
        [SerializeField, TextArea]
        private string Guide;

        [ContextMenu("PrintGuide")]
        private void PrintGuide()
        {
            // Get all available MonoBehaviour types **only once**
            List<Type> componentTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => typeof(PCGRequest).IsAssignableFrom(type) && !type.IsAbstract)
                .ToList();

            Guide = "";
            string MyLine;
            for (int i = 0; i < componentTypes.Count; i++)
            {
                MyLine = $"{i + 1}.{componentTypes[i]}";
                Guide += MyLine + Environment.NewLine;
            }
        }

    }



}