using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace WindowsGame1
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D nerd;
        Texture2D background;
        Texture2D poop;
        Texture2D poopsplat;
        Texture2D explosion;

        SoundEffect fart;
        SoundEffectInstance frt;
        SoundEffect splatSound;
        SoundEffectInstance splt;

        Random rand;

        int deltaX;
        int poopX;
        int poopY;
        int score;
        int totalGames=4;

        Texture2D[] game;
        
        int[] xDirection;
        int[] yDirection;

        int[] xPosition;
        int[] yPosition;
        
        Boolean[] gameExists;
        Boolean[] shouldPoopSplat;

        Boolean poopExists;
        Boolean justPooped;
        Boolean justDied;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            rand=new Random();

            spriteBatch = new SpriteBatch(GraphicsDevice);
            nerd=Content.Load<Texture2D>(@"Images/avgn2");
            background=Content.Load<Texture2D>(@"Images/background");
            poop = Content.Load<Texture2D>(@"Images/poop");
            poopsplat = Content.Load<Texture2D>(@"Images/poopsplat");
            explosion = Content.Load<Texture2D>(@"Images/explosion");

            fart = Content.Load<SoundEffect>(@"fart");
            frt = fart.CreateInstance();

            splatSound = Content.Load<SoundEffect>(@"Sounds/splat");
            splt = splatSound.CreateInstance();

            KeyboardState ks = Keyboard.GetState();

            poopExists = false;
            justPooped = false;

            xDirection = new int[totalGames];
            yDirection = new int[totalGames];
            xPosition = new int[totalGames];
            yPosition = new int[totalGames];
            gameExists = new Boolean[totalGames];
            shouldPoopSplat = new Boolean[totalGames];
            game = new Texture2D[totalGames];
            score = 0;

            // Setting the initial directions (more or less random)

            xDirection[0] = 1;
            yDirection[0] = 1;

            xDirection[1] = 1;
            yDirection[1] = -1;

            xDirection[2] = -1;
            yDirection[2] = -1;

            xDirection[3] = -1;
            yDirection[3] = 1;
          
            // Loading games

            gameExists[0] = true;
            game[0] = Content.Load<Texture2D>(@"games/NES/super_pitfall");

            gameExists[1] = true;
            game[1] = Content.Load<Texture2D>(@"games/NES/dr_jekyll");

            gameExists[2] = true;
            game[2] = Content.Load<Texture2D>(@"games/NES/action_52");

            gameExists[3] = true;
            game[3] = Content.Load<Texture2D>(@"games/NES/hydlide");

            for (int x = 0; x < totalGames; x++)
            {
                xPosition[x] = rand.Next(50, Window.ClientBounds.Width-50);
                yPosition[x] = rand.Next(0, Window.ClientBounds.Height/2);
            }
        }

        protected override void UnloadContent()
        {
           
        }
        protected override void Update(GameTime gameTime)
        {
            justDied = false;
            //bounce

            for (int x = 0; x < totalGames; x++)
            {
                if (gameExists[x])
                {
                    if (xPosition[x] < 0 || xPosition[x] > Window.ClientBounds.Width-50)
                    {
                        xDirection[x] = -1 * xDirection[x];
                    }
                    if (yPosition[x] < 0 || yPosition[x] > Window.ClientBounds.Height-50)
                    {
                        yDirection[x] = -1 * yDirection[x];
                    }
                }
                shouldPoopSplat[x] = false;

                // collision detection

                if (poopExists && poopX < xPosition[x] + 50 && poopX > xPosition[x]-40 && poopY < yPosition[x]+40  && poopY > yPosition[x]-110 && gameExists[x])
                {
                    shouldPoopSplat[x] = true;
                    gameExists[x] = false;
                    poopExists = false;
                    score++;
                }
                if (xPosition[x] > Window.ClientBounds.Width/2 + deltaX && xPosition[x] < Window.ClientBounds.Width/2 + deltaX + 50 && yPosition[x]<Window.ClientBounds.Height-50)
                {
                    justDied = true;
                }
            }

            //make the games move

            for (int x = 0; x < totalGames; x++)
            {
                if (gameExists[x])
                {
                    xPosition[x] += xDirection[x];
                    yPosition[x] += yDirection[x];
                }
            }

            justPooped = false;     // Disables pooping temporarily
            
            if (poopExists)
            {
                poopY-=16;
            }
            if (poopY < -45)
            {
                poopExists = false;
            }

            // Read input from the user

            KeyboardState ks = Keyboard.GetState();
            if (ks.IsKeyDown(Keys.Left) && deltaX>-Window.ClientBounds.Width/2)
            {
                deltaX = deltaX - 4;
            }
            if (ks.IsKeyDown(Keys.Right)&&deltaX<Window.ClientBounds.Width/2-100)
            {
                deltaX = deltaX + 4;
            }
            if (ks.IsKeyDown(Keys.Space) && !poopExists)
            {
                poopExists = true;
                justPooped = true;
                poopX = Window.ClientBounds.Width / 2 + deltaX + 50; 
                poopY = Window.ClientBounds.Height-130;
            }
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            
            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            spriteBatch.Draw(background, Vector2.Zero, Color.White);
  //          if (!justDied)
  //          {
                spriteBatch.Draw(nerd, new Vector2((Window.ClientBounds.Width / 2 + deltaX) % Window.ClientBounds.Width, Window.ClientBounds.Height - 100), Color.White);
  //          }
  //          else
   //         {
       //         spriteBatch.Draw(explosion, new Vector2((Window.ClientBounds.Width / 2 + deltaX) % Window.ClientBounds.Width, Window.ClientBounds.Height - 100), Color.White);
     //       }
            if (poopExists)
            {
                spriteBatch.Draw(poop, new Vector2(poopX, poopY), Color.White);
            }
            if (justPooped)
            {
                frt.Play();
            }
            for (int x = 0; x < totalGames; x++)
            {
                if (gameExists[x])
                {
                    spriteBatch.Draw(game[x], new Vector2(xPosition[x], yPosition[x]), Color.White);
                }
                else
                {
                    spriteBatch.Draw(poopsplat, new Vector2(xPosition[x], yPosition[x]), Color.White);
                }
                if (shouldPoopSplat[x])
                {
                    splt.Play();
                }
            }
            spriteBatch.DrawString(Content.Load<SpriteFont>("SpriteFont1"), score.ToString(), new Vector2(0, 0), Color.Red);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}