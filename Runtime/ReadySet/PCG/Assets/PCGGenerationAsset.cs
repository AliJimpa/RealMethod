using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace RealMethod
{
    [Serializable]
    public struct PCGData
    {
        public string CodeName;
        public int SourceID { get; private set; }
        public int PrefabIndex { get; private set; }
        public int DataIndex { get; private set; }
        public Vector3 Position;
        public Vector3 Rotation;
        public Vector3 Scale;

        public PCGData(int sourceindex, int prefabindex, int dataindex)
        {
            SourceID = sourceindex;
            PrefabIndex = prefabindex;
            DataIndex = dataindex;
            CodeName = $"{dataindex} {sourceindex}-{prefabindex}";
            Position = Vector3.zero;
            Rotation = Vector3.zero;
            Scale = Vector3.one;
        }

        public PCGSourceLayer GetLayer(PCGResourceAsset Resource)
        {
            return Resource.GetSource(SourceID).Layer;
        }

    }
    [Serializable]
    public struct PCGOrder
    {
        public string Command;
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

        //SortVariable
        private List<int> background = new List<int>();
        private List<int> middleground = new List<int>();
        private List<int> foreground = new List<int>();

        private List<PCGData> Result = new List<PCGData>();
        private PCGResourceAsset MyResource;


        public List<PCGData> GetFullProcess(PCGResourceAsset resource)
        {
            Result.Clear();
            PreProcess(resource);
            BackgroundProcess();
            MiddlegroundProcess();
            ForegroundProcess();
            PostProcess();
            return Result;
        }
        public List<PCGData> GetResult()
        {
            return Result;
        }
        public void PreProcess(PCGResourceAsset resource)
        {
            MyResource = resource;
            Stage_Sort();
            Stage_BackgroundGenerat();
            Stage_MiddlegroundGenerat();
            Stage_ForegroundGenerat();
        }
        public void BackgroundProcess()
        {
            for (int i = 0; i < Result.Count; i++) // Iterate using index to modify the list directly
            {
                if (Result[i].GetLayer(MyResource) == PCGSourceLayer.Background)
                {
                    PCGData tempData = Result[i];
                    if (Stage_BackgroundProcess(ref tempData))
                    {
                        Result[i] = tempData;
                        return;
                    }
                }
            }
        }
        public void MiddlegroundProcess()
        {
            for (int i = 0; i < Result.Count; i++) // Iterate using index to modify the list directly
            {
                if (Result[i].GetLayer(MyResource) == PCGSourceLayer.Middleground)
                {
                    PCGData tempData = Result[i];
                    if (Stage_MiddlegroundProcess(ref tempData))
                    {
                        Result[i] = tempData;
                        return;
                    }
                }
            }
        }
        public void ForegroundProcess()
        {
            for (int i = 0; i < Result.Count; i++) // Iterate using index to modify the list directly
            {
                if (Result[i].GetLayer(MyResource) == PCGSourceLayer.Foreground)
                {
                    PCGData tempData = Result[i];
                    if (Stage_ForegroundProcess(ref tempData))
                    {
                        Result[i] = tempData;
                        return;
                    }
                }
            }
        }
        public void PostProcess()
        {
            for (int i = 0; i < Result.Count; i++) // Iterate using index to modify the list directly
            {
                PCGData tempData = Result[i];
                if (Stage_PostProcess(ref tempData))
                {
                    Result[i] = tempData;
                    return;
                }
            }
        }


        protected PCGSource GetSource(PCGSourceLayer layer, int index)
        {
            switch (layer)
            {
                case PCGSourceLayer.Background:
                    return MyResource.GetSource(background[index]);
                case PCGSourceLayer.Middleground:
                    return MyResource.GetSource(middleground[index]);
                case PCGSourceLayer.Foreground:
                    return MyResource.GetSource(foreground[index]);
                default:
                    Debug.LogError("PCG Cant find your layer");
                    return new PCGSource();
            }
        }



        private void Stage_Sort()
        {
            for (int i = 0; i < MyResource.GetLength(); i++)
            {
                foreach (var TLabel in LabelFilter)
                {
                    if (MyResource.GetSource(i).Label == TLabel)
                    {
                        return;
                    }
                }

                switch (MyResource.GetSource(i).Layer)
                {
                    case PCGSourceLayer.Background:
                        background.Add(i);
                        break;
                    case PCGSourceLayer.Middleground:
                        middleground.Add(i);
                        break;
                    case PCGSourceLayer.Foreground:
                        foreground.Add(i);
                        break;
                }
            }
        }
        private void Stage_BackgroundGenerat()
        {
            if (Generat_BG == 0)
                return;

            foreach (var sourceIndex in background)
            {
                PCGSource sourceTarget = MyResource.GetSource(sourceIndex);

                for (int i = 0; i < sourceTarget.Count; i++)
                {
                    if (i < (sourceTarget.Count * (Generat_BG / 100)))
                    {
                        Result.Add(new PCGData(sourceIndex, i, Result.Count));
                    }
                }
            }
        }
        private void Stage_MiddlegroundGenerat()
        {
            if (Generat_MG == 0)
                return;

            foreach (var sourceIndex in middleground)
            {
                PCGSource sourceTarget = MyResource.GetSource(sourceIndex);

                for (int i = 0; i < sourceTarget.Count; i++)
                {
                    if (i < (sourceTarget.Count * (Generat_MG / 100)))
                    {
                        Result.Add(new PCGData(sourceIndex, i, Result.Count));
                    }
                }
            }
        }
        private void Stage_ForegroundGenerat()
        {
            if (Generat_FG == 0)
                return;

            foreach (var sourceIndex in foreground)
            {
                PCGSource sourceTarget = MyResource.GetSource(sourceIndex);

                for (int i = 0; i < sourceTarget.Count; i++)
                {
                    if (i < (sourceTarget.Count * (Generat_FG / 100)))
                    {
                        Result.Add(new PCGData(sourceIndex, i, Result.Count));
                    }
                }
            }
        }

        private bool Stage_BackgroundProcess(ref PCGData Data)
        {
            int CommandResult = 0;
            PCGCommand MyCommand = null;
            foreach (var order in Background)
            {
                if (order.Command != null)
                {
                    Type CommandType = Type.GetType(order.Command);
                    if (CommandType == null)
                    {
                        Debug.LogError($"Command type '{order.Command}' not found.");
                        return false;
                    }
                    MyCommand = (PCGCommand)Activator.CreateInstance(CommandType);
                    CommandResult = (int)MyCommand.Process(ref Data, order.StringParam, order.FloatParam, order.VectorParam);
                }
                if (CommandResult == 2)
                {
                    Debug.LogWarning($"{this.name}> The Command({MyCommand} is Faield)");
                }
                if (CommandResult == 3)
                {
                    return false;
                }
            }
            return true;
        }
        private bool Stage_MiddlegroundProcess(ref PCGData Data)
        {
            int CommandResult = 0;
            PCGCommand MyCommand = null;
            foreach (var Oerder in Middleground)
            {
                if (Oerder.Command != null)
                {
                    Type CommandType = Type.GetType(Oerder.Command);
                    if (CommandType == null)
                    {
                        Debug.LogError($"Command type '{Oerder.Command}' not found.");
                        return false;
                    }
                    MyCommand = (PCGCommand)Activator.CreateInstance(CommandType);
                    CommandResult = (int)MyCommand.Process(ref Data, Oerder.StringParam, Oerder.FloatParam, Oerder.VectorParam);
                }
                if (CommandResult == 2)
                {
                    Debug.LogWarning($"{this.name}> The Command({MyCommand} is Faield)");
                }
                if (CommandResult == 3)
                {
                    return false;
                }
            }
            return true;
        }
        private bool Stage_ForegroundProcess(ref PCGData Data)
        {
            int CommandResult = 0;
            PCGCommand MyCommand = null;
            foreach (var Oerder in Foreground)
            {
                if (Oerder.Command != null)
                {
                    Type CommandType = Type.GetType(Oerder.Command);
                    if (CommandType == null)
                    {
                        Debug.LogError($"Command type '{Oerder.Command}' not found.");
                        return false;
                    }
                    MyCommand = (PCGCommand)Activator.CreateInstance(CommandType);
                    CommandResult = (int)MyCommand.Process(ref Data, Oerder.StringParam, Oerder.FloatParam, Oerder.VectorParam);
                }
                if (CommandResult == 2)
                {
                    Debug.LogWarning($"{this.name}> The Command({MyCommand} is Faield)");
                }
                if (CommandResult == 3)
                {
                    return false;
                }
            }
            return true;
        }
        private bool Stage_PostProcess(ref PCGData Data)
        {
            int CommandResult = 0;
            PCGCommand MyCommand = null;
            foreach (var Oerder in FullItem)
            {
                if (Oerder.Command != null)
                {
                    Type CommandType = Type.GetType(Oerder.Command);
                    if (CommandType == null)
                    {
                        Debug.LogError($"Command type '{Oerder.Command}' not found.");
                        return false;
                    }
                    MyCommand = (PCGCommand)Activator.CreateInstance(CommandType);
                    CommandResult = (int)MyCommand.Process(ref Data, Oerder.StringParam, Oerder.FloatParam, Oerder.VectorParam);
                }
                if (CommandResult == 1)
                {
                    Debug.LogWarning($"{this.name}> The Command({MyCommand} is Faield)");
                }
                if (CommandResult == 2)
                {
                    return false;
                }
            }
            return true;
        }
    }



    [CreateAssetMenu(fileName = "PCG_Generation", menuName = "RealMethod/PCG/Generation", order = 2)]
    public class PCGGenerationAsset : PCGBaseGenerationAsset
    {
        [Header("Advance")]
        [SerializeField, TextArea]
        private string Guide;

        private void OnValidate()
        {
            // Get all available MonoBehaviour types **only once**
            List<Type> componentTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => typeof(PCGCommand).IsAssignableFrom(type) && !type.IsAbstract)
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