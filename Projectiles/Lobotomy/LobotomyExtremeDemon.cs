﻿using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaModding.External.AI;

namespace VanillaModding.Projectiles.Lobotomy
{
    // This Example show how to implement simple homing projectile
    // Can be tested with ExampleCustomAmmoGun
    public class LobotomyExtremeDemon : ModProjectile
    {
        public static SoundStyle SoundEpicDeep()
        {
            SoundStyle SS = new SoundStyle($"{nameof(VanillaModding)}/SFX/Lobotomy/FireInTheHole")
            {
                Volume = 1f,
                Pitch = 0.25f,
                PitchVariance = 0.25f,
                MaxInstances = 25,
            };
            return SS;
        }
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
        }

        // Setting the default parameters of the projectile
        // You can check most of Fields and Properties here https://github.com/tModLoader/tModLoader/wiki/Projectile-Class-Documentation
        int timeLast = 600;
        public override void SetDefaults()
        {
            Projectile.width = 42; // The width of projectile hitbox
            Projectile.height = 38; // The height of projectile hitbox

            Projectile.aiStyle = 0; // The ai style of the projectile (0 means custom AI). For more please reference the source code of Terraria
            Projectile.DamageType = DamageClass.Ranged; // What type of damage does this projectile affect?
            Projectile.friendly = true; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.light = 1f; // How much light emit around the projectile
            Projectile.tileCollide = false; // Can the projectile collide with tiles?
            Projectile.timeLeft = timeLast; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)


        }

        // Custom AI
        public override void AI()
        {
            float maxDetectRadius = 960f; // The maximum radius at which a projectile can detect a target
            float projSpeed = 40f; // The speed at which the projectile moves towards the target

            // Trying to find NPC closest to the projectile
            NPC closestNPC = AdvAI.FindClosestNPC(maxDetectRadius, Projectile);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (closestNPC == null)
                return;

            Projectile.velocity = -Vector2.Lerp(-Projectile.velocity, (Projectile.Center - closestNPC.Center).SafeNormalize(Vector2.Zero) * projSpeed, 0.1f);


        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //base.OnHitNPC(target, hit, damageDone);
            //Projectile.damage += 10;
            explodeLobotomy();
        }

        public void explodeLobotomy()
        {
            var position = Projectile.position;
            var speedX = Projectile.velocity.X;
            var speedY = Projectile.velocity.Y;
            float speedMul = 2f;
            float numberProjectiles = 3; // 3 shots
            float rotation = MathHelper.ToRadians(45);//Shoots them in a 45 degree radius. (This is technically 90 degrees because it's 45 degrees up from your cursor and 45 degrees down)
            position += Vector2.Normalize(new Vector2(speedX, speedY)) * 45f; //45 should equal whatever number you had on the previous line
            var enS = Projectile.GetSource_FromThis();
            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * .2f; // Vector for spread. Watch out for dividing by 0 if there is only 1 projectile.
                Projectile.NewProjectile(enS, new Vector2(position.X, position.Y), new Vector2(perturbedSpeed.X, perturbedSpeed.Y) * speedMul, ModContent.ProjectileType<LobotomyNormal>(), Projectile.damage / 2, Projectile.damage / 2, Projectile.owner); //Creates a new projectile with our new vector for spread.
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundEpicDeep(), Projectile.position);
        }
        public override void OnSpawn(IEntitySource source)
        {
            //SoundEngine.PlaySound(SoundEpicDeep(), Projectile.position);
        }
    }
}