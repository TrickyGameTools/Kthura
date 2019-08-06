// Lic:
// Kthura for C#
// Pre-core
// 
// 
// 
// (c) Jeroen P. Broks, 
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 
// Please note that some references to data like pictures or audio, do not automatically
// fall under this licenses. Mostly this is noted in the respective files.
// 
// Version: 19.08.06
// EndLic


#undef DEBUG_PROJECT_MAP




using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace KthuraEdit
{
    /// <summary>
    /// This is the very core stuff. But all it does is set the actual "Core" of the editor in motion. 
    /// </summary>
    public class Kthura_EditCore : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public bool Quit = false;

        public Kthura_EditCore()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Core.CoreInit(this);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            #region Go into Full Screen
            graphics.HardwareModeSwitch = false;
            graphics.IsFullScreen = true;
            graphics.ApplyChanges();
            #endregion

            Core.Start(graphics,GraphicsDevice,spriteBatch,this);
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
            Core.StartStep3(spriteBatch);
#if DEBUG_PROJECT_MAP
            Core.Project = "Test";
            Core.MapFile = "TestMap";
#else
            //Core.Crash("Project from parameters not yet set");
            Core.FLOG("Parameter Check!");
            for (int i = 0; i < Core.args.Length; i++) Core.FLOG($"{i}/{Core.args.Length-1}:> {Core.args[i]}");
            if (Core.args.Length < 3) {
                Core.Crash("No arguments given!\nUsage: TeddyEdit <project> <map>\n \n If you are not sure how to use this tool, use the launcher in stead!");
                return;
            }
            Core.Project = Core.args[1];
            Core.MapFile = Core.args[2];

#endif
            Core.InitLua();
            EditorSpecificDrawFunctions.init();
            NSKthura.KthuraDrawMonoGame.UseMe();
            NSKthura.KthuraDraw.IgnoreVisibility = true;
            KthuraExport_NS.ExportBasis.Init();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        bool UpdatedOnce = false;
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>        
        protected override void Update(GameTime gameTime)
        {
            if (Quit) {
                Core.Save();
                Exit();
            }

            // TODO: Add your update logic here
            if (!UpdatedOnce) Debug.WriteLine("First Update call");
            UpdatedOnce = true;
            Core.PerformUpdate();
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap, null, null);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.LinearWrap, null, null);
            if (!UpdatedOnce)
                Debug.WriteLine("Skipping draw as the first update has not yet taken place! (Security measure to prevent needless crashes)");
            else
                Core.PerformDraw();
            spriteBatch.End();

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}








