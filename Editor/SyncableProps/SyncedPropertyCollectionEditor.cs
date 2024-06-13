using SyncedProperty;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Packages.Clayze.Editor
{
	[CustomEditor(typeof(SyncedPropertyCollection))]
	public class SyncedPropertyCollectionEditor : UnityEditor.Editor
	{
		private SyncedPropertyCollection _collection;

		public override VisualElement CreateInspectorGUI()
		{
			_collection = target as SyncedPropertyCollection;
			var root = new VisualElement();
			root.Add(new Label("Connection Settings"));
			//socket settings
			var socketSettingsProp = serializedObject.FindProperty("_socketSettings");
			var socketSettingsElement = new PropertyField(socketSettingsProp);
			root.Add(socketSettingsElement);


			//socket settings
			var valuesProp = serializedObject.FindProperty("_values");
			var valuesElement = new PropertyField(valuesProp);
			root.Add(valuesElement);

			return root;
		}
		
	}
}