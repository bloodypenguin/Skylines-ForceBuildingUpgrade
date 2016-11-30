using System;
using System.Reflection;
using ColossalFramework;
using ColossalFramework.Math;
using ColossalFramework.Steamworks;
using ColossalFramework.UI;
using UnityEngine;

namespace ForceLevelUp
{
    public class GamePanelExtender : MonoBehaviour
    {
        private static readonly MethodInfo StartUpgradingMethod = typeof(PrivateBuildingAI).GetMethod("StartUpgrading",
        BindingFlags.NonPublic | BindingFlags.Instance);

        private bool _initialized;
        private ZonedBuildingWorldInfoPanel _zonedBuildingInfoPanel;
        private DistrictWorldInfoPanel _districtWorldInfoPanel;
//        private CityServiceWorldInfoPanel _cityServiceInfoPanel;
        private UIButton _zonedBuildingLevelUpButton1;
        private UIButton _buildingSetOnFireButton1;
//        private UIButton _zonedBuildingLevelUpButton2;
//        private UIButton _buildingSetOnFireButton2;

        private void Update()
        {

            if (!_initialized)
            {
                _zonedBuildingInfoPanel = GameObject.Find("(Library) ZonedBuildingWorldInfoPanel").GetComponent<ZonedBuildingWorldInfoPanel>();
                _zonedBuildingLevelUpButton1 = UIUtil.CreateLevelUpButton(_zonedBuildingInfoPanel.component, BuildingLevelUpHandler);
                _buildingSetOnFireButton1 = UIUtil.CreateSetOnFireButton(_zonedBuildingInfoPanel.component, BuildingSetOnFireHandler);
//                _cityServiceInfoPanel = GameObject.Find("(Library) CityServiceWorldInfoPanel").GetComponent<CityServiceWorldInfoPanel>();
//                _zonedBuildingLevelUpButton2 = UIUtil.CreateLevelUpButton(_cityServiceInfoPanel.component, BuildingLevelUpHandler);
//                _buildingSetOnFireButton2 = UIUtil.CreateSetOnFireButton(_cityServiceInfoPanel.component, BuildingSetOnFireHandler);
                _districtWorldInfoPanel = GameObject.Find("(Library) DistrictWorldInfoPanel").GetComponent<DistrictWorldInfoPanel>();
                UIUtil.CreateLevelUpButton(_districtWorldInfoPanel.component, DistrictLevelUpHandler);
                UIUtil.CreateSetOnFireButton(_districtWorldInfoPanel.component, DistrictSetOnFireHandler);
                _initialized = true;
            }
            if (!_zonedBuildingInfoPanel.component.isVisible)
            {
                return;
            }
            SetZonedBuildingButtonsVisibility();
//            SetServiceBuildingButtonsVisibility();
        }

        private void SetZonedBuildingButtonsVisibility()
        {
            var instance = (InstanceID)GetInstanceField(typeof(ZonedBuildingWorldInfoPanel), _zonedBuildingInfoPanel, "m_InstanceID");
            var id = instance.Building;
            var building = Singleton<BuildingManager>.instance.m_buildings.m_buffer[id];
            _zonedBuildingLevelUpButton1.isVisible = CanBuildingLevelUp(building);
            _buildingSetOnFireButton1.isVisible = CanBuildingBurn(building);
        }

//        private void SetServiceBuildingButtonsVisibility()
//        {
//            var instance = (InstanceID)GetInstanceField(typeof(CityServiceWorldInfoPanel), _cityServiceInfoPanel, "m_InstanceID");
//            var id = instance.Building;
//            var building = Singleton<BuildingManager>.instance.m_buildings.m_buffer[id];
//            _zonedBuildingLevelUpButton2.isVisible = CanBuildingLevelUp(building);
//            _buildingSetOnFireButton2.isVisible = CanBuildingBurn(building);
//        }

        private void BuildingLevelUpHandler(UIComponent component, UIMouseEventParameter param)
        {
            BuildingHandler(LevelUpBuilding);
        }

        private void BuildingSetOnFireHandler(UIComponent component, UIMouseEventParameter param)
        {
            BuildingHandler(SetOnFire);
        }

        private void  BuildingHandler(Action<ushort> action) {
        var instance = (InstanceID)GetInstanceField(typeof(ZonedBuildingWorldInfoPanel), _zonedBuildingInfoPanel, "m_InstanceID");
            var id = instance.Building;
            Singleton<SimulationManager>.instance.AddAction(() =>
            {
                action.Invoke(id);
            });
        }

        private void DistrictLevelUpHandler(UIComponent component, UIMouseEventParameter param)
        {
            DistrictHandler(LevelUpBuilding);
        }

        private void DistrictSetOnFireHandler(UIComponent component, UIMouseEventParameter param)
        {
            DistrictHandler(SetOnFire);
        }
        
        private void DistrictHandler(Action<ushort> action){

        var instance = (InstanceID)GetInstanceField(typeof(DistrictWorldInfoPanel), _districtWorldInfoPanel, "m_InstanceID");
            var districtId = instance.District;

            var mBuildings = BuildingManager.instance.m_buildings;
            for (ushort index = 0; index < mBuildings.m_size; index++)
            {
                var building = mBuildings.m_buffer[index];
                if (building.m_flags == Building.Flags.None)
                {
                    continue;
                }
                var district = DistrictManager.instance.GetDistrict(building.m_position);
                if (district != districtId)
                {
                    continue;
                }
                var id = index;
                Singleton<SimulationManager>.instance.AddAction(() =>
                {
                    action.Invoke(id);
                });
            }
        }


        private static void LevelUpBuilding(ushort id)
        {
            var building = Singleton<BuildingManager>.instance.m_buildings.m_buffer[id];
            if (building.m_flags.IsFlagSet(Building.Flags.Upgrading))
            {
                return;
            }
            var info = building.Info;
            if (info == null)
            {
                return;
            }
            var ai = info.m_buildingAI;
            if (ai == null)
            {
                return;
            }

            if (!CanBuildingLevelUp(building))
            {
                return;
            }
            var parameters = new object[] { id, building };

            StartUpgradingMethod.Invoke(ai, parameters);
            Singleton<BuildingManager>.instance.m_buildings.m_buffer[id] = (Building)parameters[1];
        }


        private static void SetOnFire(ushort id)
        {
            if (!CanBuildingBurn(BuildingManager.instance.m_buildings.m_buffer[(int)id]))
            {
                return;
            }
            var info3 = BuildingManager.instance.m_buildings.m_buffer[(int)id].Info;
            int fireHazard4;
            int fireSize5;
            int fireTolerance6;
            var fireParameters = info3.m_buildingAI.GetFireParameters(id, ref BuildingManager.instance.m_buildings.m_buffer[(int)id], out fireHazard4, out fireSize5, out fireTolerance6);
            if (!fireParameters)
            {
                return;
            }

//            if (fireHazard4 == 0)
//            {
//                return;
//            }
            var waterLevel7 = Singleton<TerrainManager>.instance.WaterLevel(VectorUtils.XZ(BuildingManager.instance.m_buildings.m_buffer[(int)id].m_position));
            if (!((double)waterLevel7 <= (double)BuildingManager.instance.m_buildings.m_buffer[(int)id].m_position.y))
            {
                return;
            }
            var oldFlags8 = BuildingManager.instance.m_buildings.m_buffer[(int)id].m_flags;
            BuildingManager.instance.m_buildings.m_buffer[(int)id].m_fireIntensity = byte.MaxValue;
            info3.m_buildingAI.BuildingDeactivated(id, ref BuildingManager.instance.m_buildings.m_buffer[(int)id]);
            var newFlags9 = BuildingManager.instance.m_buildings.m_buffer[(int)id].m_flags;
            Singleton<BuildingManager>.instance.UpdateBuildingRenderer(id, true);
            Singleton<BuildingManager>.instance.UpdateBuildingColors(id);
            if (newFlags9 == oldFlags8)
            {
                return;
            }
            BuildingManager.instance.UpdateFlags(id, newFlags9 ^ oldFlags8);
        }

        private static bool CanBuildingBurn(Building building)
        {
            return (building.m_flags & (Building.Flags.Created | Building.Flags.Deleted | Building.Flags.Untouchable)) == Building.Flags.Created && (int)building.m_fireIntensity == 0 && (int)building.GetLastFrameData().m_fireDamage == 0;
        }


        private static bool CanBuildingLevelUp(Building building)
        {
            if ((building.m_flags & (Building.Flags.Created | Building.Flags.Deleted | Building.Flags.Untouchable)) == Building.Flags.Created)
            {
                var info = building.Info;
                if (info.m_buildingAI is ResidentialBuildingAI && info.m_class.m_level < ItemClass.Level.Level5)
                {
                    return true;
                }
                else if (info.m_buildingAI is CommercialBuildingAI && info.m_class.m_level < ItemClass.Level.Level3 &&
                    info.m_class.m_subService != ItemClass.SubService.CommercialLeisure && info.m_class.m_subService != ItemClass.SubService.CommercialTourist)
                {
                    return true;
                }
                else if (info.m_buildingAI is OfficeBuildingAI && info.m_class.m_level < ItemClass.Level.Level3)
                {
                    return true;
                }
                else if (info.m_buildingAI is IndustrialBuildingAI && info.m_class.m_level < ItemClass.Level.Level3)
                {
                    return true;
                }
            }
            return false;
        }

        internal static object GetInstanceField(Type type, object instance, string fieldName)
        {
            const BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                                           | BindingFlags.Static;
            var field = type.GetField(fieldName, bindFlags);
            if (field == null)
            {
                throw new Exception(string.Format("Type '{0}' doesn't have field '{1}", type, fieldName));
            }
            return field.GetValue(instance);
        }

    }

}
