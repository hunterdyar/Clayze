using SyncedProperty;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Packages.Clayze.Editor
{
	[CustomEditor(typeof(SyncableBase))]
	public class SyncableBaseEditor : UnityEditor.Editor
	{
		private SyncableBase obj;
		public override VisualElement CreateInspectorGUI()
		{
			obj = (target as SyncableBase);
			var root = new VisualElement();
			var idProp = serializedObject.FindProperty("ID");
			var idElement = new PropertyField(idProp);
			idElement.SetEnabled(false);
			root.Add(idElement);

			return root;
		}
	}
}