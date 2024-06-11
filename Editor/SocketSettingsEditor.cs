using System.Collections.Generic;
using Connection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Packages.Clayze.Editor
{
	[CustomPropertyDrawer(typeof(SocketSettings))]
	public class SocketSettingsEditor : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			VisualElement root = new VisualElement();
			var serverURLProp = property.FindPropertyRelative("connectionURL");
			var recentConn = property.FindPropertyRelative("recentConnectionURLs");

			var serverURLElement = new PropertyField(serverURLProp);
			root.Add(serverURLElement);

			if (recentConn.arraySize > 0)
			{
				var recents = new List<string>();
				for (int i = 0; i < recentConn.arraySize; i++)
				{
					recents.Add(StringUtility.ConvertSlashToUnicodeSlash(recentConn.GetArrayElementAtIndex(i).stringValue));
				}
				var urlPresets = new DropdownField("Recent Servers", recents, 0, (s) =>
				{
					s = StringUtility.ConvertUnicodeSlashToSlash(s);
					serverURLProp.stringValue = s;
					// serverURLProp.
					property.serializedObject.ApplyModifiedProperties();
					return s;
				});
				root.Add(urlPresets);
			}

			return root;
		}
	}
}