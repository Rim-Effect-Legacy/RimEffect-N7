using System.Linq;

namespace RimEffectN7
{
    using RimWorld;
    using Verse;
    using VFECore.Abilities;


    public class HediffComp_DarkChannel : HediffComp_Ability
    {
        public override bool CompShouldRemove => 
            this.Pawn.health.InPainShock;

        public override void Notify_PawnDied()
        {
            base.Notify_PawnDied();
            this.Hop();
        }

        public override void CompPostPostRemoved()
        {
            base.CompPostPostRemoved();
            this.Hop();
        }

        public void Hop()
        {
            if ((this.Pawn?.Spawned ?? false) && this.ability != null)
            {
                if (GenRadial.RadialCellsAround(this.Pawn.PositionHeld, 3f, true).SelectMany(ivc => ivc.GetThingList(this.Pawn.Map)).OfType<Pawn>().Where(p =>
                                                                                                                                                              p != this.Pawn && !p.Downed && this.ability.Caster.HostileTo(p)).TryRandomElement(out Pawn target))
                {
                    Projectile projectile = GenSpawn.Spawn(REN7_DefOf.REN7_DarkChannel.GetModExtension<AbilityExtension_Projectile>().projectile, this.Pawn.Position, this.Pawn.Map) as Projectile;
                    if (projectile is AbilityProjectile abilityProjectile)
                        abilityProjectile.ability = this.ability;
                    projectile?.Launch(this.Pawn, this.Pawn.DrawPos, target, target, ProjectileHitFlags.IntendedTarget);
                }
            }
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            if (this.Pawn.IsHashIntervalTick(GenTicks.TicksPerRealSecond))
            {
                Hediff hediff = HediffMaker.MakeHediff(REN7_DefOf.REN7_DarkChannelPainHediff, this.Pawn);
                hediff.Severity = 0.1f;
                base.Pawn.health.AddHediff(hediff);
            }
        }
    }
}
