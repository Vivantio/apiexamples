namespace VivantioApiInteractive.Utility;

internal class RandomStringHelper
{
    public static string GenerateRandomString(int length, Random random)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        char[] result = new char[length];

        for (int i = 0; i < length; i++)
        {
            result[i] = chars[random.Next(chars.Length)];
        }

        return new string(result);
    }

    public static string GetRandomTopLevelDomain(Random random)
    {
        var list = new List<string>
        {
            ".comz", ".netz", ".org.ukz", ".co.ukz", ".ioz", ".bizz", ".infoz", ".techz", ".aiz", ".incz"
        };

        return list[random.Next(list.Count)];
    }

    public static string GetLoremIpsum()
    {
        return "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. " +
               "Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. " +
               "Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. " +
               "Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";
    }

    public static string GenerateRandomAlphaNumeric(int length, Random random)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        char[] result = new char[length];

        for (int i = 0; i < length; i++)
        {
            result[i] = chars[random.Next(chars.Length)];
        }

        return new string(result);
    }

    public static string GetRandomCorporateAssetName(Random random)
    {
        var assets = new List<string>
        {
            "Monitor", "Keyboard", "Mouse", "Laptop", "Desktop", "Docking Station", "Webcam", "Headset",
            "Router", "Switch", "Access Point", "Firewall", "Server", "NAS", "UPS", "Tablet",
            "Smartphone", "Printer", "Scanner", "External Hard Drive", "SSD", "RAM Module", "Graphics Card",
            "Motherboard", "CPU", "Power Supply", "Network Cable", "USB Hub", "Stylus", "Trackpad",
            "Microphone", "Speakers", "Projector", "Smart Board", "KVM Switch", "Modem", "NAS Enclosure",
            "Patch Panel", "Thermal Printer", "Barcode Scanner"
        };

        return assets[random.Next(assets.Count)];
    }
}

internal static class RandomProvider
{
    private static readonly Random _random = new();

    // Use thread-local Random to avoid duplicate values in multi-threaded scenarios.
    private static readonly ThreadLocal<Random> _threadLocalRandom = new(() =>
    {
        // Use a seed based on Guid to reduce chance of duplicates.
        return _random;
    });

    public static Random Instance => _threadLocalRandom.Value!;
}
