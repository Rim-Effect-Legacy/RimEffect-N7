using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RimEffectN7
{
    using RimWorld;
    using UnityEngine;
    using Verse;

    public class BlackHoleProjectile : Projectile_Explosive
    {

        public float curRotation = 0f;

        public static float rotSpeed = 5f;

        public static float lerpSpeed = 0.01f;

        [Unsaved]
        public Graphic graphic;

        [Unsaved]
        private List<Pawn> tmpPawns = new List<Pawn>();

        public  float radius = 5f;

        public ThingDef mote;
        public Mote     moteThing;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            this.mote = ThingDef.Named("REN7_Mote_BioticDistortionBlackhole");
        }

        public override Graphic Graphic
        {
            get
            {
                if (this.graphic == null || Math.Abs(this.graphic.drawSize.x - (this.radius * 2)) > float.Epsilon)
                    this.graphic = GraphicDatabase.Get(this.def.graphicData.graphicClass, this.def.graphicData.texPath, this.def.graphicData.shaderType.Shader,
                                                       new Vector2(this.radius * 2, this.radius * 2), this.def.graphicData.color, this.def.graphicData.colorTwo, this.def.graphicData,
                                                       this.def.graphicData.shaderParameters);
                return this.graphic;
            }
        }

        public override void Tick()
        {
            this.curRotation += rotSpeed % 360f;
            foreach (Pawn pawn in this.tmpPawns)
                if (!pawn.DestroyedOrNull() && pawn.Spawned)
                {
                    Vector3 vector3 = this.DrawPos; // Vector3.Lerp(pawn.Position.ToVector3(), this.DrawPos, lerpSpeed);
                    pawn.Position = vector3.ToIntVec3();
                    pawn.Notify_Teleported();
                    pawn.pather.nextCell = this.DrawPos.ToIntVec3();
                }

            if (this.moteThing is null)
            {
                this.moteThing = MoteMaker.MakeStaticMote(this.DrawPos, this.Map, mote);
            }
            else
            {
                this.moteThing.Maintain();
                this.moteThing.exactPosition = this.DrawPos;
            }

            if (this.IsHashIntervalTick(GenTicks.TickRareInterval / 25))
            {
                int rangeForGrabbingPawns = Mathf.RoundToInt(this.radius * 2);
                IntVec2 rangeIntVec2 = new IntVec2(rangeForGrabbingPawns, rangeForGrabbingPawns);

                foreach (IntVec3 intVec3 in GenAdj.OccupiedRect(this.DrawPos.ToIntVec3(), this.Rotation, rangeIntVec2))
                {
                    if (!intVec3.InBounds(Find.CurrentMap))
                        continue;
                    List<Thing> cellThings = Find.CurrentMap.thingGrid.ThingsListAt(intVec3);
                    if (cellThings == null)
                        continue;

                    for (int i = 0; i < cellThings.Count; i++)
                    {
                        Thing t = cellThings[i];

                        if (!this.tmpPawns.Contains(t) && t is Pawn p && p.HostileTo(this.Launcher))
                            this.tmpPawns.Add(p);
                    }
                }
            }

            base.Tick();
        }

        protected override void Explode()
        {
            foreach (Pawn tmpPawn in this.tmpPawns)
            {
                tmpPawn.Position = DrawPos.ToIntVec3();
            }
            GenExplosion.DoExplosion(this.Position, this.Map, this.radius * 2f, this.def.projectile.damageDef, this.Launcher, Mathf.RoundToInt(DamageAmount), 1f, this.def.soundImpactDefault);
            base.Explode();
        }

        public override void Draw()
        {
            float arcHeightFactor  = def.projectile.arcHeightFactor;
            float arcHeightFactor2 = (destination - origin).MagnitudeHorizontalSquared();
            if (arcHeightFactor * arcHeightFactor > arcHeightFactor2 * 0.2f * 0.2f)
            {
                arcHeightFactor = Mathf.Sqrt(arcHeightFactor2) * 0.2f;
            }

            float   num      = arcHeightFactor * GenMath.InverseParabola(DistanceCoveredFraction);
            Vector3 drawPos  = DrawPos;
            Vector3 position = drawPos + new Vector3(0f, 0f, 1f) * num;

            //Graphics.DrawMesh(MeshPool.GridPlane(def.graphicData.drawSize), position, ExactRotation, def.DrawMatSingle, 0);

            Mesh       mesh = this.Graphic.MeshAt(this.Rotation);
            Quaternion quat = Quaternion.AngleAxis(this.curRotation, Vector3.up);

            Material mat = this.Graphic.MatAt(this.Rotation, this);

            Graphics.DrawMesh(mesh, position, quat, mat, 0);



            Comps_PostDraw();
        }
    }
}
