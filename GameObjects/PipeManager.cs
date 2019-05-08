using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SuperMarioBros
{
    public enum TeleportType { Top_Top,Top_Bot,Bot_Bot,Bot_Top,None}

    public static class PipeManager
    {
        public static void ClearPipes() { pipes.Clear(); }
        public static Dictionary<int, Pipe> pipes = new Dictionary<int, Pipe>();
        public static void AddPipe(int number,Pipe pipe)
        {
            pipes.Add(number, pipe);
        }

        public static void AddTeleport(int from,int to, TeleportType type)
        {
            Pipe FromPipe = pipes[from];
            Pipe ToPipe = pipes[to];
            foreach (var item in pipes)
            {
               // Console.WriteLine(item.Key + " " + item.Value.number);
            }
            switch (type)
            {
                case TeleportType.Top_Top:
                    FromPipe.TopActive = true;
                    ToPipe.TopActive = true;
                    FromPipe.TP_TOP_destination = new Vector2(ToPipe.rectangle.X, ToPipe.rectangle.Y );
                    ToPipe.TP_TOP_destination = new Vector2(FromPipe.rectangle.X, FromPipe.rectangle.Y );
                    FromPipe.top_tp_type = TeleportType.Top_Top;
                    ToPipe.top_tp_type = TeleportType.Top_Top;
                    break;
                case TeleportType.Top_Bot:
                    FromPipe.TopActive = true;
                    ToPipe.BotActive = true;
                    FromPipe.TP_TOP_destination = new Vector2(ToPipe.rectangle.X, ToPipe.rectangle.Bot);
                    ToPipe.TP_BOT_destination = new Vector2(FromPipe.rectangle.X, FromPipe.rectangle.Y );
                    FromPipe.top_tp_type = TeleportType.Top_Bot;
                    ToPipe.bot_tp_type = TeleportType.Bot_Top;
                    break;
                case TeleportType.Bot_Bot:
                    FromPipe.BotActive = true;
                    ToPipe.BotActive = true;
                    FromPipe.TP_BOT_destination = new Vector2(ToPipe.rectangle.X, ToPipe.rectangle.Bot);
                    ToPipe.TP_BOT_destination = new Vector2(FromPipe.rectangle.X, FromPipe.rectangle.Bot);
                    FromPipe.bot_tp_type = TeleportType.Bot_Bot;
                    ToPipe.bot_tp_type = TeleportType.Bot_Bot;
                    break;
                case TeleportType.Bot_Top:
                    FromPipe.BotActive = true;
                    ToPipe.TopActive = true;
                    FromPipe.TP_BOT_destination = new Vector2(ToPipe.rectangle.X, ToPipe.rectangle.Y );
                    ToPipe.TP_TOP_destination = new Vector2(FromPipe.rectangle.X, FromPipe.rectangle.Bot);
                    FromPipe.bot_tp_type = TeleportType.Bot_Top;
                    ToPipe.top_tp_type = TeleportType.Top_Bot;
                    break;
            }

        }
        
    }
    public class Pipe : Tile , IHittableFromBot, IHittableFromTop
    {
        public bool TopActive = false;
        public bool BotActive = false;
        public Vector2 TP_TOP_destination;
        public Vector2 TP_BOT_destination;
        public int number;
        public TeleportType bot_tp_type;
        public TeleportType top_tp_type;

        public Pipe(Rectangle rectangle, Texture2D texture,int number) : base(rectangle, texture,true)
        {
            this.number = number;
        }

        private void ExecuteTopTP()
        {
            //scene.player.TopBotPipeMove(TP_BOT_destination);
        }
        private void ExecuteBotTP()
        {
            Console.WriteLine(bot_tp_type);
            scene.player.StartPipeMovement(TP_BOT_destination, bot_tp_type);
        }

        void IHittableFromBot.GetHit()
        {
            if (BotActive && (Math.Abs(rectangle.center().X - scene.player.rectangle.center().X) < 16) )
                ExecuteBotTP();
        }

        void IHittableFromTop.GetHit()
        {
            if (TopActive && (Math.Abs(rectangle.center().X - scene.player.rectangle.center().X) < 16) && Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                scene.player.StartPipeMovement(TP_TOP_destination, top_tp_type);
            }
        }
    }
}