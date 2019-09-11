using System;
using System.Collections.Generic;
using System.Linq;

namespace SuperMarioBros
{

    public class CollisionDetector
    {
        public static float turtlediff = 15;
        public Scene scene;

        public CollisionDetector(Scene scene)
        {
            this.scene = scene;
        }

        public void DamageCollisions()
        {
            foreach (var damager in scene.enemiesToUpdate.OfType<IDamager>().Where(d => d.IsDamaging))
            {

                Rectangle dmgrRect = damager.GetRect;
                foreach (var damaged in scene.enemiesToUpdate.Where(e => e.collidable).OfType<IDamaged>().Where(d => !d.Equals(damager)))
                {
                    Rectangle dmgdRect = damaged.GetRect;
                    if (isIntersect(dmgrRect, dmgdRect))
                    {
                        damaged.GetDamaged();
                        if (damager is FireBall fire)
                        {
                            fire.enemydead = true;
                        }
                    }
                }
            }
        }

        public void DetectCollision(Mob m, float deltaTime)
        {
            List<Tile> solidtiles = scene.tilesToUpdate.Where(t => t.solid).ToList();
            float minDistance = float.MaxValue;

            checkBlockCollision(m, scene.tilesToUpdate, deltaTime, ref minDistance);
            checkCollisionX(m, solidtiles, deltaTime);
            checkCollisionYtop(m, solidtiles, deltaTime);
            if (!m.collisionYbot) checkCollisionYbot(m, solidtiles, deltaTime, ref minDistance);
            if (!m.collisionX && !m.collisionYbot && !m.collisionYtop)
            {
                Rectangle mobrectangle = m.rectangle;
                if (m is Turtle)
                {
                    mobrectangle.X += turtlediff;
                    mobrectangle.width -= turtlediff * 2;
                }
                Vector2 mobcenter = mobrectangle.center();
                Vector2 offset = new Vector2(m.velocityX * deltaTime, m.velocityY * deltaTime);
                Rectangle destination = mobrectangle + offset;
                Vector2 center = destination.center();
                bool foundCollisionBot = false;
                bool foundCollisionTop = false;
                bool foundCollisionSide = false;

                foreach (var t in solidtiles)
                {
                    Rectangle rect = t.rectangle + destination;
                    if (rect.IsPointOutside(center)) { continue; }
                    else
                    {
                        Vector2 tileCenter = t.rectangle.center();
                        Vector2 distance = new Vector2(center.X - tileCenter.X, mobcenter.Y - tileCenter.Y);
                        if ((Math.Abs(distance.Y) - Math.Abs(distance.X)) >= 0.001f)
                        {
                            if (distance.Y < 0) { foundCollisionBot = true; }
                            else { foundCollisionTop = true; }
                        }
                        if ((Math.Abs(distance.X) - Math.Abs(distance.Y)) >= 0.001f)
                        {
                            foundCollisionSide = true;
                        }
                        if (foundCollisionBot)
                        {
                            float dist = t.rectangle.Y - mobrectangle.height - mobrectangle.Y;
                            if (dist < minDistance) minDistance = dist;
                        }
                    }
                }
                m.collisionYtop = foundCollisionTop;
                m.collisionYbot = foundCollisionBot;
                m.collisionX = foundCollisionSide;

            }
            if (m.collisionYbot && !(m is Plant) && !(m is Platform) && minDistance < 64 && minDistance > -16)
            {
                m.rectangle.Y += minDistance;
                if (m is Player p)
                {
                    if (p.collidable)
                        p.TextureRect.Y += minDistance;
                }
            }

        }

        public void DetectCollisions(float deltaTime)
        {
            foreach (var m in scene.mobsToUpdate)
            {
                DetectCollision(m, deltaTime);
            }
            foreach (var enemy in scene.enemiesToUpdate.Where(x => x.collidable))
            {
                DetectCollision(enemy, deltaTime);
            }
        }

        public void MarioMobsCollisions(float deltaTime)
        {
            Random rnd = new Random(DateTime.Today.Second);
            Player player = scene.player;
            foreach (Mob mob in scene.mobsToUpdate)
            {
                if (mob == player) { continue; }
                if (isIntersect(mob.rectangle, player.rectangle))
                {
                    switch (mob)
                    {
                        case Shroom shroom:
                            player.GetHit(shroom);
                            break;
                        case Bullet bullet:
                            player.GetHit(bullet);
                            break;
                    }
                }
            }
            foreach (var ihittable in scene.tilesToUpdate.OfType<IHittable>())
            {
                Tile t = (Tile)ihittable;
                if (isIntersect(t.rectangle, player.rectangle))
                {
                    ihittable.GetHit();
                }
            }


        }

        public void MarioEnemysCollisions(float deltaTime)
        {
            Player mario = scene.player;
            foreach (Enemy enemy in scene.enemiesToUpdate.Where(e => e.collidable))
            {
                Vector2 mariooffset = new Vector2(mario.velocityX * deltaTime, mario.velocityY * deltaTime);
                Rectangle mariodestination = mario.rectangle + mariooffset;
                Vector2 mariocenter = mariodestination.center();

                //Vector2 enemyoffset = new Vector2(enemy.velocityX * deltaTime, enemy.velocityY * deltaTime);
                Rectangle enemydestination = enemy.rectangle;// + enemyoffset;
                //enemydestination += mario.rectangle;

                /////////////////Triangles and points
                ///
                if (!enemydestination.IsPointOutside(mariocenter))
                {
                    if (PointInTriangle(mariocenter, enemydestination.TopLeftCorner, enemydestination.TopMid, enemydestination.center()))
                    {
                        mario.JumpedOnSth(enemy);
                        enemy.GetHitTopLeft = true;
                        enemy.GetHitSide = false;
                        enemy.GetHitTopRight = false;
                    }
                    else if (PointInTriangle(mariocenter, enemydestination.TopRightCorner, enemydestination.TopMid, enemydestination.center()))
                    {
                        mario.JumpedOnSth(enemy);
                        enemy.GetHitTopLeft = false;
                        enemy.GetHitSide = false;
                        enemy.GetHitTopRight = true;
                    }
                    else
                    {
                        enemy.GetHitTopLeft = false;
                        enemy.GetHitSide = true;
                        enemy.GetHitTopRight = false;
                    }
                }
                else
                {
                    enemy.GetHitTopLeft = false;
                    enemy.GetHitSide = false;
                    enemy.GetHitTopRight = false;
                }
            }
        }

        private void checkCollisionX(Mob m, List<Tile> tiles, float deltaTime)
        {
            Rectangle futureRectX = new Rectangle(m.rectangle.X + deltaTime * m.velocityX, m.rectangle.Y + 1f, m.rectangle.width, m.rectangle.height - 2f);

            foreach (var t in tiles)
            {
                if (isIntersect(futureRectX, t.rectangle))
                {
                    if (m is Player p)
                        if (t.rectangle.X < p.rectangle.X)
                        {
                            p.onleft = true;
                        }
                        else p.onleft = false;

                    m.collisionX = true;
                    if (t is DestroyableTile destroyable && ((m is Turtle turtle && turtle.state == TurtleState.roll) || m is SimpleDamage))
                    {
                        destroyable.Destroy();
                    }
                    if (t is IHittableFromBot hittable && m is Turtle turtle1 && turtle1.state == TurtleState.roll)
                    {
                        hittable.GetHit();
                    }
                    return;
                }
            }
            m.collisionX = false;
        }

        private void checkCollisionYtop(Mob m, List<Tile> tiles, float deltaTime)
        {
            Rectangle futureRectY = new Rectangle(m.rectangle.X + 1f, m.rectangle.Y + deltaTime * m.velocityY - 0.5f, m.rectangle.width - 2f, m.rectangle.height);
            if (m.velocityY <= 0)
                foreach (var t in tiles)
                {
                    if (isIntersect(futureRectY, t.rectangle))
                    {
                        m.collisionYtop = true;
                        if (m is Player p && p.collidable && t is IHittableFromBot hittable) { hittable.GetHit(); }
                        return;
                    }
                }
            m.collisionYtop = false;
        }

        private void checkCollisionYbot(Mob m, List<Tile> tiles, float deltaTime, ref float minDistance)
        {
            Rectangle futureRectY = new Rectangle(m.rectangle.X + 1f, m.rectangle.Y + deltaTime * m.velocityY + 0.5f, m.rectangle.width - 2f, m.rectangle.height);
            if (m is Turtle)
            {
                futureRectY.X += turtlediff;
                futureRectY.width -= turtlediff * 2;
            }
            if (m.velocityY >= 0)
                foreach (var t in tiles)
                {
                    if (isIntersect(futureRectY, t.rectangle))
                    {
                        if (m is Player p && p.collidable && t is IHittableFromTop hittable) { p.onpipe = true; hittable.GetHit(); }

                        float dist = t.rectangle.Y - m.rectangle.height - m.rectangle.Y;
                        if (dist < minDistance) minDistance = dist;
                        m.collisionYbot = true; return;
                    }
                }
            if (m is Player p1 && p1.collidable) { p1.onpipe = false; }

            m.collisionYbot = false;
        }

        private void checkBlockCollision(Mob m, List<Tile> tiles, float deltaTime, ref float minDistance)
        {

            Rectangle futureRectY = new Rectangle(m.rectangle.X, m.rectangle.Y + deltaTime * m.velocityY, m.rectangle.width, m.rectangle.height);
            if (m.velocityY >= 0)
                foreach (Block block in tiles.OfType<Block>())
                {
                    Rectangle smallblock = new Rectangle(block.rectangle.X, block.rectangle.Y, block.rectangle.width, block.rectangle.height);

                    if (m.rectangle.Y + m.rectangle.height <= block.rectangle.Y && isIntersect(futureRectY, smallblock))
                    {
                        if (m is Player p1)
                        {
                            float dist = block.rectangle.Y - m.rectangle.height - m.rectangle.Y;
                            if (dist < minDistance) minDistance = dist;
                        }
                        m.collisionYbot = true;
                        return;
                    }
                }
            m.collisionYbot = false;
        }

        public static bool isIntersect(
        Rectangle A, Rectangle B)
        {
            return
              B.X + B.width > A.X &&
              B.Y + B.height > A.Y &&
              A.X + A.width > B.X &&
              A.Y + A.height > B.Y;
        }

        public static bool isIntersect(
        int Ax, int Ay, int Aw, int Ah,
        int Bx, int By, int Bw, int Bh)
        {
            return
              Bx + Bw > Ax &&
              By + Bh > Ay &&
              Ax + Aw > Bx &&
              Ay + Ah > By;
        }

        public static float sign(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            return (p1.X - p3.X) * (p2.Y - p3.Y) - (p2.X - p3.X) * (p1.Y - p3.Y);
        }

        public static bool PointInTriangle(Vector2 pt, Vector2 v1, Vector2 v2, Vector2 v3)
        {
            float d1, d2, d3;
            bool has_neg, has_pos;

            d1 = sign(pt, v1, v2);
            d2 = sign(pt, v2, v3);
            d3 = sign(pt, v3, v1);

            has_neg = (d1 < 0) || (d2 < 0) || (d3 < 0);
            has_pos = (d1 > 0) || (d2 > 0) || (d3 > 0);

            return !(has_neg && has_pos);
        }
    }
}
