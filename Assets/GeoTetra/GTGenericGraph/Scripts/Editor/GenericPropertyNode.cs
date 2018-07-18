using System;
using System.Linq;
using UnityEngine;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph;

namespace GeoTetra.GTGenericGraph
{
    [Title("Input", "Property")]
    public class GenericPropertyNode : AbstractGenericNode, IGeneratesGraphLogic, IOnAssetEnabled
    {
        private Guid m_PropertyGuid;

        [SerializeField]
        private string m_PropertyGuidSerialized;

        public const int OutputSlotId = 0;

        public GenericPropertyNode()
        {
            name = "Property";
            UpdateNodeAfterDeserialization();
        }

        public override string DocumentationUrl
        {
            get { return "https://github.com/Unity-Technologies/ShaderGraph/wiki/Property-Node"; }
        }

        private void UpdateNode()
        {
            var graph = owner as AbstractGenericGraph;
            var property = graph.properties.FirstOrDefault(x => x.guid == propertyGuid);
            if (property == null)
                return;

            if (property is Vector1GenericProperty)
            {
                AddSlot(new Vector1GenericSlot(OutputSlotId, property.displayName, "Out", SlotType.Output, 0));
                RemoveSlotsNameNotMatching(new[] {OutputSlotId});
            }
//            else if (property is Vector2ShaderProperty)
//            {
//                AddSlot(new Vector2MaterialSlot(OutputSlotId, property.displayName, "Out", SlotType.Output, Vector4.zero));
//                RemoveSlotsNameNotMatching(new[] {OutputSlotId});
//            }
//            else if (property is Vector3ShaderProperty)
//            {
//                AddSlot(new Vector3MaterialSlot(OutputSlotId, property.displayName, "Out", SlotType.Output, Vector4.zero));
//                RemoveSlotsNameNotMatching(new[] {OutputSlotId});
//            }
//            else if (property is Vector4ShaderProperty)
//            {
//                AddSlot(new Vector4MaterialSlot(OutputSlotId, property.displayName, "Out", SlotType.Output, Vector4.zero));
//                RemoveSlotsNameNotMatching(new[] {OutputSlotId});
//            }
//            else if (property is ColorShaderProperty)
//            {
//                AddSlot(new Vector4MaterialSlot(OutputSlotId, property.displayName, "Out", SlotType.Output, Vector4.zero));
//                RemoveSlotsNameNotMatching(new[] {OutputSlotId});
//            }
//            else if (property is TextureShaderProperty)
//            {
//                AddSlot(new Texture2DMaterialSlot(OutputSlotId, property.displayName, "Out", SlotType.Output));
//                RemoveSlotsNameNotMatching(new[] {OutputSlotId});
//            }
//            else if (property is CubemapShaderProperty)
//            {
//                AddSlot(new CubemapMaterialSlot(OutputSlotId, property.displayName, "Out", SlotType.Output));
//                RemoveSlotsNameNotMatching(new[] { OutputSlotId });
//            }
//            else if (property is BooleanShaderProperty)
//            {
//                AddSlot(new BooleanMaterialSlot(OutputSlotId, property.displayName, "Out", SlotType.Output, false));
//                RemoveSlotsNameNotMatching(new[] { OutputSlotId });
//            }
        }

        public void GenerateNodeLogic(GraphLogicGenerator visitor)
        {

        }

        public Guid propertyGuid
        {
            get { return m_PropertyGuid; }
            set
            {
                if (m_PropertyGuid == value)
                    return;

                var graph = owner as AbstractGenericGraph;
                var property = graph.properties.FirstOrDefault(x => x.guid == value);
                if (property == null)
                    return;
                m_PropertyGuid = value;

                UpdateNode();

                Dirty(ModificationScope.Topological);
            }
        }

        public override string GetVariableNameForSlot(int slotId)
        {
            var graph = owner as AbstractMaterialGraph;
            var property = graph.properties.FirstOrDefault(x => x.guid == propertyGuid);

            if (!(property is TextureShaderProperty) && !(property is CubemapShaderProperty))
                return base.GetVariableNameForSlot(slotId);

            return property.referenceName;
        }

        protected override bool CalculateNodeHasError()
        {
            var graph = owner as AbstractMaterialGraph;

            if (!graph.properties.Any(x => x.guid == propertyGuid))
                return true;

            return false;
        }

        public override void OnBeforeSerialize()
        {
            base.OnBeforeSerialize();
            m_PropertyGuidSerialized = m_PropertyGuid.ToString();
        }

        public override void OnAfterDeserialize()
        {
            base.OnAfterDeserialize();
            if (!string.IsNullOrEmpty(m_PropertyGuidSerialized))
                m_PropertyGuid = new Guid(m_PropertyGuidSerialized);
        }

        public void OnEnable()
        {
            UpdateNode();
        }
    }
}
