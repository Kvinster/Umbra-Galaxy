using UnityEngine;

using System.Collections.Generic;
using System.Xml;

using STP.Common;
using STP.Utils.Xml;

namespace STP.Core.State {
	public sealed class UpgradesControllerState : BaseState {
		public sealed class Characteristic : IXmlNodeSerializable {
			public PlayerCharacteristicType Type;
			public int                      Level;

			public Characteristic() { }

			public Characteristic(PlayerCharacteristicType type, int level) {
				Type  = type;
				Level = level;
			}

			public void Load(XmlNode node) {
				Type  = node.GetAttrValue("type", PlayerCharacteristicType.Unknown);
				Level = node.GetAttrValue("level", 0);
			}

			public void Save(XmlElement elem) {
				elem.AddAttrValue("type", Type);
			}
		}

		public int                  UpgradePoints;
		public List<Characteristic> Characteristics = new List<Characteristic>();

		public override string Name => "upgrades";

		public override void Load(XmlNode node) {
			UpgradePoints   = node.GetAttrValue("upgrade_points", 0);
			Characteristics = node.LoadNodeList<Characteristic>("characteristics", "characteristic", null);
		}

		public override void Save(XmlElement elem) {
			elem.AddAttrValue("upgrade_points", UpgradePoints);
			elem.SaveNodeList("characteristics", "characteristic", Characteristics);
		}

		public int GetCharacteristicLevel(PlayerCharacteristicType characteristicType) {
			return GetOrCreateCharacteristic(characteristicType).Level;
		}

		public void SetCharacteristicLevel(PlayerCharacteristicType characteristicType, int level) {
			GetOrCreateCharacteristic(characteristicType).Level = level;
		}

		Characteristic GetOrCreateCharacteristic(PlayerCharacteristicType characteristicType) {
			var characteristic = GetCharacteristic(characteristicType, true);
			if ( characteristic == null ) {
				characteristic = new Characteristic(characteristicType, 0);
				Characteristics.Add(characteristic);
			}
			return characteristic;
		}

		Characteristic GetCharacteristic(PlayerCharacteristicType characteristicType, bool silent = false) {
			foreach ( var characteristic in Characteristics ) {
				if ( characteristic.Type == characteristicType ) {
					return characteristic;
				}
			}
			if ( !silent ) {
				Debug.LogErrorFormat(
					"UpgradesControllerState.GetCharacteristic: can't find Characteristic for type '{0}'",
					characteristicType.ToString());
			}
			return null;
		}
	}
}
