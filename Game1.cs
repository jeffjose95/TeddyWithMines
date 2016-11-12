using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using TeddyMineExplosion;

namespace ProgrammingAssignment5
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;                                             //Games graphic manager
        SpriteBatch spriteBatch;                                                    //Spritebatch on which everything wil be drawn

        public static int WindowWidth = 800;                                        //Games set windowWidth
        public static int WindowHeight = 600;                                       //Games set windowHeight

        //Textures for the 3 different elements of this game
        public Texture2D mineSprite;   
        public Texture2D teddybearSprite;
        public Texture2D explosionSprite;

        //Lists to draw the 3 different elements of this game
        List<Mine> mineList = new List<Mine>();
        List<TeddyBear> bearList = new List<TeddyBear>();
        List<Explosion> explosionList = new List<Explosion>();


        //Checks if I had just clicked
        public bool justClicked;
   
        public int presentTime;                                                     //The present time in the game
        public int bearSpawnTime;                                                   //The next moment a bear must spawn in the game
        public int bearSpawnInterval;                                               //Reference to the time till the next bear spawns

        Random RDG;                                                                 //Common random number generator

        //Set the initial game information
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = WindowWidth;
            graphics.PreferredBackBufferHeight = WindowHeight;
            IsMouseVisible = true;
            presentTime = 0;
            bearSpawnTime = 0;
            RDG = new Random();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            //Loads the graphics for the different elements
            mineSprite = Content.Load<Texture2D>("Graphics//mine");
            teddybearSprite = Content.Load<Texture2D>("Graphics//teddybear");
            explosionSprite = Content.Load<Texture2D>("Graphics//explosion");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {

            presentTime += gameTime.ElapsedGameTime.Milliseconds;       //Adds a millisecond each frame to the present time

            MouseState myMouseData = Mouse.GetState();                  //Gets the mouse information from the computer

            //If left mouse was pressed
            if (myMouseData.LeftButton == ButtonState.Pressed)
            {
                justClicked = true;
            }

            //If you release the left mouse button right after clicking
            if (myMouseData.LeftButton == ButtonState.Released && justClicked)
            {
                justClicked = false;

                //Gets mouse position data
                int XPosition = myMouseData.Position.X;
                int YPosition = myMouseData.Position.Y;

                //Creates a new mine at the position of the mouse
                Mine newMine = new Mine(mineSprite, XPosition, YPosition);
                newMine.Active = true;                                  //Sets the new mine to active
                mineList.Add(newMine);                                  //Adds it to the list of mines
            }

            //If the present time is greater than the last bearSpawnTime
            if (presentTime >= bearSpawnTime)
            {
                bearSpawnInterval = 1000 + RDG.Next(3)*1000;        //Sets the time till the next bear spawn
                bearSpawnTime += bearSpawnInterval;                 //BearSpawnTime is changed to the next bearSpawnTime

                //Gets a random X and Y velocity for the bear between -0.5 and 0.5
                double XVelocity = 0.5 * RDG.Next(-1,1);
                double YVelocity = 0.5 * RDG.Next(-1,1);
                
                // The new bear velocity
                Vector2 bearVelocity = new Vector2((float)XVelocity, (float)YVelocity); 
                
                //Creates a new teddy bear
                TeddyBear newBear = new TeddyBear(teddybearSprite, bearVelocity, WindowWidth, WindowHeight);
                newBear.Active = true;
                bearList.Add(newBear);                                  //Adds it to the list of teddybears
            }

            //For every teddybear in the list
            foreach (TeddyBear thisBear in bearList)
            {
                thisBear.Update(gameTime);                              //Update the position of the teddybear
                
                //For every mine in the list
                foreach (Mine thisMine in mineList)
                {

                    //If the bear is in contact with the mine
                    if (((thisBear.CollisionRectangle.X - teddybearSprite.Width/2 <= thisMine.CollisionRectangle.X) && (thisMine.CollisionRectangle.X <= thisBear.CollisionRectangle.X + thisBear.CollisionRectangle.Width)) && ((thisBear.CollisionRectangle.Y - teddybearSprite.Height/2<=thisMine.CollisionRectangle.Y)&&(thisBear.CollisionRectangle.Y+thisBear.CollisionRectangle.Height>=thisMine.CollisionRectangle.Y)))
                    {
                        //Deactivate the bear and mine
                        thisBear.Active = false;
                        thisMine.Active = false;

                        //Create a new explosion and add it to the list 
                        Explosion newExplosion = new Explosion(explosionSprite, thisMine.CollisionRectangle.X, thisMine.CollisionRectangle.Y);
                        explosionList.Add(newExplosion);
                    }
                }
            }

            //Updates the explosion
            foreach (Explosion thisExplosion in explosionList)
            {
                thisExplosion.Update(gameTime);
            }

            //Removes inactive mines from the list for garbage collection
            for (int i = mineList.Count - 1; i >= 0; i--)
            {
                if (mineList[i].Active == false)
                {
                    mineList.RemoveAt(i);
                }
            }

            //Removes inactive bears from the list for garbage collection
            for (int i = bearList.Count - 1; i >= 0; i--)
            {
                if (bearList[i].Active == false)
                {
                    bearList.RemoveAt(i);
                }
            }

            //Removes inactive explosions from the list for garbage collection
            for (int i = explosionList.Count - 1; i >= 0; i--)
            {
                if (explosionList[i].Playing == false)
                {
                    explosionList.RemoveAt(i);
                }
            }
         

            
           
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            
            

            // TODO: Add your drawing code here

            base.Draw(gameTime);

            //Draws on the present spritebatch
            spriteBatch.Begin();

            //Draw for each mine in the list
            foreach (Mine thisMine in mineList)
            {
              thisMine.Draw(spriteBatch);
            }

            //Draw for each teddybear in the list
            foreach (TeddyBear thisBear in bearList)
            {
                thisBear.Draw(spriteBatch);
            }

            //Draw for each explosion in the list
            foreach (Explosion thisExplosion in explosionList)
            {
                thisExplosion.Draw(spriteBatch);
            }

            //Ends the draw on the spritebach
            spriteBatch.End();
        }
    }
}
