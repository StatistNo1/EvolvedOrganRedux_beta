﻿namespace EvolvedOrgansRedux {
	public class PatchOperationAdd_BodyPartAffinity_RequireResearchEnabled : Verse.PatchOperationPathed {
		protected override bool ApplyWorker(System.Xml.XmlDocument xml) {
			bool result = false;
			System.Xml.XmlNode node = this.value.node;
			foreach (object obj in xml.SelectNodes(this.xpath)) {
				result = true;
				if (Settings.RequireResearchProject) {
					System.Xml.XmlNode xmlNode = obj as System.Xml.XmlNode;
					if (this.order == Order.Append) {
						System.Collections.IEnumerator enumerator2 = node.ChildNodes.GetEnumerator();
						while (enumerator2.MoveNext()) {
							object obj2 = enumerator2.Current;
							System.Xml.XmlNode node2 = (System.Xml.XmlNode)obj2;
							xmlNode.AppendChild(xmlNode.OwnerDocument.ImportNode(node2, true));
						}
						continue;
					}
					if (this.order == Order.Prepend) {
						for (int i = node.ChildNodes.Count - 1; i >= 0; i--) {
							xmlNode.PrependChild(xmlNode.OwnerDocument.ImportNode(node.ChildNodes[i], true));
						}
					}
				}
			}
			return result;
		}

		private Verse.XmlContainer value;

		private Order order;

		private enum Order {
			Append,
			Prepend
		}
	}
}
