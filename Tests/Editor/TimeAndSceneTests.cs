using NUnit.Framework;

namespace LSH.Core.Tests
{
    public class TimeAndSceneTests
    {
        private enum TestScene
        {
            Entry = 0,
            Title = 1,
            Loading = 99
        }

        [Test]
        public void SceneNameUtility_AddsTwoDigitIndexPrefix()
        {
            Assert.AreEqual("01_Title", SceneNameUtility.From(TestScene.Title).Value);
            Assert.AreEqual("99_Loading", SceneNameUtility.From(TestScene.Loading).Value);
        }

        [Test]
        public void TimeChannel_PauseResumeAndReset_AreStable()
        {
            TimeChannel channel = new();

            channel.Tick(0.25f);
            Assert.AreEqual(0.25f, channel.Time);

            channel.Pause();
            channel.Tick(1f);
            Assert.AreEqual(0f, channel.DeltaTime);
            Assert.AreEqual(0.25f, channel.Time);

            channel.Resume();
            channel.Tick(1f);
            Assert.AreEqual(0f, channel.DeltaTime);
            Assert.AreEqual(0.25f, channel.Time);

            channel.Tick(0.5f);
            Assert.AreEqual(0.75f, channel.Time);

            channel.Reset();
            Assert.AreEqual(0f, channel.Time);
            Assert.AreEqual(0f, channel.FixedTime);
            Assert.IsFalse(channel.IsPaused);
        }
    }
}
