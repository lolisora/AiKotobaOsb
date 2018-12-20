using OpenTK;
using OpenTK.Graphics;
using StorybrewCommon.Mapset;
using StorybrewCommon.Scripting;
using StorybrewCommon.Storyboarding;
using StorybrewCommon.Storyboarding.Util;
using StorybrewCommon.Subtitles;
using StorybrewCommon.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StorybrewScripts
{
    public class Bg : StoryboardObjectGenerator
    {
        public override void Generate()
        {
		    var layer = GetLayer("MySampleEffect");
            var bg = layer.CreateSprite("BG.jpg", OsbOrigin.Centre);
            bg.Scale(0, 1080.0 / 1920);
            bg.Fade(0, 2000, 0, 1);
            bg.Fade(240000, 245630, 1, 0);  
            
        }
    }
}
