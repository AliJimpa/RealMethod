using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;

namespace RealMethod
{
	[Serializable]
	public class PrintData
	{

		[SerializeField] string message;
		[SerializeField] float duration;
		[SerializeField] float x, y;
		[SerializeField] int size;
		[SerializeField] Color TextColor;
		[SerializeField] bool console;
		[SerializeField] LogType messagetype;

		public PrintData(string Message)
		{
			message = Message;
			duration = 2f;
			x = -1;
			y = -1;
			size = 1;
			TextColor = Color.black;
			console = true;
			messagetype = LogType.Log;
			PrintOnConsole();
		}

		public PrintData(string Message, LogType type)
		{
			message = Message;
			duration = 2f;
			x = -1;
			y = -1;
			size = 1;
			TextColor = Color.black;
			console = true;
			messagetype = type;
			PrintOnConsole();
		}
		public PrintData(string Message, bool Console)
		{
			message = Message;
			duration = 2f;
			x = -1;
			y = -1;
			size = 1;
			TextColor = Color.black;
			console = Console;
			messagetype = LogType.Log;
			PrintOnConsole();
		}

		public PrintData(string Message, float Duration)
		{
			message = Message;
			duration = Duration;
			x = -1;
			y = -1;
			size = 1;
			TextColor = Color.black;
			console = true;
			messagetype = LogType.Log;
			PrintOnConsole();
		}
		public PrintData(string Message, float Duration, LogType type)
		{
			message = Message;
			duration = Duration;
			x = -1;
			y = -1;
			size = 1;
			TextColor = Color.black;
			console = true;
			messagetype = type;
			PrintOnConsole();
		}
		public PrintData(string Message, float Duration, bool Console)
		{
			message = Message;
			duration = Duration;
			x = -1;
			y = -1;
			size = 1;
			TextColor = Color.black;
			console = Console;
			messagetype = LogType.Exception;
			PrintOnConsole();
		}

		public PrintData(string Message, float Duration, Color TargetColor)
		{
			message = Message;
			duration = Duration;
			x = -1;
			y = -1;
			size = 1;
			TextColor = TargetColor;
			console = false;
			messagetype = LogType.Log;
			PrintOnConsole();
		}

		public PrintData(string Message, Color TargetColor)
		{
			message = Message;
			duration = 2f;
			x = -1;
			y = -1;
			size = 1;
			TextColor = TargetColor;
			console = false;
			messagetype = LogType.Log;
			PrintOnConsole();
		}

		public PrintData(string Message, float Duration, float X, float Y)
		{
			message = Message;
			duration = Duration;
			x = X;
			y = Y;
			size = 1;
			TextColor = Color.black;
			console = true;
			messagetype = LogType.Log;
			PrintOnConsole();
		}
		public PrintData(string Message, float Duration, float X, float Y, Color TargetColor)
		{
			message = Message;
			duration = Duration;
			x = X;
			y = Y;
			size = 1;
			TextColor = TargetColor;
			console = false;
			messagetype = LogType.Log;
			PrintOnConsole();
		}

		public PrintData(string Message, float X, float Y)
		{
			message = Message;
			duration = 2f;
			x = X;
			y = Y;
			size = 1;
			TextColor = Color.black;
			console = true;
			messagetype = LogType.Log;
			PrintOnConsole();
		}

		public PrintData(string Message, float Duration, float X, float Y, int Size)
		{
			message = Message;
			duration = Duration;
			x = X;
			y = Y;
			size = Size;
			TextColor = Color.black;
			console = true;
			messagetype = LogType.Log;
			PrintOnConsole();
		}

		public PrintData(string Message, float X, float Y, int Size, bool Console, LogType type)
		{
			message = Message;
			duration = 2f;
			x = X;
			y = Y;
			size = Size > 0 ? Size : 1;
			TextColor = Color.black;
			console = Console;
			messagetype = type;
			PrintOnConsole();
		}

		public PrintData(string Message, float Duration, float X, float Y, int Size, Color TargetColor)
		{
			message = Message;
			duration = Duration;
			x = X;
			y = Y;
			size = Size > 0 ? Size : 1;
			TextColor = TargetColor;
			console = true;
			messagetype = LogType.Log;
			PrintOnConsole();
		}
		public PrintData(string Message, float Duration, float X, float Y, int Size, bool Console, LogType type)
		{
			message = Message;
			duration = Duration;
			x = X;
			y = Y;
			size = Size > 0 ? Size : 1;
			TextColor = Color.black;
			console = Console;
			messagetype = type;
			PrintOnConsole();
		}



		public void UpdateMessage(string newmessage)
		{
			message = newmessage;
			PrintOnConsole();
		}

		public string GetMessageText()
		{
			return message;
		}

		public float GetDuration()
		{
			return duration;
		}

		public Vector2 GetPosition()
		{
			return new Vector2(x, y);
		}

		public int GetSize()
		{
			return size;
		}

		public Color GetTextColor()
		{
			return TextColor; ;
		}

		private void PrintOnConsole()
		{
			if (console)
			{
				switch (messagetype)
				{
					case LogType.Log:
						TextColor = Color.black;
						Debug.Log(message);
						break;
					case LogType.Warning:
						TextColor = Color.yellow;
						Debug.LogWarning(message);
						break;
					case LogType.Error:
						TextColor = Color.red;
						Debug.LogError(message);
						break;
					case LogType.Assert:
						TextColor = Color.white;
						Debug.LogAssertion(message);
						break;
					case LogType.Exception:
						TextColor = Color.blue;
						System.Exception myexeption = new System.Exception(message);
						Debug.LogException(myexeption);
						break;
				}
			}
		}

	}

	[Serializable]
	public class ButtonData
	{
		[SerializeField] string Name = "Button";
		[SerializeField] string Description = "This is a tooltip for the button";

		[SerializeField] object TargetObject;
		[SerializeField] string MethodName;
		bool IsFunctionMethod = false;
		[SerializeField] object[] MethodParametr;

		public ButtonData(string name, string description, object targetObject, string methodName, object[] methodParametr)
		{
			Name = name;
			Description = description;
			TargetObject = targetObject;
			MethodName = methodName;
			IsFunctionMethod = true;
			MethodParametr = methodParametr;
		}

		public ButtonData(string name, string description, object targetObject, string methodName)
		{
			Name = name;
			Description = description;
			TargetObject = targetObject;
			MethodName = methodName;
		}

		public ButtonData(string name, object targetObject, string methodName)
		{
			Name = name;
			TargetObject = targetObject;
			MethodName = methodName;
		}

		public bool ButtonPressed()
		{
			bool Result;
			if (IsFunctionMethod)
			{
				Type type = TargetObject.GetType();
				MethodInfo method = type.GetMethod(MethodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

				if (method != null)
				{
					method.Invoke(TargetObject, MethodParametr);
					Result = true;
				}
				else
				{
					Debug.LogWarning("Method " + MethodName + " not found on " + TargetObject);
					Result = false;
				}
			}
			else
			{
				// Get the type of the target object
				Type type = TargetObject.GetType();

				// Find the method with the specified name
				MethodInfo method = type.GetMethod(MethodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

				if (method != null)
				{
					// Invoke the method on the target object
					method.Invoke(TargetObject, null);
					Result = true;
				}
				else
				{
					Debug.LogWarning("Method " + MethodName + " not found on " + TargetObject);
					Result = false;
				}
			}

			return Result;
		}

		public string GetName()
		{
			return Name;
		}

		public string GetDescription()
		{
			return Description;
		}

	}

	public abstract class DebugManager : MonoBehaviour, IGameManager
	{
		[Header("Print")]
		[SerializeField]
		private Vector2 DefualtPrintPosition = new Vector2(10, 10);
		public float PrintSpace = 2;

		[Header("Button")]
		[SerializeField]
		private Vector2 ScrollPosition = Vector2.zero;
		[SerializeField]
		private Vector2 ButtonSize = new Vector2(200, 40);
		[SerializeField]
		private float ButtonMargin = 20;



		[Header("Advance")]
		[SerializeField]
		[ReadOnly]
		private List<PrintData> Printers = new List<PrintData>();
		[SerializeField]
		[ReadOnly]
		private List<ButtonData> Buttons = new List<ButtonData>();


		//IGameManager Interface
		public MonoBehaviour GetManagerClass()
		{
			return this;
		}
		public void InitiateManager(bool AlwaysLoaded)
		{
		}
		public void InitiateService(Service service)
		{

		}


		private void OnEnable()
		{
			Game.Instance.OnWorldUpdated += WorldUpdated;
		}
		private void OnDisable()
		{
			if (Game.IsValid())
				Game.Instance.OnWorldUpdated -= WorldUpdated;
		}
		private void WorldUpdated(World Target)
		{
			Printers.Clear();
			Buttons.Clear();
		}



		public void Log(PrintData DataTarget)
		{
			if (DataTarget.GetDuration() > 0)
			{
				StartCoroutine(RemovePrint(DataTarget));
				Printers.Add(DataTarget);
			}
			else
			{
				if (DataTarget.GetDuration() == 0)
					Printers.Add(DataTarget);
			}
		}
		public void Button(ButtonData buttonData)
		{
			Buttons.Add(buttonData);
		}
		public Vector2 GetDefualtPrintPosition()
		{
			return DefualtPrintPosition;
		}






		// GUI Methods
		private void CreateGUILabel(PrintData Data, int CurrentIndex)
		{
			int w = (Screen.width * Data.GetSize());
			int h = (Screen.height * Data.GetSize());
			float Xpos = Data.GetPosition().x > 0 ? Data.GetPosition().x : DefualtPrintPosition.x;
			float Ypos = Data.GetPosition().y > 0 ? Data.GetPosition().y : DefualtPrintPosition.y + (CurrentIndex * PrintSpace);

			GUIStyle style = new GUIStyle();
			Rect rect = new Rect(Xpos, Ypos, w, h * 2 / 100);
			style.alignment = TextAnchor.UpperLeft;
			style.fontSize = h * 2 / 100;
			style.normal.textColor = Data.GetTextColor();
			GUI.Label(rect, Data.GetMessageText(), style);
		}
		private void CreateGUIBuuton()
		{

			RectOffset padding = GUI.skin.button.padding;
			RectOffset margin = GUI.skin.button.margin;

			// TODO: The height calculation should be done more correctly.
			Rect viewRect = new Rect(0, 0, ButtonSize.x,
				((ButtonSize.y + (padding.vertical + margin.vertical)) * Buttons.Count) - ButtonSize.y);


			ScrollPosition = GUI.BeginScrollView(
				position: new Rect(Screen.width - ButtonSize.x - ButtonMargin, 10, ButtonSize.x + ButtonMargin, Screen.height - 10),
				scrollPosition: ScrollPosition,
				viewRect: viewRect,
				alwaysShowHorizontal: false,
				alwaysShowVertical: false
			);

			for (int i = 0; i < Buttons.Count; i++)
			{
				if (GUI.Button(new Rect(0, 50 * i, ButtonSize.x, ButtonSize.y), Buttons[i].GetName()))
				{
					Buttons[i].ButtonPressed();
				}
			}
			GUI.EndScrollView();
		}
		/// Unity Methods
		void OnGUI()
		{

			if (Buttons.Count > 0)
			{
				CreateGUIBuuton();
			}

			for (int i = 0; i < Printers.Count; i++)
			{
				CreateGUILabel(Printers[i], i);
			}


		}

		///Enumerators
		private IEnumerator RemovePrint(PrintData Target)
		{
			yield return new WaitForSeconds(Target.GetDuration());
			Printers.Remove(Target);
		}


	}

}

