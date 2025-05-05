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
        public int SourceID { get; private set; }
        public int Index { get; private set; }
        public Vector3 Position;
        public Vector3 Rotation;
        public Vector3 Scale;

        public PCGData(int sourceindex, int index)
        {
            SourceID = sourceindex;
            Index = index;
            CodeName = sourceindex + "-" + index;
            Position = Vector3.zero;
            Rotation = Vector3.zero;
            Scale = Vector3.one;
        }
    }

    public enum PCGCommandResult
    {
        Success = 0,
        Failed = 1,
        Break = 3
    }

    public abstract class PCGCommand
    {
        private PCGGeneration Owner;
        protected void Print(string message, bool iserror)
        {
            if (iserror)
            {
                Debug.LogError($"{Owner.name}>{this.GetType().Name}: {message}");
            }
            else
            {
                Debug.LogWarning($"{Owner.name}>{this.GetType().Name}: {message}");
            }
        }
        public void Initialize(PCGGeneration owner)
        {
            Owner = owner;
        }

        public abstract void Initialized();
        public abstract PCGCommandResult Process(ref PCGData Context, string Labelparam, float Radiusparam, Vector3 Locationparam);
    }

    [Serializable]
    public struct PCGOrder
    {
        public string ModuleName;
        public string LabelParam;
        public float Radiusparam;
        public Vector3 Locationparam;
    }



    public abstract class PCGGeneration : DataAsset
    {
        [Header("Generation")]
        [SerializeField]
        private bool BackgroundStage = true;
        [SerializeField, Range(0, 100)]
        private float backgroundLimit = 100;
        [SerializeField]
        private bool MiddlegroundStage = true;
        [SerializeField, Range(0, 100)]
        private float middlegroundLimit = 100;
        [SerializeField]
        private bool ForegroundStage = true;
        [SerializeField, Range(0, 100)]
        private float foregroundLimit = 100;


        private List<int> background = new List<int>();
        private List<int> middleground = new List<int>();
        private List<int> foreground = new List<int>();
        private List<PCGData> Result = new List<PCGData>();
        private PCGResource MyResource;


        public void Generate(PCGResource resource)
        {
            MyResource = resource;
            Stage_Sort();
            PreProcess();
            if (BackgroundStage)
            {
                Stage_Background();
            }
            if (MiddlegroundStage)
            {
                Stage_Middleground();
            }
            if (ForegroundStage)
            {
                Stage_Foreground();
            }
            PostProcess();
        }
        public List<PCGData> GetResult()
        {
            return Result;
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
        private void Stage_Background()
        {
            if (backgroundLimit == 0)
                return;

            foreach (var sourceIndex in background)
            {
                PCGSource sourceTarget = MyResource.GetSource(sourceIndex);

                for (int i = 0; i < sourceTarget.Count; i++)
                {
                    if (i < (sourceTarget.Count * (backgroundLimit / 100)))
                    {
                        PCGData NewData = new PCGData(sourceIndex, i);
                        if (Background(ref NewData))
                        {
                            Result.Add(NewData);
                        }
                    }
                }
            }
        }
        private void Stage_Middleground()
        {
            if (middlegroundLimit == 0)
                return;

            foreach (var sourceIndex in middleground)
            {
                PCGSource sourceTarget = MyResource.GetSource(sourceIndex);

                for (int i = 0; i < sourceTarget.Count; i++)
                {
                    if (i < (sourceTarget.Count * (middlegroundLimit / 100)))
                    {
                        PCGData NewData = new PCGData(sourceIndex, i);
                        if (Middleground(ref NewData))
                        {
                            Result.Add(NewData);
                        }
                    }
                }
            }
        }
        private void Stage_Foreground()
        {
            if (foregroundLimit == 0)
                return;

            foreach (var sourceIndex in foreground)
            {
                PCGSource sourceTarget = MyResource.GetSource(sourceIndex);

                for (int i = 0; i < sourceTarget.Count; i++)
                {
                    if (i < (sourceTarget.Count * (foregroundLimit / 100)))
                    {
                        PCGData NewData = new PCGData(sourceIndex, i);
                        if (Foreground(ref NewData))
                        {
                            Result.Add(NewData);
                        }
                    }
                }
            }
        }

        protected abstract void PreProcess();
        protected abstract bool Background(ref PCGData Data);
        protected abstract bool Middleground(ref PCGData Data);
        protected abstract bool Foreground(ref PCGData Data);
        protected abstract void PostProcess();
    }



    [CreateAssetMenu(fileName = "PCG_Process", menuName = "RealMethod/PCG/Process", order = 2)]
    public class PCGProcess : PCGGeneration
    {
        [Header("Layers")]
        [SerializeField]
        private List<PCGOrder> BackgroundCommands = new List<PCGOrder>();
        [SerializeField]
        private List<PCGOrder> MiddlegroundCommands = new List<PCGOrder>();
        [SerializeField]
        private List<PCGOrder> ForegroundCommands = new List<PCGOrder>();
        [Header("Advance")]
        [SerializeField, ShowOnly]
        private string[] Commands;

        private void OnValidate()
        {
            // Get all available MonoBehaviour types **only once**
            List<Type> componentTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => typeof(PCGCommand).IsAssignableFrom(type) && !type.IsAbstract)
                .ToList();

            Commands = componentTypes.Select(t => t.FullName).ToArray();
        }


        protected override void PreProcess()
        {
        }
        protected override bool Background(ref PCGData Data)
        {
            int CommandResult = 0;
            PCGCommand MyCommand = null;
            foreach (var command in BackgroundCommands)
            {
                if (command.ModuleName != null)
                {
                    Type CommandType = Type.GetType(command.ModuleName);
                    if (CommandType == null)
                    {
                        Debug.LogError($"Command type '{command.ModuleName}' not found.");
                        return false;
                    }
                    MyCommand = (PCGCommand)Activator.CreateInstance(CommandType);
                    CommandResult = (int)MyCommand.Process(ref Data, command.LabelParam, command.Radiusparam, command.Locationparam);
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
        protected override bool Middleground(ref PCGData Data)
        {
            int CommandResult = 0;
            PCGCommand MyCommand = null;
            foreach (var command in MiddlegroundCommands)
            {
                if (command.ModuleName != null)
                {
                    Type CommandType = Type.GetType(command.ModuleName);
                    if (CommandType == null)
                    {
                        Debug.LogError($"Command type '{command.ModuleName}' not found.");
                        return false;
                    }
                    MyCommand = (PCGCommand)Activator.CreateInstance(CommandType);
                    CommandResult = (int)MyCommand.Process(ref Data, command.LabelParam, command.Radiusparam, command.Locationparam);
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
        protected override bool Foreground(ref PCGData Data)
        {
            int CommandResult = 0;
            PCGCommand MyCommand = null;
            foreach (var command in ForegroundCommands)
            {
                if (command.ModuleName != null)
                {
                    Type CommandType = Type.GetType(command.ModuleName);
                    if (CommandType == null)
                    {
                        Debug.LogError($"Command type '{command.ModuleName}' not found.");
                        return false;
                    }
                    MyCommand = (PCGCommand)Activator.CreateInstance(CommandType);
                    CommandResult = (int)MyCommand.Process(ref Data, command.LabelParam, command.Radiusparam, command.Locationparam);
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
        protected override void PostProcess()
        {
        }

    }



}