using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Empire_Rewritten.Territories
{
    [UsedImplicitly]
    public class TerritoryDrawer : WorldLayer
    {
        public static bool dirty = true;
        private readonly Dictionary<Territory, List<int>> tilesDrawnOn = new Dictionary<Territory, List<int>>();

        public override bool ShouldRegenerate => dirty;

        protected override float Alpha => TerritoryUtils.TerritoryAlpha;

        public override IEnumerable Regenerate()
        {
            dirty = false;
            for (int i = 0; i < subMeshes.Count; i++)
            {
                subMeshes[i].Clear(MeshParts.All);
            }

            WorldGrid worldGrid = Find.WorldGrid;
            DrawAllTerritories(worldGrid);
            FinalizeMesh(MeshParts.All);
            yield break;
        }

        private void DrawTerritory(WorldGrid worldGrid, Territory territory)
        {
            Color factionColor = territory.Faction.Color;

            Material material = MaterialPool.MatFrom("World/SelectedTile", ShaderDatabase.WorldOverlayTransparentLit, factionColor, WorldMaterials.WorldObjectRenderQueue);
            if (!tilesDrawnOn.ContainsKey(territory))
            {
                tilesDrawnOn.Add(territory, new List<int>());
            }

            foreach (int tile in territory.Tiles.Where(tile => !tilesDrawnOn[territory].Contains(tile)))
            {
                tilesDrawnOn[territory].Add(tile);
                LayerSubMesh layerSubMesh = GetSubMesh(material);
                Vector3 vector = Find.WorldGrid.GetTileCenter(tile);
                WorldRendererUtility.PrintQuadTangentialToPlanet(vector, vector, worldGrid.averageTileSize, 1, layerSubMesh, false, false, false);
            }
        }

        private void DrawAllTerritories(WorldGrid worldGrid)
        {
            foreach (Territory territory in TerritoryManager.GetTerritoryManager.Territories)
            {
                DrawTerritory(worldGrid, territory);
            }
        }
    }
}
