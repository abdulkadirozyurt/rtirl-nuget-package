using RtIrl.Api;
using RtIrl.Api.Models;

Console.WriteLine("RTIRL Saf WebSocket (Firebase Realtime) Testi...");

string provider = "twitch";
string userId = "187089846";

Console.WriteLine($"\n{provider}:{userId} için WebSocket üzerinden veri bekleniyor...");
Console.WriteLine("Çıkış için Enter'a basın.\n");

// Orijinal kütüphaneyi kullan
using var client = RealtimeIRL.ForStreamer(provider, userId);

// Sadece WebSocket olaylarını dinle
using var sub = client.AddLocationListener(loc => {
    if (loc != null)
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Yeni WebSocket Verisi: {loc.Latitude}, {loc.Longitude}");
    else
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Konum verisi gizli veya yayıncı kapalı.");
});

Console.ReadLine();
Console.WriteLine("Test sonlandırıldı.");
