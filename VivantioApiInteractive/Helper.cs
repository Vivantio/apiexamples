namespace VivantioApiInteractive
{
    public static class Helper
    {
        public const string PlatformUrlEnvVar = "VIVANTIO_PLATFORM_URL";
        public const string UsernameEnvVar = "VIVANTIO_USERNAME";
        public const string PasswordEnvVar = "VIVANTIO_PASSWORD";
        public const string ApplicationName = "Vivantio API Interactive";
        public static readonly string ExternalSource = ApplicationName.Replace(" ", "-").ToLower();


        public static bool AreEnvVarsSet()
        {
            string? platformUrl = Environment.GetEnvironmentVariable(PlatformUrlEnvVar);
            string? username = Environment.GetEnvironmentVariable(UsernameEnvVar);
            string? password = Environment.GetEnvironmentVariable(PasswordEnvVar);

            return !string.IsNullOrWhiteSpace(platformUrl) &&
                   !string.IsNullOrWhiteSpace(username) &&
                   !string.IsNullOrWhiteSpace(password);
        }

        public static async Task<bool> CanConnect()
        {
            var response = await ApiUtility.SendRequestAsync<BaseResponse>("Configuration/TicketTypeSelectAll");

            if (response == null)
            {
                return false;
            }

            return response.Successful;
        }

        public static string GetPlatformUrl()
        {
            string? platformUrl = Environment.GetEnvironmentVariable(PlatformUrlEnvVar);
            if (string.IsNullOrWhiteSpace(platformUrl))
            {
                throw new InvalidOperationException($"Environment variable '{PlatformUrlEnvVar}' is not set.");
            }
            return platformUrl.Substring(0, platformUrl.Length - 1);
        }

        public static string GetRandomCompanyName(Random random)
        {
            var list = new List<string>
            {
                "Apple Inc", "Microsoft Corporation", "Amazoncom Inc", "Alphabet Inc", "Meta Platforms Inc", "Tesla Inc", "International Business Machines", "Intel Corporation",
                "Samsung Electronics", "Sony Corporation", "Oracle Corporation", "Cisco Systems", "Adobe Inc", "Netflix Inc", "NVIDIA Corporation", "Salesforce Inc",
                "Dell Technologies", "Hewlett-Packard", "Qualcomm Inc", "Uber Technologies", "Airbnb Inc", "Spotify Technology", "Lyft Inc", "eBay Inc",
                "PayPal Holdings", "Snap Inc", "ByteDance Ltd", "Alibaba Group", "Tencent Holdings", "Baidu Inc", "Xiaomi Corporation", "Zoom Video Communications",
                "Reddit Inc", "Slack Technologies", "Dropbox Inc", "Block Inc", "Stripe Inc", "Shopify Inc", "Pinterest Inc", "Peloton Interactive",
                "LG Electronics", "HTC Corporation", "Motorola Solutions", "Nokia Corporation", "Lenovo Group", "AsusTek Computer", "Acer Inc", "Panasonic Corporation",
                "Philips NV", "Siemens AG", "Honeywell International", "General Electric", "3M Company", "The Boeing Company", "Ford Motor Company", "General Motors",
                "Volkswagen Group", "Toyota Motor Corporation", "Honda Motor Co", "BMW AG", "Mercedes-Benz Group", "Audi AG", "Hyundai Motor Company", "Kia Corporation",
                "PepsiCo Inc", "The Coca-Cola Company", "Nestlé SA", "Unilever PLC", "Procter & Gamble", "Johnson & Johnson", "Pfizer Inc", "Moderna Inc",
                "AstraZeneca PLC", "Novartis AG", "GlaxoSmithKline PLC", "Bayer AG", "Shell plc", "BP plc", "Chevron Corporation", "ExxonMobil Corporation",
                "Visa Inc", "Mastercard Inc", "American Express", "Goldman Sachs", "JPMorgan Chase", "Wells Fargo", "Citigroup Inc", "Morgan Stanley",
                "Bank of America", "Charles Schwab Corporation", "Fidelity Investments", "Robinhood Markets", "BlackRock Inc", "Vanguard Group", "T Rowe Price", "TikTok Ltd",
                "Roku Inc", "Hulu LLC", "Paramount Global", "Warner Bros Discovery", "The Walt Disney Company", "NBCUniversal Media", "Comcast Corporation", "AT&T Inc",
                "Verizon Communications", "T-Mobile US", "Sprint Corporation", "ZoomInfo Technologies", "RingCentral Inc", "DocuSign Inc", "Twilio Inc", "Square Enix",
                "Epic Games", "Electronic Arts", "Activision Blizzard", "Take-Two Interactive", "Ubisoft Entertainment", "Capcom Co", "Bandai Namco", "Valve Corporation",
                "Unity Technologies", "HCL Technologies", "Infosys Limited", "Tata Consultancy Services", "Wipro Limited", "Nimbus Industries", "Nova Network", "Blue Group",
                "Bright Systems", "Apex Enterprises", "Echo Industries", "Next Industries", "Future Corporation", "Zenith Global", "Prime Solutions", "Green Ventures",
                "Apex Industries", "Nimbus Enterprises", "Nimbus Systems", "Solar Technologies", "Bright Global", "Omni Industries", "Omni Ventures", "Apex Technologies",
                "Solar Group", "Prime Enterprises", "Quantum Network", "Nimbus Technologies", "Bright Industries", "Bright Ventures", "Next Group", "Blue Enterprises",
                "Blue Corporation", "Nova Group", "Cyber Industries", "Cyber Technologies", "Nimbus Network", "Quantum Enterprises", "Apex Ventures", "Quantum Solutions",
                "Future Group", "Zenith Solutions", "Apex Systems", "Zenith Systems", "Bright Technologies", "Omni Enterprises", "Cyber Network", "Cyber Ventures",
                "Zenith Ventures", "Quantum Group", "Nova Solutions", "Quantum Industries", "Cyber Enterprises", "Green Corporation", "Omni Group", "Nova Industries",
                "Prime Corporation", "Quantum Systems", "Nova Global", "Future Systems", "Next Solutions", "Future Network", "Bright Enterprises", "Solar Enterprises",
                "Echo Enterprises", "Apex Network", "Nimbus Global", "Green Enterprises", "Apex Corporation", "Quantum Global", "Zenith Enterprises", "Zenith Network",
                "Apex Group", "Nova Corporation", "Prime Ventures", "Solar Solutions", "Cyber Systems", "Prime Industries", "Bright Corporation", "Nova Systems",
                "Apex Global", "Prime Network", "Green Industries", "Echo Solutions", "Prime Systems", "Cyber Solutions", "Green Network", "Solar Network",
                "Prime Global", "Nimbus Group", "Nova Technologies", "Echo Systems", "Zenith Industries", "Omni Global", "Nova Ventures", "Green Systems",
                "Nimbus Solutions", "Future Global", "Solar Corporation", "Omni Solutions", "Omni Technologies", "Blue Network", "Next Systems", "Nimbus Corporation",
                "Green Solutions", "Green Technologies", "Quantum Corporation", "Cyber Group", "Nova Enterprises", "Echo Ventures", "Next Ventures", "Solar Systems",
                "Solar Industries", "Green Global", "Green Group", "Bright Solutions", "Prime Group", "Next Network", "Echo Global", "Blue Ventures",
                "Solar Ventures", "Bright Group", "Cyber Global", "Apex Solutions", "Future Solutions", "Omni Network", "Echo Network", "Prime Technologies",
                "Zenith Corporation", "Omni Systems", "Future Enterprises", "Zenith Technologies", "Next Corporation", "Blue Global", "Zenith Group", "Echo Corporation",
                "Future Ventures", "Future Industries", "Future Technologies", "Nimbus Ventures", "Echo Technologies", "Cyber Corporation", "Echo Group", "Blue Technologies",
                "Solar Global", "Blue Industries", "Next Technologies", "Bright Network", "Omni Corporation", "Quantum Technologies", "Blue Systems", "Blue Solutions",
                "Next Enterprises", "Quantum Ventures", "Next Global",
            };

            return list[random.Next(list.Count)];
        }

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

        public static string GetRandomCityName(Random random)
        {
            var list = new List<string>
            {
                "New York", "London", "Tokyo", "Paris", "Sydney", "Beijing", "Dubai", "Singapore",
                "Los Angeles", "Toronto", "Shanghai", "Hong Kong", "Berlin", "Madrid", "Rome", "Chicago",
                "São Paulo", "Mexico City", "Moscow", "Seoul", "Mumbai", "Bangkok", "Istanbul", "Jakarta",
                "Barcelona", "Buenos Aires", "Cairo", "Lagos", "San Francisco", "Melbourne", "Vienna", "Zurich",
                "Kuala Lumpur", "Amsterdam", "Dublin", "Riyadh", "Abu Dhabi", "Doha", "Warsaw", "Lisbon",
                "Manila", "Athens", "Helsinki", "Oslo", "Stockholm", "Prague", "Budapest", "Copenhagen",
                "Brussels", "Montreal", "Vancouver", "Boston", "Washington D.C.", "Philadelphia", "Phoenix", "Miami",
                "Atlanta", "Houston", "Dallas", "Seattle", "Denver", "San Diego", "Detroit", "Minneapolis",
                "Austin", "Portland", "San Jose", "Tampa", "Charlotte", "Orlando", "Columbus", "Indianapolis",
                "Cleveland", "Kansas City", "Nashville", "Pittsburgh", "St. Louis", "Cincinnati", "Milwaukee", "New Orleans",
                "Birmingham", "Raleigh", "Salt Lake City", "Las Vegas", "Oklahoma City", "Richmond", "Buffalo", "Albany",
                "Anchorage", "Honolulu", "Des Moines", "Boise", "Tucson", "Albuquerque", "Fresno", "Sacramento",
                "Chennai", "Karachi", "Lima", "Tehran", "Hanoi", "Cape Town", "Casablanca", "Addis Ababa"
            };

            return list[random.Next(list.Count)];
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

        public static string GetRandomFirstName(Random random)
        {
            var list = new List<string>
            {
                "James", "Mary", "John", "Patricia", "Robert", "Jennifer", "Michael", "Linda",
                "William", "Elizabeth", "David", "Barbara", "Richard", "Susan", "Joseph", "Jessica",
                "Thomas", "Sarah", "Charles", "Karen", "Christopher", "Nancy", "Daniel", "Lisa",
                "Matthew", "Betty", "Anthony", "Margaret", "Donald", "Sandra", "Mark", "Ashley",
                "Paul", "Kimberly", "Steven", "Emily", "Andrew", "Donna", "Kenneth", "Michelle",
                "George", "Dorothy", "Joshua", "Carol", "Kevin", "Amanda", "Brian", "Melissa",
                "Edward", "Deborah", "Ronald", "Stephanie", "Timothy", "Rebecca", "Jason", "Sharon",
                "Jeffrey", "Laura", "Ryan", "Cynthia", "Jacob", "Kathleen", "Gary", "Amy",
                "Nicholas", "Shirley", "Eric", "Angela", "Stephen", "Helen", "Jonathan", "Anna",
                "Larry", "Brenda", "Justin", "Pamela", "Scott", "Nicole", "Brandon", "Emma",
                "Frank", "Samantha", "Benjamin", "Katherine", "Gregory", "Christine", "Samuel", "Debra",
                "Raymond", "Rachel", "Patrick", "Catherine", "Alexander", "Carolyn", "Jack", "Janet"
            };

            return list[random.Next(list.Count)];
        }

        public static string GetRandomLastName(Random random)
        {
            var list = new List<string>
            {
                "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez",
                "Hernandez", "Lopez", "Gonzalez", "Wilson", "Anderson", "Thomas", "Taylor", "Moore", "Jackson", "Martin",
                "Lee", "Perez", "Thompson", "White", "Harris", "Sanchez", "Clark", "Ramirez", "Lewis", "Robinson",
                "Walker", "Young", "Allen", "King", "Wright", "Scott", "Torres", "Nguyen", "Hill", "Flores",
                "Green", "Adams", "Nelson", "Baker", "Hall", "Rivera", "Campbell", "Mitchell", "Carter", "Roberts",
                "Gomez", "Phillips", "Evans", "Turner", "Diaz", "Parker", "Cruz", "Edwards", "Collins", "Reyes",
                "Stewart", "Morris", "Morales", "Murphy", "Cook", "Rogers", "Gutierrez", "Ortiz", "Morgan", "Cooper",
                "Peterson", "Bailey", "Reed", "Kelly", "Howard", "Ramos", "Kim", "Cox", "Ward", "Richardson",
                "Watson", "Brooks", "Chavez", "Wood", "James", "Bennett", "Gray", "Mendoza", "Ruiz", "Hughes",
                "Price", "Alvarez", "Castillo", "Sanders", "Patel", "Myers", "Long", "Ross", "Foster"
            };

            return list[random.Next(list.Count)];
        }
    }

    public static class RandomProvider
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
}
