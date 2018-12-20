using OpenTK;
using StorybrewCommon.Animations;
using StorybrewCommon.Scripting;
using StorybrewCommon.Storyboarding;
using System;

namespace StorybrewScripts
{
    /// <summary>
    /// An example of a spectrum effect.
    /// </summary>
    public class Spectrum : StoryboardObjectGenerator
    {
        public int StartTime = 7857;
        public int EndTime = 245630;
        public Vector2 Position = new Vector2(-107, 480);
        public float Width = 844;
        public int BeatDivisor = 16;
        public int BarCount = 50;
        public string SpritePath = "sb/bar.png";
        public OsbOrigin SpriteOrigin = OsbOrigin.CentreLeft;
        public Vector2 Scale = new Vector2(1, 100);
        public int LogScale = 600;
        public double Tolerance = 0.2;
        public int CommandDecimals = 1;
        public float MinimalHeight = 0.05f;
        public OsbEasing FftEasing = OsbEasing.InExpo;

        public override void Generate()
        {
            var endTime = Math.Min(EndTime, (int)AudioDuration);
            var startTime = Math.Min(StartTime, endTime);
            var bitmap = GetMapsetBitmap(SpritePath);

            var heightKeyframes = new KeyframedValue<float>[BarCount];
            for (var i = 0; i < BarCount; i++)
                heightKeyframes[i] = new KeyframedValue<float>(null);

            var fftTimeStep = Beatmap.GetTimingPointAt(startTime).BeatDuration / BeatDivisor;
            var fftOffset = fftTimeStep * 0.2;
            for (var time = (double)startTime; time < endTime; time += fftTimeStep)
            {
                var fft = GetFft(time + fftOffset, BarCount, null, FftEasing);
                for (var i = 0; i < BarCount; i++)
                {
                    var height = (float)Math.Log10(1 + fft[i] * LogScale) * Scale.Y / bitmap.Height;
                    if (height < MinimalHeight) height = MinimalHeight;

                    heightKeyframes[i].Add(time, height);
                }
            }

            var layer = GetLayer("Spectrum");
            var barWidth = Width / BarCount;
            for (var i = 0; i < BarCount; i++)
            {
                var keyframes = heightKeyframes[i];
                keyframes.Simplify1dKeyframes(Tolerance, h => h);

                var bar = layer.CreateSprite(SpritePath, SpriteOrigin, new Vector2(Position.X + i * barWidth, Position.Y));
                bar.ColorHsb(startTime, 0, 0, 0.2 + Random(0.2));
                bar.Additive(startTime, endTime);

                var scaleX = Scale.X * barWidth / bitmap.Width;
                scaleX = (float)Math.Floor(scaleX * 10) / 10.0f;

                var hasScale = false;
                keyframes.ForEachPair(
                    (start, end) =>
                    {
                        hasScale = true;
                        bar.ScaleVec(start.Time, end.Time,
                            scaleX, start.Value,
                            scaleX, end.Value);
                    },
                    MinimalHeight,
                    s => (float)Math.Round(s, CommandDecimals)
                );
                if (!hasScale) bar.ScaleVec(startTime, scaleX, MinimalHeight);
            }
        }
    }
}
