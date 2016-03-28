﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace GP_Final
{
    //Should have made this inherit from a generalized GameRoundManager
    class MonogameRoundManager : DrawableSprite
    {
        public GameRound round;
        public LevelBorder border;

        Texture2D placeHolder;
        Song music;
        SpriteFont font;
        Color fontColor, instructionsColor, timerColor;

        public int PlayerScore
        {
            get
            {
                return this.round.Points;
            }
        }
        private int highScore;

        public int HighScore
        {
            get
            {
                if (PlayerScore > highScore)
                    highScore = PlayerScore;

                return highScore;
            }
        }

        public bool HasStartedRound, FirstRoundStartHasStarted;

        private bool firstRoundOver;
        
        private float fontScale, midpointLeft, midpointRight, timeLeftInRound;     

        public MonogameRoundManager(Game game) : base(game)
        {
            this.round = new GameRound(game);
        }

        protected override void LoadContent()
        {
            this.font = content.Load<SpriteFont>("ConsoleFont");

            this.placeHolder = content.Load<Texture2D>("Instructions");
            this.spriteTexture = content.Load<Texture2D>("SpriteMarker");
            this.music = content.Load<Song>("Jean_Luc");
         
            MediaPlayer.Play(music);
            MediaPlayer.Volume = .1f;
            MediaPlayer.IsRepeating = true;

            this.FirstRoundStartHasStarted = this.firstRoundOver = false;
            this.HasStartedRound = false;

            this.highScore = 0;

            if (!Lanko_And_Glub.utility.ShowInstructions)
                this.instructionsColor = new Color(0, 0, 0, 0);
            else
                this.instructionsColor = Color.White;

            this.scale = 0;

            this.fontScale = 1f;
            this.fontColor = timerColor = Color.White;

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (!Lanko_And_Glub.utility.GamePaused)
            {
                updateGameRound(gameTime);
            }

            if (Lanko_And_Glub.utility.GamePaused && this.FirstRoundStartHasStarted)
                this.instructionsColor = new Color(255, 255, 255, 255);
            else if (!Lanko_And_Glub.utility.GamePaused && this.FirstRoundStartHasStarted)
            {
                if (this.instructionsColor.A != 0)
                    this.instructionsColor = new Color(0, 0, 0, 0);
            }


            base.Update(gameTime);
        }

        private void updateGameRound(GameTime gameTime)
        {
            this.round.Update(gameTime);

            if (this.timeLeftInRound < 10 && this.timeLeftInRound > 0)
                timerColor = Color.Red;
            else
                timerColor = Color.White;

            if (gameTime.TotalGameTime.TotalSeconds > 6)
                FadeOutInstructionsImage();

            if (FirstRoundStartHasStarted == false && instructionsColor.R == 0)
                StartFirstRound();
            
            if (this.round.RoundIsOver == false)
                this.timeLeftInRound = (this.round.MaxRoundLength - this.round.CurrentRoundTime);

            else if (this.round.RoundIsOver && this.HasStartedRound)
            {
                //if (this.PlayerScore > this.HighScore)
                //    this.HighScore = this.PlayerScore;
                this.firstRoundOver = true;

                this.timeLeftInRound = 0;

                this.HasStartedRound = false;

                this.round.ResetRound();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            if (this.PlayerScore != 0)
            {
                spriteBatch.DrawString(font, this.PlayerScore.ToString(),
                    new Vector2(midpointRight, this.Game.GraphicsDevice.Viewport.Bounds.Top + 200),
                    fontColor, 0f, new Vector2(0, 0), fontScale, SpriteEffects.None, 0f);
            }

            //Draws the amount of time left in a round
            spriteBatch.DrawString(font, this.timeLeftInRound.ToString("0.00"),
                new Vector2(midpointRight - 50, this.Game.GraphicsDevice.Viewport.Bounds.Top + 30),
                timerColor, 0f, new Vector2(0, 0), fontScale, SpriteEffects.None, 0f);

            if (this.HighScore != 0 && this.firstRoundOver)
            {
                spriteBatch.DrawString(font, "High: " + this.HighScore.ToString(),
                    new Vector2(midpointRight, this.border.Walls[2].LocationRect.Top - 100),
                    Color.Purple, 0f, new Vector2(0, 0), fontScale / 2, SpriteEffects.None, 0f);
            }

            if(this.round.RoundIsOver)
            {
                spriteBatch.DrawString(font, "Start Round: \nRight Mouse",
                    new Vector2(midpointRight - 75, this.border.Walls[2].LocationRect.Top),
                    fontColor, 0f, new Vector2(0, 0), fontScale / 2, SpriteEffects.None, 0f);
            }

            spriteBatch.Draw(placeHolder, this.Location, null, this.instructionsColor, 0, new Vector2(0, 0), .76f,
                SpriteEffects.None, 0);
               
            spriteBatch.End();

            base.Draw(gameTime);
        }
        
        private void FadeOutInstructionsImage()
        {
            if (instructionsColor.R != 0)
                instructionsColor.R -= 5;

            if (instructionsColor.B != 0)
                instructionsColor.B -= 5;

            if (instructionsColor.G != 0)
                instructionsColor.G -= 5;

            if (instructionsColor.A != 0)
                instructionsColor.A -= 5;
        }

        //Calculated for the midpoints between the left and right edges of screen
        //and the left and right Level borders to help with text placement
        private void CalculateMidpoints()
        {
            this.midpointLeft = 
                ((this.border.Walls[3].LocationRect.Left - this.Game.GraphicsDevice.Viewport.Bounds.Left) / 2) +
                this.Game.GraphicsDevice.Viewport.Bounds.Left;

            this.midpointRight = 
                ((this.Game.GraphicsDevice.Viewport.Bounds.Right - this.border.Walls[1].LocationRect.Right) / 2) +
                this.border.Walls[1].LocationRect.Right;
        }

        public void FirstTimeSetup()
        {
            CalculateMidpoints();

            this.Location =
                new Vector2(this.border.Walls[3].LocationRect.Right + 115,
                this.border.Walls[0].LocationRect.Bottom);
        }

        private void StartFirstRound()
        {
            this.FirstRoundStartHasStarted = true;

            if (this.round.RoundIsOver == true)
            {
                this.round.RoundIsOver = false;
                this.HasStartedRound = true;
            }
        }


    }

}