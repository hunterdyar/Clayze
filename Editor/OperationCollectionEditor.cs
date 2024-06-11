using System.Collections.Generic;
using System.Linq;
using Connection;
using Marching.Operations;
using UnityEditor;
using UnityEditor.Graphs;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Packages.Clayze.Editor
{
	[CustomEditor(typeof(OperationCollection))]
	public class OperationCollectionEditor : UnityEditor.Editor
	{
		public override VisualElement CreateInspectorGUI()
		{
			 var opCol = (target as OperationCollection);
			 
			 VisualElement root = new VisualElement();

			 var connectionLabel = new Label($"Connection Status: {opCol.ConnectionStatus}");
			 opCol.OnConnectionStatusChanged += status =>
			 {
				 connectionLabel.text = $"Connection Status: {opCol.ConnectionStatus}";
			 };
			 root.Add(connectionLabel);
			 var connnStatusProp = serializedObject.FindProperty("ConnectionStatus");
			 // var connStatusElement = new PropertyField(connnStatusProp);
			 // connStatusElement.SetEnabled(false);
			 // root.Add(connStatusElement);

			 var localStatusProp = serializedObject.FindProperty("localStatus");
			 var localStatusElement = new PropertyField(localStatusProp);
			 localStatusElement.SetEnabled(false);
			 root.Add(localStatusElement);

			 var reconDelayProp = serializedObject.FindProperty("reconnectionDelay");
			 var reconDelayElement = new PropertyField(reconDelayProp);
			 root.Add(reconDelayElement);

			 var connectionButtons = new VisualElement();
			 var reconnectButton = new Button();
			 reconnectButton.text = "Reconnect";
			 reconnectButton.SetEnabled(opCol.ConnectionStatus == ConnectionStatus.Connected);
			 reconnectButton.clicked += () =>
			 {
				 if (opCol.ConnectionStatus != ConnectionStatus.Connected)
				 {
					 opCol.InitAndConnect();
				 }
			 };
			 connectionButtons.Add(reconnectButton);
			 root.Add(connectionButtons);

			 
			 root.Add(new Label("Connection Settings"));
			 var serverURLProp = serializedObject.FindProperty("connectionURL");
			 var serverURLElement= new PropertyField(serverURLProp);
			 root.Add(serverURLElement);
			 
			 if (opCol.recentConnectionURLs.Count > 0)
			 {
				 var recents = opCol.recentConnectionURLs.Select(StringUtility.ConvertSlashToUnicodeSlash).ToList();
				 var urlPresets = new DropdownField("Server Presets", recents, 0, (s) =>
				 {
					 s = StringUtility.ConvertUnicodeSlashToSlash(s);
					 opCol.connectionURL = s;
					 return s;
				 });
				 root.Add(urlPresets);
			 }

			 var opsLabel = new Label("Operations");
			 opsLabel.style.paddingTop = new StyleLength(20);
			 root.Add(opsLabel);
			 var opsProp = serializedObject.FindProperty("_operations");
			 var opsElement = new PropertyField(opsProp);
			 opsElement.SetEnabled(false);
			 root.Add(opsElement);
			 return root;
		}
	}
}