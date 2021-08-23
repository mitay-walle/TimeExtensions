#if UNITY_EDITOR
using UnityEditor.ShortcutManagement;
#endif
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;
using UnityEditor;
using System;

public class TimeExtendor : MonoBehaviour
{
	private static TimeExtendor Inst;
	
	[RuntimeInitializeOnLoadMethod]
	private static void Init()
	{
		var go = new GameObject("timeExtendor");
		go.AddComponent<TimeExtendor>();
		go.hideFlags = HideFlags.DontSave | HideFlags.HideInHierarchy;
		DontDestroyOnLoad(go);
	}

	private void Awake()
	{
		Inst = this;
	}

	void Update()
	{
		#if UNITY_EDITOR
		
		if (Input.GetKeyDown(KeyCode.F5))
		{
			Debug.Break();
		}
		
		if (Input.GetKeyDown(KeyCode.Space) && Input.GetKey(KeyCode.LeftShift))
		{
			TimeExtensions.ToggleGameViewMaximize();
		}
		#endif
	}
}
public class TimeExtensions 
{
#if UNITY_EDITOR

	[Shortcut("Pause Editor",KeyCode.F5)]
	public static void PauseEditor()
	{
		Debug.Break();
	}
	public static void ToggleGameViewMaximize()
	{
		var w = EditorWindow.focusedWindow;
		
		if (w && w.GetType().ToString() == "UnityEditor.GameView")
		{
			w.maximized = !w.maximized;
			TimeExtensions.SetFieldOrPropertyValue("maximizeOnPlay", w, w.maximized);
		}
	}
#endif
	
	public static void Pause()
	{
		Time.timeScale = 0f;
	}

	public static void Unpause()
	{
		Time.timeScale = 1f;
	}


	
	private static bool SetFieldOrPropertyValue(string fieldName, object obj, object value, bool includeAllBases = false, BindingFlags bindings = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
	{
		FieldInfo field = obj.GetType().GetField(fieldName, bindings);
		if (field != null)
		{
			field.SetValue(obj, value);
			return true;
		}

		PropertyInfo property = obj.GetType().GetProperty(fieldName, bindings);
		if (property != null)
		{
			property.SetValue(obj, value, null);
			return true;
		}

		if (includeAllBases)
		{
			foreach (Type type in GetBaseClassesAndInterfaces(obj.GetType()))
			{
				field = type.GetField(fieldName, bindings);
				if (field != null)
				{
					field.SetValue(obj, value);
					return true;
				}

				property = type.GetProperty(fieldName, bindings);
				if (property != null)
				{
					property.SetValue(obj, value, null);
					return true;
				}
			}
		}
		return false;
	}
	
	private static IEnumerable<Type> GetBaseClassesAndInterfaces(Type type, bool includeSelf = false)
	{
		var allTypes = new List<Type>();

		if (includeSelf) allTypes.Add(type);

		if (type.BaseType == typeof(object))
		{
			allTypes.AddRange(type.GetInterfaces());
		}
		else
		{
			allTypes.AddRange(
				Enumerable
					.Repeat(type.BaseType, 1)
					.Concat(type.GetInterfaces())
					.Concat(GetBaseClassesAndInterfaces(type.BaseType))
					.Distinct());
		}

		return allTypes;
	}

}

