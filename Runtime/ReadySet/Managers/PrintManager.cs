using System;
using System.Collections.Generic;
using UnityEngine;

namespace RealMethod
{
	public interface IPrint : IIdentifier
	{
		void Update(string Message);
		void Update(Color Color);
		void Update(Vector2 Offcet);
	}

	[Serializable]
	public class LogData : IDraw, IPrint
	{
		private PrintManager MyOwner;
		[SerializeField]
		private string MyMessage;
		private int MySize => MyOwner.PrintSize;
		private float StartTime;
		private Vector2 Offcet = Vector2.zero;
		private Color MyColor = Color.green;

		public bool IsStatic { get; private set; } = false;
		[field: SerializeField]
		public LogType Type { get; private set; }
		public float Duration => IsStatic == false ? GetDuration(Type) : 0;
		public bool IsFinished => IsStatic == false ? !(Time.time - StartTime <= Duration) : false;
		public Color Color => IsStatic == false ? GetColor(Type) : MyColor;

		public LogData(string Message, LogType Type)
		{
			MyMessage = Message;
			this.Type = Type;
		}
		public LogData(Vector2 offcet)
		{
			Offcet = offcet;
			IsStatic = true;
		}

		// Implement IIdentifier Interface
		Name16 IIdentifier.NameID => GetHashCode().ToString();
		// Implement IGUIDrawer Interface
		bool IDraw.Start(IGameManager Manager)
		{
			if (Manager.GetManagerClass() is PrintManager target)
			{
				MyOwner = target;
				StartTime = Time.time;
				return true;
			}
			else
			{
				Debug.LogWarning($"LogData can't created the start manager should be {typeof(PrintManager)}");
				return false;
			}
		}
		bool IDraw.CanDraw()
		{
			if (IsStatic)
				return true;
			return !IsFinished;
		}
		void IDraw.Draw(Vector2 Pivot, int Index)
		{
			int w = Screen.width * MySize;
			int h = Screen.height * MySize;
			float Xpos = Pivot.x + Offcet.x;
			float Ypos = Pivot.y + Offcet.y + (Index * MyOwner.PrintSpace);

			GUIStyle style = new GUIStyle();
			Rect rect = new Rect(Xpos, Ypos, w, h * 2 / 100);
			style.alignment = TextAnchor.UpperLeft;
			style.fontSize = h * 2 / 100;
			style.normal.textColor = Color;
			GUI.Label(rect, MyMessage, style);
		}
		void IDraw.End()
		{

		}
		// Implement IPrint Interface
		void IPrint.Update(string message)
		{
			MyMessage = message;
		}
		void IPrint.Update(Color color)
		{
			MyColor = color;
		}
		void IPrint.Update(Vector2 offcet)
		{
			Offcet = offcet;
		}

		private float GetDuration(LogType type)
		{
			switch (type)
			{
				case LogType.Log:
					return 3;
				case LogType.Warning:
					return 5;
				case LogType.Error:
					return 8;
				case LogType.Assert:
					return 12;
				case LogType.Exception:
					return 15;
				default:
					return 0;
			}
		}
		private Color GetColor(LogType type)
		{
			switch (type)
			{
				case LogType.Log:
					return Color.cyan;
				case LogType.Warning:
					return Color.yellow;
				case LogType.Error:
					return Color.red;
				case LogType.Assert:
					return Color.white;
				case LogType.Exception:
					return Color.blue;
				default:
					return Color.black;
			}
		}


	}


	[AddComponentMenu("RealMethod/Manager/PrintManager")]
	public sealed class PrintManager : GUIManager<LogData>
	{
		[Header("Printer")]
		[SerializeField]
		private float printSpace = 20;
		public float PrintSpace => printSpace;
		[SerializeField]
		private int printSize = 1;
		public int PrintSize => printSize;

		[SerializeField, ReadOnly]
		private List<LogData> StaticData = new List<LogData>(10);


		// GUIManager Methods
		public override void InitiateManager(bool AlwaysLoaded)
		{
		}
		public override void ResolveService(Service service, bool active)
		{
		}

		public void Print(string Message, LogType Type)
		{
			Add(new LogData(Message, Type));
		}
		public IPrint PrintStatic(Vector2 offcet)
		{
			var Result = new LogData(offcet);
			((IDraw)Result).Start(this);
			StaticData.Add(Result);
			return Result;
		}
		public bool RemoveStatic(IPrint ptinter)
		{
			IIdentifier ID = ptinter;
			for (int i = 0; i < StaticData.Count; i++)
			{
				if (StaticData[i] == ID)
				{
					((IDraw)StaticData[i]).End();
					StaticData.RemoveAt(i);
					return true;
				}
			}
			return false;
		}
		public void SetSize(int newSize)
		{
			printSize = newSize;
		}

#if UNITY_EDITOR || DEVELOPMENT_BUILD
		protected override void PreDraw()
		{
			base.PreDraw();
			foreach (var item in StaticData)
			{
				if (item is IDraw Drawer)
				{
					Drawer.Draw(Pivot, 0);
				}
			}

		}
		protected override void PostDraw()
		{
			base.PostDraw();
			for (int i = 0; i < DrawList.Count; i++)
			{
				if (DrawList[i].IsFinished)
				{
					DrawList.Remove(DrawList[i]);
				}
			}
		}
#endif
	}
}

