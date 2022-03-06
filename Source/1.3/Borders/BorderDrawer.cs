using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Empire_Rewritten.Borders
{
    public class BorderDrawer : WorldLayer
    {
        public static bool dirty = true;
        private readonly Dictionary<Border, List<int>> tilesDrawnOn = new Dictionary<Border, List<int>>();

        public override bool ShouldRegenerate => dirty;

        protected override float Alpha => BorderUtils.BorderAlpha;

        public override IEnumerable Regenerate()
        {
            WorldGrid worldGrid = Find.WorldGrid;
            DrawAllBorders(worldGrid);
            FinalizeMesh(MeshParts.All);
            dirty = false;
            yield break;
        }

        private void DrawBorder(WorldGrid worldGrid, Border border)
        {
            Color factionColor = border.Faction.Color;

            /* TODO: struct is never null. When was this supposed to fire?
            if (factionColor == null)
            {
                factionColor = ColorsFromSpectrum.Get(border.Faction.def.colorSpectrum, border.Faction.colorFromSpectrum);
            }
            */

            Material material = MaterialPool.MatFrom("World/SelectedTile", ShaderDatabase.WorldOverlayTransparentLit, factionColor, WorldMaterials.WorldObjectRenderQueue);
            if (!tilesDrawnOn.ContainsKey(border))
            {
                tilesDrawnOn.Add(border, new List<int>());
            }

            foreach (int tile in border.Tiles.Where(tile => !tilesDrawnOn[border].Contains(tile)))
            {
                tilesDrawnOn[border].Add(tile);
                LayerSubMesh layerSubMesh = GetSubMesh(material);
                Vector3 vector = Find.WorldGrid.GetTileCenter(tile);
                WorldRendererUtility.PrintQuadTangentialToPlanet(vector, vector, worldGrid.averageTileSize, 1, layerSubMesh, false, false, false);
            }
        }

        private void DrawAllBorders(WorldGrid worldGrid)
        {
            foreach (Border border in BorderManager.GetBorderManager.Borders)
            {
                DrawBorder(worldGrid, border);
            }
        }
    }
}