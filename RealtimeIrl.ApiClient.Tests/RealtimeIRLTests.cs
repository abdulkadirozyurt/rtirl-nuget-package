using RealtimeIrl.ApiClient;
using RealtimeIrl.ApiClient.Abstractions;
using RealtimeIrl.ApiClient.Models;
using Xunit;

namespace RealtimeIrl.ApiClient.Tests;

public class RealtimeIRLTests
{
    [Fact]
    public void InitializeAppRejectsNullConfig()
    {
        Assert.Throws<ArgumentNullException>(() => RealtimeIRL.InitializeApp(null!));
    }

    [Fact]
    public void InitializeAppRejectsEmptyDatabaseUrl()
    {
        var config = new FirebaseAppConfig { DatabaseUrl = "" };

        Assert.Throws<ArgumentException>(() => RealtimeIRL.InitializeApp(config));
    }

    [Fact]
    public void ForPullKeyRejectsEmptyPullKey()
    {
        var app = new RealtimeIRLApp(new FirebaseAppConfig());

        Assert.Throws<ArgumentException>(() => app.ForPullKey(" "));
    }

    [Fact]
    public void ForStreamerRejectsEmptyProvider()
    {
        var app = new RealtimeIRLApp(new FirebaseAppConfig());

        Assert.Throws<ArgumentException>(() => app.ForStreamer("", "123"));
    }

    [Fact]
    public void ForStreamerRejectsEmptyUserId()
    {
        var app = new RealtimeIRLApp(new FirebaseAppConfig());

        Assert.Throws<ArgumentException>(() => app.ForStreamer("twitch", ""));
    }

    [Fact]
    public void ForPullKeyReturnsPullKeyClient()
    {
        var app = new RealtimeIRLApp(new FirebaseAppConfig());

        var client = app.ForPullKey("abc");

        Assert.IsAssignableFrom<IPullKeyClient>(client);
        client.Dispose();
    }

    [Fact]
    public void ForStreamerReturnsStreamerClient()
    {
        var app = new RealtimeIRLApp(new FirebaseAppConfig());

        var client = app.ForStreamer("twitch", "123");

        Assert.IsAssignableFrom<IStreamerClient>(client);
        client.Dispose();
    }

    [Fact]
    public void PullKeyListenersRejectNullCallbacksBeforeSubscribing()
    {
        using var client = new RealtimeIRLApp(new FirebaseAppConfig()).ForPullKey("abc");

        Assert.Throws<ArgumentNullException>(() => client.AddLocationListener(null!));
        Assert.Throws<ArgumentNullException>(() => client.AddSpeedListener(null!));
        Assert.Throws<ArgumentNullException>(() => client.AddSessionIdListener(null!));
        Assert.Throws<ArgumentNullException>(() => client.AddListener(null!));
    }

    [Fact]
    public void StreamerLocationListenerRejectsNullCallbackBeforeSubscribing()
    {
        using var client = new RealtimeIRLApp(new FirebaseAppConfig()).ForStreamer("twitch", "123");

        Assert.Throws<ArgumentNullException>(() => client.AddLocationListener(null!));
    }
}
