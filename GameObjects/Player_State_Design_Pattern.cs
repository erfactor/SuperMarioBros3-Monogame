using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMarioBros
{
    public interface PlayerState
    {
        void GetHitByShroom(Shroom shroom);
        void GetHitByFeather(Feather feather);
        void GetHitByFlower(Flower flower);
        void GetDamaged();
    }

    class SmallMarioState : PlayerState
    {
        Player player;

        public SmallMarioState(Player player)
        {
            this.player = player;
        }

        public void GetDamaged()
        {
            player.alive = false;
        }

        public void GetHitByShroom(Shroom shroom)
        {
            player.SetState(MarioState.big);
            player.BiggerCollisionBox();
            player.BiggerTexture();
            shroom.enemydead = true;
            ContentManager.SoundEffects["Mushroom Obtained"].Play();
            player.MakeImmune();
            player.GotBigger();
            player.scene.MakeFuzzy();
        }

        public void GetHitByFeather(Feather feather)
        {
            player.SetState(MarioState.raccoon);
            player.BiggerCollisionBox();
            player.BiggerTexture();
            ContentManager.SoundEffects["Raccoon Leaf Obtained"].Play();
            player.MakeImmune();
            player.GotBigger();
        }

        public void GetHitByFlower(Flower flower)
        {
            player.SetState(MarioState.fire);
            player.BiggerCollisionBox();
            player.BiggerTexture();
            flower.alive = false;
            //ContentManager.SoundEffects["Mushroom Obtained"].Play();
            player.MakeImmune();
            player.GotBigger();
        }
    }

    class BigMarioState : PlayerState
    {
        Player player;

        public BigMarioState(Player player)
        {
            this.player = player;
        }

        public void GetDamaged()
        {
            player.SetState(MarioState.small);
            player.SmallerCollisionBox();
            player.SmallerTexture();
            ContentManager.SoundEffects["Downed"].Play();
            player.MakeImmune();
        }

        public void GetHitByShroom(Shroom shroom)
        {
           
        }

        public void GetHitByFeather(Feather feather)
        {
            player.SetState(MarioState.raccoon);
            ContentManager.SoundEffects["Raccoon Leaf Obtained"].Play();
            player.MakeImmune();
        }

        public void GetHitByFlower(Flower flower)
        {
            player.SetState(MarioState.fire);
            flower.alive = false;
            //ContentManager.SoundEffects["Mushroom Obtained"].Play();
            player.MakeImmune();
        }
    }

    class RaccoonMarioState : PlayerState
    {
        Player player;

        public RaccoonMarioState(Player player)
        {
            this.player = player;
        }

        public void GetDamaged()
        {
            player.SetState(MarioState.big);
            ContentManager.SoundEffects["Downed"].Play();
            player.MakeImmune();
        }

        public void GetHitByShroom(Shroom shroom)
        {

        }

        public void GetHitByFeather(Feather feather)
        {
            ContentManager.SoundEffects["Raccoon Leaf Obtained"].Play();
            player.MakeImmune();
        }

        public void GetHitByFlower(Flower flower)
        {
            player.SetState(MarioState.fire);
            flower.alive = false;
            //ContentManager.SoundEffects["Mushroom Obtained"].Play();
            player.MakeImmune();
        }
    }

    class FireMarioState : PlayerState
    {
        Player player;

        public FireMarioState(Player player)
        {
            this.player = player;
        }

        public void GetDamaged()
        {
            player.SetState(MarioState.big);
            ContentManager.SoundEffects["Downed"].Play();
            player.MakeImmune();
        }

        public void GetHitByShroom(Shroom shroom)
        {

        }

        public void GetHitByFeather(Feather feather)
        {
            player.SetState(MarioState.raccoon);
            ContentManager.SoundEffects["Raccoon Leaf Obtained"].Play();
            player.MakeImmune();
        }

        public void GetHitByFlower(Flower flower)
        {
            flower.alive = false;
            player.MakeImmune();
        }
    }
}
