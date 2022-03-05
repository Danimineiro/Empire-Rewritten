using System.Collections;
using System.Collections.Generic;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Empire_Rewritten.Borders
{
    public class BorderDrawer : WorldLayer
    {
        public static bool dirty = true;
        private Dictionary<Border,List<int>> tilesDrawnOn =new Dictionary<Border, List<int>>();
        protected override int Layer => base.Layer;

        public override IEnumerable Regenerate()
        {
            WorldGrid worldGrid = Find.WorldGrid;
            DrawAllBorders(worldGrid);
            base.FinalizeMesh(MeshParts.All);
            yield break;
        }

        public override bool ShouldRegenerate
        {
            get
            {
                bool result = dirty;
                dirty = false;
                return result;
            }
        }

        protected override float Alpha
        {
            get
            {
                return BorderUtils.BorderAlpha;
            }
        }
        private void DrawBorder(WorldGrid worldGrid, Border border)
        {
            Color factionColor = border.Faction.Color;
            if (factionColor == null)
                factionColor = ColorsFromSpectrum.Get(border.Faction.def.colorSpectrum, border.Faction.colorFromSpectrum);

            Material material = MaterialPool.MatFrom("World/SelectedTile", ShaderDatabase.WorldOverlayTransparentLit, factionColor, WorldMaterials.WorldObjectRenderQueue);
            if (!tilesDrawnOn.ContainsKey(border))
                tilesDrawnOn.Add(border, new List<int>());
            foreach (int tile in border.Tiles)
            {
                if (!tilesDrawnOn[border].Contains(tile))
                {
                    try
                    {
                        tilesDrawnOn[border].Add(tile);
                        LayerSubMesh layerSubMesh = base.GetSubMesh(material);
                        Vector3 vector = Find.WorldGrid.GetTileCenter(tile);
                        WorldRendererUtility.PrintQuadTangentialToPlanet(vector, vector, worldGrid.averageTileSize, 1, layerSubMesh, false, false, false);
                    }
                    catch
                    {

                    }
                }
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
